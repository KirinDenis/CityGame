using CityGame.DTOs;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using CityGame.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGame.Models
{
    public class BaseModel 
    {
        private const int left = -1;
        private const int top = -1;

        private const int center = 0;

        private const int right = 1;
        private const int bottom = 1;

        /// <summary>
        /// Inline Left, Top
        /// </summary>
        protected Func<ushort, ushort> CLeft = delegate (ushort x)
        {
            return --x;
        };

        protected Func<ushort, ushort> CTop = delegate (ushort y)
        {
            return --y;
        };

        /// <summary>
        /// Inline Right, Bottom
        /// </summary>
        protected Func<ushort, ushort> CRight = delegate (ushort x)
        {
            return ++x;
        };

        protected Func<ushort, ushort> CBottom = delegate (ushort y)
        {
            return ++y;
        };

        Func<int, int, int> CoordToOffset = delegate (int x, int y)
        {
            return (x << 0x10) + y;
        };

        Func<int, int> OffsetToX = delegate (int flat)
        {
            return flat >> 0x10;
        };

        Func<int, int> OffsetToY = delegate (int flat)
        {
            return flat & 0xFF;
        };


    }
}
