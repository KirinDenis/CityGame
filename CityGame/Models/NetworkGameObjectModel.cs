using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;

namespace CityGame.Models
{
    internal class NetworkGameObjectModel : GameObjectModel
    {
        public NetworkGameObjectModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {
        }

        public bool[,] BuildNetworkItem(ushort x, ushort y, bool current = false)
        {
            bool[,] FS = new bool[3, 3]; //FS means friendly sprites, the sprites of selected network item type
            bool[,] FB = new bool[3, 3]; //FB meand building sprites near selected network position
            if ((spriteBusiness == null) || (startingGroup == null))
            {
                return FS;
            }
            int ox = 0;
            for (ushort tx = CLeft(x); tx < CRight(x) + 1; tx++, ox++)
            {
                int oy = 0;
                for (ushort ty = CLeft(y); ty < CRight(y) + 1; ty++, oy++)
                {
                    if (!terrainModel.TestRange(new PositionDTO() { x = tx, y = ty }))
                    {
                        continue;
                    }

                    FS[ox, oy] = false;

                    if ((startingGroup.Frames != null) && (startingGroup.Frames[0] != null) && (startingGroup.Frames[0].Sprites != null))
                    {
                        PositionDTO?[,]? positions = startingGroup.Frames[0].Sprites;
                        if (positions != null)
                        {
                            foreach (PositionDTO? position in positions)
                            {
                                if (position != null)
                                {
                                    if (terrainModel.terrain?[tx, ty] == position)
                                    {
                                        FS[ox, oy] = true;
                                        break;
                                    }
                                }
                            }
                        }
                        //near buildings
                        GroupDTO? group = spriteBusiness.GetGroupBySpritePosition(terrainModel.terrain?[tx, ty]);
                        if (group != null)
                        {
                            if (spriteBusiness.GetObjectTypeByGrop(group) == ObjectType.building)                            
                            {
                                FB[ox, oy] = true; 
                            }
                        }
                    }
                }
            }

            PositionDTO? previosSptite = terrainModel.terrain?[x, y];
            PositionDTO spritePosition;

            //check previos sprite 
            GroupDTO? previosGroup = spriteBusiness.GetGroupBySpritePosition(previosSptite);
            GroupDTO? cross = spriteBusiness.GetGroupByName(SpritesGroupEnum.cross);
            if ((previosGroup != null) && (cross != null) && (cross.Frames != null) && (cross.Frames[0] != null) && (cross.Frames[0].Sprites != null))
            {
                if ((spriteBusiness.GetObjectTypeByGrop(previosGroup) == ObjectType.network)
                    ||
                    (spriteBusiness.GetObjectTypeByGrop(previosGroup) == ObjectType.water))
                {
                    PositionDTO?[,]? positions = cross.Frames[0].Sprites;
                    if (positions != null)
                    {
                        foreach (PositionDTO? position in positions)
                        {
                            if ((position != null)
                                && (position.x == terrainModel?.terrain?[x, y]?.x)
                                && (position.y == terrainModel?.terrain?[x, y]?.y))
                            {
                                return FS;
                            }
                        }

                        if (current)
                        {
                            if (!string.IsNullOrEmpty(previosGroup.Name) && (!previosGroup.Name.Equals(startingGroup?.Name)))
                            {
                                if (previosGroup.Name.Equals(SpritesGroupEnum.rail))
                                {
                                    if ((previosGroup?.Frames[0]?.Sprites?[3, 0]?.x == terrainModel?.terrain?[x, y]?.x)
                                        &&
                                        (previosGroup?.Frames[0]?.Sprites?[3, 0]?.y == terrainModel?.terrain?[x, y]?.y))
                                    {
                                        spritePosition = new PositionDTO() { x = 3, y = 2 }; // startingGroup?.Frames[0].Sprites[3, 2];
                                    }
                                    else
                                    {
                                        //spritePosition = startingGroup?.Frames[0].Sprites[4, 2];
                                        spritePosition = new PositionDTO() { x = 4, y = 2 };
                                    }
                                    //terrainModel.PutSprite(x, y, terrainModel.terrain[x, y].x, terrainModel.terrain[x, y].y);
                                    terrainModel?.PutSprite(x, y, startingGroup, spritePosition);
                                    return FS;
                                }
                                else
                                if (previosGroup.Name.Equals(SpritesGroupEnum.road))
                                {
                                    if ((previosGroup?.Frames[0]?.Sprites?[3, 0]?.x == terrainModel?.terrain?[x, y]?.x)
                                        &&
                                        (previosGroup?.Frames[0]?.Sprites?[3, 0]?.y == terrainModel?.terrain?[x, y]?.y))
                                    {
                                        //spritePosition = startingGroup?.Frames[0].Sprites[3, 1];
                                        spritePosition = new PositionDTO() { x = 3, y = 1 };
                                    }
                                    else
                                    {
                                        //spritePosition = startingGroup?.Frames[0].Sprites[4, 1];
                                        spritePosition = new PositionDTO() { x = 4, y = 1 };
                                    }
                                    //terrainModel.PutSprite(x, y, terrainModel.terrain[x, y].x, terrainModel.terrain[x, y].y);
                                    //terrainModel.PutSprite(x, y, startingGroup, 0, terrainModel.terrain[x, y].x, terrainModel.terrain[x, y].y);
                                    terrainModel?.PutSprite(x, y, startingGroup, spritePosition);
                                    return FS;
                                }
                                else
                                if (previosGroup.Name.Equals(SpritesGroupEnum.wire))
                                {
                                    if ((previosGroup?.Frames[0]?.Sprites?[3, 0]?.x == terrainModel?.terrain?[x, y]?.x)
                                        &&
                                        (previosGroup?.Frames[0]?.Sprites?[3, 0]?.y == terrainModel?.terrain?[x, y]?.y))
                                    {
                                        //spritePosition = startingGroup?.Frames[0].Sprites[3, 3];
                                        spritePosition = new PositionDTO() { x = 3, y = 3 };
                                    }
                                    else
                                    {
                                        //spritePosition = startingGroup?.Frames[0].Sprites[4, 3];
                                        spritePosition = new PositionDTO() { x = 4, y = 3 };
                                    }
                                    //terrainModel.PutSprite(x, y, terrainModel.terrain[x, y].x, terrainModel.terrain[x, y].y);
                                    //terrainModel.PutSprite(x, y, startingGroup, 0, terrainModel.terrain[x, y].x, terrainModel.terrain[x, y].y);
                                    terrainModel?.PutSprite(x, y, startingGroup, spritePosition);
                                    return FS;
                                }
                                else
                                if (previosGroup.Name.Equals(SpritesGroupEnum.water))
                                {
                                    //spritePosition = startingGroup?.Frames[0].Sprites[3, 4];
                                    spritePosition = new PositionDTO() { x = 3, y = 4 };
                                    // terrainModel.terrain[x, y] = startingGroup?.Frames[0].Sprites[4, 4];

                                    //terrainModel.PutSprite(x, y, terrainModel.terrain[x, y].x, terrainModel.terrain[x, y].y);
                                    //terrainModel.PutSprite(x, y, startingGroup, 0, terrainModel.terrain[x, y].x, terrainModel.terrain[x, y].y);
                                    terrainModel?.PutSprite(x, y, startingGroup, spritePosition);
                                    return FS;
                                }
                            }
                        }
                    }
                }
            }
            //spritePosition = startingGroup?.Frames[0].Sprites[4, 0];
            spritePosition = new PositionDTO() { x = 4, y = 0 };

            int l = 0;
            int c = 1;
            int r = 2;
            int t = 0;
            int b = 2;

            bool[,] StoredFS = new bool[3,3];
            for(int sx = 0; sx < 3; sx++)
            {
                for (int sy = 0; sy < 3; sy++)
                {
                    StoredFS[sx, sy] = FS[sx, sy];
                    FS[sx, sy] = FS[sx, sy] | FB[sx, sy];
                }
            }

            //Central cross of 4 roads
            if (FS[c, t] & FS[c, b] & FS[l, c] & FS[r, c])
            {
                //spritePosition = startingGroup?.Frames[0].Sprites[1, 1];
                spritePosition = new PositionDTO() { x = 1, y = 1 };
            }
            //Left Right Top cross of 3 roads  
            else
            if (FS[c, t] & FS[l, c] & FS[r, c])
            {
                //spritePosition = startingGroup?.Frames[0].Sprites[1, 2];
                spritePosition = new PositionDTO() { x = 1, y = 2 };
            }
            //Left Right Bottom cross of 3 roads 
            else
            if (FS[c, b] & FS[l, c] & FS[r, c])
            {
                //spritePosition = startingGroup?.Frames[0].Sprites[1, 0];
                spritePosition = new PositionDTO() { x = 1, y = 0 };
            }
            //Left Top Bottom cross of 3 roads 
            else
            if (FS[c, t] & FS[c, b] & FS[l, c])
            {
                //spritePosition = startingGroup?.Frames[0].Sprites[2, 1];
                spritePosition = new PositionDTO() { x = 2, y = 1 };
            }
            //Right Top Bottom cross of 3 roads 
            else
            if (FS[c, t] & FS[c, b] & FS[r, c])
            {
                //spritePosition = startingGroup?.Frames[0].Sprites[0, 1];
                spritePosition = new PositionDTO() { x = 0, y = 1 };
            }
            //Right Top turn of 2 roads 
            else
            if (FS[c, b] & FS[r, c])
            {
                //spritePosition = startingGroup?.Frames[0].Sprites[0, 0];
                spritePosition = new PositionDTO() { x = 0, y = 0 };
            }
            //Left Top turn of 2 roads 
            else
            if (FS[c, b] & FS[l, c])
            {
                //spritePosition = startingGroup?.Frames[0].Sprites[2, 0];
                spritePosition = new PositionDTO() { x = 2, y = 0 };
            }
            //Right Bottom turn of 2 roads 
            else
            if (FS[c, t] & FS[r, c])
            {
                //spritePosition = startingGroup?.Frames[0].Sprites[0, 2];
                spritePosition = new PositionDTO() { x = 0, y = 2 };
            }
            //Left Bottom turn of 2 roads 
            else
            if (FS[c, t] & FS[l, c])
            {
                //spritePosition = startingGroup?.Frames[0].Sprites[2, 2];
                spritePosition = new PositionDTO() { x = 2, y = 2 };
            }
            //Horisontal road
            else
            if (FS[r, c] | FS[l, c])
            {
                //spritePosition = startingGroup?.Frames[0].Sprites[3, 0];
                spritePosition = new PositionDTO() { x = 3, y = 0 };
            }
            //Vertical road
            else
            if (FS[c, t] | FS[c, b])
            {
                //spritePosition = startingGroup?.Frames[0].Sprites[4, 0];
                spritePosition = new PositionDTO() { x = 4, y = 0 };
            }
            //else default single road


            //terrainModel.PutSprite(x, y, terrainModel.terrain[x, y].x, terrainModel.terrain[x, y].y);
            //terrainModel.PutSprite(x, y, startingGroup, 0, terrainModel.terrain[x, y].x, terrainModel.terrain[x, y].y);
            terrainModel.PutSprite(x, y, startingGroup, spritePosition);

            return StoredFS;
        }

        public override GameObjectModelDTO Build(PositionDTO positionDTO)
        {
            ushort x = positionDTO.x;
            ushort y = positionDTO.y;

            bool[,] FS = BuildNetworkItem(x, y, true);

            //Rebuild near roads            
            ushort ox = 0;
            for (ushort tx = CLeft(x); tx < CRight(x) + 1; tx++, ox++)
            {
                ushort oy = 0;
                for (ushort ty = CLeft(y); ty < CRight(y) + 1; ty++, oy++)
                {
                    if (FS[ox, oy])
                    {
                        BuildNetworkItem(tx, ty);
                    }
                }
            }
            return new GameObjectModelDTO()
            {
                positionDTO = positionDTO,
                Group = this.startingGroup
            };

        }
    }
}
