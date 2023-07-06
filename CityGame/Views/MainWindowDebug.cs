using CityGame.Business;
using CityGame.Data.DTO;
using CityGame.DTOs.Const;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using CityGame.Views;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CityGame
{
    public partial class MainWindow: Window
    {
        private Dictionary<PositionDTO, DebugOutControl> debugOutControls = new Dictionary<PositionDTO, DebugOutControl>();

        DebugOutControl draggedControl = null;
        Point draggedOffset;

        private void GameBusiness_DebugMessage(object? sender, DebugMessageDTO e)
        {
            if (DebugInfo)
            {
                if ((e.Position != null) && (Application.Current != null))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        DebugOutControl debugOutControl = debugOutControls.GetValueOrDefault(e.Position);
                        if (debugOutControl == null)
                        {
                            double actualSpriteSizeInPixels = TerrainImage.DesiredSize.Width / gameBusiness.GetTerrainSize();

                            Line debugConnectedLine = new Line();
                            debugConnectedLine.X1 = debugConnectedLine.X2 = e.Position.x * actualSpriteSizeInPixels;
                            debugConnectedLine.Y1 = debugConnectedLine.Y2 = e.Position.y * actualSpriteSizeInPixels;
                            debugConnectedLine.StrokeThickness = 2;
                            debugConnectedLine.Stroke = Brushes.Yellow;
                            debugConnectedLine.SetValue(Panel.ZIndexProperty, 0xFFFF);
                            TerrainGrid.Children.Add(debugConnectedLine);

                            debugOutControl = new DebugOutControl();
                            debugOutControl.HorizontalAlignment = HorizontalAlignment.Left;
                            debugOutControl.VerticalAlignment = VerticalAlignment.Top;
                            debugOutControl.Margin = new Thickness(e.Position.x * actualSpriteSizeInPixels, e.Position.y * actualSpriteSizeInPixels, 0, 0);
                            debugOutControl.SetValue(Panel.ZIndexProperty, 0xFFFF);
                            debugOutControl.Tag = debugConnectedLine;

                            TerrainGrid.Children.Add(debugOutControl);
                            debugOutControls.Add(e.Position, debugOutControl);

                            debugOutControl.PreviewMouseDown += DebugOutControl_PreviewMouseDown;
                        }
                        debugOutControl.SetDebug(e.Properties);                        
                    });
                }
            }
        }

        private void DebugOutControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            draggedControl = sender as DebugOutControl;
            draggedOffset = e.GetPosition(this.TerrainGrid);
            draggedOffset.Y -= draggedControl.Margin.Top;
            draggedOffset.X -= draggedControl.Margin.Left;
            TerrainGrid.CaptureMouse();
            e.Handled = true;
        }

        private void DebugOutControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
        }

        private void DebugOutControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
