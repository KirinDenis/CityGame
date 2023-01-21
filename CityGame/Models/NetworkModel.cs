using CityGame.DataModels;
using CityGame.DataModels.Enum;
using CityGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CityGame.Models
{
    internal class NetworkModel: BaseModel
    {
        private SpriteBusiness spriteBusiness = new SpriteBusiness();
        private TerrainModel terrainModel;
        private GroupsModel groupsModel;
        public NetworkModel(GroupsModel groupsModel, TerrainModel terrainModel)
        {
            this.groupsModel = groupsModel;
            this.terrainModel = terrainModel;
        }

        public SpriteModel[,] BuildNetworkItem(int x, int y, int? networkGroupId, List<SpriteModel>? networkGroup)
        {
            PositionModel[,] offsets = new PositionModel[3, 3];
            int ox = 0;
            for (int tx = CLeft(x); tx < CRight(x) + 1; tx++, ox++)
            {
                int oy = 0;
                for (int ty = CLeft(y); ty < CRight(y) + 1; ty++, oy++)
                {
                    offsets[ox, oy] = terrainModel.terrain[tx, ty];
                }
            }
            SpriteModel[,] sprites = spriteBusiness.GetSpritesByOffsets(offsets);

            terrainModel.terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(networkGroup, 4, 0);

            int l = 0;
            int c = 1;
            int r = 2;
            int t = 0;
            int b = 2;


            //Central cross of 4 roads
            if ((sprites[c, t].groupId & sprites[c, b].groupId & sprites[l, c].groupId & sprites[r, c].groupId) == networkGroupId)
            {
                terrainModel.terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(networkGroup, 1, 1);
            }
            //Left Right Top cross of 3 roads  
            else
            if ((sprites[c, t].groupId & sprites[l, c].groupId & sprites[r, c].groupId) == networkGroupId)
            {
                terrainModel.terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(networkGroup, 1, 2);
            }
            //Left Right Bottom cross of 3 roads 
            else
            if ((sprites[c, b].groupId & sprites[l, c].groupId & sprites[r, c].groupId) == networkGroupId)
            {
                terrainModel.terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(networkGroup, 1, 0);
            }
            //Left Top Bottom cross of 3 roads 
            else
            if ((sprites[c, t].groupId & sprites[c, b].groupId & sprites[l, c].groupId) == networkGroupId)
            {
                terrainModel.terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(networkGroup, 2, 1);
            }
            //Right Top Bottom cross of 3 roads 
            else
            if ((sprites[c, t].groupId & sprites[c, b].groupId & sprites[r, c].groupId) == networkGroupId)
            {
                terrainModel.terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(networkGroup, 0, 1);
            }
            //Right Top turn of 2 roads 
            else
            if ((sprites[c, b].groupId & sprites[r, c].groupId) == networkGroupId)
            {
                terrainModel.terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(networkGroup, 0, 0);
            }
            //Left Top turn of 2 roads 
            else
            if ((sprites[c, b].groupId & sprites[l, c].groupId) == networkGroupId)
            {
                terrainModel.terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(networkGroup, 2, 0);
            }
            //Right Bottom turn of 2 roads 
            else
            if ((sprites[c, t].groupId & sprites[r, c].groupId) == networkGroupId)
            {
                terrainModel.terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(networkGroup, 0, 2);
            }
            //Left Bottom turn of 2 roads 
            else
            if ((sprites[c, t].groupId & sprites[l, c].groupId) == networkGroupId)
            {
                terrainModel.terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(networkGroup, 2, 2);
            }
            //Horisontal road
            else
            if ((sprites[r, c].groupId == networkGroupId) || (sprites[l, c].groupId == networkGroupId))
            {
                terrainModel.terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(networkGroup, 3, 0);
            }
            //Vertical road
            else
            if ((sprites[c, t].groupId == networkGroupId) || (sprites[c, b].groupId == networkGroupId))
            {
                terrainModel.terrain[x, y] = spriteBusiness.GetSpriteOffsetByGroupPosition(networkGroup, 4, 0);
            }
            //else default single road

            terrainModel.PutImage(x, y, terrainModel.terrain[x, y].x, terrainModel.terrain[x, y].y);

            return sprites;
        }

        public void PutNetworkItem(int x, int y, NetworkType networkType)
        {
            int? networkGroupId;
            List<SpriteModel>? networkGroup;

            switch (networkType)
            {
                case NetworkType.road:
                    networkGroupId = spriteBusiness.GetGroupId("road");
                    networkGroup = groupsModel.GetGroup(SpritesGroupEnum.road);
                    break;
                case NetworkType.rail:
                    networkGroupId = spriteBusiness.GetGroupId("rail");
                    networkGroup = groupsModel.GetGroup(SpritesGroupEnum.rail);
                    break;
                default:
                    networkGroupId = spriteBusiness.GetGroupId("wire");
                    networkGroup = groupsModel.GetGroup(SpritesGroupEnum.wire);
                    break;
            }


            SpriteModel[,] sprites = BuildNetworkItem(x, y, networkGroupId, networkGroup);


            //Rebuild near roads            
            int ox = 0;
            for (int tx = CLeft(x); tx < CRight(x) + 1; tx++, ox++)
            {
                int oy = 0;
                for (int ty = CLeft(y); ty < CRight(y) + 1; ty++, oy++)
                {
                    if (sprites[ox, oy].groupId == networkGroupId)
                    {
                        BuildNetworkItem(tx, ty, networkGroupId, networkGroup);
                    }
                }
            }

        }

    }
}
