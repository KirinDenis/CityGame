using CityGame.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGame.Graphics
{
    internal class BlocksManager
    {
        private List<BlockItemModel>? blocks;

        public List<BlockItemModel> GetBlocks()
        {
            if (blocks == null)
            {
                blocks = new List<BlockItemModel>();
            }


            return blocks;
        }
    }
}
