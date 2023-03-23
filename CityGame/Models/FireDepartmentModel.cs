using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;

namespace CityGame.Models
{
    internal class FireDepartmentModel : GameObjectModel
    {
        public FireDepartmentModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {
            startingGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.firedepartment);
        }
        protected override void LiveCycle(GameObjectModelDTO gameObjectModelDTO)
        {

        }
    }
}
