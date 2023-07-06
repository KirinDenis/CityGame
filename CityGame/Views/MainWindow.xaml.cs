

/* ----------------------------------------------------------------------------
Simple City
Copyright 2023 by:
- Denis Kirin (deniskirinacs@gmail.com)

This file is part of Simple City

OWLOS is free software : you can redistribute it and/or modify it under the
terms of the GNU General Public License as published by the Free Software
Foundation, either version 3 of the License, or (at your option) any later
version.

Simple City is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
FOR A PARTICULAR PURPOSE.
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along
with Simple City. If not, see < https://www.gnu.org/licenses/>.

GitHub: https://github.com/KirinDenis/CityGame

--------------------------------------------------------------------------------------*/

using CityGame.Business;
using CityGame.Data.DTO;
using CityGame.DTOs.Const;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using CityGame.Views;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CityGame
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int terrainSize = 256;

        private readonly GameBusiness? gameBusiness;

        private readonly SpriteBusiness spriteBusiness = new();

        private readonly DrawingVisual drawingVisual = new();

        public GroupDTO? selectedGroup;

        private readonly Image[,] previewImages = new Image[GameConsts.GroupSize, GameConsts.GroupSize];

        private Button? saveDashboardButton = null;

        private readonly int zoom = 2;


        private bool DebugInfo = true;

        public MainWindow()
        {
            InitializeComponent();

            if (SpriteRepository.ResourceInfo == null)
            {
                return;
            }

            gameBusiness = new GameBusiness("new city", terrainSize);
            gameBusiness.TerrainRenderCompleted += CityGameEngine_TerrainRenderCompleted;
            gameBusiness.MapRenderCompleted += CityGameEngine_MapRenderCompleted;
            gameBusiness.DebugMessage += GameBusiness_DebugMessage;
            gameBusiness.BudgetChanged += GameBusiness_BudgetChanged;

            TerrainImage.Width = TerrainImage.Height = gameBusiness.GetTerrainSize() * SpriteRepository.ResourceInfo.SpriteSize * zoom;

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
                    previewImages[x, y].OpacityMask = new SolidColorBrush(Colors.LightGreen) { Opacity = 0.5 };
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

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10000);
            timer.Tick += Timer_Tick;
            timer.Start();
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {
            EcosystemCanvase.Children.Clear();

            if (Application.Current != null)
            {
                Dispatcher dispatcher = Application.Current.Dispatcher;

                // Запустить метод таймера с низким приоритетом в главном потоке
                dispatcher.Invoke(() =>
                {
                    Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                    EcosystemCanvase.Width = EcosystemCanvase.Height = terrainSize;
                    for (int x = 0; x < terrainSize; x++)
                    {
                        for (int y = 0; y < terrainSize; y++)
                        {
                            if (gameBusiness.ecosystem[x, y].Population > 0)
                            {
                                double fraction = (double)gameBusiness.ecosystem[x, y].Population / 255;

                                // Создать градиентный цвет от голубого до ярко-красного
                                Color color = Color.FromArgb(
                                   240,
                                    (byte)(255 * fraction),           // Красный канал
                                    (byte)(255 * (1 - fraction) / 1.5),
                                    (byte)(255 * (1 - fraction))     // Синий канал

                                );
                                System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();
                                r.Width = 1;
                                r.Height = 1;
                                Canvas.SetLeft(r, x);
                                Canvas.SetTop(r, y);
                                r.StrokeThickness = 2;
                                r.Fill = new SolidColorBrush(color);
                                r.Stroke = new SolidColorBrush(color);
                                EcosystemCanvase.Children.Add(r);
                            }
                        }
                    }
                });
            }
        }

        private void GameBusiness_BudgetChanged(object? sender, EventArgs e)
        {
            BudgetValueTextBlock.Text = gameBusiness.budget.ToString();
        }

        private void CityGameEngine_TerrainRenderCompleted(object? sender, EventArgs e)
        {
            using DrawingContext drawingContext = drawingVisual.RenderOpen();
            if (gameBusiness != null)
            {
                drawingContext.DrawImage(gameBusiness.GetTerrainBitmap(), new Rect(0, 0, gameBusiness.GetTerrainSize(), gameBusiness.GetTerrainSize()));
                drawingContext.Close();
                TerrainImage.Source = new DrawingImage(drawingVisual.Drawing);
            }
        }

        private void CityGameEngine_MapRenderCompleted(object? sender, EventArgs e)
        {
            using DrawingContext drawingContext = drawingVisual.RenderOpen();
            if (gameBusiness != null)
            {
                drawingContext.DrawImage(gameBusiness.GetMapBitmap(), new Rect(0, 0, gameBusiness.GetTerrainSize(), gameBusiness.GetTerrainSize()));
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
            gameBusiness?.GenerateTerrain();
        }

        private void BenchmarkButton_Click(object sender, RoutedEventArgs e)
        {
            int groupIndex = 0;
            int count = 0;

            for (ushort x = 3; x < terrainSize - GameConsts.GroupSize; x += 4, groupIndex++)
            {
                for (ushort y = 3; y < terrainSize - GameConsts.GroupSize; y += 4, groupIndex++)
                {
                    if (count >= gameBusiness.gameObjects.Count - 1)
                    {
                        count = 0;
                    }
                    gameBusiness?.BuildObject(new PositionDTO() { x = x, y = y }, gameBusiness.gameObjects[count].gameObjectModel.startingGroup);
                    count++;
                }
            }
            System.GC.Collect();
            Title = "Benchmark result " + count + " objects";
        }

        private PositionDTO GetTerrainPosition(MouseEventArgs e)
        {
            if (gameBusiness == null)
            {
                return new PositionDTO();
            }
            double actualSpriteSizeInPixels = TerrainImage.ActualWidth / gameBusiness.GetTerrainSize();

            return new PositionDTO()
            {
                x = (ushort)((e.GetPosition(TerrainImage).X - (e.GetPosition(TerrainImage).X % actualSpriteSizeInPixels)) / actualSpriteSizeInPixels),
                y = (ushort)((e.GetPosition(TerrainImage).Y - (e.GetPosition(TerrainImage).Y % actualSpriteSizeInPixels)) / actualSpriteSizeInPixels),
            };
        }

        private void TerrainGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source.GetType() != typeof(DebugOutControl))
            {
                PositionDTO p = GetTerrainPosition(e);

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    gameBusiness?.BuildObject(p, selectedGroup);
                }
                else
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    gameBusiness?.DestroyObjectAtPosition(p);
                }
            }
        }

        private void GameViewGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (gameBusiness == null)
            {
                return;
            }

            if (draggedControl != null)
            {
                var position = e.GetPosition(sender as IInputElement);
                draggedControl.Margin = new Thickness(position.X - this.draggedOffset.X, position.Y - this.draggedOffset.Y, 0, 0);
                Line debugConnectedLine = draggedControl.Tag as Line;
                debugConnectedLine.X2 = position.X - this.draggedOffset.X;
                debugConnectedLine.Y2 = position.Y - this.draggedOffset.Y;
            }
            else
            {

                double actualSpriteSizeInPixels = TerrainImage.DesiredSize.Width / gameBusiness.GetTerrainSize();

                double x = e.GetPosition(TerrainGrid).X - (e.GetPosition(TerrainGrid).X % actualSpriteSizeInPixels);
                double y = e.GetPosition(TerrainGrid).Y - (e.GetPosition(TerrainGrid).Y % actualSpriteSizeInPixels);

                TerrainSelector.Width = TerrainSelector.Height = actualSpriteSizeInPixels;

                if ((x < TerrainImage.ActualWidth) && (y < TerrainImage.ActualHeight))
                {
                    TerrainSelector.Margin = new Thickness(x, y, 0, 0);

                    PositionDTO position = GetTerrainPosition(e);

                    ObjectType[,]? newPositionMap = gameBusiness?.TestPosition(selectedGroup, position)?.PositionArea;

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
                                (newPositionMap != null) && (newPositionMap?[px, py] != null))

                            {
                                switch (newPositionMap[px, py])
                                {
                                    case ObjectType.terrain:
                                    case ObjectType.forest:
                                    case ObjectType.network:
                                        previewImages[px, py].Source = previewImages[px, py].Tag as ImageSource;
                                        break;
                                    default:
                                        previewImages[px, py].Source = SpriteRepository.GetSprite(spriteBusiness.GetGroupByName(SpritesGroupEnum.select)?.Frames?[0]?.Sprites?[0, 0]);
                                        break;
                                }
                            }
                        }
                    }
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        gameBusiness?.BuildObject(position, selectedGroup);
                    }
                }
                MoveMapSelector();
            }
        }

        private void MoveMapSelector()
        {
            double scale = TerrainImage.DesiredSize.Width / 300.0;
            MapSelector.Width = TerrainScroll.ActualWidth / scale;
            MapSelector.Height = TerrainScroll.ActualHeight / scale;
            MapSelector.Margin = new Thickness(TerrainScroll.HorizontalOffset / scale, TerrainScroll.VerticalOffset / scale, 0, 0);
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                switch (e.Key)
                {
                    case Key.R: new ResourceExplorerWindow().Show(); break;
                    case Key.T: gameBusiness?.GenerateTerrain(); break;
                }
            }

            switch (e.Key)
            {
                case Key.F2: new ResourceExplorerWindow().Show(); break;
                case Key.F5: gameBusiness?.GenerateTerrain(); break;
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
                    previewImages[0, 0].Source = SpriteRepository.GetSprite(selectedGroup?.Frames?[0]?.Sprites?[4, 0]);
                    previewImages[0, 0].Tag = previewImages[0, 0].Source;
                }
                else
                if (spriteBusiness.GetObjectTypeByGrop(group) == ObjectType.building)
                {
                    for (int x = 0; x < GameConsts.GroupSize; x++)
                    {
                        for (int y = 0; y < GameConsts.GroupSize; y++)
                        {
                            previewImages[x, y].Source = SpriteRepository.GetSprite(selectedGroup?.Frames?[0]?.Sprites?[x, y]);
                            previewImages[x, y].Tag = previewImages[x, y].Source;
                        }
                    }
                }
            }
        }

        private void BuildButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                SelectGroup(spriteBusiness.GetGroupByName((string)button.Tag));

                if (saveDashboardButton != null)
                {
                    saveDashboardButton.Background = null;
                }
                button.Background = new SolidColorBrush(Colors.Yellow);
                saveDashboardButton = (sender as Button);
            }
            e.Handled = true;
        }

        private void BuildMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                SelectGroup(spriteBusiness.GetGroupByName((string)menuItem.Tag));

                if (saveDashboardButton != null)
                {
                    saveDashboardButton.Background = null;
                }
                PowerPlantButton.Background = new SolidColorBrush(Colors.Yellow);
                saveDashboardButton = PowerPlantButton;
            }
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

        private void MapImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(MapImage);
            TerrainScroll.ScrollToHorizontalOffset(p.X * (TerrainImage.ActualWidth / 300));
            TerrainScroll.ScrollToVerticalOffset(p.Y * (TerrainImage.ActualHeight / 300));
            MoveMapSelector();
        }

        private void TerrainScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            MoveMapSelector();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            gameBusiness.paused = true;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            DebugInfo = !DebugInfo;

            (sender as MenuItem).IsChecked = DebugInfo;

            foreach (var debugItem in debugOutControls)
            {
                if (DebugInfo)
                {
                    (debugItem.Value as DebugOutControl).Visibility = Visibility.Visible;
                }
                else
                {
                    (debugItem.Value as DebugOutControl).Visibility = Visibility.Hidden;
                }
                ((debugItem.Value as DebugOutControl).Tag as Line).Visibility = (debugItem.Value as DebugOutControl).Visibility;
            }
        }

        private void TerrainGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (draggedControl != null)
            {
                draggedControl = null;
                TerrainGrid.ReleaseMouseCapture();
            }
        }
    }
}

