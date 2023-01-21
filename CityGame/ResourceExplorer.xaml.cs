﻿using CityGame.DataModels;
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
        private SpriteBusiness spriteBusiness = new SpriteBusiness();

        private Image[,] groupsPreviewImages = new Image[7, 7];

        private bool spriteEditMode = false;

        private int animationFrame = 1;
        public ResourceExplorer()
        {
            InitializeComponent();

            SourceImageInfoTextBlock.Text = string.Format("Image {0}x{1}", ResourceImage.Source.Width, ResourceImage.Source.Height);
            SourceSpriteInfoTextBlock.Text = string.Format("Sprite {0}x{1}", SpriteRepository.SizeInPixels, SpriteRepository.SizeInPixels);
            SourceCountsInfoTextBlock.Text = string.Format("Counts {0}x{1}", SpriteRepository.Width, SpriteRepository.Height);
           
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

                    textBlock.Tag = new PositionModel()
                    {
                        x = (ushort)x,
                        y = (ushort)y
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
            SpriteModel sprite = (SpriteModel)PreviewImage.Tag;
            if (sprite != null)
            {
                animationFrame++;
                List<SpriteModel>? spriteItemModels = spriteBusiness.GetSpriteByGroupIndexAnimationOnly(sprite.groupId);
                if ((spriteItemModels != null) && (spriteItemModels.Count > 0))
                {
                    List<SpriteModel>? nextFrameSprites = spriteItemModels?.FindAll(p => p.animationFrame == animationFrame);

                    if ((nextFrameSprites == null) || (nextFrameSprites?.Count == 0))
                    {
                        animationFrame = 1;
                        nextFrameSprites = spriteItemModels?.FindAll(p => p.animationFrame == animationFrame);
                    }
                    if ((nextFrameSprites != null) || (nextFrameSprites?.Count == 0))
                    {
                        foreach (var spriteItemModel in nextFrameSprites)
                        {
                            if (spriteItemModel.groupPosition != null)
                            {
                                groupsPreviewImages[spriteItemModel.groupPosition.x, spriteItemModel.groupPosition.y].Source
                                    = SpriteRepository.GetSprite(spriteItemModel.position.x, spriteItemModel.position.y);
                            }
                        }
                    }
                }
            }
        }


        private void ResourceExplorer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SpriteModel sprite = (SpriteModel)PreviewImage.Tag;

            if (sprite != null)
            {
                sprite.groupPosition = (PositionModel)((TextBlock)sender).Tag;
                spriteBusiness.SetSprites();
                RefreshGroupImages(sprite);
            }
        }

        private PositionModel GetSpritePositionByMouse(MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(ResourceImage);
            return new PositionModel()
            {
                x = (ushort)((mousePosition.X / (ResourceImage.ActualWidth / ResourceImage.Source.Width)) / SpriteRepository.SizeInPixels),
                y = (ushort)((mousePosition.Y / (ResourceImage.ActualHeight / ResourceImage.Source.Height)) / SpriteRepository.SizeInPixels)
            };
        }

        /// <summary>
        /// Select sprite by mouse move over source image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResourceImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (!spriteEditMode)
            {
                PositionModel position = GetSpritePositionByMouse(e);
                SpriteModel? sprite = spriteBusiness.GetSpriteByPosition(GetSpritePositionByMouse(e));

                PreviewImage.Tag = sprite;
                PreviewImage.Source = SpriteRepository.GetSprite(position.x, position.y);

                SpriteInfoPositionTextBlock.Text = string.Format("{0}:{1}", sprite.position.x, sprite.position.y);

                SpriteInfoNameTextBox.Text = sprite.name;

                SpriteInfoGroupComboBox.SelectedIndex = sprite.groupId;

                AnimationFrameComboBox.SelectedIndex = sprite.animationFrame ?? 0;

                RefreshGroupImages(sprite);

                //Resourve image selector
                Point actualMargin = ResourceImage.TransformToAncestor(MainGrid).Transform(new Point(0, 0));
                actualMargin.X -= MainGrid.ColumnDefinitions[0].ActualWidth;
                actualMargin.Y -= MainGrid.RowDefinitions[0].ActualHeight;

                double actualSpriteSizeInPixels = ResourceImage.ActualWidth / SpriteRepository.Width;

                double x = e.GetPosition(ResourceImage).X - (e.GetPosition(ResourceImage).X % actualSpriteSizeInPixels) + actualMargin.X;
                double y = e.GetPosition(ResourceImage).Y - (e.GetPosition(ResourceImage).Y % actualSpriteSizeInPixels) + actualMargin.Y;

                ResourceSelectorBorder.Width = ResourceSelectorBorder.Height = actualSpriteSizeInPixels;

                ResourceSelectorBorder.Margin = new Thickness(x, y, 0, 0);             
            }
        }

        private void RefreshGroupsList()
        {
            SpriteInfoGroupComboBox.ItemsSource = spriteBusiness.groups;
        }

        private void RefreshGroupImages(SpriteModel sprite)
        {

            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    groupsPreviewImages[x, y].Source = null;
                }
            }

            if (sprite != null)
            {
                List<SpriteModel>? spritesItems = spriteBusiness.GetSpriteByGroupIndex(sprite.groupId);
                if (spritesItems != null)
                {
                    foreach (SpriteModel spriteItem in spritesItems)
                    {
                        if (spriteItem.groupPosition != null)
                        {
                            if ((spriteItem.groupPosition.x >= 0) && (spriteItem.groupPosition.y >= 0))
                            {
                                groupsPreviewImages[spriteItem.groupPosition.x, spriteItem.groupPosition.y].Source = SpriteRepository.GetSprite(spriteItem.position.x, spriteItem.position.y);
                            }
                        }
                    }
                }
            }
        }

        private void ResourceImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            spriteEditMode = !spriteEditMode;

            if (spriteEditMode)
            {
                ResourceMapModeTextBlock.Text = "[Edit mode]".ToUpper();
                SpriteModeTextBlock.Text = "Selected sprite edit";
                SpriteModeTextBlock.Foreground = ResourceMapModeTextBlock.Foreground = ResourceSelectorBorder.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                ResourceMapModeTextBlock.Text = "[Select mode]".ToUpper();
                SpriteModeTextBlock.Text = "Selected sprite view";
                ResourceSelectorBorder.BorderBrush = new SolidColorBrush(Colors.LightGreen);
                SpriteModeTextBlock.Foreground = ResourceMapModeTextBlock.Foreground = new SolidColorBrush(Colors.Green);
            }
        }

        private void NewSprite_Click(object sender, RoutedEventArgs e)
        {
            if (spriteBusiness.groups.Find(p => p.Equals(SpriteInfoGroupComboBox.Text)) == null)
            {
                spriteBusiness.groups.Add(SpriteInfoGroupComboBox.Text);
                spriteBusiness.SetGroups();

                SpriteInfoGroupComboBox.ItemsSource = spriteBusiness.groups;
                SpriteInfoGroupComboBox.SelectedIndex = SpriteInfoGroupComboBox.Items.Count - 1;

                SpriteModel sprite = (SpriteModel)PreviewImage.Tag;
                sprite.groupId = SpriteInfoGroupComboBox.SelectedIndex;
                spriteBusiness.SetSprites();
                RefreshGroupImages(sprite);
            }

        }

        private void SaveSpritesButton_Click(object sender, RoutedEventArgs e)
        {
            spriteBusiness.SetSprites(DateTime.Now.ToString(" dd-M-yyyy HH-mm-ss"));
        }

        private void SpriteInfoGroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SpriteModel sprite = (SpriteModel)PreviewImage.Tag;
            if (sprite != null)
            {
                sprite.groupId = SpriteInfoGroupComboBox.SelectedIndex;
                spriteBusiness.SetSprites();
                RefreshGroupImages(sprite);
            }
        }

        private void AnimationFrameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SpriteModel sprite = (SpriteModel)PreviewImage.Tag;
            if (sprite != null)
            {
                sprite.animationFrame = AnimationFrameComboBox.SelectedIndex;
                spriteBusiness.SetSprites();
                RefreshGroupImages(sprite);
            }
        }

        private void SpriteInfoNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SpriteModel sprite = (SpriteModel)PreviewImage.Tag;
            if (sprite != null)
            {
                sprite.name = SpriteInfoNameTextBox.Text;
                spriteBusiness.SetSprites();
                RefreshGroupImages(sprite);
            }
        }
    }
}
