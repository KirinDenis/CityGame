using CityGame.Data.DTO;
using CityGame.Models;
using System;
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

                //temp
                if (gameObjectBusinessDTO.electrified)
                {
                    if (gameObjectBusinessDTO.EcosystemItem.Population < 100)
                    {
                        gameObjectBusinessDTO.EcosystemItem.Population += (byte)(gameBusiness.gameDay - gameObjectBusinessDTO.lastDay);
                        gameObjectBusinessDTO.EcosystemItem.Population += 2;
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

                        int classFromCount = 9 - (int)((gameObjectBusinessDTO.EcosystemItem.Population - 25 - (100 / 4 * (currentClass-1))) / 25.0 * 9.0);
                        classFromCount = (classFromCount < 0) || (classFromCount > 8) ? 0 : classFromCount;

                        int classToCount = (int)((gameObjectBusinessDTO.EcosystemItem.Population - (100 / 4 * (currentClass - 1))) / 25.0 * 9.0);
                        classToCount = (classToCount < 0) || (classToCount > 9) ? 0 : classToCount;

                        currentResidentSections[currentClass] = classFromCount + classToCount;
                        currentResidentSections[0] -= currentResidentSections[currentClass];
                    }

                    Debug.WriteLine("class 0 : " + currentResidentSections[0]);
                    Debug.WriteLine("class 1 : " + currentResidentSections[1]);
                    Debug.WriteLine("class 2 : " + currentResidentSections[2]);
                    Debug.WriteLine("class 3 : " + currentResidentSections[3]);
                    Debug.WriteLine("class 4 : " + currentResidentSections[4]);



                    /*
                    currentResidentSections[1] = class1FromCount + class1ToCount;

                    int class2FromCount = 9 - (int)((gameObjectBusinessDTO.EcosystemItem.Population - 25 - (100 / 4 * 1)) / 25.0 * 9.0);
                    class2FromCount = (class2FromCount < 0) || (class2FromCount > 8) ? 0 : class2FromCount;

                    int class2ToCount = (int)((gameObjectBusinessDTO.EcosystemItem.Population - (100 / 4 * 1)) / 25.0 * 9.0);
                    class2ToCount = (class2ToCount < 0) || (class2ToCount > 9) ? 0 : class2ToCount;

                    currentResidentSections[2] = class2FromCount + class2ToCount;


                    int class3Count = (int)((gameObjectBusinessDTO.EcosystemItem.Population - (100 / 4 * 2)) / 25.0 * 9.0);
                    class3Count = (class3Count < 0) || (class3Count > 9) ? 0 : class3Count;

                    int class4Count = (int)((gameObjectBusinessDTO.EcosystemItem.Population - (100 / 4 * 3)) / 25.0 * 9.0);
                    class4Count = (class4Count < 0) || (class4Count > 9) ? 0 : class4Count;
                    */

                    //Debug.WriteLine("class 1 from:" + class1FromCount);
                    //Debug.WriteLine("class 1 to:" + class1ToCount);

                    //Debug.WriteLine("class 2 from:" + class2FromCount);
                    //Debug.WriteLine("class 2 to:" + class2ToCount);

                    Debug.WriteLine("class 1 : " + currentResidentSections[1]);
                    Debug.WriteLine("class 2 : " + currentResidentSections[2]);


                    int houseCount = 0;

                    /*
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
                    */
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
