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

(Этот файл — часть Ready IoT Solution - OWLOS.

OWLOS - свободная программа: вы можете перераспространять ее и/или изменять
ее на условиях Стандартной общественной лицензии GNU в том виде, в каком она
была опубликована Фондом свободного программного обеспечения; версии 3
лицензии, любой более поздней версии.

OWLOS распространяется в надежде, что она будет полезной, но БЕЗО ВСЯКИХ
ГАРАНТИЙ; даже без неявной гарантии ТОВАРНОГО ВИДА или ПРИГОДНОСТИ ДЛЯ
ОПРЕДЕЛЕННЫХ ЦЕЛЕЙ.
Подробнее см.в Стандартной общественной лицензии GNU.

Вы должны были получить копию Стандартной общественной лицензии GNU вместе с
этой программой. Если это не так, см. <https://www.gnu.org/licenses/>.)
--------------------------------------------------------------------------------------*/


using CityGame.DTOs;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using System.Windows.Media.Imaging;

namespace CityGame.Graphics
{
    public class SpriteRepository
    {
        /// <summary>
        /// Size of one sprite at sprite's image
        /// </summary>
        /// 
        public static ushort SizeInPixels = 16;
        /// <summary>
        /// sprite's count at row 
        /// </summary>
        private static short _Width = -1;

        private static short _Height = -1;

        public static int Width
        {
            get
            {
                if (_Width == -1)
                {
                    CalcCounters();
                }
                return _Width;
            }
        }

        public static int Height
        {
            get
            {
                if (_Height == -1)
                {
                    CalcCounters();
                }
                return _Height;
            }
        }

        /// <summary>
        /// Путь к файлу с иконками
        /// </summary>
        public static string spritesImageFilePath = @"\Resources\resources256.bmp";
        private static Bitmap? source = null;

        private static readonly Dictionary<Point, BitmapImage> bufferBitmaps = new Dictionary<Point, BitmapImage>();
        private static readonly Dictionary<Point, byte[]> bufferPixels = new Dictionary<Point, byte[]>();

        private static void CalcCounters()
        {
            if (source == null)
            {
                source = new Bitmap(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + spritesImageFilePath);
            }

            _Width = (short)(source.Width / SizeInPixels);
            _Height = (short)(source.Height / SizeInPixels);
        }

        public static byte[] GetPixels(int x, int y)
        {
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

                source.Clone(new Rectangle(x * SizeInPixels, y * SizeInPixels, SizeInPixels, SizeInPixels), source.PixelFormat).Save(memory, System.Drawing.Imaging.ImageFormat.Png);
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

        /// <summary>
        /// Получить иконку по относительным координатом внутри картинки с иконками
        /// Координаты НЕ В ПИКСЕЛЯХ, а в количестве иконк
        /// </summary>
        /// <param name="x">номер иконки по X от 0..Width</param>
        /// <param name="y">номер иконки по Y от 0..NULL POINTER EXCEPTION</param>
        /// <returns></returns>
        public static BitmapImage? GetSprite(int x, int y)
        {


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
                source.Clone(new Rectangle(x * SizeInPixels, y * SizeInPixels, SizeInPixels, SizeInPixels), source.PixelFormat).Save(memory, System.Drawing.Imaging.ImageFormat.Png);
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

        public static BitmapImage? GetSprite(PositionDTO position)
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

        /// <summary>
        /// Получить иконку по ее порядковому нимеру, слева направа сверху вниз
        /// </summary>
        /// <param name="number">номер иконки от 0..OUT OF RANGE</param>
        /// <returns></returns>
        public static BitmapImage? GetSprite(int number)
        {
            int x = number - number / Width * Width;
            int y = number / Width;
            return GetSprite(x, y);
        }
    }
}
