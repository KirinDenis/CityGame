using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGame.DataModels
{
    /// <summary>
    /// Block point class
    /// </summary>
    public class BlockPoint
    {
        /// <summary>
        /// block x location
        /// </summary>
        public int x { get; set; } = 0;

        /// <summary>
        /// block y location
        /// </summary>
        public int y { get; set; } = 0;
    }

    /// <summary>
    /// Resource block item model
    /// </summary>
    public class BlockItemModel
    {
        /// <summary>
        /// block position inside resource (image)
        /// </summary>
        public BlockPoint position { get; set; } = new BlockPoint();

        /// <summary>
        /// block name for human
        /// </summary>
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// block is part of block's group, null by default -> not in group 
        /// </summary>
        public int? groupId { get; set; }

        /// <summary>
        /// block location inside group, null by default
        /// </summary>
        public BlockPoint? groupPosition { get; set; }
    }
}
