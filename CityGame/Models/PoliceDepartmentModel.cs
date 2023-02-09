using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using System.Windows;

namespace CityGame.Models
{
    internal class PoliceDepartmentModel : GameObjectModel
    {
        public PoliceDepartmentModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {        
            startingGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.policedepartment);
        }
        protected override void LiveCycle(GameObjectDTO gameObject)
        {

        }
    }
}
