using CityGame.DTOs;
using CityGame.DTOs.Enum;
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

        //TEMP: migration
        //private string SGroupsFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"..\..\..\..\Resources\SGroups.json";
#endif

        private List<SpriteDTO>? _sprites;
        public List<SpriteDTO> sprites
        {
            get
            {
                if (_sprites == null)
                {
                    if (File.Exists(string.Format(spritesFile, string.Empty)))
                    {
                        _sprites = JsonConvert.DeserializeObject<List<SpriteDTO>>(File.ReadAllText(string.Format(spritesFile, string.Empty)));
                    }

                    if (_sprites == null)
                    {
                        return new List<SpriteDTO>();
                    }
                }
                return _sprites;
            }
        }

        private List<GroupDTO>? _groups;
        public List<GroupDTO> groups
        {
            get
            {
                if (_groups == null)
                {
                    if (File.Exists(groupsFile))
                    {
                        _groups = JsonConvert.DeserializeObject<List<GroupDTO>>(File.ReadAllText(groupsFile));
                    }
                    if (_groups == null)
                    {
                        _groups = new List<GroupDTO>();
                        _groups.Add(new GroupDTO()
                        {
                            Id = 0,
                            Name = "nogroup"
                        });
                    }
                }
                return _groups;
            }
        }

        public SpriteBusiness()
        {
            _sprites = sprites;
            _groups = groups;

            //Create default groups if empty (Restore)
            bool foundNotExists = false;
            foreach (string groupName in SpritesGroupEnum.groups)
            {
                if (GetGroupByName(groupName) == null)
                {
                    GroupDTO groupDTO = new GroupDTO()
                    {
                        Name = groupName.ToLower()
                    };
                    AddGroup(groupDTO);
                    foundNotExists = true;
                }
            }
            for (int i = 0; i < _groups.Count; i++)
            {
                //fix case
                _groups[i].Name = _groups[i].Name.ToLower();
            }

            if (foundNotExists)
            {
                SetGroups();
            }


            //Migrate grups temporary code 
            /*
            List<GroupDTO> sGroups = new List<GroupDTO>();

            for(int i=0; i < groups.Count; i++)
            {
                GroupDTO groupDTO = new GroupDTO();
                groupDTO.Id = i;
                groupDTO.Name = groups[i];

                List<SpriteDTO>? sprites = GetSpriteByGroupIndex(i);

                foreach(SpriteDTO sprite in sprites)
                {
                    while (groupDTO.Sprites.Count - 1 <sprite.animationFrame)
                    {
                        groupDTO.Sprites.Add(new GroupSpritesDTO());
                    }
                    int frame = (int)sprite?.animationFrame;

                    if (sprite.groupPosition == null)
                    {
                        sprite.groupPosition = new PositionDTO();
                    }

                    groupDTO.Sprites[frame].Sprites[(int)(sprite.groupPosition?.x), (int)(sprite.groupPosition?.y)] = sprite.position;
                }
                sGroups.Add(groupDTO);
            }
            File.WriteAllText(SGroupsFile, JsonConvert.SerializeObject(sGroups));
            */
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

        public void AddGroup(GroupDTO group)
        {
            group.Id = groups.Count - 1; //TODO: Calculate uniq Id
            groups.Add(group);
        }

        public GroupDTO? GetGroupByName(string groupItemName)
        {
            if (!string.IsNullOrEmpty(groupItemName))
            {
                return groups.FirstOrDefault(g => g.Name.ToLower().Equals(groupItemName.ToLower()));
            }
            return null; //no group by default        
        }

        public int? GetGroupId(string groupItemName)
        {
            if (!string.IsNullOrEmpty(groupItemName))
            {
                return groups.FirstOrDefault(g => g.Name.ToLower().Equals(groupItemName.ToLower()))?.Id;
            }
            return 0; //no group by default        
        }


        public SpriteDTO? GetSpriteByPosition(PositionDTO position)
        {
            SpriteDTO? sprite = _sprites?.FirstOrDefault(p => p.position.x == position.x && p.position.y == position.y);

            if (sprite == null)
            {
                sprite = new SpriteDTO() { position = position };
                _sprites?.Add(sprite);
                SetSprites();
            }
            return sprite;
        }


        public GroupSpritesDTO GetSpritesByGroupIndex(int? gorupId, int? animationFrame = 0)
        {
            if (gorupId != null)
            {
                GroupDTO? groupDTO = groups.FirstOrDefault(g => g.Id == gorupId);
                if (groupDTO != null)
                {
                    if (groupDTO.Sprites.Count < animationFrame)
                    {
                        return groupDTO.Sprites[(int)animationFrame];
                    }
                }
            }

            return new GroupSpritesDTO();
        }

        public GroupSpritesDTO GetSpritesByGroupName(string groupItemName, int? animationFrame = 0)
        {
            return GetSpritesByGroupIndex(GetGroupId(groupItemName), animationFrame);
        }

        public List<SpriteDTO>? GetSpritesByGroupPosition(List<SpriteDTO>? targetSprites, int gx, int gy)
        {
            return targetSprites?.FindAll(p => p.groupPosition != null && (p.groupPosition.x == gx && p.groupPosition.y == gy));
        }

        public PositionDTO GetSpriteOffsetByGroupPosition(List<SpriteDTO>? targetSprites, int gx, int gy, int spriteIndex = 0)
        {
            SpriteDTO? spriteModel = targetSprites?.FindAll(p => p.groupPosition != null && (p.groupPosition.x == gx && p.groupPosition.y == gy))[spriteIndex];
            if (spriteModel != null)
            {
                //return (spriteModel.position.x << 0x10) + spriteModel.position.y;
                return spriteModel.position;
            }
            else
            {
                return new PositionDTO()
                {
                    x = 0,
                    y = 0
                };
            }
        }
        public List<SpriteDTO>? GetSpriteByGroupIndexAnimationOnly(int gorupId)
        {
            if (gorupId > 0)
            {
                return _sprites?.FindAll(p => p.groupId == gorupId && p.animationFrame != 0);
            }

            return new List<SpriteDTO>();
        }

        public List<SpriteDTO> GetSpritesByOffsets(PositionDTO[] offsets)
        {
            List<SpriteDTO> findSprites = new List<SpriteDTO>();
            foreach (PositionDTO offset in offsets)
            {
                SpriteDTO? spriteItemModel = GetSpriteByPosition(new PositionDTO()
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

        public SpriteDTO[,] GetSpritesByOffsets(PositionDTO[,] offsets)
        {
            SpriteDTO[,] findSprites = new SpriteDTO[offsets.GetLength(0), offsets.GetLength(1)];

            for (int x = 0; x < offsets.GetLength(0); x++)
            {
                for (int y = 0; y < offsets.GetLength(1); y++)
                {

                    SpriteDTO? spriteItemModel = GetSpriteByPosition(new PositionDTO()
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
