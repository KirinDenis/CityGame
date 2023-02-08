﻿using CityGame.DTOs.Enum;
using CityGame.Graphics;

namespace CityGame.Models
{
    internal class ResidentModel : GameObjectModel
    {
        private SpriteBusiness spriteBusiness;
        public ResidentModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {
            Group = spriteBusiness.GetGroupByName(SpritesGroupEnum.resident0);

        }
        protected override void LiveCycle()
        {

        }
    }
}
