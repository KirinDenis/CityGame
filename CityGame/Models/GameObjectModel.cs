using CityGame.DTOs;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using CityGame.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGame.Models
{
    public class GameObjectModel : BaseModel, IGameObjectModel
    {

        protected SpriteBusiness spriteBusiness = new SpriteBusiness();
        protected TerrainModel terrainModel;
        protected GroupsModel groupsModel;

        public int? GroupId { get; set; }
        public List<SpriteDTO>? SpritesGroup { get; set; }


        public GameObjectModel(GroupsModel groupsModel, TerrainModel terrainModel, ObjectType objectType)
        {
            this.groupsModel = groupsModel;
            this.terrainModel = terrainModel;

            GroupId = spriteBusiness.GetGroupId(SpritesGroupEnum.ByObjectType(objectType));
            SpritesGroup = groupsModel.GetGroup(SpritesGroupEnum.ByObjectType(objectType));
        }
        public CheckPositionDTO CheckPosition(ushort x, ushort y)
        {
            throw new NotImplementedException();
        }

        public virtual bool Put(ushort x, ushort y)
        {
            throw new NotImplementedException();
        }
    }
}
