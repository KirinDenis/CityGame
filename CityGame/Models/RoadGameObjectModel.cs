using CityGame.DTOs.Enum;
using CityGame.Graphics;

namespace CityGame.Models
{
    internal class RoadGameObjectModel : NetworkGameObjectModel
    {
        public RoadGameObjectModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {
            startingGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.road);
        }
    }
}
