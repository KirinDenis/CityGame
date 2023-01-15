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

    public enum terrainType
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

        //Terrain map
        //0 - water level
        //1 - land
        //2 - forest
        private terrainType[,] terrain = new terrainType[terrainSize, terrainSize];

        private int waterLevel = 120;

        private int landLevel = 130; //upper this is forest        

        private int roughness = 4;

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

            DrawDiamand();
            BuildMap();

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
            
            count++;
            Title = (count / (DateTime.Now - enter).TotalSeconds) + " FPS";
        }


        private void BuildMap()
        {
            List<BlockItemModel>? waterGroup = blocksManager.GetBlockByGroupName("water");
            List<BlockItemModel>? forestGroup = blocksManager.GetBlockByGroupName("forest");
            byte randomIndex = 0;
            for (int x = 1; x < terrainSize - 1; x++)
            {
                randomIndex = randomIndex != 0 ? (byte)0 : (byte)1;
                for (int y = 1; y < terrainSize - 1; y++)
                {
                    randomIndex = randomIndex != 0 ? (byte)0 : (byte)1;

                    //delete singe 
                    terrainType s = (terrain[x - 1, y - 1] | terrain[x, y - 1] | terrain[x + 1, y - 1] |
                        terrain[x - 1, y] | terrain[x + 1, y] |
                        terrain[x - 1, y + 1] | terrain[x, y + 1] | terrain[x + 1, y + 1]) & terrain[x, y];

                    if (s != terrain[x, y])
                    {
                        terrain[x, y] = terrainType.land;
                    }



                    switch (terrain[x, y])
                    {
                        case terrainType.water:
                            {
                                if ((terrain[x - 1, y] != terrainType.water) && (terrain[x, y - 1] != terrainType.water))
                                {
                                    List<BlockItemModel>? wBlocks = blocksManager.GetBlockByGroupPosition(waterGroup, 0, 0);
                                    PutImage(x, y, wBlocks[randomIndex].position.x, wBlocks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x - 1, y] != terrainType.water) && (terrain[x, y + 1] != terrainType.water))
                                {
                                    List<BlockItemModel>? wBlocks = blocksManager.GetBlockByGroupPosition(waterGroup, 0, 2);
                                    PutImage(x, y, wBlocks[randomIndex].position.x, wBlocks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x + 1, y] != terrainType.water) && (terrain[x, y - 1] != terrainType.water))
                                {
                                    List<BlockItemModel>? wBlocks = blocksManager.GetBlockByGroupPosition(waterGroup, 2, 0);
                                    PutImage(x, y, wBlocks[randomIndex].position.x, wBlocks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x + 1, y] != terrainType.water) && (terrain[x, y + 1] != terrainType.water))
                                {
                                    List<BlockItemModel>? wBlocks = blocksManager.GetBlockByGroupPosition(waterGroup, 2, 2);
                                    PutImage(x, y, wBlocks[randomIndex].position.x, wBlocks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x - 1, y] != terrainType.water))
                                {
                                    List<BlockItemModel>? wBlocks = blocksManager.GetBlockByGroupPosition(waterGroup, 0, 1);

                                    PutImage(x, y, wBlocks[randomIndex].position.x, wBlocks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x, y - 1] != terrainType.water))
                                {
                                    List<BlockItemModel>? wBlocks = blocksManager.GetBlockByGroupPosition(waterGroup, 1, 0);

                                    PutImage(x, y, wBlocks[randomIndex].position.x, wBlocks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x, y + 1] != terrainType.water))
                                {
                                    List<BlockItemModel>? wBlocks = blocksManager.GetBlockByGroupPosition(waterGroup, 1, 2);

                                    PutImage(x, y, wBlocks[randomIndex].position.x, wBlocks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x + 1, y] != terrainType.water))
                                {
                                    List<BlockItemModel>? wBlocks = blocksManager.GetBlockByGroupPosition(waterGroup, 2, 1);

                                    PutImage(x, y, wBlocks[randomIndex].position.x, wBlocks[randomIndex].position.y);
                                }

                                else
                                {
                                    List<BlockItemModel>? wBlocks = blocksManager.GetBlockByGroupPosition(waterGroup, 1, 1);

                                    PutImage(x, y, wBlocks[randomIndex].position.x, wBlocks[randomIndex].position.y);
                                }
                                break;
                            }
                        case terrainType.land:
                            {
                                PutImage(x, y, 0, 0);

                                break;
                            }
                        default:
                            {
                                if ((terrain[x - 1, y] != terrainType.forest) && (terrain[x, y - 1] != terrainType.forest))
                                {
                                    List<BlockItemModel>? fBlocks = blocksManager.GetBlockByGroupPosition(forestGroup, 0, 0);
                                    PutImage(x, y, fBlocks[randomIndex].position.x, fBlocks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x - 1, y] != terrainType.forest) && (terrain[x, y + 1] != terrainType.forest))
                                {
                                    List<BlockItemModel>? fBlocks = blocksManager.GetBlockByGroupPosition(forestGroup, 0, 2);
                                    PutImage(x, y, fBlocks[randomIndex].position.x, fBlocks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x + 1, y] != terrainType.forest) && (terrain[x, y - 1] != terrainType.forest))
                                {
                                    List<BlockItemModel>? fBlocks = blocksManager.GetBlockByGroupPosition(forestGroup, 2, 0);
                                    PutImage(x, y, fBlocks[randomIndex].position.x, fBlocks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x + 1, y] != terrainType.forest) && (terrain[x, y + 1] != terrainType.forest))
                                {
                                    List<BlockItemModel>? fBlocks = blocksManager.GetBlockByGroupPosition(forestGroup, 2, 2);
                                    PutImage(x, y, fBlocks[randomIndex].position.x, fBlocks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x - 1, y] != terrainType.forest))
                                {
                                    List<BlockItemModel>? fBlocks = blocksManager.GetBlockByGroupPosition(forestGroup, 0, 1);

                                    PutImage(x, y, fBlocks[randomIndex].position.x, fBlocks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x, y - 1] != terrainType.forest))
                                {
                                    List<BlockItemModel>? fBlocks = blocksManager.GetBlockByGroupPosition(forestGroup, 1, 0);

                                    PutImage(x, y, fBlocks[randomIndex].position.x, fBlocks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x, y + 1] != terrainType.forest))
                                {
                                    List<BlockItemModel>? fBlocks = blocksManager.GetBlockByGroupPosition(forestGroup, 1, 2);

                                    PutImage(x, y, fBlocks[randomIndex].position.x, fBlocks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x + 1, y] != terrainType.forest))
                                {
                                    List<BlockItemModel>? fBlocks = blocksManager.GetBlockByGroupPosition(forestGroup, 2, 1);

                                    PutImage(x, y, fBlocks[randomIndex].position.x, fBlocks[randomIndex].position.y);
                                }

                                else
                                {
                                    List<BlockItemModel>? fBlocks = blocksManager.GetBlockByGroupPosition(forestGroup, 1, 1);

                                    PutImage(x, y, fBlocks[randomIndex].position.x, fBlocks[randomIndex].position.y);
                                }

                            }
                            break;
                    }
                }
            }
        }

        private void PutImage(int x, int y, int bx, int by)
        {
            Int32Rect rect = new Int32Rect(x * 16, y * 16, 16, 16);
            bitmapSource.WritePixels(rect, ResourcesManager.GetPixels(bx, by), 16, 0);
        }


        private void DrawDiamand()
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
                        sourceTerraing[x, y] = random.Next(255);
                    }
                }
            }

            Canvas.Children.Clear();

            for (int x = 0; x < terrainSize; x++)
            {
                for (int y = 0; y < terrainSize; y++)
                {

                    if (sourceTerraing[x, y] <= waterLevel)
                    {
                        terrain[x, y] = terrainType.water;
                    }
                    else
                    if (sourceTerraing[x, y] <= landLevel)
                    {
                        terrain[x, y] = terrainType.land;
                    }
                    else
                        terrain[x, y] = terrainType.forest;

                    /*
                    if ((x > 5) && (x < 15) && (y > 5) && (y < 15))
                    {
                        terrain[x, y] = terrainType.water;
                    }
                    else
                    {
                        terrain[x, y] = terrainType.land;
                    }
                    */

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
            }
        }

        private void ResourceExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            new ResourceExplorer().Show();
        }

        private void GenerateMapButton_Click(object sender, RoutedEventArgs e)
        {
            BuildMap();
        }

        private void GenerateLandButton_Click(object sender, RoutedEventArgs e)
        {
            DrawDiamand();
            BuildMap();
        }


        private void SetWaterLevelButton_Click(object sender, RoutedEventArgs e)
        {
            int wl;
            if (int.TryParse(WaterLevelTextBox.Text, out wl))
            {
                waterLevel = wl;
                DrawDiamand();
                BuildMap();
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
                DrawDiamand();
                BuildMap();
            }
            else
            {
                RoughnessTextBox.Text = roughness.ToString();
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Activated(object sender, EventArgs e)
        {

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
                lockScroll= true;

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
                    double yLambda = (scrollBorder / ( mousePosition.Y)) * startScrollSpeed;
                    TerrainScroll.ScrollToVerticalOffset(TerrainScroll.VerticalOffset - yLambda);
                }
                lockScroll = false;
            }
        }
    }
}
