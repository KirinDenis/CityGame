/* ----------------------------------------------------------------------------
Simple City
Copyright 2023 by:
- Denys Kirin (deniskirinacs@gmail.com)

This file is part of Simple City

OWLOS is free software : you can redistribute it and/or modify it under the
terms of the GNU General Public License as published by the Free Software
Foundation, either version 3 of the License, or (at your option) any later
version.

Simple City is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
FOR A PARTICULAR PURPOSE.
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along
with Simple City. If not, see < https://www.gnu.org/licenses/>.

GitHub: https://github.com/KirinDenis/CityGame

--------------------------------------------------------------------------------------*/


using CityGame.Data.DTO;
using CityGame.Data.Enum;
using CityGame.DTOs.Enum;
using CityGame.Graphics;

namespace CityGame.Models
{
    /// <summary>
    /// Network objects model for RAILroad, ROAD and WIRE game objects
    /// </summary>
    internal class NetworkGameObjectModel : GameObjectModel
    {
        public NetworkGameObjectModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {
        }

        /// <summary>
        /// In order to better understand the principle by which network objects are built, connected and intersected (cross), such as a railroad (rail), a road and wires - 
        /// first of all, study the principle by which the sprites for these three groups are located. 
        /// The location of sprites within netgroups plays a major role in this algorithm.
        /// 
        /// SEE: menu File/Development/Resource Explorer
        /// 
        /// Network sprite group:
        ///   |      0         |       1        |        2        |             3              |             4              |
        /// 
        /// 0 | u.right turn   |  h.cross down  |   u.left turn   |  single horizontal         |  single vertical           |
        ///    ---------------------------------------------------------------------------------------------------------------
        /// 1 | v.cross right  |  4 cross       |   v.cross left  |  h.cross object with road  |  v.cross object with road  |
        ///    ---------------------------------------------------------------------------------------------------------------
        /// 2 | d.right turn   |  h.cross up    |   d.left turn   |  h.cross object with rail  |  v.cross object with rail  |
        ///    ---------------------------------------------------------------------------------------------------------------
        /// 3 |                |                |                 |  h.cross object with wire  |  v.cross object with wire  |
        /// 
        /// it means, if current network object is ROAD and it cross with vertical RAIL the sprite position is X=4, Y=2 at ROAD sprites group
        /// 
        /// </summary>
        /// <param name="x">X position on game map</param>
        /// <param name="y">Y position on game map</param>
        /// <param name="current">if true it is new network object buildings, means not rebuild exists near objects</param>
        /// <returns>returns near FS (F.riendly S.prites) what must be rebuilded to draw connections with new network object</returns>
        public bool[,] BuildNetworkItem(ushort x, ushort y, bool current = false)
        {
            bool[,] FS = new bool[3, 3]; //FS means friendly sprites, the sprites of selected network item type
            bool[,] FB = new bool[3, 3]; //FB meand building sprites near selected network position, only for connect wire to building
            if ((spriteBusiness == null) || (startingGroup == null))
            {
                return FS;
            }
            //scan neighboring objects and collect data about their types
            //scaning range 3x3 with center of new network object coords
            int ox = 0;
            for (ushort tx = CLeft(x); tx < CRight(x) + 1; tx++, ox++)
            {
                int oy = 0;
                for (ushort ty = CLeft(y); ty < CRight(y) + 1; ty++, oy++)
                {
                    //just check we not of out of game map range 
                    if (!terrainModel.TestRange(new PositionDTO() { x = tx, y = ty }))
                    {
                        continue;
                    }

                    //by default the current scaned object it is not a friend
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
                                        //friendly object found
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
                                //building object found
                                FB[ox, oy] = true; 
                            }
                        }
                    }
                }
            }

            PositionDTO? previosSptite = terrainModel.terrain?[x, y];
            PositionDTO spritePosition;

            int l = 0; //left
            int c = 1; //centre
            int r = 2; //right
            int t = 0; //top
            int b = 2; //bottom

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
                                //if it cross with other network object
                                if (previosGroup.Name.Equals(SpritesGroupEnum.rail) || previosGroup.Name.Equals(SpritesGroupEnum.road) || previosGroup.Name.Equals(SpritesGroupEnum.wire))
                                {
                                    NetworkCrossType networkCrossType = NetworkCrossType.crossOnCross;
                                    //if it cross with simple horisontal network object 
                                    if ((previosGroup?.Frames[0]?.Sprites?[3, 0]?.x == terrainModel?.terrain?[x, y]?.x)
                                        &&
                                        (previosGroup?.Frames[0]?.Sprites?[3, 0]?.y == terrainModel?.terrain?[x, y]?.y))
                                    {
                                        networkCrossType = NetworkCrossType.horisontal;
                                    }
                                    else //else if it cross with simple vertical rail 
                                    if ((previosGroup?.Frames[0]?.Sprites?[4, 0]?.x == terrainModel?.terrain?[x, y]?.x)
                                        &&
                                        (previosGroup?.Frames[0]?.Sprites?[4, 0]?.y == terrainModel?.terrain?[x, y]?.y))
                                    {
                                        networkCrossType = NetworkCrossType.vartical;
                                    }
                                    else //if it is not horizontal or vertical cross - return, we can't cross with crosses of other network objects
                                    {
                                        return FS;
                                    }    

                                    //Draw cross for current combination of new and exists network objects
                                    //SEE: the method comment UP^ (description and sprite location inside group table)
                                    switch(previosGroup?.Name)
                                    {
                                        case SpritesGroupEnum.rail: 
                                            if (networkCrossType == NetworkCrossType.horisontal)
                                            {
                                                spritePosition = new PositionDTO() { x = 3, y = 2 }; //Horisontal railroad cross
                                            }
                                            else
                                            {
                                                spritePosition = new PositionDTO() { x = 4, y = 2 }; //Vertical railroad cross 
                                            }
                                            break;

                                        case SpritesGroupEnum.road:
                                            if (networkCrossType == NetworkCrossType.horisontal)
                                            {
                                                spritePosition = new PositionDTO() { x = 3, y = 1 };
                                            }
                                            else
                                            {
                                                spritePosition = new PositionDTO() { x = 4, y = 1 };
                                            }
                                            break;
                                        case SpritesGroupEnum.wire:
                                        default:  
                                            if (networkCrossType == NetworkCrossType.horisontal)
                                            {
                                                spritePosition = new PositionDTO() { x = 3, y = 3 };
                                            }
                                            else
                                            {
                                                spritePosition = new PositionDTO() { x = 4, y = 3 };
                                            }
                                            break;
                                    }
                                    terrainModel?.PutSprite(x, y, startingGroup, spritePosition);
                                    return FS;
                                }
                                else
                                if (previosGroup.Name.Equals(SpritesGroupEnum.water))
                                {
                                    //horizontal
                                    if (FS[r, c] | FS[l, c])
                                    {
                                        spritePosition = new PositionDTO() { x = 3, y = 4 };
                                    }
                                    else //Vertical                                     
                                    if (FS[c, t] | FS[c, b])
                                    {
                                        spritePosition = new PositionDTO() { x = 4, y = 4 };
                                    }
                                    else
                                    {
                                        return FS;
                                    } 
                                        
                                    terrainModel?.PutSprite(x, y, startingGroup, spritePosition);
                                    return FS;
                                }
                            }
                        }
                    }
                }
            }            
            spritePosition = new PositionDTO() { x = 4, y = 0 };

            bool[,] StoredFS = new bool[3,3];
            for(int sx = 0; sx < 3; sx++)
            {
                for (int sy = 0; sy < 3; sy++)
                {
                    StoredFS[sx, sy] = FS[sx, sy];
                    //only WIRE connects to buildings
                    if (startingGroup?.Name?.Equals(SpritesGroupEnum.wire) == true)
                    {
                        FS[sx, sy] = FS[sx, sy] | FB[sx, sy];
                    }
                }
            }

            //Central cross of 4 roads (means any network object of equal type)
            if (FS[c, t] & FS[c, b] & FS[l, c] & FS[r, c])
            {            
                spritePosition = new PositionDTO() { x = 1, y = 1 };
            }
            //Left Right Top cross of 3 roads = network object 
            else
            if (FS[c, t] & FS[l, c] & FS[r, c])
            {             
                spritePosition = new PositionDTO() { x = 1, y = 2 };
            }
            //Left Right Bottom cross of 3 roads 
            else
            if (FS[c, b] & FS[l, c] & FS[r, c])
            {             
                spritePosition = new PositionDTO() { x = 1, y = 0 };
            }
            //Left Top Bottom cross of 3 roads 
            else
            if (FS[c, t] & FS[c, b] & FS[l, c])
            {             
                spritePosition = new PositionDTO() { x = 2, y = 1 };
            }
            //Right Top Bottom cross of 3 roads 
            else
            if (FS[c, t] & FS[c, b] & FS[r, c])
            {             
                spritePosition = new PositionDTO() { x = 0, y = 1 };
            }
            //Right Top turn of 2 roads 
            else
            if (FS[c, b] & FS[r, c])
            {             
                spritePosition = new PositionDTO() { x = 0, y = 0 };
            }
            //Left Top turn of 2 roads 
            else
            if (FS[c, b] & FS[l, c])
            {
                spritePosition = new PositionDTO() { x = 2, y = 0 };
            }
            //Right Bottom turn of 2 roads 
            else
            if (FS[c, t] & FS[r, c])
            {
                spritePosition = new PositionDTO() { x = 0, y = 2 };
            }
            //Left Bottom turn of 2 roads 
            else
            if (FS[c, t] & FS[l, c])
            {                
                spritePosition = new PositionDTO() { x = 2, y = 2 };
            }
            //Horisontal road
            else
            if (FS[r, c] | FS[l, c])
            {             
                spritePosition = new PositionDTO() { x = 3, y = 0 };
            }
            //Vertical road
            else
            if (FS[c, t] | FS[c, b])
            {             
                spritePosition = new PositionDTO() { x = 4, y = 0 };
            }
            //else default single road

            terrainModel.PutSprite(x, y, startingGroup, spritePosition);

            return StoredFS;
        }

        /// <summary>
        /// Тhe appearance of a new network object on the playing field changes the state of other objects in conflict with it. 
        /// For this reason, after placing an object, it is necessary to recalculate all neighboring network objects
        /// </summary>
        /// <param name="positionDTO">new object position on the game map</param>
        /// <returns>new game object represent model DTO</returns>
        public override GameObjectModelDTO Build(PositionDTO positionDTO)
        {
            ushort x = positionDTO.x;
            ushort y = positionDTO.y;

            //build new network object
            bool[,] FS = BuildNetworkItem(x, y, true);

            //rebuild near network object with using collected FS (F.riendly S.prites)           
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
