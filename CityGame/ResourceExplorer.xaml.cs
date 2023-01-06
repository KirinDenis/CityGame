using CityGame.DataModels;
using CityGame.Graphics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
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

        private BlockPoint GetBlockPositionByMouse(MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(ResourceImage);
            return new BlockPoint()
            {
                x = (int)(mousePosition.X / (ResourceImage.ActualWidth / ResourceImage.Source.Width)) / ResourcesManager.iconsSizeInPixels,
                y = (int)(mousePosition.Y / (ResourceImage.ActualHeight / ResourceImage.Source.Height)) / ResourcesManager.iconsSizeInPixels
            };
        }

        /// <summary>
        /// Select block by mouse move over source image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResourceImage_MouseMove(object sender, MouseEventArgs e)
        {
            BlockPoint position = GetBlockPositionByMouse(e);
            BlockItemModel block = blocksManager.GetBlockInfoByPosition(GetBlockPositionByMouse(e));

            CoordTextBlock.Text = string.Format("{0}:{1} {2}", position.x, position.y, block.name);
            PreviewImage.Source = ResourcesManager.GetBlock(position.x, position.y);
        }

        private void RefreshBlocksList()
        {            
            BlocksListBox.Items.Add(blocksManager.blocks);
        }

        private void ResourceImage_MouseDown(object sender, MouseButtonEventArgs e)
        {            
            BlockInfoGrid.Visibility = Visibility.Visible;

            BlockPoint position = GetBlockPositionByMouse(e);

            BlockItemModel block = blocksManager.GetBlockInfoByPosition(GetBlockPositionByMouse(e));

            BlockInfoGrid.Tag = block;

            BlockInfoImage.Source = ResourcesManager.GetBlock(block.position.x, block.position.y);
            BlockInfoPositionTextBlock.Text = string.Format("{0}:{1}", block.position.x, block.position.y);

            BlockInfoNameTextBox.Text = block.name;
        }

        private void BlockEditOKButton_Click(object sender, RoutedEventArgs e)
        {
            BlockItemModel block = (BlockItemModel)BlockInfoGrid.Tag;
            block.name = BlockInfoNameTextBox.Text;
            blocksManager.SetBlocks();

            BlockInfoGrid.Visibility = Visibility.Hidden;
        }
    }
}
