using CityGame.DataModels;
using CityGame.DataModels.Enum;
using CityGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGame.Models
{
    internal class ResidentModel
    {
        private SpriteBusiness spriteBusiness = new SpriteBusiness();
        private TerrainModel terrainModel;
        private GroupsModel groupsModel;

        public ResidentModel(GroupsModel groupsModel, TerrainModel terrainModel)
        {
            this.groupsModel = groupsModel;
            this.terrainModel = terrainModel;
        }

        public void Put(ushort x, ushort y, ObjectType objectType)
        {
            List<SpriteModel>? group = null;
            switch (objectType)
            {
                case ObjectType.resident: group = groupsModel.GetGroup(SpritesGroupEnum.resident0); break;
                case ObjectType.industrial: group = groupsModel.GetGroup(SpritesGroupEnum.industrial1); break;
                case ObjectType.policeDepartment: group = groupsModel.GetGroup(SpritesGroupEnum.policeDepartment); break;
            }
            
            if (group == null) 
            { 
                return; 
            }  
                

            for (int i=0; i < group?.Count; i++)
            {
                SpriteModel? spriteModel = group[i];
                if (spriteModel?.groupPosition != null)
                {
                    terrainModel.terrain[x + spriteModel.groupPosition.x, y + spriteModel.groupPosition.y] = spriteModel.position;
                    terrainModel.PutImage(x + spriteModel.groupPosition.x, y + spriteModel.groupPosition.y, spriteModel.position.x, spriteModel.position.y);
                }
            }

        }

    }
}
