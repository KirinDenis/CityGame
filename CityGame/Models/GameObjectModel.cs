using CityGame.Data.DTO;
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

        public GroupDTO? Group { get; set; }

        public GameObjectModel(TerrainModel terrainModel)
        {            
            this.terrainModel = terrainModel;
            
        }

        public virtual bool Put(ushort x, ushort y)
        {
            //throw new NotImplementedException();
            return false;
        }
    }
}
