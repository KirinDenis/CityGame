using CityGame.DTOs.Enum;

namespace CityGame.Models
{
    internal class RoadGameObjectModel : NetworkGameObjectModel
    {
        public RoadGameObjectModel(TerrainModel terrainModel) : base(terrainModel)
        {
            Group = spriteBusiness.GetGroupByName(SpritesGroupEnum.road);
        }
    }
}
