using CityGame.DataModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CityGame.Graphics
{
    public class SpriteBusiness
    {
        private string spritesFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Resources\Blocks{0}.json";
        private string groupsFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Resources\Groups.json";

#if DEBUG
        private string developmnetSpritesFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"..\..\..\..\Resources\Blocks{0}.json";
        private string developmnetGroupsFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"..\..\..\..\Resources\Groups.json";
#endif

        private List<SpriteModel>? _sprites;
        public List<SpriteModel> sprites
        {
            get
            {
                if (File.Exists(string.Format(spritesFile, string.Empty)))
                {
                    _sprites = JsonConvert.DeserializeObject<List<SpriteModel>>(File.ReadAllText(string.Format(spritesFile, string.Empty)));
                }
                if (_sprites == null)
                {
                    return new List<SpriteModel>();
                }
                return _sprites;
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

        public SpriteBusiness()
        {
            _sprites = sprites;
            _groups = groups;
        }

        public void SetSprites(string? backupExt = null)
        {
            File.WriteAllText(string.Format(spritesFile, backupExt), JsonConvert.SerializeObject(_sprites));
#if DEBUG
            File.WriteAllText(string.Format(developmnetSpritesFile, backupExt), JsonConvert.SerializeObject(_sprites));
#endif
        }

        public void SetGroups()
        {
            File.WriteAllText(groupsFile, JsonConvert.SerializeObject(_groups));
#if DEBUG
            File.WriteAllText(developmnetGroupsFile, JsonConvert.SerializeObject(_groups));
#endif
        }

        public SpriteModel? GetSpriteByPosition(PositionModel position)
        {
            SpriteModel? sprite = _sprites?.FirstOrDefault(p => p.position.x == position.x && p.position.y == position.y);

            if (sprite == null)
            {
                sprite = new SpriteModel() { position = position };
                _sprites?.Add(sprite);
                SetSprites();
            }
            return sprite;
        }

        public List<SpriteModel>? GetSpriteByGroupIndex(int? gorupId)
        {
            if (gorupId > 0)
            {
                return _sprites?.FindAll(p => p.groupId == gorupId);
            }

            return new List<SpriteModel>();
        }

        public int? GetGroupId(string groupItemName)
        {
            return groups?.IndexOf(groupItemName);
        }

        public List<SpriteModel>? GetSpriteByGroupName(string groupItemName)
        {
            return GetSpriteByGroupIndex(GetGroupId(groupItemName));
        }

        public List<SpriteModel>? GetSpritesByGroupPosition(List<SpriteModel>? targetSprites, int gx, int gy)
        {
            return targetSprites?.FindAll(p => p.groupPosition != null && (p.groupPosition.x == gx && p.groupPosition.y == gy));
        }

        public PositionModel GetSpriteOffsetByGroupPosition(List<SpriteModel>? targetSprites, int gx, int gy, int spriteIndex = 0)
        {
            SpriteModel? spriteModel = targetSprites?.FindAll(p => p.groupPosition != null && (p.groupPosition.x == gx && p.groupPosition.y == gy))[spriteIndex];
            if (spriteModel != null)
            {
                //return (spriteModel.position.x << 0x10) + spriteModel.position.y;
                return spriteModel.position;
            }
            else
            {
                return new PositionModel()
                {
                    x = 0,
                    y = 0
                };
            }
        }
        public List<SpriteModel>? GetSpriteByGroupIndexAnimationOnly(int gorupId)
        {
            if (gorupId > 0)
            {
                return _sprites?.FindAll(p => p.groupId == gorupId && p.animationFrame != 0);
            }

            return new List<SpriteModel>();
        }

        public List<SpriteModel> GetSpritesByOffsets(PositionModel[] offsets)
        {
            List<SpriteModel> findSprites = new List<SpriteModel>();
            foreach (PositionModel offset in offsets)
            {
                SpriteModel? spriteItemModel = GetSpriteByPosition(new PositionModel()
                {
                    x = offset.x,
                    y = offset.y
                }
                );
                if (spriteItemModel == null)
                {
                    continue;
                }
                findSprites.Add(spriteItemModel);
            }

            return findSprites;
        }

        public SpriteModel[,] GetSpritesByOffsets(PositionModel[,] offsets)
        {
            SpriteModel[,] findSprites = new SpriteModel[offsets.GetLength(0), offsets.GetLength(1)];

            for (int x = 0; x < offsets.GetLength(0); x++)
            {
                for (int y = 0; y < offsets.GetLength(1); y++)
                {

                    SpriteModel? spriteItemModel = GetSpriteByPosition(new PositionModel()
                    {
                        x = offsets[x, y].x,
                        y = offsets[x, y].y
                    }
                );
                    if (spriteItemModel == null)
                    {
                        continue;
                    }
                    findSprites[x, y] = spriteItemModel;
                }
            }

            return findSprites;
        }



    }
}
