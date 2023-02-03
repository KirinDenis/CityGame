using CityGame.DTOs;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using CityGame.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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


        private DrawingVisual drawingVisual = new DrawingVisual();

        private int scrollBorder = 100;

        private double startScrollSpeed = 0.1f;

        private bool lockScroll = false;

        private CityGameEngine cityGameEngine;

        private double zoom = 2;

        int count = 0;
        DateTime enter;
        private int animationFrame = 0;

        public ObjectType selectedType = ObjectType.rail;

        public MainWindow()
        {
            InitializeComponent();

            enter = DateTime.Now;

            if (SpriteRepository.ResourceInfo == null)
            {
                return;
            }

            cityGameEngine = new CityGameEngine("new city", 100);
            cityGameEngine.RenderCompleted += CityGameEngine_RenderCompleted;

            //WaterLevelTextBox.Text = cityGameEngine.waterLevel.ToString();
            //RoughnessTextBox.Text = cityGameEngine.roughness.ToString();

            TerrainImage.Width = TerrainImage.Height = cityGameEngine.GetTerrainSize() * SpriteRepository.ResourceInfo.SpriteSize * zoom;
            
            TerrainScroll.ScrollToVerticalOffset(TerrainImage.Width / 2.0f);
            TerrainScroll.ScrollToHorizontalOffset(TerrainImage.Height / 2.0f);

            /*
            this.Loaded += delegate
            {
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Elapsed += delegate
                {
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        Mouse.Capture(this);
                        Point pointToWindow = Mouse.GetPosition(this);
                        Point pointToScreen = PointToScreen(pointToWindow);

                        scrollBorder = (int)(TerrainGrid.ActualWidth + TerrainGrid.ActualHeight) / 20;

                        //prior is x
                        if (pointToScreen.X > this.Width + this.Left - scrollBorder)
                        {
                            double xLambda = (scrollBorder / (this.Width + this.Left - pointToScreen.X));
                            TerrainScroll.ScrollToHorizontalOffset(TerrainScroll.HorizontalOffset + xLambda);
                        }
                        else
                        if (pointToScreen.X < this.Left)
                        {
                            double xLambda = (scrollBorder / (pointToScreen.X)) ;
                            TerrainScroll.ScrollToHorizontalOffset(TerrainScroll.HorizontalOffset - xLambda);
                        }

                        if (pointToScreen.Y > TerrainGrid.ActualHeight - scrollBorder)
                        {
                            double yLambda = (scrollBorder / (TerrainGrid.ActualHeight - pointToScreen.Y)) * startScrollSpeed;
                            TerrainScroll.ScrollToVerticalOffset(TerrainScroll.VerticalOffset + yLambda);
                        }
                        else
                        if (pointToScreen.Y < scrollBorder)
                        {
                            double yLambda = (scrollBorder / (pointToScreen.Y)) * startScrollSpeed;
                            TerrainScroll.ScrollToVerticalOffset(TerrainScroll.VerticalOffset - yLambda);
                        }


                        Mouse.Capture(null);
                    }));
                };
                timer.Interval = 1;
                timer.Start();
            };
            */
        }

        private void CityGameEngine_RenderCompleted(object? sender, EventArgs e)
        {
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(cityGameEngine.GetTerrainBitmap(), new Rect(0, 0, cityGameEngine.GetTerrainSize(), cityGameEngine.GetTerrainSize()));
                drawingContext.Close();
            }
            TerrainImage.Source = new DrawingImage(drawingVisual.Drawing);

        }

        private void ResourceExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            new ResourceExplorerWindow().Show();
        }

        private void GenerateMapButton_Click(object sender, RoutedEventArgs e)
        {
            cityGameEngine.GenerateTerrain();
        }

        private void GenerateLandButton_Click(object sender, RoutedEventArgs e)
        {
            cityGameEngine.GenerateTerrain();
        }


        private void SetWaterLevelButton_Click(object sender, RoutedEventArgs e)
        {
            int wl;
            if (int.TryParse(WaterLevelTextBox.Text, out wl))
            {
                //cityGameEngine.waterLevel = wl;
                //cityGameEngine.GenerateNewTerrain();
            }
            else
            {
                //WaterLevelTextBox.Text = cityGameEngine.waterLevel.ToString();
            }
        }

        private void SetRoughnessButton_Click(object sender, RoutedEventArgs e)
        {
            int r;
            if (int.TryParse(RoughnessTextBox.Text, out r))
            {
                //cityGameEngine.roughness = r;
                //cityGameEngine.GenerateNewTerrain();
            }
            else
            {
                //RoughnessTextBox.Text = cityGameEngine.roughness.ToString();
            }
        }

        private void Terrain_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (zoom < 4)
                {
                    zoom+= 0.1;
                }
            }
            else
            {
                if (zoom > 0)
                {
                    zoom-= 0.1;
                }
            }

            if ((cityGameEngine.GetTerrainSize() * SpriteRepository.ResourceInfo.SpriteSize * zoom > TerrainGrid.ActualWidth)
                &&
                ((cityGameEngine.GetTerrainSize() * SpriteRepository.ResourceInfo.SpriteSize * zoom > TerrainGrid.ActualHeight)))
            {
                 Point mousePosition = e.GetPosition((Grid)sender);

                //  TerrainScroll.ScrollToHorizontalOffset(TerrainScroll.HorizontalOffset + xLambda);


                double saveImageSize = TerrainImage.Width;
                TerrainImage.Width = TerrainImage.Height = cityGameEngine.GetTerrainSize() * SpriteRepository.ResourceInfo.SpriteSize * zoom;
                double imageLamda = saveImageSize - TerrainImage.Width;

                double xLambda = (mousePosition.X - this.ActualWidth / 2.0) * (this.ActualWidth / TerrainImage.ActualWidth);
                double yLambda = (mousePosition.Y - this.ActualHeight / 2.0) * (this.ActualHeight / TerrainImage.ActualHeight);


                TerrainScroll.ScrollToHorizontalOffset(TerrainScroll.HorizontalOffset - imageLamda / 2.0  + xLambda / 2.0);
                TerrainScroll.ScrollToVerticalOffset(TerrainScroll.VerticalOffset - imageLamda / 2.0 + yLambda / 2.0);

                Debug.WriteLine("XL:" + xLambda);
                Debug.WriteLine("IL:" + imageLamda / 2);
                Debug.WriteLine("SBH:" + TerrainScroll.HorizontalOffset);

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

                if ((mousePosition.X <= 0) || (mousePosition.Y <= 0))
                {
                    lockScroll = false;
                    return;
                }

                scrollBorder = (int)(this.ActualWidth + this.ActualHeight) / 10;

                //prior is x
                if (mousePosition.X > this.ActualWidth - scrollBorder)
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

                if (mousePosition.Y > this.ActualHeight - scrollBorder)
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


            double actualSpriteSizeInPixels = TerrainImage.ActualWidth / cityGameEngine.GetTerrainSize();

            double x = e.GetPosition(TerrainImage).X - (e.GetPosition(TerrainImage).X % actualSpriteSizeInPixels); // + TerrainScroll.HorizontalOffset;
            double y = e.GetPosition(TerrainImage).Y - (e.GetPosition(TerrainImage).Y % actualSpriteSizeInPixels); // + TerrainScroll.VerticalOffset;

            TerrainSelector.Width = TerrainSelector.Height = actualSpriteSizeInPixels;

            if ((x < TerrainImage.ActualWidth) && (y < TerrainImage.ActualHeight))
            {
                TerrainSelector.Margin = new Thickness(x, y, 0, 0);
            }

        }


        private void TerrainGrid_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            double actualSpriteSizeInPixels = TerrainImage.ActualWidth / cityGameEngine.GetTerrainSize();

            ushort x = (ushort)((e.GetPosition(TerrainImage).X - (e.GetPosition(TerrainImage).X % actualSpriteSizeInPixels)) / actualSpriteSizeInPixels);
            ushort y = (ushort)((e.GetPosition(TerrainImage).Y - (e.GetPosition(TerrainImage).Y % actualSpriteSizeInPixels)) / actualSpriteSizeInPixels);

            cityGameEngine.PutObject(x, y, selectedType);
        }

        private void RoadButton_Click(object sender, RoutedEventArgs e)
        {
            selectedType = ObjectType.road;
        }

        private void RailButton_Click(object sender, RoutedEventArgs e)
        {
            selectedType = ObjectType.rail;
        }

        private void WireButton_Click(object sender, RoutedEventArgs e)
        {
            selectedType = ObjectType.wire;
        }

        private void ResidentButton_Click(object sender, RoutedEventArgs e)
        {
            selectedType = ObjectType.resident;
        }

        private void IndustrialButton_Click(object sender, RoutedEventArgs e)
        {
            selectedType = ObjectType.industrial;
        }

        private void Grid_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            
        }

        private void PoliceDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            selectedType = ObjectType.policeDepartment;
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
              switch (e.Key)
                {
                    case Key.R: new ResourceExplorerWindow().Show(); break;
                    case Key.T: cityGameEngine.GenerateTerrain(); break;
                }            
            }

            switch (e.Key)
            {
                case Key.F2: new ResourceExplorerWindow().Show(); break;
                case Key.F5: cityGameEngine.GenerateTerrain(); break;
            }


        }
    }
}
