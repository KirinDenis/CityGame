using CityGame.Data.DTO;
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


        public ResidentModel(TerrainModel terrainModel, PositionDTO positionDTO, GroupDTO group) : base(terrainModel, positionDTO, group)
        {
            this.terrainModel = terrainModel;

        }


    }
}
