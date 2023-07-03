using CityGame.Business;
using CityGame.Data.DTO;
using CityGame.DTOs.Const;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
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
        private Dictionary<PositionDTO, TextBlock> debugTextBlocks = new Dictionary<PositionDTO, TextBlock>();
        private void GameBusiness_DebugMessage(object? sender, DebugMessageDTO e)
        {
            if (DebugInfo)
            {
                if ((e.Position != null) && (Application.Current != null))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        TextBlock textBlock = debugTextBlocks.GetValueOrDefault(e.Position);
                        if (textBlock == null)
                        {
                            double actualSpriteSizeInPixels = TerrainImage.DesiredSize.Width / gameBusiness.GetTerrainSize();
                            textBlock = new TextBlock();
                            textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                            textBlock.VerticalAlignment = VerticalAlignment.Top;
                            textBlock.Margin = new Thickness(e.Position.x * actualSpriteSizeInPixels, e.Position.y * actualSpriteSizeInPixels, 0, 0);
                            textBlock.SetValue(Panel.ZIndexProperty, 0xFFFF);
                            textBlock.FontSize = 12;

                            textBlock.Foreground = Brushes.White;
                            textBlock.Background = Brushes.Navy;
                            textBlock.OpacityMask = new SolidColorBrush(Colors.LightGreen) { Opacity = 0.8 };

                            TerrainGrid.Children.Add(textBlock);
                            debugTextBlocks.Add(e.Position, textBlock);
                        }
                        textBlock.Text = e.Properties.Values.ToString();
                    });
                }
            }
        }

    }
}
