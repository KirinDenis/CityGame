using CityGame.DataModels;
using CityGame.Graphics;
using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
        water = 0,
        land = 1,
        forest = 2
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int terrainSize = 100;
        
        //Terrain map
        //0 - water level
        //1 - land
        //2 - forest
        private terrainType[,] terrain = new terrainType[terrainSize, terrainSize];

        private double waterLevel = 0.3;

        private double landLevel = 0.5; //upper this is forest

        private Image[,] images = new Image[terrainSize, terrainSize];

        private int roughness = 0;
        private double seed = 1.0;

        private BlocksManager blocksManager = new BlocksManager();

        public MainWindow()
        {
            InitializeComponent();

            //new ResourceExplorer().Show();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += Timer_Tick;
            // timer.Start();

            WaterLevelTextBox.Text = waterLevel.ToString();
            RoughnessTextBox.Text = roughness.ToString();
            SeedTextBox.Text = seed.ToString();


            DrawDiamand();
            BuildMap();
        }

        private void BuildMap()
        {
            List<BlockItemModel>? waterGroup = blocksManager.GetBlockByGroupName("water");
            byte randomIndex = 0;
            for (int x = 1; x < terrainSize-1; x++)
            {
                randomIndex = randomIndex != 0 ? (byte)0 : (byte)1;
                for (int y = 1; y < terrainSize-1; y++)
                {
                    randomIndex = randomIndex != 0 ? (byte) 0 : (byte)1;
                    switch (terrain[x, y])
                    {
                        case terrainType.water:
                            {
                                if ((terrain[x - 1, y] != terrainType.water) && (terrain[x, y - 1] != terrainType.water))
                                {
                                    List<BlockItemModel>? wBlcoks = blocksManager.GetBlockByGroupPosition(waterGroup, 0, 0);
                                    PutImage(x, y, wBlcoks[randomIndex].position.x, wBlcoks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x - 1, y] != terrainType.water) && (terrain[x, y + 1] != terrainType.water))
                                {
                                    List<BlockItemModel>? wBlcoks = blocksManager.GetBlockByGroupPosition(waterGroup, 0, 2);
                                    PutImage(x, y, wBlcoks[randomIndex].position.x, wBlcoks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x + 1, y] != terrainType.water) && (terrain[x, y - 1] != terrainType.water))
                                {
                                    List<BlockItemModel>? wBlcoks = blocksManager.GetBlockByGroupPosition(waterGroup, 2, 0);
                                    PutImage(x, y, wBlcoks[randomIndex].position.x, wBlcoks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x + 1, y] != terrainType.water) && (terrain[x, y + 1] != terrainType.water))
                                {
                                    List<BlockItemModel>? wBlcoks = blocksManager.GetBlockByGroupPosition(waterGroup, 2, 2);
                                    PutImage(x, y, wBlcoks[randomIndex].position.x, wBlcoks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x - 1, y] != terrainType.water))
                                {
                                    List<BlockItemModel>? wBlcoks = blocksManager.GetBlockByGroupPosition(waterGroup, 0, 1);
                                    
                                    PutImage(x, y, wBlcoks[randomIndex].position.x, wBlcoks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x, y - 1] != terrainType.water))
                                {
                                    List<BlockItemModel>? wBlcoks = blocksManager.GetBlockByGroupPosition(waterGroup, 1, 0);

                                    PutImage(x, y, wBlcoks[randomIndex].position.x, wBlcoks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x, y + 1] != terrainType.water))
                                {
                                    List<BlockItemModel>? wBlcoks = blocksManager.GetBlockByGroupPosition(waterGroup, 1, 2);

                                    PutImage(x, y, wBlcoks[randomIndex].position.x, wBlcoks[randomIndex].position.y);
                                }
                                else
                                if ((terrain[x+1, y] != terrainType.water))
                                {
                                    List<BlockItemModel>? wBlcoks = blocksManager.GetBlockByGroupPosition(waterGroup, 2, 1);

                                    PutImage(x, y, wBlcoks[randomIndex].position.x, wBlcoks[randomIndex].position.y);
                                }

                                else
                                {
                                    List<BlockItemModel>? wBlcoks = blocksManager.GetBlockByGroupPosition(waterGroup, 1, 1);

                                    PutImage(x, y, wBlcoks[randomIndex].position.x, wBlcoks[randomIndex].position.y);
                                }
                                    break;
                            }
                        case terrainType.land:
                            {
                                PutImage(x, y, 0, 0);

                                break;
                            }
                        default: PutImage(x, y, 9, 1); break;
                    }
                }
            }
        }

        private void PutImage(int x, int y, int bx, int by)
        {
            if (images[x, y] == null)
            {
                images[x, y] = new Image();
                images[x, y].Margin = new Thickness(x * 32, y * 32, 0, 0);
                images[x, y].Width = 32;
                images[x, y].Height = 32;
                images[x, y].VerticalAlignment = VerticalAlignment.Top;
                images[x, y].HorizontalAlignment = HorizontalAlignment.Left;

                RenderOptions.SetBitmapScalingMode(images[x, y], BitmapScalingMode.NearestNeighbor);

                LandingImage.Children.Add(images[x, y]);
            }
            images[x, y].Source = ResourcesManager.GetBlock(bx, by);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
        }

        private void DrawDiamand()
        {

            DiamondSquare diamondSquare = new DiamondSquare(terrainSize, 1);
            double[,] sourceTerraing = diamondSquare.getData();

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
            double wl;
            if (double.TryParse(WaterLevelTextBox.Text, out wl))
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

        private void SetSeedButton_Click(object sender, RoutedEventArgs e)
        {
            double s;
            if (double.TryParse(SeedTextBox.Text, out s))
            {
                seed = s;
                DrawDiamand();
                BuildMap();
            }
            else
            {
                SeedTextBox.Text = seed.ToString();
            }
        }


    }
}
