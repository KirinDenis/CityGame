using CityGame.Graphics;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CityGame
{
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

            new ResourceExplorer().Show();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += Timer_Tick;
            // timer.Start();
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
    }
}
