using CityGame.DTOs.Enum;
using CityGame.Graphics;

namespace CityGame.Models
{
    internal class GardenModel : GameObjectModel
    {
        private SpriteBusiness spriteBusiness;
        public GardenModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {
            Group = spriteBusiness.GetGroupByName(SpritesGroupEnum.garden);
        }
        protected override void LiveCycle()
        {

        }
    }
}
