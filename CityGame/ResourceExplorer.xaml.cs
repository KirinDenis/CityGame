using CityGame.DataModels;
using CityGame.Graphics;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CityGame
{
    /// <summary>
    /// Interaction logic for ResourceExplorer.xaml
    /// </summary>
    public partial class ResourceExplorer : Window
    {
        private BlocksManager blocksManager = new BlocksManager();

        private Image[,] groupsPreviewImages = new Image[7, 7];

        private bool blockEditMode = false;

        private int animationFrame = 1;
        public ResourceExplorer()
        {
            InitializeComponent();

            SourceImageInfoTextBlock.Text = string.Format("Image {0}x{1}", ResourceImage.Source.Width, ResourceImage.Source.Height);
            SourceBlockInfoTextBlock.Text = string.Format("Block {0}x{1}", ResourcesManager.iconsSizeInPixels, ResourcesManager.iconsSizeInPixels);
            SourceCountsInfoTextBlock.Text = string.Format("Counts {0}x{1}", ResourcesManager.iconsCountByX, ResourcesManager.iconsCountByY);
           
            RefreshGroupsList();

            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    groupsPreviewImages[x, y] = new Image();
                    groupsPreviewImages[x, y].SetValue(Grid.ColumnProperty, x);
                    groupsPreviewImages[x, y].SetValue(Grid.RowProperty, y);

                    groupsPreviewImages[x, y].Width = groupsPreviewImages[x, y].Height = 50;

                    RenderOptions.SetBitmapScalingMode(groupsPreviewImages[x, y], BitmapScalingMode.NearestNeighbor);

                    GroupViewGrid.Children.Add(groupsPreviewImages[x, y]);

                    TextBlock textBlock = new TextBlock();
                    textBlock.SetValue(Grid.ColumnProperty, x);
                    textBlock.SetValue(Grid.RowProperty, y);

                    textBlock.VerticalAlignment = VerticalAlignment.Stretch;
                    textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
                    textBlock.TextAlignment = TextAlignment.Center;

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

                    if ((nextFrameBlocks == null) || (nextFrameBlocks?.Count == 0))
                    {
                        animationFrame = 1;
                        nextFrameBlocks = blockItemModels?.FindAll(p => p.animationFrame == animationFrame);
                    }
                    if ((nextFrameBlocks != null) || (nextFrameBlocks?.Count == 0))
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
                BlockItemModel? block = blocksManager.GetBlockByPosition(GetBlockPositionByMouse(e));

                PreviewImage.Tag = block;
                PreviewImage.Source = ResourcesManager.GetBlock(position.x, position.y);

                BlockInfoPositionTextBlock.Text = string.Format("{0}:{1}", block.position.x, block.position.y);

                BlockInfoNameTextBox.Text = block.name;

                BlockInfoGroupComboBox.SelectedIndex = block.groupId;

                AnimationFrameComboBox.SelectedIndex = block.animationFrame ?? 0;

                RefreshGroupImages(block);

                //Resourve image selector
                Point actualMargin = ResourceImage.TransformToAncestor(MainGrid).Transform(new Point(0, 0));
                actualMargin.X -= MainGrid.ColumnDefinitions[0].ActualWidth;
                actualMargin.Y -= MainGrid.RowDefinitions[0].ActualHeight;

                double actualIconSizeInPixels = ResourceImage.ActualWidth / ResourcesManager.iconsCountByX;

                double x = e.GetPosition(ResourceImage).X - (e.GetPosition(ResourceImage).X % actualIconSizeInPixels) + actualMargin.X;
                double y = e.GetPosition(ResourceImage).Y - (e.GetPosition(ResourceImage).Y % actualIconSizeInPixels) + actualMargin.Y;

                ResourceSelectorBorder.Width = ResourceSelectorBorder.Height = actualIconSizeInPixels;

                ResourceSelectorBorder.Margin = new Thickness(x, y, 0, 0);             
            }
        }

        private void RefreshGroupsList()
        {
            BlockInfoGroupComboBox.ItemsSource = blocksManager.groups;
        }

        private void RefreshGroupImages(BlockItemModel block)
        {

            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    groupsPreviewImages[x, y].Source = null;
                }
            }

            if (block != null)
            {
                List<BlockItemModel>? blocksItems = blocksManager.GetBlockByGroupIndex(block.groupId);
                if (blocksItems != null)
                {
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
        }

        private void ResourceImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            blockEditMode = !blockEditMode;

            if (blockEditMode)
            {
                ResourceMapModeTextBlock.Text = "[Edit mode]".ToUpper();
                BlockModeTextBlock.Text = "Selected block edit";
                BlockModeTextBlock.Foreground = ResourceMapModeTextBlock.Foreground = ResourceSelectorBorder.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                ResourceMapModeTextBlock.Text = "[Select mode]".ToUpper();
                BlockModeTextBlock.Text = "Selected block view";
                ResourceSelectorBorder.BorderBrush = new SolidColorBrush(Colors.LightGreen);
                BlockModeTextBlock.Foreground = ResourceMapModeTextBlock.Foreground = new SolidColorBrush(Colors.Green);
            }
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
            blocksManager.SetBlocks(DateTime.Now.ToString(" dd-M-yyyy HH-mm-ss"));
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

        private void BlockInfoNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BlockItemModel block = (BlockItemModel)PreviewImage.Tag;
            if (block != null)
            {
                block.name = BlockInfoNameTextBox.Text;
                blocksManager.SetBlocks();
                RefreshGroupImages(block);
            }
        }
    }
}
