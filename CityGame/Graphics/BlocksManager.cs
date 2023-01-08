﻿using CityGame.DataModels;
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
        private string groupsFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Resources\Groups.json";

#if DEBUG
        private string developmnetBlocksFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"..\..\..\..\Resources\Blocks.json";
        private string developmnetGroupsFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"..\..\..\..\Resources\Groups.json";
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

        public void SetBlocks(BlockItemModel block = null)
        {
            if (block != null)
            {
                BlockItemModel existBlock = GetBlockByPosition(block.position);
                existBlock = block;
            }

           File.WriteAllText(blocksFile, JsonConvert.SerializeObject(_blocks));
#if DEBUG
            File.WriteAllText(developmnetBlocksFile, JsonConvert.SerializeObject(_blocks));
#endif
        }

        public void SetGroups()
        {
            File.WriteAllText(groupsFile, JsonConvert.SerializeObject(_groups));
#if DEBUG
            File.WriteAllText(developmnetGroupsFile, JsonConvert.SerializeObject(_groups));
#endif
        }

        public BlockItemModel GetBlockByPosition(BlockPoint position)
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

        public List<BlockItemModel> GetBlockByGroupIndex(int gorupId)
        {
            if (gorupId > 0)
            {                
                return blocks.FindAll(p => p.groupId == gorupId);
            }

            return new List<BlockItemModel>();
        }
    }
}
