using CityGame.Data.DTO;
using CityGame.Models;
using System;
using System.Collections.Generic;

namespace CityGame.Business
{
    public class CoalPowerPlantBusiness : BasePowerPlantBusiness
    {

        public CoalPowerPlantBusiness(GameBusiness gameBusiness, GameObjectModel gameObjectModel) : base(gameBusiness, gameObjectModel)
        {
            this.gameObjectModel = gameObjectModel;
            cost = 3000;
        }

        public override GameObjectBusinessDTO BuildDeligate(GameObjectBusinessDTO gameObjectBusinessDTO)
        {

            gameObjectBusinessDTO.electrified = true;
            gameObjectBusinessDTO.powerTarget = 0;
            gameObjectBusinessDTO.powerSource = 500;
            gameObjectBusinessDTO.powerPlantId = DateTime.Now.Millisecond + DateTime.Now.Minute;

            return gameObjectBusinessDTO;
        }

    }
}
