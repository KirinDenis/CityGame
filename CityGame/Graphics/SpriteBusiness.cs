﻿using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CityGame.Graphics
{
    public class SpriteBusiness
    {
        private string groupsFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Resources\Groups.json";

#if DEBUG     
        private string developmnetGroupsFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"..\..\..\..\Resources\Groups.json";
#endif

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
                if ((_groups[i] != null) && (!string.IsNullOrEmpty(_groups[i].Name)))
                {
                    _groups[i].Name = _groups[i]?.Name?.ToLower();
                }
            }

            if (foundNotExists)
            {
                SetGroups();
            }
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
                return groups.FirstOrDefault(g => !string.IsNullOrEmpty(g.Name) && g.Name.ToLower().Equals(groupItemName.ToLower()));
            }
            return null; //no group by default        
        }

        public int? GetGroupId(string groupItemName)
        {
            if (!string.IsNullOrEmpty(groupItemName))
            {
                return groups.FirstOrDefault(g => !string.IsNullOrEmpty(g.Name) && g.Name.ToLower().Equals(groupItemName.ToLower()))?.Id;
            }
            return 0; //no group by default        
        }

        public GroupSpritesDTO GetSpritesByGroupIndex(int? gorupId, int animationFrame = 0)
        {
            if (gorupId != null)
            {
                GroupDTO? groupDTO = groups.FirstOrDefault(g => g.Id == gorupId);
                if (groupDTO != null)
                {
                    if (animationFrame < 0)
                    {
                        animationFrame = 0;
                    }

                    if (groupDTO.Frames.Count > animationFrame)
                    {
                        return groupDTO.Frames[animationFrame];
                    }
                }
            }

            return new GroupSpritesDTO();
        }

        public GroupSpritesDTO GetSpritesByGroupName(string groupItemName, int animationFrame = 0)
        {
            return GetSpritesByGroupIndex(GetGroupId(groupItemName), animationFrame);
        }

        public GroupDTO? GetGroupBySpritePosition(PositionDTO? position)
        {
            if (position != null)
            {
                foreach (GroupDTO group in groups)
                {
                    foreach (GroupSpritesDTO groupSprites in group.Frames)
                    {
                        if ((groupSprites == null) || (groupSprites.Sprites == null))
                        {
                            continue;
                        }
                        for (int x = 0; x < groupSprites?.Sprites.GetLength(0); x++)
                        {
                            for (int y = 0; y < groupSprites?.Sprites.GetLength(1); y++)
                            {
                                if ((groupSprites.Sprites[x, y] != null) && (groupSprites?.Sprites?[x, y]?.x != null) && (groupSprites?.Sprites?[x, y]?.y != null) &&
                                    (groupSprites?.Sprites?[x, y]?.x == position.x) && (groupSprites?.Sprites?[x, y]?.y == position.y))
                                {
                                    return group;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public ObjectType GetObjectTypeByGrop(GroupDTO? group)
        {
            switch (group?.Name)
            {
                case SpritesGroupEnum.road:
                case SpritesGroupEnum.rail:
                case SpritesGroupEnum.wire:
                    return ObjectType.network;
                case SpritesGroupEnum.garden:
                    return ObjectType.garden;
                case SpritesGroupEnum.water:
                    return ObjectType.water;
                case SpritesGroupEnum.terrain:
                    return ObjectType.terrain;
                case SpritesGroupEnum.forest:
                    return ObjectType.forest;
                default:
                    return ObjectType.building;
            }
        }



    }
}
