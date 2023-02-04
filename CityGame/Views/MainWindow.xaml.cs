using CityGame.Data.DTO;
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

        private double startScrollSpeed = 0.02f;

        private bool lockScroll = false;

        private CityGameEngine cityGameEngine;

        private double zoom = 2;

        int count = 0;
        DateTime enter;
        private int animationFrame = 0;

        public GroupDTO selectedGroup = null;

        private SpriteBusiness spriteBusiness = new SpriteBusiness();

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

            foreach(GroupDTO group in spriteBusiness.groups)
            {
                ObjectsListBox.Items.Add(new ListBoxItem()
                {
                    Content = group.Name,
                    Tag = group
                });
            }

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
                    zoom+= 1;
                }
            }
            else
            {
                if (zoom > 0)
                {
                    zoom-= 1;
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

                scrollBorder = (int)(this.ActualWidth + this.ActualHeight) / 30;

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

            cityGameEngine.PutObject(x, y, selectedGroup);
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


        private void ObjectsListBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = ObjectsListBox.SelectedItem as ListBoxItem;
            selectedGroup = item.Tag as GroupDTO;

        }
    }
}
