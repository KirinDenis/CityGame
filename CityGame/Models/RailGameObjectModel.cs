using CityGame.DTOs.Enum;
using CityGame.Graphics;

namespace CityGame.Models
{
    internal class RailGameObjectModel : NetworkGameObjectModel
    {
        public RailGameObjectModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {
            startingGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.rail);
        }
    }
}
