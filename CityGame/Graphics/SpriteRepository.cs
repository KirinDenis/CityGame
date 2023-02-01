/* ----------------------------------------------------------------------------
Ready IoT Solution - OWLOS
Copyright 2019, 2020, 2021, 2022, 2023 by:
- Konstantin Brul (konstabrul@gmail.com)
- Vitalii Glushchenko (cehoweek@gmail.com)
- Denys Melnychuk (meldenvar@gmail.com)
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


using CityGame.DTOs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using System.Windows.Media.Imaging;

namespace CityGame.Graphics
{
    public class SpriteRepository
    {
        public static string LastError = string.Empty;

        private static string spritesImageFilePath = @"\Resources\resources256.bmp";

        private const int spriteSize = 16;

        private static ResourceInfoDTO? _resourceInfo = GetResourceInfo();
        public static ResourceInfoDTO? ResourceInfo
        {
            get
            {
                return _resourceInfo;
            }
        }
        private static Bitmap? source = null;

        private static readonly Dictionary<Point, BitmapImage> bufferBitmaps = new Dictionary<Point, BitmapImage>();
        private static readonly Dictionary<Point, byte[]> bufferPixels = new Dictionary<Point, byte[]>();

        private static ResourceInfoDTO? GetResourceInfo()
        {
            try
            {
                if (source == null)
                {
                    string resourceFileName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + spritesImageFilePath;
                    if (File.Exists(resourceFileName))
                    {
                        source = new Bitmap(resourceFileName);
                    }
                    else
                    {
                        LastError = "Resource file not exists: " + resourceFileName;
                        return null;
                    }
                }

                return new ResourceInfoDTO()
                {
                    Width = source.Width,
                    Height = source.Height,
                    CountX = source.Width / spriteSize,
                    CountY = source.Height / spriteSize,
                    SpriteSize = spriteSize
                };
            }
            catch(Exception e)
            {
                LastError = e.Message;
            }
            return null;
        }

        public static byte[] GetPixels(int x, int y)
        {
            if ((x >= ResourceInfo.CountX) || (y >= ResourceInfo.CountY))
            {
                return null;
            }

            if (bufferPixels.ContainsKey(new Point(x, y)))
            {
                return bufferPixels[new Point(x, y)];
            }

            //loading once 
            if (source == null)
            {
                source = new Bitmap(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + spritesImageFilePath);
            }

            using (MemoryStream memory = new MemoryStream())
            {
                source.Clone(new Rectangle(x * spriteSize, y * spriteSize, spriteSize, spriteSize), source.PixelFormat).Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();

                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                bufferPixels[new Point(x, y)] = new byte[16 * 16];
                bitmapimage.CopyPixels(new System.Windows.Int32Rect(0, 0, 16, 16), bufferPixels[new Point(x, y)], 16, 0);
            }
            return bufferPixels[new Point(x, y)];
        }

        public static byte[]? GetPixels(PositionDTO? position)
        {
            if (position != null)
            {
                return GetPixels(position.x, position.y);
            }
            else
            {
                return null;
            }
        }

            public static BitmapImage? GetSprite(int x, int y)
        {
            if ((ResourceInfo == null) || (x >= ResourceInfo.CountX) || (y >= ResourceInfo.CountY))
            {
                return null;
            }

            if (bufferBitmaps.ContainsKey(new Point(x, y)))
            {
                return bufferBitmaps[new Point(x, y)];
            }

            //loading once 
            if (source == null)
            {
                source = new Bitmap(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + spritesImageFilePath);
            }

            BitmapImage bitmapimage;
            using (MemoryStream memory = new MemoryStream())
            {
                source.Clone(new Rectangle(x * spriteSize, y * spriteSize, spriteSize, spriteSize), source.PixelFormat).Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
            }

            if (!bufferBitmaps.ContainsKey(new Point(x, y)))
            {
                bufferBitmaps[new Point(x, y)] = bitmapimage;
            }
            return bitmapimage;
        }

        public static BitmapImage? GetSprite(PositionDTO? position)
        {
            if (position != null)
            {
                return GetSprite(position.x, position.y);
            }
            else
            {
                return null;
            }
        }
    }
}
