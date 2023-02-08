using CityGame.DTOs.Enum;
using CityGame.Graphics;

namespace CityGame.Models
{
    internal class RailGameObjectModel : NetworkGameObjectModel
    {
        public RailGameObjectModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {
            Group = spriteBusiness.GetGroupByName(SpritesGroupEnum.rail);
        }
    }
}
