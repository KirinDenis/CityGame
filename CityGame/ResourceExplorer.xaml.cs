using CityGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CityGame
{
    /// <summary>
    /// Interaction logic for ResourceExplorer.xaml
    /// </summary>
    public partial class ResourceExplorer : Window
    {
        public ResourceExplorer()
        {
            InitializeComponent();            
        }

        private void ResourceImage_MouseMove(object sender, MouseEventArgs e)
        {
            PreviewImage.Source = ResourcesManager.GetBlock(2, 2);
        }
    }
}
