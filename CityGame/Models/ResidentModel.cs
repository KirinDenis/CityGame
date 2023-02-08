using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CityGame.Models
{
    internal class ResidentModel : GameObjectModel
    {
        private SpriteBusiness spriteBusiness = new SpriteBusiness();
        private TerrainModel terrainModel;


        public ResidentModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {
            this.terrainModel = terrainModel;
            Group = spriteBusiness.GetGroupByName(SpritesGroupEnum.stadium);

        }

        protected override void LiveCycle()
        {
            
        }

    }
}
