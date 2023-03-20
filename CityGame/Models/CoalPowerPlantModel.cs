using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using System.Windows;

namespace CityGame.Models
{
    internal class CoalPowerPlantModel : GameObjectModel
    {
        public CoalPowerPlantModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {        
            startingGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.coalpowerplant);
        }
        protected override void LiveCycle(GameObjectDTO gameObject)
        {

        }
    }
}
