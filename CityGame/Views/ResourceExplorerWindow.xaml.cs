/* ----------------------------------------------------------------------------
Ready IoT Solution - OWLOS
Copyright 2023 by:
- Denis Kirin (deniskirinacs@gmail.com)

This file is part of Ready IoT Solution - OWLOS

OWLOS is free software : you can redistribute it and/or modify it under the
terms of the GNU General Public License as published by the Free Software
Foundation, either version 3 of the License, or (at your option) any later
version.

OWLOS is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
FOR A PARTICULAR PURPOSE.
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along
with OWLOS. If not, see < https://www.gnu.org/licenses/>.

GitHub: https://github.com/KirinDenis/owlos

--------------------------------------------------------------------------------------*/

using CityGame.Data.DTO;
using CityGame.DTOs.Const;
using CityGame.Graphics;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CityGame
{
    /// <summary>
    /// Interaction logic for ResourceExplorer.xaml
    /// </summary>
    public partial class ResourceExplorerWindow : Window
    {
        private SpriteBusiness spriteBusiness = new SpriteBusiness();

        private Image[,] groupsPreviewImages = new Image[GameConsts.GroupSize, GameConsts.GroupSize];

        private bool spriteEditMode = false;
        
        private List<Border?> groupSelectorBorders = new List<Border?>();

        /// <summary>
        /// Sprite step for resource image view
        /// </summary>
        private const byte SP = 4;

        private SolidColorBrush brush = new SolidColorBrush(Colors.Yellow);
        private ColorAnimation animation = new ColorAnimation(Colors.Yellow, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), new Duration(TimeSpan.FromSeconds(2)));

        private GroupDTO? selectedGroup = null;
        private PositionDTO? selectedPosition = null;

        private int animationFrameCount = 0;
        private WriteableBitmap? animationPriviewBitmap;

        Func<bool> EngineReady = delegate ()
        {
            return SpriteRepository.ResourceInfo != null ? true : false;
        };

        public ResourceExplorerWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if ((!string.IsNullOrEmpty(SpriteRepository.LastError)) || (!EngineReady()))
            {
                ErrorTextBlock.Text = SpriteRepository.LastError;
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            SourceImageInfoTextBlock.Text = string.Format("Image {0}x{1}", SpriteRepository.ResourceInfo.Width, SpriteRepository.ResourceInfo.Height);
            SourceSpriteInfoTextBlock.Text = string.Format("Sprite size {0}x{1}", SpriteRepository.ResourceInfo.SpriteSize, SpriteRepository.ResourceInfo.SpriteSize);
            SourceCountsInfoTextBlock.Text = string.Format("Sprites map size {0}x{1}", SpriteRepository.ResourceInfo.CountX, SpriteRepository.ResourceInfo.CountY);

            int resourceWidth = SpriteRepository.ResourceInfo.CountX * (SpriteRepository.ResourceInfo.SpriteSize + SP);
            int resourceHeight = SpriteRepository.ResourceInfo.CountY * (SpriteRepository.ResourceInfo.SpriteSize + SP);

            BitmapImage? sImage = SpriteRepository.GetSprite(0, 0);

            if (sImage != null)
            {
                WriteableBitmap bitmapSource = new WriteableBitmap(
                    resourceWidth,
                    resourceHeight,
                    sImage.DpiX, sImage.DpiY, sImage.Format, sImage.Palette);

                animationPriviewBitmap = new WriteableBitmap(
                    SpriteRepository.ResourceInfo.SpriteSize * GameConsts.GroupSize,
                    SpriteRepository.ResourceInfo.SpriteSize * GameConsts.GroupSize,
                    sImage.DpiX, sImage.DpiY, sImage.Format, sImage.Palette);

                int spritePlaceSize = ((SpriteRepository.ResourceInfo.SpriteSize + SP) * (SpriteRepository.ResourceInfo.SpriteSize + SP)) * (sImage.Format.BitsPerPixel >> 0b10);
                byte[] spriteBackground = new byte[spritePlaceSize];
                for (int i = 0; i < spritePlaceSize; i++)
                {
                    spriteBackground[i] = 0xFF;
                }

                for (int x = 0; x < SpriteRepository.ResourceInfo.CountX; x++)
                {
                    for (int y = 0; y < SpriteRepository.ResourceInfo.CountY; y++)
                    {
                        Int32Rect rect = new Int32Rect(x * (SpriteRepository.ResourceInfo.SpriteSize + SP), y * (SpriteRepository.ResourceInfo.SpriteSize + SP), SpriteRepository.ResourceInfo.SpriteSize + SP, SpriteRepository.ResourceInfo.SpriteSize + SP);
                        bitmapSource.WritePixels(rect, spriteBackground, (SpriteRepository.ResourceInfo.SpriteSize + SP) * 4, 0);

                        rect = new Int32Rect(x * (SpriteRepository.ResourceInfo.SpriteSize + SP) + (SP >> 1), y * (SpriteRepository.ResourceInfo.SpriteSize + SP) + (SP >> 1), SpriteRepository.ResourceInfo.SpriteSize, SpriteRepository.ResourceInfo.SpriteSize);

                        bitmapSource.WritePixels(rect, SpriteRepository.GetPixels(x, y), SpriteRepository.ResourceInfo.SpriteSize * 4, 0);
                    }
                }

                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.DrawImage(bitmapSource, new Rect(0, 0, resourceWidth, resourceHeight));
                    drawingContext.Close();
                }

                ResourceImage.Source = new DrawingImage(drawingVisual.Drawing);
                RefreshGroupsList();

                for (int x = 0; x < GameConsts.GroupSize; x++)
                {
                    for (int y = 0; y < GameConsts.GroupSize; y++)
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
                        textBlock.PreviewMouseDown += GroupSrite_MouseDown;

                        GroupViewGrid.Children.Add(textBlock);

                        textBlock.SetValue(Panel.ZIndexProperty, -1);
                        groupsPreviewImages[x, y].SetValue(Panel.ZIndexProperty, -1);
                    }
                }

                brush = new SolidColorBrush(Colors.Red);
                animation = new ColorAnimation(Color.FromArgb(0xF0, 0xFF, 0x00, 0x00), Color.FromArgb(0x40, 0xFF, 0x00, 0x00), new Duration(TimeSpan.FromSeconds(0.5)));
                animation.AutoReverse = true;
                animation.RepeatBehavior = RepeatBehavior.Forever;
                brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(300);
                timer.Tick += Timer_Tick;
                timer.Start();
            }
            else
            {
                ErrorTextBlock.Text = "Error parsing resource image file";
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }
        
        private PositionDTO? GetSpritePositionByMouse(MouseEventArgs e)
        {
            if (!EngineReady())
            {
                return null;
            }

            Point mousePosition = e.GetPosition(ResourceImage);
            return new PositionDTO()
            {
                x = (ushort)((mousePosition.X / (ResourceImage.ActualWidth / ResourceImage.Source.Width)) / (SpriteRepository.ResourceInfo.SpriteSize + SP)),
                y = (ushort)((mousePosition.Y / (ResourceImage.ActualHeight / ResourceImage.Source.Height)) / (SpriteRepository.ResourceInfo.SpriteSize + SP))
            };
        }

        private void RefreshGroupsList()
        {
            SpriteGroupsTreeView.Items.Clear();

            if (!EngineReady())
            {
                return;
            }

            foreach (GroupDTO group in spriteBusiness.groups)
            {
                TreeViewItem groupItem = new TreeViewItem();
                groupItem.Tag = group;
                groupItem.Header = group.Name;

                if ((selectedGroup != null) && (selectedGroup == group))
                {
                    groupItem.IsSelected = true;
                }

                //Group sprites with animation frames to TreeViewItem 
                foreach (GroupSpritesDTO groupSprites in group.Sprites)
                {
                    //Frame tree item
                    TreeViewItem frameItem = new TreeViewItem();
                    //Frame grid 8x8
                    Grid frameGrid = new Grid();

                    frameGrid.Width = frameGrid.Height = SpriteRepository.ResourceInfo.SpriteSize * GameConsts.GroupSize;

                    for (int x = 0; x < groupSprites.Sprites.GetLength(0); x++)
                    {
                        frameGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(SpriteRepository.ResourceInfo.SpriteSize) });
                        for (int y = 0; y < groupSprites.Sprites.GetLength(1); y++)
                        {
                            frameGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(SpriteRepository.ResourceInfo.SpriteSize) });
                            if (groupSprites.Sprites[x, y] != null)
                            {
                                Image spriteImage = new Image();
                                spriteImage.SetValue(Grid.ColumnProperty, x);
                                spriteImage.SetValue(Grid.RowProperty, y);

                                spriteImage.Width = spriteImage.Height = SpriteRepository.ResourceInfo.SpriteSize;

                                RenderOptions.SetBitmapScalingMode(spriteImage, BitmapScalingMode.NearestNeighbor);

                                spriteImage.Source = SpriteRepository.GetSprite(groupSprites.Sprites[x, y]);

                                frameGrid.Children.Add(spriteImage);
                            }
                            else
                            {
                                Border border = new Border();
                                border.BorderBrush = Brushes.Gray;
                                border.BorderThickness = new Thickness(1);
                                border.SetValue(Grid.ColumnProperty, x);
                                border.SetValue(Grid.RowProperty, y);

                                frameGrid.Children.Add(border);
                            }
                        }
                    }
                    frameItem.Items.Add(frameGrid);
                    groupItem.Items.Add(frameItem);
                    frameItem.Header = "Animation frame " + (groupItem.Items.Count);
                }
                groupItem.Selected += GroupItem_Selected;
                SpriteGroupsTreeView.Items.Add(groupItem);
            }
        }

        private void RefreshGroupSprites(GroupDTO? group)
        {
            if (group == null)
            {
                for (int x = 0; x < GameConsts.GroupSize; x++)
                {
                    for (int y = 0; y < GameConsts.GroupSize; y++)
                    {
                        groupsPreviewImages[x, y].Source = null;
                    }
                }
                return;
            }

            GroupSpritesDTO sprites = spriteBusiness.GetSpritesByGroupIndex(group.Id, AnimationFrameComboBox.SelectedIndex);
            for (int x = 0; x < GameConsts.GroupSize; x++)
            {
                for (int y = 0; y < GameConsts.GroupSize; y++)
                {
                    groupsPreviewImages[x, y].Source = SpriteRepository.GetSprite(sprites.Sprites[x, y]);
                }
            }
        }

        private void GroupSrite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedGroup != null)
            {
                PositionDTO groupPosition = (PositionDTO)((TextBlock)sender).Tag;

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (selectedPosition != null)
                    {
                        if (selectedGroup.Sprites.Count == 0)
                        {
                            AnimationFrameComboBox.Items.Add(1);
                            AnimationFrameComboBox.SelectedIndex = 0;
                            selectedGroup.Sprites.Add(new GroupSpritesDTO());
                            spriteBusiness.SetGroups();
                        }
                        selectedGroup.Sprites[AnimationFrameComboBox.SelectedIndex].Sprites[groupPosition.x, groupPosition.y] = selectedPosition;
                    }
                }
                else
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    selectedGroup.Sprites[AnimationFrameComboBox.SelectedIndex].Sprites[groupPosition.x, groupPosition.y] = null;
                }
                spriteBusiness.SetGroups();
                RefreshGroupsList();
                RefreshGroupSprites(selectedGroup);
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (!EngineReady())
            {
                return;
            }

            if (AnimationPreviewImage.Tag != selectedGroup)
            {
                animationPriviewBitmap?.WritePixels(new Int32Rect()
                {
                    X = 0,
                    Y = 0,
                    Width = animationPriviewBitmap.PixelWidth,
                    Height = animationPriviewBitmap.PixelHeight
                }, Enumerable.Repeat((byte)0xFF, animationPriviewBitmap.PixelWidth * animationPriviewBitmap.PixelHeight * (animationPriviewBitmap.Format.BitsPerPixel / 8)).ToArray(), animationPriviewBitmap.PixelWidth * 4, 0);

                AnimationPreviewImage.Tag = selectedGroup;
                animationFrameCount = 0;
            }
            if (selectedGroup != null)
            {
                if (animationFrameCount > selectedGroup.Sprites.Count - 1)
                {
                    animationFrameCount = 0;
                }

                GroupSpritesDTO sprites = spriteBusiness.GetSpritesByGroupIndex(selectedGroup.Id, animationFrameCount);
                for (int x = 0; x < GameConsts.GroupSize; x++)
                {
                    for (int y = 0; y < GameConsts.GroupSize; y++)
                    {
                        Int32Rect rect = new Int32Rect(x * SpriteRepository.ResourceInfo.SpriteSize, y * SpriteRepository.ResourceInfo.SpriteSize, SpriteRepository.ResourceInfo.SpriteSize, SpriteRepository.ResourceInfo.SpriteSize);

                        byte[]? pixels = SpriteRepository.GetPixels(sprites.Sprites[x, y]);

                        if (pixels != null)
                        {
                            animationPriviewBitmap?.WritePixels(rect, pixels, SpriteRepository.ResourceInfo.SpriteSize * 4, 0);
                        }
                    }
                }
                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.DrawImage(animationPriviewBitmap, new Rect(0, 0, SpriteRepository.ResourceInfo.SpriteSize * GameConsts.GroupSize, SpriteRepository.ResourceInfo.SpriteSize * GameConsts.GroupSize));
                    drawingContext.Close();
                }
                AnimationPreviewImage.Source = new DrawingImage(drawingVisual.Drawing);

                animationFrameCount++;
            }
        }

        /// <summary>
        /// Select sprite by mouse move over source image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResourceImage_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!EngineReady())
            {
                return;
            }

            if (!spriteEditMode)
            {
                PositionDTO? position = GetSpritePositionByMouse(e);

                if (position == null)
                {
                    return;
                }

                GroupDTO? group = spriteBusiness.GetGroupBySpritePosition(position);

                if ((group == null) && (SpriteGroupsTreeView.SelectedItem != null))
                {
                    group = (GroupDTO)(((TreeViewItem)SpriteGroupsTreeView.SelectedItem).Tag);
                }

                selectedPosition = position;
                selectedGroup = group;

                PreviewImage.Source = SpriteRepository.GetSprite(position.x, position.y);

                SpriteInfoPositionTextBlock.Text = string.Format("{0}:{1}", position.x, position.y);

                if (group != null)
                {
                    foreach (TreeViewItem treeViewItem in SpriteGroupsTreeView.Items)
                    {
                        if (treeViewItem.Tag == group)
                        {
                            treeViewItem.IsSelected = true;
                            break;
                        }
                    }
                }
                else
                {
                    //  (SpriteGroupsTreeView.Items[0] as TreeViewItem).IsSelected = true;
                }

                //Resourve image selector
                Point actualMargin = ResourceImage.TransformToAncestor(MainGrid).Transform(new Point(0, 0));             
                actualMargin.Y -= MainGrid.RowDefinitions[0].ActualHeight;

                double actualSpriteSizeInPixels = ResourceImage.ActualWidth / SpriteRepository.ResourceInfo.CountX;

                double x = e.GetPosition(ResourceImage).X - (e.GetPosition(ResourceImage).X % actualSpriteSizeInPixels) + actualMargin.X;
                double y = e.GetPosition(ResourceImage).Y - (e.GetPosition(ResourceImage).Y % actualSpriteSizeInPixels) + actualMargin.Y;

                ResourceSelectorBorder.Width = ResourceSelectorBorder.Height = actualSpriteSizeInPixels;
                ResourceSelectorBorder.Margin = new Thickness(x, y, 0, 0);
            }
        }

        private void ResourceImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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
                ResourceImage_PreviewMouseMove(sender, e);
            }
        }

        private void AnimationFrameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshGroupSprites(selectedGroup);
        }

        private void GroupItem_Selected(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < groupSelectorBorders.Count; i++)
            {
                MainGrid.Children.Remove(groupSelectorBorders[i]);
                groupSelectorBorders[i] = null;
            }
            groupSelectorBorders.Clear();

            if (!EngineReady())
            {
                return;
            }

            selectedGroup = (GroupDTO)((TreeViewItem)sender).Tag;
            if (selectedGroup != null)
            {

                // Create the Border that is the target of the animation.
                SolidColorBrush animatedBrush = new SolidColorBrush();
                animatedBrush.Color = Color.FromArgb(255, 0, 255, 0);

                foreach (GroupSpritesDTO groupSprites in selectedGroup.Sprites)
                {
                    for (int x = 0; x < groupSprites.Sprites.GetLength(0); x++)
                    {
                        for (int y = 0; y < groupSprites.Sprites.GetLength(1); y++)
                        {
                            if (groupSprites.Sprites[x, y] != null)
                            {
                                Border border = new Border();

                                double WH = ResourceImage.ActualHeight / SpriteRepository.ResourceInfo.CountX;

                                border.Width = border.Height = WH;

                                Point actualMargin = ResourceImage.TransformToAncestor(MainGrid).Transform(new Point(0, 0));
                                actualMargin.X -= MainGrid.ColumnDefinitions[0].ActualWidth;
                                actualMargin.Y -= MainGrid.RowDefinitions[0].ActualHeight;

                                border.Margin = new Thickness(groupSprites.Sprites[x, y].x * WH + actualMargin.X, groupSprites.Sprites[x, y].y * WH + actualMargin.Y, 0, 0);
                                border.HorizontalAlignment = HorizontalAlignment.Left;
                                border.VerticalAlignment = VerticalAlignment.Top;
                                border.SetValue(Grid.ColumnProperty, 1);
                                border.SetValue(Grid.RowProperty, 1);
                                border.BorderThickness = new Thickness(SP / 2);
                                border.BorderBrush = brush;

                                groupSelectorBorders.Add(border);
                                MainGrid.Children.Add(border);
                            }
                        }
                    }
                }
            }

            if (selectedGroup != null)
            {
                GroupWidthTextBlock.Text = selectedGroup.Width.ToString();
                GroupHeightTextBlock.Text = selectedGroup.Height.ToString();
                GroupCenterXTextBlock.Text = selectedGroup.CenterX.ToString();
                GroupCenterYTextBlock.Text = selectedGroup.CenterY.ToString();
                RefreshGridSizeSelector();

                AnimationFrameComboBox.Items.Clear();
                for (int i = 0; i < selectedGroup.Sprites.Count; i++)
                {
                    AnimationFrameComboBox.Items.Add(i + 1);
                }
                AnimationFrameComboBox.SelectedIndex = 0;
            }
            RefreshGroupSprites(selectedGroup);
        }

        private void GroupWidthUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(GroupWidthTextBlock.Text) < GameConsts.GroupSize)
            {
                GroupWidthTextBlock.Text = (ushort.Parse(GroupWidthTextBlock.Text) + 1).ToString();
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
            if (int.Parse(GroupHeightTextBlock.Text) < GameConsts.GroupSize)
            {
                GroupHeightTextBlock.Text = (int.Parse(GroupHeightTextBlock.Text) + 1).ToString();
            }
            RefreshGridSizeSelector();
        }

        private void RefreshGridSizeSelector()
        {
            GroupSizeSelectorBorder.SetValue(Grid.ColumnSpanProperty, int.Parse(GroupWidthTextBlock.Text));
            GroupSizeSelectorBorder.SetValue(Grid.RowSpanProperty, int.Parse(GroupHeightTextBlock.Text));
            GroupCenterSelectorBorder.SetValue(Grid.ColumnProperty, int.Parse(GroupCenterXTextBlock.Text));
            GroupCenterSelectorBorder.SetValue(Grid.RowProperty, int.Parse(GroupCenterYTextBlock.Text));

            if (selectedGroup != null)
            {
                selectedGroup.Width = ushort.Parse(GroupWidthTextBlock.Text);
                selectedGroup.Height = ushort.Parse(GroupHeightTextBlock.Text);
                selectedGroup.CenterX = ushort.Parse(GroupCenterXTextBlock.Text);
                selectedGroup.CenterY = ushort.Parse(GroupCenterYTextBlock.Text);
            }
        }

        private void GroupCenterXDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(GroupCenterXTextBlock.Text) > 0)
            {
                GroupCenterXTextBlock.Text = (int.Parse(GroupCenterXTextBlock.Text) - 1).ToString();
            }
            RefreshGridSizeSelector();
        }
        private void GroupCenterXhUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(GroupCenterXTextBlock.Text) < 6)
            {
                GroupCenterXTextBlock.Text = (int.Parse(GroupCenterXTextBlock.Text) + 1).ToString();
            }
            RefreshGridSizeSelector();
        }

        private void GroupCenterYDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(GroupCenterYTextBlock.Text) > 0)
            {
                GroupCenterYTextBlock.Text = (int.Parse(GroupCenterYTextBlock.Text) - 1).ToString();
            }
            RefreshGridSizeSelector();
        }

        private void GroupCenterYUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(GroupCenterYTextBlock.Text) < 6)
            {
                GroupCenterYTextBlock.Text = (int.Parse(GroupCenterYTextBlock.Text) + 1).ToString();
            }
            RefreshGridSizeSelector();
        }

        private void AddAnimationFrameButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGroup != null)
            {
                selectedGroup.Sprites.Add(new GroupSpritesDTO());
                spriteBusiness.SetGroups();
                RefreshGroupsList();
                GroupItem_Selected(SpriteGroupsTreeView.SelectedItem, null);
                AnimationFrameComboBox.SelectedIndex = selectedGroup.Sprites.Count - 1;
            }
        }

        private void DeleteAnimationFrameButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGroup != null)
            {
                selectedGroup.Sprites.RemoveAt(AnimationFrameComboBox.SelectedIndex);
                spriteBusiness.SetGroups();
                RefreshGroupsList();
                GroupItem_Selected(SpriteGroupsTreeView.SelectedItem, null);
            }

        }

        private void AutoSelectSpritesButton_Click(object sender, RoutedEventArgs e)
        {
            if ((selectedPosition != null) && (selectedGroup != null))
            {
                int offset = selectedPosition.y * SpriteRepository.ResourceInfo.CountX + selectedPosition.x;

                int gx = 0;
                int gy = 0;

                for (int i = offset; i < offset + 3 * 3; i++)
                {
                    PositionDTO selPposition = new PositionDTO()
                    {
                        x = (ushort)(i - (i / SpriteRepository.ResourceInfo.CountX * SpriteRepository.ResourceInfo.CountX)),
                        y = (ushort)(i / SpriteRepository.ResourceInfo.CountX)
                    };

                    if (selectedGroup.Sprites.Count == 0)
                    {
                        selectedGroup.Sprites.Add(new GroupSpritesDTO());
                        AnimationFrameComboBox.Items.Add(1);
                        AnimationFrameComboBox.SelectedIndex = 0;
                    }

                    selectedGroup.Sprites[AnimationFrameComboBox.SelectedIndex].Sprites[gx, gy] = selPposition;
                    gx++;
                    if (gx > 2)
                    {
                        gx = 0;
                        gy++;
                    }
                }
            }
            spriteBusiness.SetGroups();
            RefreshGroupsList();
            GroupItem_Selected(SpriteGroupsTreeView.SelectedItem, null);
        }
    }
}
