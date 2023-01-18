using CityGame.DataModels;
using CityGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CityGame.Models
{
    public class CityGameEngine
    {
        /// <summary>
        /// Terrain width and height at sprites
        /// Current sprite size is 16x16 piexels
        /// The terrain size at pixels is terrainSize * 16
        /// </summary>
        public int terrainSize = 400;

        /// <summary>
        /// If true, the Diamond generator is not called, the terrain is simple random values 2D array 
        /// The fast way of debuging all not related to terrain
        /// </summary>
        private const bool fackeDiomand = false;

        //Terrain map - sprite offsets
        private int[,] terrain; 

        public int waterLevel = 120;

        public int landLevel = 130; //upper this is forest        

        public int roughness = 0;

        private SpriteBusiness spriteBusiness = new SpriteBusiness();

        private Random random = new Random();

        public WriteableBitmap bitmapSource;
       
        private const int left = -1;
        private const int top = -1;

        private const int center = 0;

        private const int right = 1;
        private const int bottom = 1;

        /// <summary>
        /// Inline Left, Top
        /// </summary>
        Func<int, int> CLeft = delegate (int x)
        {
            return --x;
        };

        Func<int, int> CTop = delegate (int y)
        {
            return --y;
        };

        /// <summary>
        /// Inline Right, Bottom
        /// </summary>
        Func<int, int> CRight = delegate (int x)
        {
            return ++x;
        };

        Func<int, int> CBottom = delegate (int y)
        {
            return ++y;
        };

        private List<SpriteModel>? waterGroup;
        private List<SpriteModel>? forestGroup;
        private List<SpriteModel>? roadGroup;

        Func<int, int, int> CoordToOffset = delegate (int x, int y)
        {
            return (x << 0x10) + y;
        };

        Func<int, int> OffsetToX = delegate (int flat)
        {
            return flat >> 0x10;
        };

        Func<int, int> OffsetToY = delegate (int flat)
        {
            return flat & 0xFF;
        };

        public event EventHandler RenderCompleted;


        public CityGameEngine(string cityName, int width = 400, int height = 400)
        {
            terrain = new int[terrainSize, terrainSize];

            BitmapImage sImage = SpriteRepository.GetSprite(0, 0);
            bitmapSource = new WriteableBitmap(terrainSize * 16, terrainSize * 16, sImage.DpiX, sImage.DpiY, sImage.Format, sImage.Palette);
           
            waterGroup = spriteBusiness.GetSpriteByGroupName("water");
            forestGroup = spriteBusiness.GetSpriteByGroupName("forest");
            roadGroup = spriteBusiness.GetSpriteByGroupName("road");

            GenerateNewTerrain();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick; ;
            timer.Start();


        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            OnRenderCompleted(EventArgs.Empty);
        }

        protected virtual void OnRenderCompleted(EventArgs e)
        {
            RenderCompleted?.Invoke(this, e);
        }

        private void PutImage(int x, int y, int bx, int by)
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

                    switch (sourceTerraing[x, y])
                    {
                        case (int)terrainType.water:
                            {
                                if ((sourceTerraing[CLeft(x), y] != (int)terrainType.water) && (sourceTerraing[x, CTop(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(waterGroup, 0, 0, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CLeft(x), y] != (int)terrainType.water) && (sourceTerraing[x, CBottom(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(waterGroup, 0, 2, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.water) && (sourceTerraing[x, CTop(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(waterGroup, 2, 0, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.water) && (sourceTerraing[x, CBottom(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(waterGroup, 2, 2, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CLeft(x), y] != (int)terrainType.water))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(waterGroup, 0, 1, randomIndex);
                                }
                                else
                                if ((sourceTerraing[x, CTop(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(waterGroup, 1, 0, randomIndex);
                                }
                                else
                                if ((sourceTerraing[x, CBottom(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(waterGroup, 1, 2, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.water))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(waterGroup, 2, 1, randomIndex);
                                }

                                else
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(waterGroup, 1, 1, randomIndex);
                                }
                                break;
                            }
                        case (int)terrainType.land:
                            {
                                terrain[x, y] = 0;
                                break;
                            }
                        default:
                            {
                                if ((sourceTerraing[CLeft(x), y] != (int)terrainType.forest) && (sourceTerraing[x, CTop(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(forestGroup, 0, 0, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CLeft(x), y] != (int)terrainType.forest) && (sourceTerraing[x, CBottom(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(forestGroup, 0, 2, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.forest) && (sourceTerraing[x, CTop(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(forestGroup, 2, 0, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.forest) && (sourceTerraing[x, CBottom(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(forestGroup, 2, 2, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CLeft(x), y] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(forestGroup, 0, 1, randomIndex);
                                }
                                else
                                if ((sourceTerraing[x, CTop(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(forestGroup, 1, 0, randomIndex);
                                }
                                else
                                if ((sourceTerraing[x, CBottom(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(forestGroup, 1, 2, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(forestGroup, 2, 1, randomIndex);
                                }

                                else
                                {
                                    terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(forestGroup, 1, 1, randomIndex);
                                }
                            }
                            break;
                    }

                    //PutImage(x, y, terrain[x, y] >> 0x10, terrain[x, y] & 0xFF);
                    Int32Rect rect = new Int32Rect(x * 16, y * 16, 16, 16);
                    bitmapSource.WritePixels(rect, SpriteRepository.GetPixels(terrain[x, y] >> 0x10, terrain[x, y] & 0xFF), 16, 0);
                }
            }


            /*
            //Draw map
            Rectangle rect = new Rectangle();
            rect.Margin = new Thickness(x * 5, y * 5, 0, 0);
            rect.Width = 5;
            rect.Height = 5;

            switch (terrain[x, y])
            {
                case terrainType.water: rect.Fill = new SolidColorBrush(Colors.Blue); break;
                case terrainType.land: rect.Fill = new SolidColorBrush(Colors.Brown); break;
                default: rect.Fill = new SolidColorBrush(Colors.Green); break;
            }
            Canvas.Children.Add(rect);
            */
        }

        public SpriteModel[,] BuildRoadSection(int x, int y)
        {
            int[,] offsets = new int[3, 3];
            int ox = 0;
            for (int tx = CLeft(x); tx < CRight(x) + 1; tx++, ox++)
            {
                int oy = 0;
                for (int ty = CLeft(y); ty < CRight(y) + 1; ty++, oy++)
                {
                    offsets[ox, oy] = terrain[tx, ty];
                }
            }

            SpriteModel[,] sprites = spriteBusiness.GetSpritesByOffsets(offsets);

            int? roadId = spriteBusiness.GetGroupId("road");

            terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(roadGroup, 4, 0);

            int l = 0;
            int c = 1;
            int r = 2;
            int t = 0;
            int b = 2;


            //Central cross of 4 roads
            if ((sprites[c, t].groupId & sprites[c, b].groupId & sprites[l, c].groupId & sprites[r, c].groupId) == roadId)
            {
                terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(roadGroup, 1, 1);
            }
            //Left Right Top cross of 3 roads  
            else
            if ((sprites[c, t].groupId & sprites[l, c].groupId & sprites[r, c].groupId) == roadId)
            {
                terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(roadGroup, 1, 2);
            }
            //Left Right Bottom cross of 3 roads 
            else
            if ((sprites[c, b].groupId & sprites[l, c].groupId & sprites[r, c].groupId) == roadId)
            {
                terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(roadGroup, 1, 0);
            }
            //Left Top Bottom cross of 3 roads 
            else
            if ((sprites[c, t].groupId & sprites[c, b].groupId & sprites[l, c].groupId) == roadId)
            {
                terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(roadGroup, 2, 1);
            }
            //Right Top Bottom cross of 3 roads 
            else
            if ((sprites[c, t].groupId & sprites[c, b].groupId & sprites[r, c].groupId) == roadId)
            {
                terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(roadGroup, 0, 1);
            }
            //Right Top turn of 2 roads 
            else
            if ((sprites[c, b].groupId & sprites[r, c].groupId) == roadId)
            {
                terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(roadGroup, 0, 0);
            }
            //Left Top turn of 2 roads 
            else
            if ((sprites[c, b].groupId & sprites[l, c].groupId) == roadId)
            {
                terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(roadGroup, 2, 0);
            }
            //Right Bottom turn of 2 roads 
            else
            if ((sprites[c, t].groupId & sprites[r, c].groupId) == roadId)
            {
                terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(roadGroup, 0, 2);
            }
            //Left Bottom turn of 2 roads 
            else
            if ((sprites[c, t].groupId & sprites[l, c].groupId) == roadId)
            {
                terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(roadGroup, 2, 2);
            }
            //Horisontal road
            else
            if ((sprites[r, c].groupId == roadId) || (sprites[l, c].groupId == roadId))
            {
                terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(roadGroup, 3, 0);
            }
            //Vertical road
            else
            if ((sprites[c, t].groupId == roadId) || (sprites[c, b].groupId == roadId))
            {
                terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(roadGroup, 4, 0);
            }
            //else default single road

            PutImage(x, y, OffsetToX(terrain[x, y]), OffsetToY(terrain[x, y]));

            return sprites;
        }

        public void BuildRoad(int x, int y)
        {
            SpriteModel[,] sprites = BuildRoadSection(x, y);
            int? roadId = spriteBusiness.GetGroupId("road");

            //Rebuild near roads            
            int ox = 0;
            for (int tx = CLeft(x); tx < CRight(x) + 1; tx++, ox++)
            {
                int oy = 0;
                for (int ty = CLeft(y); ty < CRight(y) + 1; ty++, oy++)
                {
                    if (sprites[ox, oy].groupId == roadId)
                    {
                        BuildRoadSection(tx, ty);
                    }
                }
            }

        }


    }
}
