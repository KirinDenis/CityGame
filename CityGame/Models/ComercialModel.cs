using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;

namespace CityGame.Models
{
    internal class ComercialModel : GameObjectModel
    {
        private SpriteBusiness spriteBusiness;        
        public ComercialModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {        
            startingGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.comercial0);
        }
        protected override void LiveCycle(GameObjectDTO gameObject)
        {

        }
    }
}
