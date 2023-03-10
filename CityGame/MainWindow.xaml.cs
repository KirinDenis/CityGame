using CityGame.DTOs;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using CityGame.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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

        private double startScrollSpeed = 0.05f;

        private bool lockScroll = false;

        private CityGameEngine cityGameEngine;

        private int zoom = 2;

        int count = 0;
        DateTime enter;
        private int animationFrame = 0;

        public ObjectType selectedType = ObjectType.rail;

        public MainWindow()
        {
            InitializeComponent();

            enter = DateTime.Now;

            //new ResourceExplorer().Show();

            cityGameEngine = new CityGameEngine("new city", 100);
            cityGameEngine.RenderCompleted += CityGameEngine_RenderCompleted;


            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();

            //WaterLevelTextBox.Text = cityGameEngine.waterLevel.ToString();
            //RoughnessTextBox.Text = cityGameEngine.roughness.ToString();

            TerrainImage.Width = TerrainImage.Height = cityGameEngine.GetTerrainSize() * SpriteRepository.SizeInPixels * zoom;
            
            TerrainScroll.ScrollToVerticalOffset(TerrainImage.Width / 2.0f);
            TerrainScroll.ScrollToHorizontalOffset(TerrainImage.Height / 2.0f);

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


        private void Timer_Tick(object? sender, EventArgs e)
        {

            //random.NextBytes(pixels);

            /*
            byte[] array = new byte[16 * 16];

            
            for (int i = 0; i < 5000; i++)
            {
                Int32Rect rect = new Int32Rect(0, 0, 16, 16);
                BitmapImage sImage = ResourcesManager.GetSprite(random.Next(20), random.Next(20));

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

                    //BitmapImage sImage = ResourcesManager.GetSprite(random.Next(32), random.Next(32));

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
            List<SpriteItemModel>? spriteItemModels = spriteBusiness.GetSpriteByGroupName("Industrial1");
            if ((spriteItemModels != null) && (spriteItemModels.Count > 0))
            {
                List<SpriteItemModel>? nextFrameSprites = spriteItemModels?.FindAll(p => p.animationFrame == 0);
                foreach (var spriteItemModel in nextFrameSprites)
                {
                    if (spriteItemModel.groupPosition != null)
                    {
                        for (int x = 0; x < terrainSize - 10; x += 9)
                        {
                            for (int y = 0; y < terrainSize - 10; y += 9)
                            {
                                PutImage(spriteItemModel.groupPosition.x + x, spriteItemModel.groupPosition.y + y, spriteItemModel.position.x, spriteItemModel.position.y);
                            }
                        }
                    }
                }

                nextFrameSprites = spriteItemModels?.FindAll(p => p.animationFrame == animationFrame);

                if ((nextFrameSprites == null) || (nextFrameSprites?.Count == 0))
                {
                    animationFrame = 1;
                    nextFrameSprites = spriteItemModels?.FindAll(p => p.animationFrame == animationFrame);
                }
                if ((nextFrameSprites != null) || (nextFrameSprites?.Count == 0))
                {
                    foreach (var spriteItemModel in nextFrameSprites)
                    {
                        if (spriteItemModel.groupPosition != null)
                        {
                            for (int x = 0; x < terrainSize - 10; x += 9)
                            {
                                for (int y = 0; y < terrainSize - 10; y += 9)
                                {
                                    PutImage(spriteItemModel.groupPosition.x + x, spriteItemModel.groupPosition.y + y, spriteItemModel.position.x, spriteItemModel.position.y);
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



        private void ResourceExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            new ResourceExplorer().Show();
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

            if ((cityGameEngine.GetTerrainSize() * SpriteRepository.SizeInPixels * zoom > TerrainGrid.ActualWidth)
                &&
                ((cityGameEngine.GetTerrainSize() * SpriteRepository.SizeInPixels * zoom > TerrainGrid.ActualHeight)))
            {

                TerrainImage.Width = TerrainImage.Height = cityGameEngine.GetTerrainSize() * SpriteRepository.SizeInPixels * zoom;
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


            double actualSpriteSizeInPixels = TerrainImage.ActualWidth / cityGameEngine.GetTerrainSize();

            double x = e.GetPosition(TerrainImage).X - (e.GetPosition(TerrainImage).X % actualSpriteSizeInPixels); // + TerrainScroll.HorizontalOffset;
            double y = e.GetPosition(TerrainImage).Y - (e.GetPosition(TerrainImage).Y % actualSpriteSizeInPixels); // + TerrainScroll.VerticalOffset;

            TerrainSelector.Width = TerrainSelector.Height = actualSpriteSizeInPixels;

            TerrainSelector.Margin = new Thickness(x, y, 0, 0);

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
                    case Key.R: new ResourceExplorer().Show(); break;
                    case Key.T: cityGameEngine.GenerateTerrain(); break;
                }            
            }

            switch (e.Key)
            {
                case Key.F2: new ResourceExplorer().Show(); break;            
            }


        }
    }
}
