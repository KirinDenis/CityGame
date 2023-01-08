using CityGame.DataModels;
using CityGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace CityGame
{
    /// <summary>
    /// Interaction logic for ResourceExplorer.xaml
    /// </summary>
    public partial class ResourceExplorer : Window
    {
        private BlocksManager blocksManager = new BlocksManager();

        private Image[,] groupsPreviewImages = new Image[5, 5];

        private bool blockEditMode = false;

        private int animationFrame = 1;
        public ResourceExplorer()
        {
            InitializeComponent();

            SourceImageInfoTextBlock.Text = string.Format("Image {0}x{1}", ResourceImage.Source.Width, ResourceImage.Source.Height);
            SourceBlockInfoTextBlock.Text = string.Format("Block {0}x{1}", ResourcesManager.iconsSizeInPixels, ResourcesManager.iconsSizeInPixels);
            SourceCountsInfoTextBlock.Text = string.Format("Counts {0}x{1}", ResourcesManager.iconsCountByX, ResourcesManager.iconsCountByY);

            RefreshGroupsList();

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    groupsPreviewImages[x, y] = new Image();
                    groupsPreviewImages[x, y].SetValue(Grid.ColumnProperty, x);
                    groupsPreviewImages[x, y].SetValue(Grid.RowProperty, y);

                    groupsPreviewImages[x, y].Width = groupsPreviewImages[x, y].Height = 50;

                    GroupViewGrid.Children.Add(groupsPreviewImages[x, y]);

                    TextBlock textBlock = new TextBlock();
                    textBlock.SetValue(Grid.ColumnProperty, x);
                    textBlock.SetValue(Grid.RowProperty, y);

                    textBlock.VerticalAlignment = VerticalAlignment.Stretch;
                    textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;

                    textBlock.Text = string.Format("{0}:{1}", x, y);

                    textBlock.Tag = new BlockPoint()
                    {
                        x = x,
                        y = y
                    };
                    textBlock.MouseDown += ResourceExplorer_MouseDown;

                    GroupViewGrid.Children.Add(textBlock);

                }
            }

            DispatcherTimer animationFrameTimer = new DispatcherTimer();
            animationFrameTimer.Interval = TimeSpan.FromMilliseconds(300);
            animationFrameTimer.Tick += AnimationFrameTimer_Tick;
            animationFrameTimer.Start();

        }

        private void AnimationFrameTimer_Tick(object? sender, EventArgs e)
        {
            BlockItemModel block = (BlockItemModel)PreviewImage.Tag;
            if (block != null)
            {
                animationFrame++;
                List<BlockItemModel>? blockItemModels = blocksManager.GetBlockByGroupIndexAnimationOnly(block.groupId);
                if ((blockItemModels != null) && (blockItemModels.Count > 0))
                {
                    List<BlockItemModel>? nextFrameBlocks = blockItemModels?.FindAll(p => p.animationFrame == animationFrame);
                    
                    if ((nextFrameBlocks == null) || (nextFrameBlocks.Count == 0))
                    {
                        animationFrame = 1;
                        nextFrameBlocks = blockItemModels?.FindAll(p => p.animationFrame == animationFrame);
                    }
                    if ((nextFrameBlocks != null) || (nextFrameBlocks.Count == 0))
                    {
                        foreach (var blockItemModel in nextFrameBlocks)
                        {
                            if (blockItemModel.groupPosition != null)
                            {
                                groupsPreviewImages[blockItemModel.groupPosition.x, blockItemModel.groupPosition.y].Source
                                    = ResourcesManager.GetBlock(blockItemModel.position.x, blockItemModel.position.y);
                            }
                        }
                    }
                }
            }
        }


        private void ResourceExplorer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BlockItemModel block = (BlockItemModel)PreviewImage.Tag;

            if (block != null)
            {
                block.groupPosition = (BlockPoint)((TextBlock)sender).Tag;
                blocksManager.SetBlocks();
                RefreshGroupImages(block);
            }


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
            if (!blockEditMode)
            {
                BlockPoint position = GetBlockPositionByMouse(e);
                BlockItemModel block = blocksManager.GetBlockByPosition(GetBlockPositionByMouse(e));

                PreviewImage.Tag = block;
                PreviewImage.Source = ResourcesManager.GetBlock(position.x, position.y);

                BlockInfoPositionTextBlock.Text = string.Format("{0}:{1}", block.position.x, block.position.y);

                BlockInfoNameTextBox.Text = block.name;

                BlockInfoGroupComboBox.SelectedIndex = block.groupId;

                AnimationFrameComboBox.SelectedIndex = block.animationFrame ?? 0;

                if (block.groupPosition != null)
                {
                    // BlockInfoGroupPositionXComboBox.SelectedIndex = block.groupPosition.x + 1;
                    //BlockInfoGroupPositionYComboBox.SelectedIndex = block.groupPosition.y + 1;                    
                }
                else
                {
                    // BlockInfoGroupPositionXComboBox.SelectedIndex = 0;
                    // BlockInfoGroupPositionYComboBox.SelectedIndex = 0;
                }
                RefreshGroupImages(block);
            }
        }

        private void RefreshGroupsList()
        {
            BlockInfoGroupComboBox.ItemsSource = blocksManager.groups;
        }

        private void RefreshGroupImages(BlockItemModel block)
        {

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    groupsPreviewImages[x, y].Source = null;
                }
            }

            if (block != null)
            {
                List<BlockItemModel> blocksItems = blocksManager.GetBlockByGroupIndex(block.groupId);
                foreach (BlockItemModel blockItem in blocksItems)
                {
                    if (blockItem.groupPosition != null)
                    {
                        if ((blockItem.groupPosition.x >= 0) && (blockItem.groupPosition.y >= 0))
                        {
                            groupsPreviewImages[blockItem.groupPosition.x, blockItem.groupPosition.y].Source = ResourcesManager.GetBlock(blockItem.position.x, blockItem.position.y);
                        }
                    }
                }
            }
        }

        private void ResourceImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            blockEditMode = !blockEditMode;
        }

        private void NewBlock_Click(object sender, RoutedEventArgs e)
        {
            if (blocksManager.groups.Find(p => p.Equals(BlockInfoGroupComboBox.Text)) == null)
            {
                blocksManager.groups.Add(BlockInfoGroupComboBox.Text);
                blocksManager.SetGroups();

                BlockInfoGroupComboBox.ItemsSource = blocksManager.groups;
                BlockInfoGroupComboBox.SelectedIndex = BlockInfoGroupComboBox.Items.Count - 1;

                BlockItemModel block = (BlockItemModel)PreviewImage.Tag;
                block.groupId = BlockInfoGroupComboBox.SelectedIndex;
                blocksManager.SetBlocks();
                RefreshGroupImages(block);
            }

        }

        private void SaveBlocksButton_Click(object sender, RoutedEventArgs e)
        {
            BlockItemModel block = (BlockItemModel)PreviewImage.Tag;
            block.name = BlockInfoNameTextBox.Text;
            block.groupId = BlockInfoGroupComboBox.SelectedIndex;
            if (block.groupPosition == null)
            {
                block.groupPosition = new BlockPoint();
            }


            blocksManager.SetBlocks();
            RefreshGroupImages(block);
        }

        private void BlockInfoGroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BlockItemModel block = (BlockItemModel)PreviewImage.Tag;
            if (block != null)
            {
                block.groupId = BlockInfoGroupComboBox.SelectedIndex;
                blocksManager.SetBlocks();
                RefreshGroupImages(block);
            }
        }

        private void AnimationFrameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BlockItemModel block = (BlockItemModel)PreviewImage.Tag;
            if (block != null)
            {
                block.animationFrame = AnimationFrameComboBox.SelectedIndex;
                blocksManager.SetBlocks();
                RefreshGroupImages(block);
            }
        }
    }
}
