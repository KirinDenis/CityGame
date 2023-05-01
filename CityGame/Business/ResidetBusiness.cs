using CityGame.Data.DTO;
using CityGame.Models;
using System;

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


            /*
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
            */

            if (gameBusiness.gameDay > gameObjectBusinessDTO.lastDay)
            {


                if (gameObjectBusinessDTO.electrified)
                {
                    if (gameObjectBusinessDTO.EcosystemItem.Population < 100)
                    {
                        gameObjectBusinessDTO.EcosystemItem.Population += (byte)(gameBusiness.gameDay - gameObjectBusinessDTO.lastDay);
                    }
                }
                else
                {
                    if (gameObjectBusinessDTO.EcosystemItem.Population > 0)
                    {
                        gameObjectBusinessDTO.EcosystemItem.Population -= (byte)((gameBusiness.gameDay - gameObjectBusinessDTO.lastDay) * 2);
                    }

                    if (gameObjectBusinessDTO.EcosystemItem.Population < 0)
                    {
                        gameObjectBusinessDTO.EcosystemItem.Population = 0;
                    }
                }

                if (gameObjectBusinessDTO.EcosystemItem.Population == 0)
                {
                    gameObjectBusinessDTO.gameObjectModelDTO.level = 0;
                    gameObjectBusinessDTO.gameObjectModelDTO.residentMode = ResidentMode.zero;
                }
                else
                    if (gameObjectBusinessDTO.EcosystemItem.Population <= 50)
                {
                    gameObjectBusinessDTO.gameObjectModelDTO.level = 1;
                    gameObjectBusinessDTO.gameObjectModelDTO.residentMode = ResidentMode.basic;
                }
                else
                {
                    gameObjectBusinessDTO.gameObjectModelDTO.level = 2;
                    gameObjectBusinessDTO.gameObjectModelDTO.residentMode = ResidentMode.standart;
                }
                gameObjectBusinessDTO.powerTarget = gameObjectBusinessDTO.EcosystemItem.Population * 5;

                if (gameObjectBusinessDTO.gameObjectModelDTO.residentMode == ResidentMode.basic)
                {
                    //0..50 citizen - 0..9 houses
                    //up to 30 citizen - high price houses (y = 4)
                    int currentHouseCost = 0; //(Y)
                    if (gameObjectBusinessDTO.EcosystemItem.Population > 40)
                    {
                        currentHouseCost = 3;
                    }
                    else
                    if (gameObjectBusinessDTO.EcosystemItem.Population > 30)
                    {
                        currentHouseCost = 2;
                    }
                    else
                    if (gameObjectBusinessDTO.EcosystemItem.Population > 10)
                    {
                        currentHouseCost = 1;
                    }

                    int houseCount = 0;

                    for (int bx = 0; bx < 3; bx++)
                    {
                        for (int by = 0; by < 3; by++)
                        {
                            houseCount++;
                            if ((gameObjectBusinessDTO.gameObjectModelDTO.basicHouses[bx, by] == null)
                                ||
                                (gameObjectBusinessDTO.gameObjectModelDTO.basicHouses[bx, by].y != currentHouseCost))
                            {
                                gameObjectBusinessDTO.gameObjectModelDTO.basicHouses[bx, by] = new PositionDTO()
                                {
                                    x = (ushort)random.Next(3),
                                    y = (ushort)currentHouseCost,
                                };
                            }
                            if ((currentHouseCost ==0) && (houseCount >= gameObjectBusinessDTO.EcosystemItem.Population))
                            {
                                break;
                            }    
                        }
                    }
                }
                else
                {
                //    gameObjectBusinessDTO.gameObjectModelDTO.basicHouses[bx, by] = null;
                }


                gameObjectBusinessDTO.lastDay = gameBusiness.gameDay;

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
