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
    public class ResidetBusiness : GameObjectBusiness
    {
        
        public ResidetBusiness(GameBusiness gameBusiness, GameObjectModel gameObjectModel) : base(gameBusiness, gameObjectModel)
        {
            this.gameObjectModel = gameObjectModel;
            defaultGameObjectBusinessDTO.cost = 100;
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

            if (gameObjectBusinessDTO.electrified)
            {
                if (gameObjectBusinessDTO.EcosystemItem.Population < 30)
                {
                    gameObjectBusinessDTO.EcosystemItem.Population += 10;
                }
            }
            else
            {
                if (gameObjectBusinessDTO.EcosystemItem.Population > 10)
                {
                    gameObjectBusinessDTO.EcosystemItem.Population -= 5;
                }
                else
                {
                    gameObjectBusinessDTO.EcosystemItem.Population = 5;
                }
            }

            if (gameObjectBusinessDTO.EcosystemItem.Population == 0)
            {
                gameObjectBusinessDTO.gameObjectModelDTO.level = 0;
            }
            else
                if (gameObjectBusinessDTO.EcosystemItem.Population <= 5)
            {
                gameObjectBusinessDTO.gameObjectModelDTO.level = 19;
            }
            else
                if (gameObjectBusinessDTO.EcosystemItem.Population <= 15)
            {
                gameObjectBusinessDTO.gameObjectModelDTO.level = 2;
            }
            else
                if (gameObjectBusinessDTO.EcosystemItem.Population <= 35)
            {
                gameObjectBusinessDTO.gameObjectModelDTO.level = 3;
            }




            /*
            gameObjectBusinessDTO.EcosystemItem.Population+=10;
            if (gameObjectBusinessDTO.EcosystemItem.Population > 255)
            {
                gameObjectBusinessDTO.EcosystemItem.Population = 0;
            }    
            if (gameObjectBusinessDTO.EcosystemItem.Population > 2 * gameObjectBusinessDTO.gameObjectModelDTO.level)
            {
                gameObjectBusinessDTO.gameObjectModelDTO.level++;

                //gameObjectBusinessDTO.gameObjectModelDTO.animationFrame++;
            } 
            */
        }
    }
}
