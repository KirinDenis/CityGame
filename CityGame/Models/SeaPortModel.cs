using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using System.Windows;

namespace CityGame.Models
{
    internal class SeaPortModel : GameObjectModel
    {
        public SeaPortModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {        
            startingGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.seaport);
        }
        protected override void LiveCycle(GameObjectDTO gameObject)
        {

        }
    }
}
