using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace RegionsTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random(0xFFFF);
        int[,] e = new int[256, 256];
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 10; i++)
            {
                e[random.Next(255), random.Next(255)] = random.Next(255);
            }

            int[,] _e = new int[256, 256];
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    if (e[x, y] != 0)
                    {
                        _e[x, y] = e[x, y];
                        FillCircle(_e, x, y, e[x, y], e[x, y] / 2);
                    }
                }
            }

            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    if (_e[x, y] != 0)
                    {
                        double fraction = (double)_e[x, y] / 255;
                        Color color = Color.FromArgb(
                        240,
                        (byte)(255 * fraction),           // Красный канал
                        (byte)(255 * (1 - fraction) / 1.5),
                        (byte)(255 * (1 - fraction))     // Синий канал

                    );

                        System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();
                        r.Width = 3;
                        r.Height = 3;
                        Canvas.SetLeft(r, x * 3);
                        Canvas.SetTop(r, y * 3);
                        r.StrokeThickness = 3;
                        r.Fill = new SolidColorBrush(color);
                        r.Stroke = new SolidColorBrush(color);
                        Canvas.Children.Add(r);
                    }
                }
            }
        }

        public void FillCircle(int[,] _ecosystem, int centerX, int centerY, int radius, int weight)
        {
            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                for (int x = centerX - radius; x <= centerX + radius; x++)
                {
                    if (Math.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY)) <= radius)
                    {
                        double dist = Math.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                        int currentWeight = (int)Math.Max(0, weight - dist);

                        if ((x >= 0) && (x < 255) && (y >= 0) && (y < 255))
                        {
                            _ecosystem[x, y] += (byte)currentWeight;
                            if (_ecosystem[x, y] > 255)
                            {
                                _ecosystem[x, y] = 255;
                            }
                        }
                    }
                }
            }
        }

    }
}
