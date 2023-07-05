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
using System.Windows.Threading;

namespace CityGame
{
    public partial class MainWindow: Window
    {
        private Dictionary<PositionDTO, DebugOutControl> debugOutControls = new Dictionary<PositionDTO, DebugOutControl>();
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
                            debugOutControl = new DebugOutControl();
                            debugOutControl.HorizontalAlignment = HorizontalAlignment.Left;
                            debugOutControl.VerticalAlignment = VerticalAlignment.Top;
                            debugOutControl.Margin = new Thickness(e.Position.x * actualSpriteSizeInPixels, e.Position.y * actualSpriteSizeInPixels, 0, 0);
                            debugOutControl.SetValue(Panel.ZIndexProperty, 0xFFFF);

                            TerrainGrid.Children.Add(debugOutControl);
                            debugOutControls.Add(e.Position, debugOutControl);
                        }
                        debugOutControl.SetDebug(e.Properties);                        
                    });
                }
            }
        }

    }
}
