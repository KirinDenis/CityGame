using CityGame.Business;
using CityGame.Data.DTO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CityGame.Views
{
    /// <summary>
    /// Interaction logic for DebugOutControl.xaml
    /// </summary>
    public partial class DebugOutControl : UserControl
    {
        private Dictionary<string, TextBlock> debugTextBlocks = new Dictionary<string, TextBlock>();

        Point offset;
        public DebugOutControl()
        {
            InitializeComponent();
        }

        public void SetDebug(Dictionary<string, string> properties)
        {
            foreach (var property in properties)
            {
                TextBlock textBlock = debugTextBlocks.GetValueOrDefault(property.Key);
                if (textBlock == null)
                {
                    textBlock = new TextBlock();
                    textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    textBlock.VerticalAlignment = VerticalAlignment.Top;

                    textBlock.OpacityMask = new SolidColorBrush(Colors.LightGreen) { Opacity = 0.8 };

                    DebugOutStackPanel.Children.Add(textBlock);
                    debugTextBlocks.Add(property.Key, textBlock);
                }
                textBlock.Text = property.Key + ": " + property.Value;
            }
        }

    }
}
