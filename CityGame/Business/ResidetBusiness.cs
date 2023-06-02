using CityGame.Data.DTO;
using CityGame.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        public byte[] GetBasicHousesState(PositionDTO[,] basicHouses)
        {
            byte[] result = new byte[5];
            for (int bx = 0; bx < 3; bx++)
            {
                for (int by = 0; by < 3; by++)
                {
                    if (basicHouses[bx,by] == null)
                    {
                        result[0]++;
                    }
                    else
                    {
                        result[basicHouses[bx, by].y+1]++;
                    }
                }
            }
            return result;
        }

        private void DeleteRandomBasicHouse(int houseClass, PositionDTO[,] basicHouses)
        {
            int tryCount = 0;
            int x;
            int y;

            while (tryCount < 9)
            {
                x = random.Next(3);
                y = random.Next(3);

                if (basicHouses[x, y] != null)
                {
                    if (houseClass == 0)
                    {
                            basicHouses[x, y] = null;
                            return;
                    }
                    else
                    {
                        if (basicHouses[x, y].y == houseClass-1)
                        {
                            basicHouses[x, y] = null;
                            return;
                        }
                    }
                }
                tryCount++;
            }

            for (x = 0; x < 3; x++)
            {
                for (y = 0; y < 3; y++)
                {
                    if (basicHouses[x, y] != null)
                    {
                        if (houseClass == 0)
                        {
                            basicHouses[x, y] = null;
                            return;
                        }
                        else
                        {
                            if (basicHouses[x, y].y == houseClass - 1)
                            {
                                basicHouses[x, y] = null;
                                return;
                            }
                        }
                    }
                }
            }           
        }

        public void BuildRandomBasicHouse(int houseClass, PositionDTO[,] basicHouses)
        {
            int tryCount = 0;
            int x;
            int y;

            while (tryCount < 9)
            {
                x = random.Next(3);
                y = random.Next(3);
                if (basicHouses[x, y] == null)
                {
                    basicHouses[x, y] = new PositionDTO()
                    {
                        x = (ushort)random.Next(3),
                        y = (ushort)(houseClass - 1)
                    };
                    return;
                }
                tryCount++;
            }

            for (x = 0; x < 3; x++)
            {
                for (y = 0; y < 3; y++)
                {
                    if (basicHouses[x, y] == null)
                    {
                        basicHouses[x, y] = new PositionDTO()
                        {
                            x = (ushort)random.Next(3),
                            y = (ushort)(houseClass - 1)
                        };
                    }
                }
            }
        }



        public override void LifeCycle(GameObjectBusinessDTO gameObjectBusinessDTO)
        {


            
          //  gameObjectBusinessDTO.gameObjectModelDTO.electrified = gameObjectBusinessDTO.electrified = false;            
            List<GameObjectBusinessDTO> GetNeighbours = gameBusiness.GetNeighbours(gameObjectBusinessDTO);

            
            foreach (GameObjectBusinessDTO currentGameObjectBusinessDTO in GetNeighbours)
            {
                if (currentGameObjectBusinessDTO.electrified)
                {
           //         gameObjectBusinessDTO.gameObjectModelDTO.electrified = gameObjectBusinessDTO.electrified = true;
           //         gameObjectBusinessDTO.gameObjectModelDTO.electrified = gameObjectBusinessDTO.electrified = true;
                    break;
                }
            }



            if (gameBusiness.gameDay > gameObjectBusinessDTO.lastDay)
            {
                //temp
                if (gameObjectBusinessDTO.electrified)
                {
                    if (gameObjectBusinessDTO.EcosystemItem.Population < 100)
                    {
                        //    gameObjectBusinessDTO.EcosystemItem.Population += (byte)(gameObjectBusinessDTO.lastDay);
                        gameObjectBusinessDTO.EcosystemItem.Population += 2;
                    }
                }
                else
                {
                    if (gameObjectBusinessDTO.EcosystemItem.Population > 0)
                    {
                        gameObjectBusinessDTO.EcosystemItem.Population -= 4;
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
                    if (gameObjectBusinessDTO.EcosystemItem.Population <= 100)
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
                //end of temp

                if (gameObjectBusinessDTO.gameObjectModelDTO.residentMode == ResidentMode.basic)
                {
                    //basic citezen count 0..100 (population)
                    //basic houses 0..5
                    //basic class 1 from 0 citizen to 25 (100 / 4 -> 0 class is empty residend section)
                    //if citizen count = 10, it means 5 section of 0 class and 4 section of class 1

                    int[] currentResidentSections = new int[5]; //needed houses located on resident area sections
                    currentResidentSections[0] = 9;
                    for (int currentClass = 1; currentClass < 5; currentClass++)
                    {

                        int classFromCount = 9 - (int)((gameObjectBusinessDTO.EcosystemItem.Population - 25 - (100 / 4 * (currentClass - 1))) / 25.0 * 9.0);
                        classFromCount = (classFromCount < 0) || (classFromCount > 8) ? 0 : classFromCount;

                        int classToCount = (int)((gameObjectBusinessDTO.EcosystemItem.Population - (100 / 4 * (currentClass - 1))) / 25.0 * 9.0);
                        classToCount = (classToCount < 0) || (classToCount > 9) ? 0 : classToCount;

                        currentResidentSections[currentClass] = classFromCount + classToCount;
                        currentResidentSections[0] -= currentResidentSections[currentClass];
                    }

                    /*
                    Debug.WriteLine("class 0 : " + currentResidentSections[0]);
                    Debug.WriteLine("class 1 : " + currentResidentSections[1]);
                    Debug.WriteLine("class 2 : " + currentResidentSections[2]);
                    Debug.WriteLine("class 3 : " + currentResidentSections[3]);
                    Debug.WriteLine("class 4 : " + currentResidentSections[4]);
                    */

                    byte[] prevResidentSections = GetBasicHousesState(gameObjectBusinessDTO.gameObjectModelDTO.basicHouses);

                    for (int i = 1; i < 5; i++)
                    {
                        if (prevResidentSections[i] > currentResidentSections[i])
                        {
                            for (int j = 0; j < prevResidentSections[i] - currentResidentSections[i]; j++)
                            {
                                DeleteRandomBasicHouse(i, gameObjectBusinessDTO.gameObjectModelDTO.basicHouses);
                            }
                        }
                    }

                    for (int i = 1; i < 5; i++)
                    {
                        if (prevResidentSections[i] < currentResidentSections[i])
                        {
                            for (int j = 0; j < currentResidentSections[i] - prevResidentSections[i]; j++)
                            {
                                BuildRandomBasicHouse(i, gameObjectBusinessDTO.gameObjectModelDTO.basicHouses);
                            }
                        }
                    }
                }
            }

        }
    }
}
