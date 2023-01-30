using CityGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using CityGame.DTOs.Enum;
using CityGame.DTOs;

namespace CityGame.Models
{
    public class TerrainModel: BaseModel
    {       
        /// <summary>
        /// Terrain width and height at sprites
        /// Current sprite size is 16x16 piexels
        /// The terrain size at pixels is terrainSize * 16
        /// </summary>
        public int terrainSize = 10400;

        /// <summary>
        /// If true, the Diamond generator is not called, the terrain is simple random values 2D array 
        /// The fast way of debuging all not related to terrain
        /// </summary>
        private const bool fackeDiomand = false;

        //Terrain map - sprite offsets
        public PositionDTO[,] terrain;

        public int waterLevel = 120;

        public int landLevel = 130; //upper this is forest        

        public int roughness = 0;

        private SpriteBusiness spriteBusiness = new SpriteBusiness();

        private Random random = new Random();

        public WriteableBitmap bitmapSource;

        public TerrainModel(int terrainSize)
        {
            
            this.terrainSize = terrainSize;

            terrain = new PositionDTO[terrainSize, terrainSize];


            BitmapImage sImage = SpriteRepository.GetSprite(0, 0);
            bitmapSource = new WriteableBitmap(terrainSize* 16, terrainSize * 16, sImage.DpiX, sImage.DpiY, sImage.Format, sImage.Palette);


            GenerateNewTerrain();

        }

        public void PutImage(int x, int y, int bx, int by)
        {
            Int32Rect rect = new Int32Rect(x * 16, y * 16, 16, 16);
            bitmapSource.WritePixels(rect, SpriteRepository.GetPixels(bx, by), 16, 0);
        }

