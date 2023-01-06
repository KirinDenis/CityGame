using CityGame.DataModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace CityGame.Graphics
{
    internal class BlocksManager
    {
        private string blocksFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Resources\Blocks.json";

#if DEBUG
        private string developmnetBlocksFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"..\..\..\..\Resources\Blocks.json";
#endif

        private List<BlockItemModel>? _blocks;
        public List<BlockItemModel> blocks
        {
            get
            {                
                if (File.Exists(blocksFile))
                {
                    _blocks = JsonConvert.DeserializeObject<List<BlockItemModel>>(File.ReadAllText(blocksFile));                                                     
                }
                if (_blocks == null)
                {
                    return new List<BlockItemModel>();
                }    
                return _blocks;
            }            
        }

        public BlocksManager()
        {
            _blocks = blocks;
        }

        public void SetBlocks()
        {
           File.WriteAllText(blocksFile, JsonConvert.SerializeObject(_blocks));
#if DEBUG
            File.WriteAllText(developmnetBlocksFile, JsonConvert.SerializeObject(_blocks));
#endif
        }
        public BlockItemModel GetBlockInfoByPosition(BlockPoint position)
        {
            BlockItemModel? block = blocks.FirstOrDefault(p => p.position.x == position.x && p.position.y == position.y);

            if (block == null)
            {
                block = new BlockItemModel() { position = position };
                _blocks?.Add(block);
                SetBlocks();
            }
            return block;
        }
    }
}
