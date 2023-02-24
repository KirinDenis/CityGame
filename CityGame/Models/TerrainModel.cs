using CityGame.Data.DTO;
using CityGame.Data.Enum;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CityGame.Models
{
    public class TerrainModel : BaseModel
    {
        /// <summary>
        /// Terrain width and height at sprites
        /// Current sprite size is 16x16 piexels
        /// The terrain size at pixels is terrainSize * 16
        /// </summary>
        public int terrainSize = 100;

        /// <summary>
        /// If true, the Diamond generator is not called, the terrain is simple random values 2D array 
        /// The fast way of debuging all not related to terrain
        /// </summary>
        private const bool fackeDiomand = false;

        //Terrain map - sprite offsets
        public PositionDTO[,] terrain;

        public int waterLevel = 140;

        public int landLevel = 170; //upper this is forest        

        public int roughness = 0;

        private SpriteBusiness spriteBusiness = new SpriteBusiness();

        private Random random = new Random();

        public WriteableBitmap terrainBitmap;
        public WriteableBitmap mapBitmap;

        public TerrainModel(int terrainSize)
        {

            if (SpriteRepository.ResourceInfo == null)
            {
                return;
            }

            this.terrainSize = terrainSize;

            terrain = new PositionDTO[terrainSize, terrainSize];


            BitmapImage sImage = SpriteRepository.GetSprite(0, 0);
            terrainBitmap = new WriteableBitmap(terrainSize * 16, terrainSize * 16, sImage.DpiX, sImage.DpiY, sImage.Format, sImage.Palette);
            mapBitmap = new WriteableBitmap(terrainSize, terrainSize, sImage.DpiX, sImage.DpiY, sImage.Format, sImage.Palette);


            GenerateNewTerrain();

        }

        //http://www.java2s.com/example/csharp/system/color-to-byte-array.html
        private byte[] ColorToByteArray(Color color)
        {
            byte[] colorArray = new byte[4];
            colorArray[0] = color.B;
            colorArray[1] = color.G;
            colorArray[2] = color.R;
            colorArray[3] = color.A;

            return colorArray;
        }

        public bool PutSprite(ushort x, ushort y, GroupDTO? group, byte animationFrame, int spriteX, int spriteY)
        {
            if ((group == null) || (group?.Sprites.Count <= animationFrame)
                || (group?.Sprites == null) || (group?.Sprites[animationFrame] == null) || (group?.Sprites[animationFrame].Sprites[spriteX, spriteY] == null)
                || (x >= terrainSize) || (y >= terrainSize))
            {
                return false;
            }

            terrain[x, y] = group?.Sprites?[animationFrame]?.Sprites[spriteX, spriteY];



            Int32Rect rect = new Int32Rect(x * SpriteRepository.ResourceInfo.SpriteSize, y * SpriteRepository.ResourceInfo.SpriteSize, SpriteRepository.ResourceInfo.SpriteSize, SpriteRepository.ResourceInfo.SpriteSize);

            terrainBitmap.WritePixels(rect, SpriteRepository.GetPixels((int)terrain[x, y].x, (int)terrain[x, y].y), SpriteRepository.ResourceInfo.SpriteSize * 4, 0);

            rect = new Int32Rect(x, y, 1, 1);

            byte[] pixel;

            switch (SpritesGroupEnum.GetObjectTypeByGroupName(group.Name))
            {
                case ObjectType.terrain: pixel = ColorToByteArray(Color.Brown); break;
                case ObjectType.forest: pixel = ColorToByteArray(Color.Green); break;
                case ObjectType.water: pixel = ColorToByteArray(Color.Blue); break;
                case ObjectType.network: pixel = ColorToByteArray(Color.Black); break;
                case ObjectType.garden: pixel = ColorToByteArray(Color.Green); break;
                case ObjectType.building: 
                    switch (group.Name)
                    {
                        case SpritesGroupEnum.firedepartment: pixel = ColorToByteArray(Color.Red); break;
                        case SpritesGroupEnum.policedepartment: pixel = ColorToByteArray(Color.LightBlue); break;
                        case SpritesGroupEnum.coalpowerplant: pixel = ColorToByteArray(Color.Red); break;
                            default: pixel = ColorToByteArray(Color.LightCyan); break;
                    }
                    break;
                default: pixel = ColorToByteArray(Color.Gray); break;
            }

            mapBitmap.WritePixels(rect, pixel, 4, 0);//SpriteRepository.ResourceInfo.SpriteSize * SpriteRepository.ResourceInfo.SpriteSize / 2);

            return true;
        }

        public bool PutSprite(ushort x, ushort y, GroupDTO? group, PositionDTO spritePosition)
        {
            return PutSprite(x, y, group, 0, spritePosition.x, spritePosition.y);
        }

        public bool BuildObject(ushort x, ushort y, GroupDTO group, byte animationFrame = 0)
        {


            if (group == null)
            {
                return false;
            }

            if (group?.Sprites.Count - 1 < animationFrame)
            {
                return false;
            }

            if ((x > terrainSize - group?.Width) || (y > terrainSize - group?.Height))
            {
                return false;
            }

            for (ushort sx = 0; sx < group?.Width; sx++)
            {
                for (ushort sy = 0; sy < group?.Height; sy++)
                {

                    if (group?.Sprites[animationFrame].Sprites[sx, sy] != null)
                    {
                        //   terrain[x + sx, y + sy] = group?.Sprites[animationFrame].Sprites[sx, sy];
                        PutSprite((ushort)(x + sx), (ushort)(y + sy), group, animationFrame, sx, sy);
                    }
                }
            }
            return true;

        }

        public bool TestRange(PositionDTO position)
        {
            if (position == null)
            {
                return false;
            }

            if ((position.x >= 0) && (position.x < terrainSize)
                &&
                (position.y >= 0) && (position.y < terrainSize))
            {
                return true;
            }
            return false;
        }

        public TestPositionDTO TestPosition(GroupDTO? group, PositionDTO position)
        {
            TestPositionDTO result = new TestPositionDTO();
            if (group == null)
            {
                return result;
            }

            if (!TestRange(position))
            {
                return result;
            }

            result.PositionArea = new ObjectType[group.Width, group.Height];
            result.CanBuild = true;

            if (spriteBusiness.GetObjectTypeByGrop(group) != ObjectType.network)
            {
                int rx, ry;
                rx = 0;
                for (int sx = position.x; sx < position.x + group.Width; sx++, rx++)
                {
                    ry = 0;
                    for (int sy = position.y; sy < position.y + group.Height; sy++, ry++)
                    {
                        if ((sx < terrainSize) && (sy < terrainSize))
                        {
                            result.PositionArea[rx, ry] = SpritesGroupEnum.GetObjectTypeByGroupName(spriteBusiness.GetGroupBySpritePosition(terrain[sx, sy])?.Name);

                            if ((result.PositionArea[rx, ry] != ObjectType.terrain)
                                &&
                                (result.PositionArea[rx, ry] != ObjectType.forest)
                                &&
                                (result.PositionArea[rx, ry] != ObjectType.network))
                            {
                                result.CanBuild = false;
                            }

                        }
                        else
                        {
                            result.PositionArea[rx, ry] = ObjectType.water;
                            result.CanBuild = false;
                        }
                    }
                }
            }
            else
            {
                result.PositionArea[0, 0] = SpritesGroupEnum.GetObjectTypeByGroupName(spriteBusiness.GetGroupBySpritePosition(terrain[position.x, position.y])?.Name);
                if ((result.PositionArea[0, 0] != ObjectType.terrain)
                    &&
                    (result.PositionArea[0, 0] != ObjectType.forest)
                    &&
                    (result.PositionArea[0, 0] != ObjectType.water)
                    &&
                    (result.PositionArea[0, 0] != ObjectType.network))
                {
                    result.CanBuild = false;
                }

            }
            return result;
        }

        protected Func<TerrainModel, ushort, ushort, int[,], TerrainType, GroupSpritesDTO, GroupSpritesDTO, bool> PutTerrainBlock = delegate (TerrainModel terrainModel, ushort x, ushort y, int[,] sourceTerraing, TerrainType groupType, GroupSpritesDTO groupSprites, GroupSpritesDTO borderSprites)
        {

            if ((x > 0) && (y > 0) && (sourceTerraing[terrainModel.CLeft(x), y] != (int)groupType) && (sourceTerraing[x, terrainModel.CTop(y)] != (int)groupType))
            {
                terrainModel.terrain[x, y] = borderSprites.Sprites[0, 0];
            }
            else
            if ((x > 0) && (y < terrainModel.terrainSize - 1) && (sourceTerraing[terrainModel.CLeft(x), y] != (int)groupType) && (sourceTerraing[x, terrainModel.CBottom(y)] != (int)groupType))
            {
                terrainModel.terrain[x, y] = borderSprites.Sprites[0, 2];
            }
            else
            if ((x < terrainModel.terrainSize - 1) && (y > 0) && (sourceTerraing[terrainModel.CRight(x), y] != (int)groupType) && (sourceTerraing[x, terrainModel.CTop(y)] != (int)groupType))
            {
                terrainModel.terrain[x, y] = borderSprites.Sprites[2, 0];
            }
            else
            if ((x < terrainModel.terrainSize - 1) && (y < terrainModel.terrainSize - 1) && (sourceTerraing[terrainModel.CRight(x), y] != (int)groupType) && (sourceTerraing[x, terrainModel.CBottom(y)] != (int)groupType))
            {
                terrainModel.terrain[x, y] = borderSprites.Sprites[2, 2];
            }
            else
            if ((x > 0) && (sourceTerraing[terrainModel.CLeft(x), y] != (int)groupType))
            {
                terrainModel.terrain[x, y] = borderSprites.Sprites[0, 1];
            }
            else
            if ((y > 0) && (sourceTerraing[x, terrainModel.CTop(y)] != (int)groupType))
            {
                terrainModel.terrain[x, y] = borderSprites.Sprites[1, 0];
            }
            else
            if ((y < terrainModel.terrainSize - 1) && (sourceTerraing[x, terrainModel.CBottom(y)] != (int)groupType))
            {
                terrainModel.terrain[x, y] = borderSprites.Sprites[1, 2];
            }
            else
            if ((x < terrainModel.terrainSize - 1) && (sourceTerraing[terrainModel.CRight(x), y] != (int)groupType))
            {
                terrainModel.terrain[x, y] = borderSprites.Sprites[2, 1];
            }
            else
            {
                terrainModel.terrain[x, y] = groupSprites.Sprites[1, 1];
            }

            return true;
        };
        public void GenerateNewTerrain()
        {
            List<GroupSpritesDTO> water = new List<GroupSpritesDTO>();
            water.Add(spriteBusiness.GetSpritesByGroupName(SpritesGroupEnum.water, 0));
            water.Add(spriteBusiness.GetSpritesByGroupName(SpritesGroupEnum.water, 1));
            water.Add(spriteBusiness.GetSpritesByGroupName(SpritesGroupEnum.water, 2));

            List<GroupSpritesDTO> coast = new List<GroupSpritesDTO>();

            coast.Add(spriteBusiness.GetSpritesByGroupName(SpritesGroupEnum.coast, 0));
            coast.Add(spriteBusiness.GetSpritesByGroupName(SpritesGroupEnum.coast, 1));


            List<GroupSpritesDTO> forest = new List<GroupSpritesDTO>();
            forest.Add(spriteBusiness.GetSpritesByGroupName(SpritesGroupEnum.forest, 1));
            forest.Add(spriteBusiness.GetSpritesByGroupName(SpritesGroupEnum.forest, 2));

            DiamondSquareFast diamondSquare = new DiamondSquareFast(terrainSize, roughness);
            int[,] sourceTerraing = diamondSquare.getData();

            for (int x = 0; x < terrainSize - 0; x++)
            {

                for (int y = 0; y < terrainSize - 0; y++)
                {

                    if (sourceTerraing[x, y] <= waterLevel)
                    {
                        sourceTerraing[x, y] = (int)TerrainType.water;
                    }
                    else
                    if (sourceTerraing[x, y] <= landLevel)
                    {
                        sourceTerraing[x, y] = (int)TerrainType.land;
                    }
                    else
                        sourceTerraing[x, y] = (int)TerrainType.forest;
                }
            }

            byte randomIndex = 0;
            for (ushort x = 0; x < terrainSize; x++)
            {
                randomIndex = randomIndex != 0 ? (byte)0 : (byte)1;
                for (ushort y = 0; y < terrainSize; y++)
                {
                    randomIndex = randomIndex != 0 ? (byte)0 : (byte)1;

                    //delete singe 

                    if (x > 0 && y > 0 && x < terrainSize - 1 && y < terrainSize - 1)
                    {
                        int s = (sourceTerraing[CLeft(x), CTop(y)] | sourceTerraing[x, CTop(y)] | sourceTerraing[CRight(x), y - 1] |
                            sourceTerraing[x - 1, y] | sourceTerraing[x + 1, y] |
                            sourceTerraing[x - 1, y + 1] | sourceTerraing[x, y + 1] | sourceTerraing[x + 1, y + 1]) & sourceTerraing[x, y];

                        if (s != sourceTerraing[x, y])
                        {
                            sourceTerraing[x, y] = (int)TerrainType.land;
                        }
                    }


                    switch (sourceTerraing[x, y])
                    {
                        case (int)TerrainType.water:

                            PutTerrainBlock(this, x, y, sourceTerraing, TerrainType.water, water[randomIndex], coast[randomIndex]);
                            break;

                        case (int)TerrainType.land:
                            {
                                //TODO: terrain sprite
                                terrain[x, y] = new PositionDTO()
                                {
                                    x = 0,
                                    y = 0
                                };
                                randomIndex = randomIndex != 0 ? (byte)0 : (byte)1;
                                break;
                            }
                        default:
                            PutTerrainBlock(this, x, y, sourceTerraing, TerrainType.forest, forest[randomIndex], forest[randomIndex]);
                            break;
                    }
                    if (SpriteRepository.GetPixels(terrain[x, y]) != null)
                    {
                        byte[] pixels = SpriteRepository.GetPixels(terrain[x, y]);
                        Int32Rect rect = new Int32Rect(x * 16, y * 16, 16, 16);
                        terrainBitmap.WritePixels(rect, pixels, 16 * 4, 0);

                        rect = new Int32Rect(x, y, 1, 1);
                        mapBitmap.WritePixels(rect, pixels, 4, 0);


                    }
                }
            }

        }
    }
}
