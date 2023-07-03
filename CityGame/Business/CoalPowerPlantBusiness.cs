using CityGame.Data.DTO;
using CityGame.DTOs.Const;
using CityGame.Models;
using System;

namespace CityGame.Business
{
    public class CoalPowerPlantBusiness : BasePowerPlantBusiness
    {

        public CoalPowerPlantBusiness(GameBusiness gameBusiness, GameObjectModel gameObjectModel) : base(gameBusiness, gameObjectModel)
        {
            this.gameObjectModel = gameObjectModel;
            cost = CostsConsts.CoalPower;
        }

        public override GameObjectBusinessDTO BuildDeligate(GameObjectBusinessDTO gameObjectBusinessDTO)
        {

            gameObjectBusinessDTO.electrified = true;
            gameObjectBusinessDTO.powerTarget = 0;
            gameObjectBusinessDTO.powerSource = PowerConsts.CoalPower;
            gameObjectBusinessDTO.powerPlantId = DateTime.Now.Millisecond + DateTime.Now.Minute;

            return gameObjectBusinessDTO;
        }

    }
}
