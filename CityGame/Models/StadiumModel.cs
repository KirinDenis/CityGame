using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;

namespace CityGame.Models
{
    internal class StadiumModel : GameObjectModel
    {
        public StadiumModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {
            startingGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.stadium);
        }
        protected override void LiveCycle(GameObjectDTO gameObject)
        {

        }
    }
}
