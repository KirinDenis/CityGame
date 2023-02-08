using CityGame.DTOs.Enum;
using CityGame.Graphics;

namespace CityGame.Models
{
    internal class ComercialModel : GameObjectModel
    {
        private SpriteBusiness spriteBusiness;        
        public ComercialModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {        
            Group = spriteBusiness.GetGroupByName(SpritesGroupEnum.comercial0);
        }
        protected override void LiveCycle()
        {

        }
    }
}
