using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using CityGame.Interfaces;
using CityGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGame.Business
{
    public class CoalPowerPlantBusiness : GameObjectBusiness
    {
        
        public CoalPowerPlantBusiness(GameBusiness gameBusiness, GameObjectModel gameObjectModel) : base(gameBusiness, gameObjectModel)
        {
            this.gameObjectModel = gameObjectModel;
            defaultGameObjectBusinessDTO.cost = 500;
            defaultGameObjectBusinessDTO.electrified = true;
        }

        public override void LifeCycle(GameObjectBusinessDTO gameObjectBusinessDTO)
        {
        }
    }
}
