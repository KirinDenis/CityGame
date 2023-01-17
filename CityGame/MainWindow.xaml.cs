using CityGame.DataModels;
using CityGame.Graphics;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace CityGame
{
    public class Graph
    {
        public Point from;
        public Point to;
        public List<Graph> child = new List<Graph>();
    }

    public enum terrainType : int
    {
        water = 1,
        land = 2,
        forest = 4
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Terrain width and height at blocks
        /// Current block size is 16x16 piexels
        /// The terrain size at pixels is terrainSize * 16
        /// </summary>
        private const int terrainSize = 400;

        /// <summary>
        /// If true, the Diamond generator is not called, the terrain is simple random values 2D array 
        /// The fast way of debuging all not related to terrain
        /// </summary>
        private const bool fackeDiomand = false;

        //Terrain map - block offsets
        private int[,] terrain = new int[terrainSize, terrainSize];

        private int waterLevel = 120;

        private int landLevel = 130; //upper this is forest        

        private int roughness = 0;

        private BlocksManager blocksManager = new BlocksManager();

        private Random random = new Random();

        private WriteableBitmap bitmapSource;

        private DrawingVisual drawingVisual = new DrawingVisual();

        private int scrollBorder = 100;

        private double startScrollSpeed = 0.05f;

        private bool lockScroll = false;


        int count = 0;
        DateTime enter;
        private int animationFrame = 0;

        private int zoom = 2;

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

        private List<BlockItemModel>? waterGroup;
        private List<BlockItemModel>? forestGroup;
        private List<BlockItemModel>? roadGroup;

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

        public MainWindow()
        {
            InitializeComponent();

            //new ResourceExplorer().Show();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();

            DispatcherTimer rtimer = new DispatcherTimer();
            rtimer.Interval = TimeSpan.FromMilliseconds(100);
            rtimer.Tick += Rtimer_Tick;
            rtimer.Start();


            WaterLevelTextBox.Text = waterLevel.ToString();
            RoughnessTextBox.Text = roughness.ToString();

            TerrainImage.Width = TerrainImage.Height = terrainSize * ResourcesManager.iconsSizeInPixels * zoom;

            BitmapImage sImage = ResourcesManager.GetBlock(0, 0);
            bitmapSource = new WriteableBitmap(terrainSize * 16, terrainSize * 16, sImage.DpiX, sImage.DpiY, sImage.Format, sImage.Palette);

            enter = DateTime.Now;

            waterGroup = blocksManager.GetBlockByGroupName("water");
            forestGroup = blocksManager.GetBlockByGroupName("forest");
            roadGroup = blocksManager.GetBlockByGroupName("road");


            GenerateNewTerrain();

            TerrainScroll.ScrollToVerticalOffset(TerrainImage.Width / 2.0f);
            TerrainScroll.ScrollToHorizontalOffset(TerrainImage.Height / 2.0f);

        }


        private void Rtimer_Tick(object? sender, EventArgs e)
        {
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(bitmapSource, new Rect(0, 0, terrainSize, terrainSize));
                drawingContext.Close();
            }
            TerrainImage.Source = new DrawingImage(drawingVisual.Drawing);
            // TerrainImage.Margin = new Thickness(TerrainImage.Margin.Left - 1, TerrainImage.Margin.Top, 0, 0);
            ((DispatcherTimer)sender).Stop();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {

            //random.NextBytes(pixels);

            /*
            byte[] array = new byte[16 * 16];

            
            for (int i = 0; i < 5000; i++)
            {
                Int32Rect rect = new Int32Rect(0, 0, 16, 16);
                BitmapImage sImage = ResourcesManager.GetBlock(random.Next(20), random.Next(20));

                sImage.CopyPixels(rect, array, 16, 0);
                
                rect.X = random.Next(terrainSize / 16) * 16;
                rect.Y = random.Next(terrainSize / 16) * 16;
                bitmapSource.WritePixels(rect, array, 16, 0);

                //bitmapSource.WritePixels(new Int32Rect(0, 0, terrainSize, terrainSize), pixels, terrainSize, 0);

            }
            */

            /*
            Int32Rect rect = new Int32Rect(0, 0, 16, 16);
            for (int x = 0; x < terrainSize - 16; x += 16)
            {
                for (int y = 0; y < terrainSize - 16; y += 16)
                {
                    //rect.X = 0;
                    //rect.Y = 0;

                    //BitmapImage sImage = ResourcesManager.GetBlock(random.Next(32), random.Next(32));

                    //sImage.CopyPixels(rect, array, 16, 0);

                    rect.X = x;
                    rect.Y = y;
                    bitmapSource.WritePixels(rect, ResourcesManager.GetPixels(random.Next(32), random.Next(32)), 16, 0);
                }

                //bitmapSource.WritePixels(new Int32Rect(0, 0, terrainSize, terrainSize), pixels, terrainSize, 0);

            }
            */

            /*
            animationFrame++;
            List<BlockItemModel>? blockItemModels = blocksManager.GetBlockByGroupName("Industrial1");
            if ((blockItemModels != null) && (blockItemModels.Count > 0))
            {
                List<BlockItemModel>? nextFrameBlocks = blockItemModels?.FindAll(p => p.animationFrame == 0);
                foreach (var blockItemModel in nextFrameBlocks)
                {
                    if (blockItemModel.groupPosition != null)
                    {
                        for (int x = 0; x < terrainSize - 10; x += 9)
                        {
                            for (int y = 0; y < terrainSize - 10; y += 9)
                            {
                                PutImage(blockItemModel.groupPosition.x + x, blockItemModel.groupPosition.y + y, blockItemModel.position.x, blockItemModel.position.y);
                            }
                        }
                    }
                }

                nextFrameBlocks = blockItemModels?.FindAll(p => p.animationFrame == animationFrame);

                if ((nextFrameBlocks == null) || (nextFrameBlocks?.Count == 0))
                {
                    animationFrame = 1;
                    nextFrameBlocks = blockItemModels?.FindAll(p => p.animationFrame == animationFrame);
                }
                if ((nextFrameBlocks != null) || (nextFrameBlocks?.Count == 0))
                {
                    foreach (var blockItemModel in nextFrameBlocks)
                    {
                        if (blockItemModel.groupPosition != null)
                        {
                            for (int x = 0; x < terrainSize - 10; x += 9)
                            {
                                for (int y = 0; y < terrainSize - 10; y += 9)
                                {
                                    PutImage(blockItemModel.groupPosition.x + x, blockItemModel.groupPosition.y + y, blockItemModel.position.x, blockItemModel.position.y);
                                }
                            }
                        }
                    }
                }
            }
            */
            count++;
            Title = (count / (DateTime.Now - enter).TotalSeconds) + " FPS";
        }

        private void PutImage(int x, int y, int bx, int by)
        {
            Int32Rect rect = new Int32Rect(x * 16, y * 16, 16, 16);
            bitmapSource.WritePixels(rect, ResourcesManager.GetPixels(bx, by), 16, 0);
        }


        private void GenerateNewTerrain()
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
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(waterGroup, 0, 0, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CLeft(x), y] != (int)terrainType.water) && (sourceTerraing[x, CBottom(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(waterGroup, 0, 2, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.water) && (sourceTerraing[x, CTop(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(waterGroup, 2, 0, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.water) && (sourceTerraing[x, CBottom(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(waterGroup, 2, 2, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CLeft(x), y] != (int)terrainType.water))
                                {
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(waterGroup, 0, 1, randomIndex);
                                }
                                else
                                if ((sourceTerraing[x, CTop(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(waterGroup, 1, 0, randomIndex);
                                }
                                else
                                if ((sourceTerraing[x, CBottom(y)] != (int)terrainType.water))
                                {
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(waterGroup, 1, 2, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.water))
                                {
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(waterGroup, 2, 1, randomIndex);
                                }

                                else
                                {
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(waterGroup, 1, 1, randomIndex);
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
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(forestGroup, 0, 0, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CLeft(x), y] != (int)terrainType.forest) && (sourceTerraing[x, CBottom(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(forestGroup, 0, 2, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.forest) && (sourceTerraing[x, CTop(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(forestGroup, 2, 0, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.forest) && (sourceTerraing[x, CBottom(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(forestGroup, 2, 2, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CLeft(x), y] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(forestGroup, 0, 1, randomIndex);
                                }
                                else
                                if ((sourceTerraing[x, CTop(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(forestGroup, 1, 0, randomIndex);
                                }
                                else
                                if ((sourceTerraing[x, CBottom(y)] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(forestGroup, 1, 2, randomIndex);
                                }
                                else
                                if ((sourceTerraing[CRight(x), y] != (int)terrainType.forest))
                                {
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(forestGroup, 2, 1, randomIndex);
                                }

                                else
                                {
                                    terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(forestGroup, 1, 1, randomIndex);
                                }
                            }
                            break;
                    }
                    
                    //PutImage(x, y, terrain[x, y] >> 0x10, terrain[x, y] & 0xFF);
                    Int32Rect rect = new Int32Rect(x * 16, y * 16, 16, 16);
                    bitmapSource.WritePixels(rect, ResourcesManager.GetPixels(terrain[x, y] >> 0x10, terrain[x, y] & 0xFF), 16, 0);
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

        private void ResourceExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            new ResourceExplorer().Show();
        }

        private void GenerateMapButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateNewTerrain();
        }

        private void GenerateLandButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateNewTerrain();
        }


        private void SetWaterLevelButton_Click(object sender, RoutedEventArgs e)
        {
            int wl;
            if (int.TryParse(WaterLevelTextBox.Text, out wl))
            {
                waterLevel = wl;
                GenerateNewTerrain();
            }
            else
            {
                WaterLevelTextBox.Text = waterLevel.ToString();
            }
        }

        private void SetRoughnessButton_Click(object sender, RoutedEventArgs e)
        {
            int r;
            if (int.TryParse(RoughnessTextBox.Text, out r))
            {
                roughness = r;
                GenerateNewTerrain();
            }
            else
            {
                RoughnessTextBox.Text = roughness.ToString();
            }
        }

        private void Terrain_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (zoom < 4)
                {
                    zoom++;
                }
            }
            else
            {
                if (zoom > 0)
                {
                    zoom--;
                }
            }

            if ((terrainSize * ResourcesManager.iconsSizeInPixels * zoom > TerrainGrid.ActualWidth)
                &&
                ((terrainSize * ResourcesManager.iconsSizeInPixels * zoom > TerrainGrid.ActualHeight)))
            {

                TerrainImage.Width = TerrainImage.Height = terrainSize * ResourcesManager.iconsSizeInPixels * zoom;
            }

            //lock scroll view            
            e.Handled = true;
        }

        private void TerrainGrid_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!lockScroll)
            {
                lockScroll = true;

                Point mousePosition = e.GetPosition((Grid)sender);

                scrollBorder = (int)(TerrainGrid.ActualWidth + TerrainGrid.ActualHeight) / 20;

                //prior is x
                if (mousePosition.X > TerrainGrid.ActualWidth - scrollBorder)
                {
                    double xLambda = (scrollBorder / (TerrainGrid.ActualWidth - mousePosition.X)) * startScrollSpeed;
                    TerrainScroll.ScrollToHorizontalOffset(TerrainScroll.HorizontalOffset + xLambda);
                }
                else
                if (mousePosition.X < scrollBorder)
                {
                    double xLambda = (scrollBorder / (mousePosition.X)) * startScrollSpeed;
                    TerrainScroll.ScrollToHorizontalOffset(TerrainScroll.HorizontalOffset - xLambda);
                }

                if (mousePosition.Y > TerrainGrid.ActualHeight - scrollBorder)
                {
                    double yLambda = (scrollBorder / (TerrainGrid.ActualHeight - mousePosition.Y)) * startScrollSpeed;
                    TerrainScroll.ScrollToVerticalOffset(TerrainScroll.VerticalOffset + yLambda);
                }
                else
                if (mousePosition.Y < scrollBorder)
                {
                    double yLambda = (scrollBorder / (mousePosition.Y)) * startScrollSpeed;
                    TerrainScroll.ScrollToVerticalOffset(TerrainScroll.VerticalOffset - yLambda);
                }
                lockScroll = false;
            }


            double actualIconSizeInPixels = TerrainImage.ActualWidth / terrainSize;

            double x = e.GetPosition(TerrainImage).X - (e.GetPosition(TerrainImage).X % actualIconSizeInPixels); // + TerrainScroll.HorizontalOffset;
            double y = e.GetPosition(TerrainImage).Y - (e.GetPosition(TerrainImage).Y % actualIconSizeInPixels); // + TerrainScroll.VerticalOffset;

            TerrainSelector.Width = TerrainSelector.Height = actualIconSizeInPixels;

            TerrainSelector.Margin = new Thickness(x, y, 0, 0);

        }

        private BlockItemModel[,] BuildRoad(int x, int y)
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

            BlockItemModel[,] blocks = blocksManager.GetBlocksByOffsets(offsets);

            int? roadId = blocksManager.GetGroupId("road");

            terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(roadGroup, 4, 0);

            int l = 0;
            int c = 1;
            int r = 2;
            int t = 0;
            int b = 2;


            //Central cross of 4 roads
            if ((blocks[c, t].groupId & blocks[c, b].groupId & blocks[l, c].groupId & blocks[r, c].groupId) == roadId)
            {
                terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(roadGroup, 1, 1);
            }
            //Left Right Top cross of 3 roads  
            else
            if ((blocks[c, t].groupId & blocks[l, c].groupId & blocks[r, c].groupId) == roadId)
            {
                terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(roadGroup, 1, 2);
            }
            //Left Right Bottom cross of 3 roads 
            else
            if ((blocks[c, b].groupId & blocks[l, c].groupId & blocks[r, c].groupId) == roadId)
            {
                terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(roadGroup, 1, 0);
            }
            //Left Top Bottom cross of 3 roads 
            else
            if ((blocks[c, t].groupId & blocks[c, b].groupId & blocks[l, c].groupId) == roadId)
            {
                terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(roadGroup, 2, 1);
            }
            //Right Top Bottom cross of 3 roads 
            else
            if ((blocks[c, t].groupId & blocks[c, b].groupId & blocks[r, c].groupId) == roadId)
            {
                terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(roadGroup, 0, 1);
            }
            //Right Top turn of 2 roads 
            else
            if ((blocks[c, b].groupId & blocks[r, c].groupId) == roadId)
            {
                terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(roadGroup, 0, 0);
            }
            //Left Top turn of 2 roads 
            else
            if ((blocks[c, b].groupId & blocks[l, c].groupId) == roadId)
            {
                terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(roadGroup, 2, 0);
            }
            //Right Bottom turn of 2 roads 
            else
            if ((blocks[c, t].groupId & blocks[r, c].groupId) == roadId)
            {
                terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(roadGroup, 0, 2);
            }
            //Left Bottom turn of 2 roads 
            else
            if ((blocks[c, t].groupId & blocks[l, c].groupId) == roadId)
            {
                terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(roadGroup, 2, 2);
            }
            //Horisontal road
            else
            if ((blocks[r, c].groupId == roadId) || (blocks[l, c].groupId == roadId))
            {
                terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(roadGroup, 3, 0);
            }
            //Vertical road
            else
            if ((blocks[c, t].groupId == roadId) || (blocks[c, b].groupId == roadId))
            {
                terrain[x, y] = blocksManager.GetBlockOffsetByGroupPosition(roadGroup, 4, 0);
            }
            //else default single road

            PutImage(x, y, OffsetToX(terrain[x, y]), OffsetToY(terrain[x, y]));

            return blocks;
        }

        private void TerrainGrid_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            double actualIconSizeInPixels = TerrainImage.ActualWidth / terrainSize;

            int x = (int)((e.GetPosition(TerrainImage).X - (e.GetPosition(TerrainImage).X % actualIconSizeInPixels)) / actualIconSizeInPixels);
            int y = (int)((e.GetPosition(TerrainImage).Y - (e.GetPosition(TerrainImage).Y % actualIconSizeInPixels)) / actualIconSizeInPixels);

            BlockItemModel[,] blocks = BuildRoad(x, y);
            int? roadId = blocksManager.GetGroupId("road");

            //Rebuild near roads            
            int ox = 0;
            for (int tx = CLeft(x); tx < CRight(x) + 1; tx++, ox++)
            {
                int oy = 0;
                for (int ty = CLeft(y); ty < CRight(y) + 1; ty++, oy++)
                {
                    if (blocks[ox, oy].groupId == roadId)
                    {
                        BuildRoad(tx, ty);
                    }
                }
            }

        }
    }
}
