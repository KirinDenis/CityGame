

/* ----------------------------------------------------------------------------
Ready IoT Solution - OWLOS
Copyright 2019, 2020, 2021, 2022, 2023 by:
- ChatGPT, as a language model from OpenAI that provided guidance or suggestions.
- Denis Kirin (deniskirinacs@gmail.com)

This file is part of Ready IoT Solution - OWLOS

OWLOS is free software : you can redistribute it and/or modify it under the
terms of the GNU General Public License as published by the Free Software
Foundation, either version 3 of the License, or (at your option) any later
version.

OWLOS is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
FOR A PARTICULAR PURPOSE.
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along
with OWLOS. If not, see < https://www.gnu.org/licenses/>.

GitHub: https://github.com/KirinDenis/CityGame

--------------------------------------------------------------------------------------*/

using CityGame.Data.DTO;
using CityGame.DTOs.Const;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using CityGame.Models;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace CityGame
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int terrainSize = 100;

        private DrawingVisual drawingVisual = new DrawingVisual();

        private int scrollBorder = 100;

        private double startScrollSpeed = 50.0;

        private bool lockScroll = false;

        private CityGameEngine cityGameEngine;

        private double zoom = 2;

        public GroupDTO selectedGroup = null;

        private SpriteBusiness spriteBusiness = new SpriteBusiness();

        private Image[,] previewImages = new Image[GameConsts.GroupSize, GameConsts.GroupSize];

        private Button? saveDashboardButton = null;
        
        private Point mousePosition;

        public MainWindow()
        {
            InitializeComponent();

            if (SpriteRepository.ResourceInfo == null)
            {
                return;
            }

            cityGameEngine = new CityGameEngine("new city", terrainSize);
            cityGameEngine.RenderCompleted += CityGameEngine_RenderCompleted;

            TerrainImage.Width = TerrainImage.Height = cityGameEngine.GetTerrainSize() * SpriteRepository.ResourceInfo.SpriteSize * zoom;

            TerrainScroll.ScrollToVerticalOffset(TerrainImage.Width / 2.0f);
            TerrainScroll.ScrollToHorizontalOffset(TerrainImage.Height / 2.0f);

            for (int x = 0; x < GameConsts.GroupSize; x++)
            {
                for (int y = 0; y < GameConsts.GroupSize; y++)
                {
                    previewImages[x, y] = new Image();
                    previewImages[x, y].Width = previewImages[x, y].Height = SpriteRepository.ResourceInfo.SpriteSize;
                    previewImages[x, y].HorizontalAlignment = HorizontalAlignment.Left;
                    previewImages[x, y].VerticalAlignment = VerticalAlignment.Top;
                    previewImages[x, y].OpacityMask = (Brush)new BrushConverter().ConvertFrom("#90000000");
                    RenderOptions.SetBitmapScalingMode(previewImages[x, y], BitmapScalingMode.NearestNeighbor);
                    TerrainGrid.Children.Add(previewImages[x, y]);
                }
            }
            BuldozerImage.Source = SpriteRepository.GetDashboard(0, 0);
            WireImage.Source = SpriteRepository.GetDashboard(0, 1);
            GardenImage.Source = SpriteRepository.GetDashboard(0, 2);
            ComercialImage.Source = SpriteRepository.GetDashboard(0, 3);
            PoliceDepartmentImage.Source = SpriteRepository.GetDashboard(0, 4);
            StadiumImage.Source = SpriteRepository.GetDashboard(0, 5);
            SeaPortImage.Source = SpriteRepository.GetDashboard(0, 6);

            RoadImage.Source = SpriteRepository.GetDashboard(1, 0);
            RailImage.Source = SpriteRepository.GetDashboard(1, 1);
            ResidentImage.Source = SpriteRepository.GetDashboard(1, 2);
            IndustrialImage.Source = SpriteRepository.GetDashboard(1, 3);
            FireDepartmentImage.Source = SpriteRepository.GetDashboard(1, 4);
            PowerplantImage.Source = SpriteRepository.GetDashboard(1, 5);
            AirPortImage.Source = SpriteRepository.GetDashboard(1, 6);
        }

        private void CityGameEngine_RenderCompleted(object? sender, EventArgs e)
        {
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(cityGameEngine.GetTerrainBitmap(), new Rect(0, 0, cityGameEngine.GetTerrainSize(), cityGameEngine.GetTerrainSize()));
                drawingContext.Close();
                TerrainImage.Source = new DrawingImage(drawingVisual.Drawing);
            }

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(cityGameEngine.GetMapBitmap(), new Rect(0, 0, cityGameEngine.GetTerrainSize(), cityGameEngine.GetTerrainSize()));
                drawingContext.Close();
                MapImage.Source = new DrawingImage(drawingVisual.Drawing);
            }
        }

        private void ResourceExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            new ResourceExplorerWindow().Show();
        }

        private void GenerateMapButton_Click(object sender, RoutedEventArgs e)
        {
            cityGameEngine.GenerateTerrain();
        }

        private void BenchmarkButton_Click(object sender, RoutedEventArgs e)
        {
            int groupIndex = 0;
            int count = 0;
            for (ushort x = 3; x < terrainSize - GameConsts.GroupSize; x += 3, groupIndex++)
            {
                for (ushort y = 3; y < terrainSize - GameConsts.GroupSize; y += 3, groupIndex++)
                {
                    if (groupIndex >= spriteBusiness.groups.Count - 1)
                    {
                        groupIndex = 0;
                    }
                    cityGameEngine.BuildObject(new PositionDTO() { x = x, y = y }, spriteBusiness.groups[groupIndex]);
                    count++;
                }
            }
            System.GC.Collect();
            Title = "Benchmark result " + count + " objects";
        }


        private void Terrain_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Get the position of the mouse relative to the TerrainScroll control
            Point mousePos = e.GetPosition(TerrainScroll);

            // Calculate the scale factor for zooming
            double zoomScale = e.Delta > 0 ? 1.1 : 0.9;


            // Get the current scroll position
            double scrollX = TerrainScroll.HorizontalOffset;
            double scrollY = TerrainScroll.VerticalOffset;

            // Calculate the new scale factor for the image
            double newScale = TerrainImage.LayoutTransform.Value.M11 * zoomScale;

            // Calculate the new dimensions of the image
            double newWidth = TerrainImage.ActualWidth * newScale;
            double newHeight = TerrainImage.ActualHeight * newScale;

            // Calculate the new scroll position so that the mouse position is centered
            double newScrollX = (mousePos.X + scrollX) * zoomScale - mousePos.X;
            double newScrollY = (mousePos.Y + scrollY) * zoomScale - mousePos.Y;

            // Constrain the new scroll position so that the image remains within the scrollable area
            newScrollX = Math.Max(0, Math.Min(newWidth - TerrainScroll.ActualWidth, newScrollX));
            newScrollY = Math.Max(0, Math.Min(newHeight - TerrainScroll.ActualHeight, newScrollY));

            // Zoom the image
            TerrainImage.LayoutTransform = new ScaleTransform(newScale, newScale);

            // Scroll to the new position
            TerrainScroll.ScrollToHorizontalOffset(newScrollX);
            TerrainScroll.ScrollToVerticalOffset(newScrollY);
        }

        private PositionDTO GetTerrainPosition(MouseEventArgs e)
        {
            double actualSpriteSizeInPixels = TerrainImage.ActualWidth / cityGameEngine.GetTerrainSize();

            return new PositionDTO()
            {
                x = (ushort)((e.GetPosition(TerrainImage).X - (e.GetPosition(TerrainImage).X % actualSpriteSizeInPixels)) / actualSpriteSizeInPixels),
                y = (ushort)((e.GetPosition(TerrainImage).Y - (e.GetPosition(TerrainImage).Y % actualSpriteSizeInPixels)) / actualSpriteSizeInPixels),
            };
        }

        private void TerrainGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            PositionDTO p = GetTerrainPosition(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                cityGameEngine.BuildObject(p, selectedGroup);
            }
            else
            if (e.RightButton == MouseButtonState.Pressed)
            {
                cityGameEngine.DestroyObjectAtPosition(p);
            }
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            new ResourceExplorerWindow().Show();
        }

        private void SelectGroup(GroupDTO? group)
        {
            selectedGroup = group;
            if (selectedGroup != null)
            {
                if ((spriteBusiness.GetObjectTypeByGrop(group) == ObjectType.network)
                    ||
                   (spriteBusiness.GetObjectTypeByGrop(group) == ObjectType.garden))
                {
                    for (int x = 0; x < GameConsts.GroupSize; x++)
                    {
                        for (int y = 0; y < GameConsts.GroupSize; y++)
                        {
                            previewImages[x, y].Source = null;
                            previewImages[x, y].Tag = null;
                        }
                    }
                    previewImages[0, 0].Source = SpriteRepository.GetSprite(selectedGroup.Sprites[0].Sprites[4, 0]);
                    previewImages[0, 0].Tag = previewImages[0, 0].Source;
                }
                else
                if (spriteBusiness.GetObjectTypeByGrop(group) == ObjectType.building)
                {
                    for (int x = 0; x < GameConsts.GroupSize; x++)
                    {
                        for (int y = 0; y < GameConsts.GroupSize; y++)
                        {
                            previewImages[x, y].Source = SpriteRepository.GetSprite(selectedGroup.Sprites[0].Sprites[x, y]);
                            previewImages[x, y].Tag = previewImages[x, y].Source;
                        }
                    }
                }
            }
        }

        private void BuildButton_Click(object sender, RoutedEventArgs e)
        {
            SelectGroup(spriteBusiness.GetGroupByName((string)(sender as Button).Tag));

            if (saveDashboardButton != null)
            {
                saveDashboardButton.Background = null;
            }
            (sender as Button).Background = new SolidColorBrush(Colors.Yellow);
            saveDashboardButton = (sender as Button);

            e.Handled = true;
        }

        private void BuildMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SelectGroup(spriteBusiness.GetGroupByName((string)(sender as MenuItem).Tag));

            if (saveDashboardButton != null)
            {
                saveDashboardButton.Background = null;
            }
            PowerPlantButton.Background = new SolidColorBrush(Colors.Yellow);
            saveDashboardButton = PowerPlantButton;
            e.Handled = true;
        }

        private void PowerPlantButton_Click(object sender, RoutedEventArgs e)
        {
            PowerPlantButton.ContextMenu.IsOpen = true;
            PowerPlantButton.ContextMenu.PlacementTarget = PowerPlantButton;
            PowerPlantButton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GameViewGrid_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPosition = e.GetPosition(this);
            if (!lockScroll)
            {
                lockScroll = true;
                TerrainGrid.CaptureMouse();


                double deltaX = currentPosition.X - mousePosition.X;
                double deltaY = currentPosition.Y - mousePosition.Y;

                double newHorizontalOffset = TerrainScroll.HorizontalOffset - deltaX;
                double newVerticalOffset = TerrainScroll.VerticalOffset - deltaY;

                if (newHorizontalOffset < 0)
                {
                    newHorizontalOffset = 0;
                }
                else if (newHorizontalOffset > TerrainScroll.ScrollableWidth)
                {
                    newHorizontalOffset = TerrainScroll.ScrollableWidth;
                }

                if (newVerticalOffset < 0)
                {
                    newVerticalOffset = 0;
                }
                else if (newVerticalOffset > TerrainScroll.ScrollableHeight)
                {
                    newVerticalOffset = TerrainScroll.ScrollableHeight;
                }

                bool edge = false;
                if (e.MiddleButton != MouseButtonState.Pressed)
                {
                    // Check if the mouse is within a certain range of the edges of the image
                    double edgeThreshold = scrollBorder; // Change this value to adjust the sensitivity of the scrolling


                    if (currentPosition.X <= edgeThreshold && newHorizontalOffset > 0)
                    {
                        newHorizontalOffset -= (edgeThreshold - currentPosition.X) / startScrollSpeed;
                        edge = true;
                    }
                    else if (currentPosition.X >= TerrainGrid.ActualWidth - edgeThreshold && newHorizontalOffset < TerrainScroll.ScrollableWidth)
                    {
                        newHorizontalOffset += (currentPosition.X - (TerrainGrid.ActualWidth - edgeThreshold)) / startScrollSpeed;
                        edge = true;
                    }

                    if (currentPosition.Y <= edgeThreshold && newVerticalOffset > 0)
                    {
                        newVerticalOffset -= (edgeThreshold - currentPosition.Y) / startScrollSpeed;
                        edge = true;
                    }
                    else if (currentPosition.Y >= TerrainGrid.ActualHeight - edgeThreshold && newVerticalOffset < TerrainScroll.ScrollableHeight)
                    {
                        newVerticalOffset += (currentPosition.Y - (TerrainGrid.ActualHeight - edgeThreshold)) / startScrollSpeed;
                        edge = true;
                    }
                }

                if ((e.MiddleButton == MouseButtonState.Pressed) || (edge))
                {
                    TerrainScroll.ScrollToHorizontalOffset(newHorizontalOffset);
                    TerrainScroll.ScrollToVerticalOffset(newVerticalOffset);


                }
                mousePosition = currentPosition;
                TerrainGrid.ReleaseMouseCapture();
                lockScroll = false;
            }

            double actualSpriteSizeInPixels = TerrainImage.DesiredSize.Width / cityGameEngine.GetTerrainSize();

            double x = e.GetPosition(TerrainGrid).X - (e.GetPosition(TerrainGrid).X % actualSpriteSizeInPixels); 
            double y = e.GetPosition(TerrainGrid).Y - (e.GetPosition(TerrainGrid).Y % actualSpriteSizeInPixels); 

            Debug.WriteLine(actualSpriteSizeInPixels);
            Debug.WriteLine("M: " + mousePosition.X);
            Debug.WriteLine("E: " + x);

            TerrainSelector.Width = TerrainSelector.Height = actualSpriteSizeInPixels;

            if ((x < TerrainImage.ActualWidth) && (y < TerrainImage.ActualHeight))
            {
                TerrainSelector.Margin = new Thickness(x, y, 0, 0);

                PositionDTO position = GetTerrainPosition(e);

                ObjectType[,] newPositionMap = cityGameEngine.TestPosition(selectedGroup, position)?.PositionArea;

                for (int px = 0; px < GameConsts.GroupSize; px++)
                {
                    for (int py = 0; py < GameConsts.GroupSize; py++)
                    {
                        previewImages[px, py].Width = previewImages[px, py].Height = actualSpriteSizeInPixels;
                        previewImages[px, py].Margin = new Thickness(x + px * actualSpriteSizeInPixels, y + py * actualSpriteSizeInPixels, 0, 0);

                        if ((newPositionMap != null)
                            && (px < newPositionMap.GetLength(0))
                            && (py < newPositionMap.GetLength(1))
                            &&
                            (newPositionMap != null) && (newPositionMap[px, py] != null))

                        {
                            switch (newPositionMap[px, py])
                            {
                                case ObjectType.terrain:
                                case ObjectType.forest:
                                case ObjectType.network:
                                    previewImages[px, py].Source = previewImages[px, py].Tag as ImageSource;
                                    break;
                                default:
                                    previewImages[px, py].Source = SpriteRepository.GetSprite(spriteBusiness.GetGroupByName(SpritesGroupEnum.select)?.Sprites[0].Sprites[0, 0]);
                                    break;
                            }
                        }
                    }
                }
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    cityGameEngine.BuildObject(position, selectedGroup);
                }
            }
        }
    }
}

