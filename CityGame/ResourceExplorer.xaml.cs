using CityGame.DataModels;
using CityGame.Graphics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace CityGame
{
    /// <summary>
    /// Interaction logic for ResourceExplorer.xaml
    /// </summary>
    public partial class ResourceExplorer : Window
    {
        private BlocksManager blocksManager = new BlocksManager();
        public ResourceExplorer()
        {
            InitializeComponent();

            SourceImageInfoTextBlock.Text = string.Format("Image {0}x{1}", ResourceImage.Source.Width, ResourceImage.Source.Height);
            SourceBlockInfoTextBlock.Text = string.Format("Block {0}x{1}", ResourcesManager.iconsSizeInPixels, ResourcesManager.iconsSizeInPixels);
            SourceCountsInfoTextBlock.Text = string.Format("Counts {0}x{1}", ResourcesManager.iconsCountByX, ResourcesManager.iconsCountByY);
        }

        /// <summary>
        /// Select block by mouse move over source image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResourceImage_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(ResourceImage);
            //Calculate destination block location
            int blockX = (int)(position.X / (ResourceImage.ActualWidth / ResourceImage.Source.Width)) / ResourcesManager.iconsSizeInPixels;
            int blockY = (int)(position.Y / (ResourceImage.ActualHeight / ResourceImage.Source.Height)) / ResourcesManager.iconsSizeInPixels;

            CoordTextBlock.Text = string.Format("{0}:{1}", blockX, blockY);
            PreviewImage.Source = ResourcesManager.GetBlock(blockX, blockY);
        }

        private void RefreshBlocksList()
        {
            List<BlockItemModel> blocks = blocksManager.GetBlocks();
            BlocksListBox.Items.Add(blocks);
        }
    }
}
