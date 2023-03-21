using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;

namespace CityGame.Models
{
    internal class NuclearPowerPlantModel : GameObjectModel
    {
        public NuclearPowerPlantModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {
            startingGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.nuclearpowerplant);
        }
        protected override void LiveCycle(GameObjectDTO gameObject)
        {

        }
    }
}
