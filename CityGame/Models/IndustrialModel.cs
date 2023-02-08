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
            Group = spriteBusiness.GetGroupByName(SpritesGroupEnum.industrial0);
        }
        protected override void LiveCycle()
        {
            timeLive++;
            if (timeLive > 3)
            {
                timeLive = 0;
                level++;
                Group = spriteBusiness.GetGroupByName(SpritesGroupEnum.industrialBase + level);
                if (Group == null)
                {
                    level = 0;
                    Group = spriteBusiness.GetGroupByName(SpritesGroupEnum.industrialBase + level);
                }
                animationFrame = 0;
            }
        }
    }
}
