using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;

namespace CityGame.Models
{
    internal class IndustrialModel : GameObjectModel
    {
        private byte level = 0;

        private uint timeLive = 0;
        public IndustrialModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {        
            startingGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.industrial0);
        }
        protected override void LiveCycle(GameObjectDTO gameObject)
        {
            timeLive++;
            if (timeLive > 3)
            {
                timeLive = 0;
                level++;
                gameObject.Group = spriteBusiness.GetGroupByName(SpritesGroupEnum.industrialBase + level);
                if (gameObject.Group == null)
                {
                    level = 0;
                    gameObject.Group = startingGroup;
                }
                gameObject.animationFrame = 0;
            }
        }
    }
}
