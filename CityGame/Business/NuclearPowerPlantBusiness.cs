using CityGame.Data.DTO;
using CityGame.DTOs.Const;
using CityGame.Models;
using System;

namespace CityGame.Business
{
    public class NuclearPowerPlantBusiness : BasePowerPlantBusiness
    {

        public NuclearPowerPlantBusiness(GameBusiness gameBusiness, GameObjectModel gameObjectModel) : base(gameBusiness, gameObjectModel)
        {
            this.gameObjectModel = gameObjectModel;
            cost = CostsConsts.NuclearPower;
        }

        public override GameObjectBusinessDTO BuildDeligate(GameObjectBusinessDTO gameObjectBusinessDTO)
        {
            gameObjectBusinessDTO.electrified = true;
            gameObjectBusinessDTO.powerTarget = 0;
            gameObjectBusinessDTO.powerSource = PowerConsts.NuclearPower;
            gameObjectBusinessDTO.powerPlantId = DateTime.Now.Millisecond + DateTime.Now.Minute;

            return gameObjectBusinessDTO;
        }
    }
}
