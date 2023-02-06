﻿using CityGame.Data.DTO;
using CityGame.Data.Enum;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using System;
using System.Collections.Generic;
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

            if (SpriteRepository.ResourceInfo == null)
            {
                return;
            }

            this.terrainSize = terrainSize;

            terrain = new PositionDTO[terrainSize, terrainSize];


            BitmapImage sImage = SpriteRepository.GetSprite(0, 0);
            bitmapSource = new WriteableBitmap(terrainSize * 16, terrainSize * 16, sImage.DpiX, sImage.DpiY, sImage.Format, sImage.Palette);


            GenerateNewTerrain();

        }

        public void PutImage(int x, int y, ushort? bx, ushort? by)
        {
            Int32Rect rect = new Int32Rect(x * 16, y * 16, 16, 16);
            bitmapSource.WritePixels(rect, SpriteRepository.GetPixels((int)bx, (int)by), 16 * 4, 0);
        }

        public ObjectType[,] TestPosition(GroupDTO? group, int x, int y)
        {
            if (group == null)
            {
                return null;
            }

            ObjectType[,] result = new ObjectType[group.Width, group.Height];

            int rx, ry;
            rx = 0;
            for (int sx = x; sx < x + group.Width; sx++, rx++)
            {
                ry = 0;
                for (int sy = y; sy < y + group.Height; sy++, ry++)
                {
                    result[rx, ry] = SpritesGroupEnum.GetObjectTypeByGroupName(spriteBusiness.GetGroupBySpritePosition(terrain[sx, sy]).Name);
                }
            }
            return result;
        }

        protected Func<TerrainModel, int, int, int[,], TerrainType, GroupSpritesDTO, GroupSpritesDTO, bool> PutTerrainBlock = delegate (TerrainModel terrainModel, int x, int y, int[,] sourceTerraing, TerrainType groupType, GroupSpritesDTO groupSprites, GroupSpritesDTO borderSprites)
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
            for (int x = 0; x < terrainSize; x++)
            {
                randomIndex = randomIndex != 0 ? (byte)0 : (byte)1;
                for (int y = 0; y < terrainSize; y++)
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
                        Int32Rect rect = new Int32Rect(x * 16, y * 16, 16, 16);

                        bitmapSource.WritePixels(rect, SpriteRepository.GetPixels(terrain[x, y]), 16 * 4, 0);
                    }
                }
            }

        }
    }
}