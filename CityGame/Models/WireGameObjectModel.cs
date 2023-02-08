using CityGame.DTOs.Enum;
using CityGame.Graphics;

namespace CityGame.Models
{
    internal class WireGameObjectModel : NetworkGameObjectModel
    {
        public WireGameObjectModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {
            Group = spriteBusiness.GetGroupByName(SpritesGroupEnum.wire);
        }
    }
}
