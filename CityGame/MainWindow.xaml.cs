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


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int x = 1;
        private int y = 1;
        private Random r = new Random(1000);

        private Image[,] images = new Image[200, 200];
        private BitmapImage[,] icon = new BitmapImage[100, 100];
        public MainWindow()
        {
            InitializeComponent();

            //new ResourceExplorer().Show();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += Timer_Tick;
            // timer.Start();

            BuildMap();
        }

        private void BuildMap()
        {
            for (int _x = 0; _x < 50; _x++)
            {
                for (int _y = 0; _y < 50; _y++)
                {
                    PutImage(_x, _y, 0, 0);
                }
            }

            
            //River Algorithm 1 ---------------------
            double riverDirectionAngle = Math.PI / 2.0f;
            double riverVectorSize = 0;

            double x = 50 / 2; //half of map
            double y = 0;

            double nextX = 0; //half of map
            double nextY = 0;

            List<Point> points = new List<Point>();
            points.Add(new Point(x, y));

            while (true)
            {
                if ((x < 0) || (x > 50) || (y < 0) || (y > 50))
                {
                    break;
                }
                PutImage((int)x, (int)y, 2, 0);
                riverVectorSize = r.NextDouble() * 5;
                nextX = riverVectorSize * Math.Cos(riverDirectionAngle) + x;
                nextY = riverVectorSize * Math.Sin(riverDirectionAngle) + y;
                points.Add(new Point(nextX, nextY));
                //draw 
                double lamdaX = (nextX - x) / (nextY - y);

                double _x = x;
                for (int _y = 0; _y < nextY; _y++)
                {
                    if ((_x < 0) || (_x > 50) || (_y < 0) || (_y > 50))
                    {
                        break;
                    }

                    PutImage((int)_x, _y, 3, 0);
                    _x += lamdaX;
                }



                x = nextX;
                y = nextY;
                riverDirectionAngle += (-0.5f + r.NextDouble()) / 10.0f;
            }
            
            //River algorithm 2 ----------------
            //RiverAlgorithm2();
        }

        private void RiverAlgorithm2()
        {
            List<Graph> graphs = new List<Graph>();
            //first graph 
            Graph first = new Graph();
            first.from = new Point(25, 0);

            double riverDirectionAngle = Math.PI / 2.0f;
            double riverVectorSize = 0;
            riverVectorSize = r.NextDouble() * 5;

            first.to = new Point(riverVectorSize * Math.Cos(riverDirectionAngle) + first.from.X, riverVectorSize * Math.Sin(riverDirectionAngle) + first.from.Y);

            graphs.Add(first);

            Graph lastGrpah = first;
            while (true)
            {
                
                graphs.Add(NextGraph(lastGrpah));
                lastGrpah = graphs[graphs.Count - 1];
                if ((lastGrpah.to.X < 0) || (lastGrpah.to.X > 50) || (lastGrpah.to.Y < 0) || (lastGrpah.to.Y > 50))
                {
                    break;
                }
            }

        }

        private Graph NextGraph(Graph sourceGraph)
        {
            Graph nextGraph = new Graph();
            nextGraph.from = sourceGraph.to;

            double riverDirectionAngle = Math.PI / 2.0f + (-0.5f + r.NextDouble()) / 10.0f;
            double riverVectorSize = 0;
            riverVectorSize = r.NextInt64(5);
            nextGraph.to = new Point(riverVectorSize * Math.Cos(riverDirectionAngle) + nextGraph.from.X, riverVectorSize * Math.Sin(riverDirectionAngle) + nextGraph.from.Y);

            if (r.Next(100) > 80)
            {

            }

            return nextGraph;
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
            for (int i = 0; i < 100 * 100; i++)
            {
                x++;
                if (x > 100)
                {
                    x = 0;
                    y++;
                    if (y > 100)
                    {
                        y = 0;
                    }
                }


                if (images[x, y] == null)
                {
                    images[x, y] = new Image();
                    images[x, y].Margin = new Thickness(x * 32, y * 32, 0, 0);
                    images[x, y].Width = 32;
                    images[x, y].Height = 32;
                    images[x, y].VerticalAlignment = VerticalAlignment.Top;
                    images[x, y].HorizontalAlignment = HorizontalAlignment.Left;
                    images[x, y].Stretch = Stretch.Uniform;


                    LandingImage.Children.Add(images[x, y]);
                }

                int ix = r.Next(40);
                int iy = r.Next(40);
                if (icon[ix, iy] == null)
                {
                    icon[ix, iy] = ResourcesManager.GetBlock(r.Next(20), r.Next(20));
                }
                images[x, y].Source = icon[ix, iy];




            }
            //LandingImage.Source = Icons.GetIcon(x, y);
            //LandingImage.UpdateLayout();


        }

        private void ResourceExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            new ResourceExplorer().Show();
        }

        private void GenerateMapButton_Click(object sender, RoutedEventArgs e)
        {
            BuildMap();
        }
    }
}
