using CityGame.Graphics;
using System.Windows;
using System.Windows.Input;

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

            SourceImageInfoTextBlock.Text = string.Format("Image {0}x{1}", ResourceImage.Source.Width, ResourceImage.Source.Height);
            SourceBlockInfoTextBlock.Text = string.Format("Block {0}x{1}", ResourcesManager.iconsSizeInPixels, ResourcesManager.iconsSizeInPixels);
            SourceCountsInfoTextBlock.Text = string.Format("Counts {0}x{1}", ResourcesManager.iconsCountByX, ResourcesManager.iconsCountByY);
        }

        private void ResourceImage_MouseMove(object sender, MouseEventArgs e)
        {

            System.Windows.Point position = e.GetPosition(ResourceImage);
            int blockX = (int)(position.X / (ResourceImage.ActualWidth / ResourceImage.Source.Width)) / ResourcesManager.iconsSizeInPixels;
            int blockY = (int)(position.Y / (ResourceImage.ActualHeight / ResourceImage.Source.Height)) / ResourcesManager.iconsSizeInPixels;

            PreviewImage.Source = ResourcesManager.GetBlock(blockX, blockY);
        }
    }
}