        public void GenerateNewTerrain()
        {
            int[,] sourceTerraing;
            if (!fackeDiomand)
            {
                DiamondSquareFast diamondSquare = new DiamondSquareFast(terrainSize, roughness);
                sourceTerraing = diamondSquare.getData();
            }
            else
            {
                sourceTerraing = new int[terrainSize, terrainSize];
                for (int x = 0; x < terrainSize; x++)
                {
                    for (int y = 0; y < terrainSize; y++)
                    {
                        sourceTerraing[x, y] = 0; // random.Next(255);
                    }
                }
            }

            // Canvas.Children.Clear();


            for (int x = 0; x < terrainSize - 0; x++)
            {

                for (int y = 0; y < terrainSize - 0; y++)
                {

                    if (sourceTerraing[x, y] <= waterLevel)
                    {
                        sourceTerraing[x, y] = (int)terrainType.water;
                    }
                    else
                    if (sourceTerraing[x, y] <= landLevel)
                    {
                        sourceTerraing[x, y] = (int)terrainType.land;
                    }
                    else
                        sourceTerraing[x, y] = (int)terrainType.forest;
                }
            }



            byte randomIndex = 0;
            for (int x = 1; x < terrainSize - 1; x++)
            {
                randomIndex = randomIndex != 0 ? (byte)0 : (byte)1;
                for (int y = 1; y < terrainSize - 1; y++)
                {

                    //delete singe 

                    int s = (sourceTerraing[CLeft(x), CTop(y)] | sourceTerraing[x, CTop(y)] | sourceTerraing[CRight(x), y - 1] |
                        sourceTerraing[x - 1, y] | sourceTerraing[x + 1, y] |
                        sourceTerraing[x - 1, y + 1] | sourceTerraing[x, y + 1] | sourceTerraing[x + 1, y + 1]) & sourceTerraing[x, y];

                    if (s != sourceTerraing[x, y])
                    {
                        sourceTerraing[x, y] = (int)terrainType.land;
                    }

                    randomIndex = randomIndex != 0 ? (byte)0 : (byte)1;

                    //TEMP
                    terrain[x, y] = new PositionDTO()
                    {
                        x = 0,
                        y = 0
                    };



                    switch (sourceTerraing[x, y])
                    {
                        case (int)terrainType.water:
                            {
                                if ((sourceTerraing[CLeft(x), y] != (int)terrainType.water) && (sourceTerraing[x, CTop(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpritesByGroupName(SpritesGroupEnum.water, randomIndex).Sprites[0, 0];
                                }
                                else
                                if ((sourceTerraing[CLeft(x), y] != (int)terrainType.water) && (sourceTerraing[x, CBottom(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpritesByGroupName(SpritesGroupEnum.water, randomIndex).Sprites[0, 2];
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.water) && (sourceTerraing[x, CTop(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpritesByGroupName(SpritesGroupEnum.water, randomIndex).Sprites[2, 0];
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.water) && (sourceTerraing[x, CBottom(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpritesByGroupName(SpritesGroupEnum.water, randomIndex).Sprites[2, 2];
                                }
                                else
                                if ((sourceTerraing[CLeft(x), y] != (int)terrainType.water))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpritesByGroupName(SpritesGroupEnum.water, randomIndex).Sprites[0, 1];
                                }
                                else
                                if ((sourceTerraing[x, CTop(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpritesByGroupName(SpritesGroupEnum.water, randomIndex).Sprites[1, 0];
                                }
                                else
                                if ((sourceTerraing[x, CBottom(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpritesByGroupName(SpritesGroupEnum.water, randomIndex).Sprites[1, 2];
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.water))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpritesByGroupName(SpritesGroupEnum.water, randomIndex).Sprites[2, 1];
                                }

                                else
                                {
                                    terrain[x, y] = spriteBusiness.GetSpritesByGroupName(SpritesGroupEnum.water, randomIndex).Sprites[1, 1];
                                }
                                break;
                            }
                        case (int)terrainType.land:
                            {
                                //TODO: terrain sprite
                                terrain[x, y] = new PositionDTO()
                                {
                                    x = 0,
                                    y = 0
                                };
                                break;
                            }
                        default:
                            {

                                /*
                                if ((sourceTerraing[CLeft(x), y] != (int)terrainType.forest) && (sourceTerraing[x, CTop(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(groupsModel.GetGroup(SpritesGroupEnum.forest), 0, 0, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CLeft(x), y] != (int)terrainType.forest) && (sourceTerraing[x, CBottom(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(groupsModel.GetGroup(SpritesGroupEnum.forest), 0, 2, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.forest) && (sourceTerraing[x, CTop(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(groupsModel.GetGroup(SpritesGroupEnum.forest), 2, 0, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.forest) && (sourceTerraing[x, CBottom(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(groupsModel.GetGroup(SpritesGroupEnum.forest), 2, 2, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CLeft(x), y] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(groupsModel.GetGroup(SpritesGroupEnum.forest), 0, 1, randomIndex);
                                }
                                else
                                if ((sourceTerraing[x, CTop(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(groupsModel.GetGroup(SpritesGroupEnum.forest), 1, 0, randomIndex);
                                }
                                else
                                if ((sourceTerraing[x, CBottom(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(groupsModel.GetGroup(SpritesGroupEnum.forest), 1, 2, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(groupsModel.GetGroup(SpritesGroupEnum.forest), 2, 1, randomIndex);
                                }

                                else
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(groupsModel.GetGroup(SpritesGroupEnum.forest), 1, 1, randomIndex);
                                }
                                */
                            }
                            break;
                    }

                    if (terrain[x,y] ==null)
                    {
                        terrain[x, y] = new PositionDTO()
                        {
                            x = 0,
                            y = 0
                        };

                    }
                    //PutImage(x, y, terrain[x, y] >> 0x10, terrain[x, y] & 0xFF);
                    Int32Rect rect = new Int32Rect(x * 16, y * 16, 16, 16);
                    bitmapSource.WritePixels(rect, SpriteRepository.GetPixels(terrain[x, y].x, terrain[x, y].y), 16, 0);
                }
            }

        }
    }
}
