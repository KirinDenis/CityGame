﻿using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;

namespace CityGame.Models
{
    internal class GardenModel : GameObjectModel
    {
        public GardenModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {
            startingGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.garden);
        }
        protected override void LiveCycle(GameObjectModelDTO gameObjectModelDTO)
        {

        }
    }
}
