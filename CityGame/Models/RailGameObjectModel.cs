using CityGame.DTOs.Enum;

namespace CityGame.Models
{
    internal class RailGameObjectModel : NetworkGameObjectModel
    {
        public RailGameObjectModel(TerrainModel terrainModel) : base(terrainModel)
        {
            Group = spriteBusiness.GetGroupByName(SpritesGroupEnum.rail);
        }
    }
}
