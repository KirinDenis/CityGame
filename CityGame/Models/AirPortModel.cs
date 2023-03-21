using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;

namespace CityGame.Models
{
    internal class AirPortModel : GameObjectModel
    {
        public AirPortModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {
            startingGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.airport);
        }
        protected override void LiveCycle(GameObjectDTO gameObject)
        {

        }
    }
}
