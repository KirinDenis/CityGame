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
    public class WireBusiness : GameObjectBusiness
    {
        
        public WireBusiness(GameBusiness gameBusiness, GameObjectModel gameObjectModel) : base(gameBusiness, gameObjectModel)
        {
            this.gameObjectModel = gameObjectModel;
            cost = 10;
        }

        public override GameObjectBusinessDTO BuildDeligate(GameObjectBusinessDTO gameObjectBusinessDTO)
        {            
            gameObjectBusinessDTO.powerTarget = 0;

            return gameObjectBusinessDTO;
        }


        public override void LifeCycle(GameObjectBusinessDTO gameObjectBusinessDTO)
        {
            List<GameObjectBusinessDTO> GetNeighbours = gameBusiness.GetNeighbours(gameObjectBusinessDTO);

            gameObjectBusinessDTO.electrified = false;
            foreach (GameObjectBusinessDTO currentGameObjectBusinessDTO in GetNeighbours)
            {
                if (currentGameObjectBusinessDTO.electrified)
                {
                    gameObjectBusinessDTO.electrified = true;
                    break;
                }
            }
        }
    }
}
