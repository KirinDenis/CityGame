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
        //TEMP
        private Random random = new Random();   
        
        public ResidetBusiness(GameBusiness gameBusiness, GameObjectModel gameObjectModel) : base(gameBusiness, gameObjectModel)
        {
            this.gameObjectModel = gameObjectModel;
            cost = 100;
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

            gameObjectBusinessDTO.powerTarget = gameObjectBusinessDTO.EcosystemItem.Population * 5;

            for (int bx = 0; bx < 3; bx++)
            {
                for (int by = 0; by < 3; by++)
                {
                    if (random.Next(100) > 15)
                    {
                        gameObjectBusinessDTO.gameObjectModelDTO.basicHouses[bx, by] = new PositionDTO()
                        {
                            x = (ushort)random.Next(3),
                            y = (ushort)random.Next(4),
                        };
                    }
                    else
                    {
                        gameObjectBusinessDTO.gameObjectModelDTO.basicHouses[bx, by] = null;
                    }
                }
            };

            gameObjectBusinessDTO.gameObjectModelDTO.residentMode = ResidentMode.basic;


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
