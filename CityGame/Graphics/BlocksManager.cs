using CityGame.DataModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CityGame.Graphics
{
    internal class BlocksManager
    {
        private string blocksFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Resources\Blocks{0}.json";
        private string groupsFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Resources\Groups.json";

#if DEBUG
        private string developmnetBlocksFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"..\..\..\..\Resources\Blocks{0}.json";
        private string developmnetGroupsFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"..\..\..\..\Resources\Groups.json";
#endif

        private List<BlockItemModel>? _blocks;
        public List<BlockItemModel> blocks
        {
            get
            {
                if (File.Exists(string.Format(blocksFile, string.Empty)))
                {
                    _blocks = JsonConvert.DeserializeObject<List<BlockItemModel>>(File.ReadAllText(string.Format(blocksFile, string.Empty)));
                }
                if (_blocks == null)
                {
                    return new List<BlockItemModel>();
                }
                return _blocks;
            }
        }

        private List<string>? _groups;
        public List<string> groups
        {
            get
            {
                if (File.Exists(groupsFile))
                {
                    _groups = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(groupsFile));
                }
                if (_groups == null)
                {
                    _groups = new List<string>();
                    _groups.Add("NoGroup");
                }
                return _groups;
            }
        }

        public BlocksManager()
        {
            _blocks = blocks;
            _groups = groups;
        }

        public void SetBlocks(string ?backupExt = null)
        {
            File.WriteAllText(string.Format(blocksFile,backupExt), JsonConvert.SerializeObject(_blocks));
#if DEBUG
            File.WriteAllText(string.Format(developmnetBlocksFile,backupExt), JsonConvert.SerializeObject(_blocks));
#endif
        }

        public void SetGroups()
        {
            File.WriteAllText(groupsFile, JsonConvert.SerializeObject(_groups));
#if DEBUG
            File.WriteAllText(developmnetGroupsFile, JsonConvert.SerializeObject(_groups));
#endif
        }

        public BlockItemModel? GetBlockByPosition(BlockPoint position)
        {
            BlockItemModel? block = _blocks?.FirstOrDefault(p => p.position.x == position.x && p.position.y == position.y);

            if (block == null)
            {
                block = new BlockItemModel() { position = position };
                _blocks?.Add(block);
                SetBlocks();
            }
            return block;
        }

        public List<BlockItemModel>? GetBlockByGroupIndex(int gorupId)
        {
            if (gorupId > 0)
            {
                return _blocks?.FindAll(p => p.groupId == gorupId);
            }

            return new List<BlockItemModel>();
        }

        public List<BlockItemModel>? GetBlockByGroupIndexAnimationOnly(int gorupId)
        {
            if (gorupId > 0)
            {
                return _blocks?.FindAll(p => p.groupId == gorupId && p.animationFrame != 0);
            }

            return new List<BlockItemModel>();
        }

    }
}
