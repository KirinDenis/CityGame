using CityGame.DTOs;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using CityGame.Models.Interfaces;
using System.Collections.Generic;

namespace CityGame.Models
{
    internal class NetworkGameObjectModel : GameObjectModel
    {
        public NetworkGameObjectModel(TerrainModel terrainModel, ObjectType networkType):base(terrainModel, networkType)
        {
        }

        public SpriteDTO[,] BuildNetworkItem(int x, int y)
        {
            PositionDTO[,] offsets = new PositionDTO[3, 3];
            int ox = 0;
            for (int tx = CLeft(x); tx < CRight(x) + 1; tx++, ox++)
            {
                int oy = 0;
                for (int ty = CLeft(y); ty < CRight(y) + 1; ty++, oy++)
                {
                    offsets[ox, oy] = terrainModel.terrain[tx, ty];
                }
            }
            SpriteDTO[,] sprites = spriteBusiness.GetSpritesByOffsets(offsets);

            terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[4, 0];

            int l = 0;
            int c = 1;
            int r = 2;
            int t = 0;
            int b = 2;


            //Central cross of 4 roads
            if ((sprites[c, t].groupId & sprites[c, b].groupId & sprites[l, c].groupId & sprites[r, c].groupId) == Group.Id)
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[1, 1];
            }
            //Left Right Top cross of 3 roads  
            else
            if ((sprites[c, t].groupId & sprites[l, c].groupId & sprites[r, c].groupId) == Group.Id)
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[ 1, 2];
            }
            //Left Right Bottom cross of 3 roads 
            else
            if ((sprites[c, b].groupId & sprites[l, c].groupId & sprites[r, c].groupId) == Group.Id)
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[ 1, 0];
            }
            //Left Top Bottom cross of 3 roads 
            else
            if ((sprites[c, t].groupId & sprites[c, b].groupId & sprites[l, c].groupId) == Group.Id)
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[ 2, 1];
            }
            //Right Top Bottom cross of 3 roads 
            else
            if ((sprites[c, t].groupId & sprites[c, b].groupId & sprites[r, c].groupId) == Group.Id)
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[ 0, 1];
            }
            //Right Top turn of 2 roads 
            else
            if ((sprites[c, b].groupId & sprites[r, c].groupId) == Group.Id)
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[ 0, 0];
            }
            //Left Top turn of 2 roads 
            else
            if ((sprites[c, b].groupId & sprites[l, c].groupId) == Group.Id)
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[ 2, 0];
            }
            //Right Bottom turn of 2 roads 
            else
            if ((sprites[c, t].groupId & sprites[r, c].groupId) == Group.Id)
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[ 0, 2];
            }
            //Left Bottom turn of 2 roads 
            else
            if ((sprites[c, t].groupId & sprites[l, c].groupId) == Group.Id)
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[ 2, 2];
            }
            //Horisontal road
            else
            if ((sprites[r, c].groupId == Group.Id) || (sprites[l, c].groupId == Group.Id))
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[ 3, 0];
            }
            //Vertical road
            else
            if ((sprites[c, t].groupId == Group.Id) || (sprites[c, b].groupId == Group.Id))
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[ 4, 0];
            }
            //else default single road

            terrainModel.PutImage(x, y, terrainModel.terrain[x, y].x, terrainModel.terrain[x, y].y);

            return sprites;
        }

        public override bool Put(ushort x, ushort y)
        {


            SpriteDTO[,] sprites = BuildNetworkItem(x, y);


            //Rebuild near roads            
            int ox = 0;
            for (int tx = CLeft(x); tx < CRight(x) + 1; tx++, ox++)
            {
                int oy = 0;
                for (int ty = CLeft(y); ty < CRight(y) + 1; ty++, oy++)
                {
                    if (sprites[ox, oy].groupId == Group?.Id)
                    {
                        BuildNetworkItem(tx, ty);
                    }
                }
            }

            return true;

        }

    }
}
