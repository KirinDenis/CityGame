using CityGame.DTOs;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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

                    textBlock.Tag = new PositionDTO()
                    {
                        x = (ushort)x,
                        y = (ushort)y
                    };
                    textBlock.MouseDown += ResourceExplorer_MouseDown;

                    GroupViewGrid.Children.Add(textBlock);

                    textBlock.SetValue(Panel.ZIndexProperty, -1);
                    groupsPreviewImages[x, y].SetValue(Panel.ZIndexProperty, -1);

                }
            }

            DispatcherTimer animationFrameTimer = new DispatcherTimer();
            animationFrameTimer.Interval = TimeSpan.FromMilliseconds(300);
            animationFrameTimer.Tick += AnimationFrameTimer_Tick;
            animationFrameTimer.Start();

        }

        private void AnimationFrameTimer_Tick(object? sender, EventArgs e)
        {
            SpriteDTO sprite = (SpriteDTO)PreviewImage.Tag;
            if (sprite != null)
            {
                animationFrame++;
                List<SpriteDTO>? spriteItemModels = spriteBusiness.GetSpriteByGroupIndexAnimationOnly(sprite.groupId);
                if ((spriteItemModels != null) && (spriteItemModels.Count > 0))
                {
                    List<SpriteDTO>? nextFrameSprites = spriteItemModels?.FindAll(p => p.animationFrame == animationFrame);

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
            SpriteDTO sprite = (SpriteDTO)PreviewImage.Tag;

            if (sprite != null)
            {
                sprite.groupPosition = (PositionDTO)((TextBlock)sender).Tag;
                spriteBusiness.SetSprites();
                RefreshGroupImages(sprite);
            }
        }

        private PositionDTO GetSpritePositionByMouse(MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(ResourceImage);
            return new PositionDTO()
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
                PositionDTO position = GetSpritePositionByMouse(e);
                SpriteDTO? sprite = spriteBusiness.GetSpriteByPosition(GetSpritePositionByMouse(e));

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

        private void RefreshGroupImages(SpriteDTO sprite)
        {

            GroupSpritesDTO sprites = spriteBusiness.GetSpritesByGroupIndex(sprite.groupId);
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    groupsPreviewImages[x, y].Source = SpriteRepository.GetSprite(sprites.Sprites[x,y]);
                }
            }

            /*
            if (sprite != null)
            {
                List<SpriteDTO>? spritesItems = spriteBusiness.GetSpriteByGroupIndex(sprite.groupId);
                if (spritesItems != null)
                {
                    foreach (SpriteDTO spriteItem in spritesItems)
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
            */
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
                GroupDTO groupDTO = new GroupDTO()
                {
                    Name = SpriteInfoGroupComboBox.Text,
                    Width = ushort.Parse(GroupWidthTextBlock.Text),
                    Height = ushort.Parse(GroupHeightTextBlock.Text),
                    CenterX = ushort.Parse(GroupCenterXTextBlock.Text),
                    CenterY = ushort.Parse(GroupCenterYTextBlock.Text),

                };

                spriteBusiness.AddGroup(groupDTO);
                spriteBusiness.SetGroups();

                SpriteInfoGroupComboBox.ItemsSource = spriteBusiness.groups;
                SpriteInfoGroupComboBox.SelectedIndex = SpriteInfoGroupComboBox.Items.Count - 1;

                SpriteDTO sprite = (SpriteDTO)PreviewImage.Tag;
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

            if (SpritesGroupEnum.CheckGroupName(SpriteInfoGroupComboBox.Text))
            {
                GroupNotLinkedTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                GroupNotLinkedTextBlock.Visibility = Visibility.Hidden;
            }

            //TODO: read current size
            GroupWidthTextBlock.Text = 1.ToString();
            GroupHeightTextBlock.Text = 1.ToString();
            RefreshGridSizeSelector();

            SpriteDTO sprite = (SpriteDTO)PreviewImage.Tag;
            if (sprite != null)
            {
                sprite.groupId = SpriteInfoGroupComboBox.SelectedIndex;
                spriteBusiness.SetSprites();
                RefreshGroupImages(sprite);
            }
        }

        private void AnimationFrameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SpriteDTO sprite = (SpriteDTO)PreviewImage.Tag;
            if (sprite != null)
            {
                sprite.animationFrame = AnimationFrameComboBox.SelectedIndex;
                spriteBusiness.SetSprites();
                RefreshGroupImages(sprite);
            }
        }

        private void SpriteInfoNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SpriteDTO sprite = (SpriteDTO)PreviewImage.Tag;
            if (sprite != null)
            {
                sprite.name = SpriteInfoNameTextBox.Text;
                spriteBusiness.SetSprites();
                RefreshGroupImages(sprite);
            }
        }

        private void SpriteInfoGroupComboBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            SpriteInfoNameTextBox.Text = SpriteInfoNameTextBox.Text.ToLower();
        }

        private void GroupWidthUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(GroupWidthTextBlock.Text) < 7)
            {
                GroupWidthTextBlock.Text = (int.Parse(GroupWidthTextBlock.Text) + 1).ToString();
            }
            RefreshGridSizeSelector();
        }

        private void GroupWidthDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(GroupWidthTextBlock.Text) > 1)
            {
                GroupWidthTextBlock.Text = (int.Parse(GroupWidthTextBlock.Text) - 1).ToString();
            }
            RefreshGridSizeSelector();
        }

        private void GroupHeightDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(GroupHeightTextBlock.Text) > 1)
            {
                GroupHeightTextBlock.Text = (int.Parse(GroupHeightTextBlock.Text) - 1).ToString();
            }
            RefreshGridSizeSelector();
        }

        private void GroupHeightUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(GroupHeightTextBlock.Text) < 7)
            {
                GroupHeightTextBlock.Text = (int.Parse(GroupHeightTextBlock.Text) + 1).ToString();
            }
            RefreshGridSizeSelector();
        }

        private void RefreshGridSizeSelector()
        {
            GroupSizeSelectorBorder.SetValue(Grid.ColumnSpanProperty, int.Parse(GroupWidthTextBlock.Text));
            GroupSizeSelectorBorder.SetValue(Grid.RowSpanProperty, int.Parse(GroupHeightTextBlock.Text));
        }

        private void GroupCenterXDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(GroupCenterXTextBlock.Text) > 0)
            {
                GroupCenterXTextBlock.Text = (int.Parse(GroupCenterXTextBlock.Text) - 1).ToString();
            }
            RefreshGridCenterSelector();
        }
        private void GroupCenterXhUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(GroupCenterXTextBlock.Text) < 6)
            {
                GroupCenterXTextBlock.Text = (int.Parse(GroupCenterXTextBlock.Text) + 1).ToString();
            }
            RefreshGridCenterSelector();
        }

        private void GroupCenterYDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(GroupCenterYTextBlock.Text) > 0)
            {
                GroupCenterYTextBlock.Text = (int.Parse(GroupCenterYTextBlock.Text) - 1).ToString();
            }
            RefreshGridCenterSelector();
        }

        private void GroupCenterYUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(GroupCenterYTextBlock.Text) < 6)
            {
                GroupCenterYTextBlock.Text = (int.Parse(GroupCenterYTextBlock.Text) + 1).ToString();
            }
            RefreshGridCenterSelector();
        }
        private void RefreshGridCenterSelector()
        {
            GroupCenterSelectorBorder.SetValue(Grid.ColumnProperty, int.Parse(GroupCenterXTextBlock.Text));
            GroupCenterSelectorBorder.SetValue(Grid.RowProperty, int.Parse(GroupCenterYTextBlock.Text));
        }
    }
}
