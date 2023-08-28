using CityGame.Data.DTO;
using CityGame.Graphics;
using CityGame.Models;
using System;
using System.Collections.Generic;

namespace CityGame.Business
{
    public class BasePowerPlantBusiness : GameObjectBusiness
    {

        public BasePowerPlantBusiness(GameBusiness gameBusiness, GameObjectModel gameObjectModel) : base(gameBusiness, gameObjectModel)
        {
        }

        public override GameObjectBusinessDTO BuildDeligate(GameObjectBusinessDTO gameObjectBusinessDTO)
        {
            return null;
        }

        public override void Destroy(GameObjectBusinessDTO gameObjectBusinessDTO)
        {
            gameObjectBusinessDTO.powerSource = 0;
            gameObjectBusinessDTO.electrified = false;
            CalculatePower(gameObjectBusinessDTO);

            base.Destroy(gameObjectBusinessDTO);
        }

        private List<GameObjectBusinessDTO> FindAllPowerNeighbours(List<GameObjectBusinessDTO> currentList, GameObjectBusinessDTO selectedObject)
        {
            if (currentList == null)
            {
                currentList = new List<GameObjectBusinessDTO>();
            }
            List<GameObjectBusinessDTO> gameObjects = gameBusiness.GetNeighbours(selectedObject);
            foreach (GameObjectBusinessDTO gameObject in gameObjects)
            {
                //Roads and rail
                if (gameObject.noPowerConnect)
                {
                    continue;
                }

                if (currentList.IndexOf(gameObject) == -1)
                {
                    currentList.Add(gameObject);
                    currentList = FindAllPowerNeighbours(currentList, gameObject);
                }
            }
            return currentList;
        }

        private void CalculatePower(GameObjectBusinessDTO gameObjectBusinessDTO)
        {
            int powerConsume = 0;

            List<GameObjectBusinessDTO> currentList = FindAllPowerNeighbours(null, gameObjectBusinessDTO);

            foreach (GameObjectBusinessDTO gameObject in currentList)
            {                
                //Roads and rail
                if (gameObject.noPowerConnect)
                {
                    continue;
                }

                if (gameObject.powerSource == 0) //the target object is not power plant 
                {

                    if ((gameObject.powerPlantId == 0) || (gameObject.powerPlantId == gameObjectBusinessDTO.powerPlantId))
                    {
                        powerConsume += gameObject.powerTarget;
                        if (powerConsume <= gameObjectBusinessDTO.powerSource)
                        {
                            gameObject.electrified = gameObject.gameObjectModelDTO.electrified = true;
                            gameObject.powerPlantId = gameObjectBusinessDTO.powerPlantId;
                        }
                        else
                        {
                            gameObject.electrified = gameObject.gameObjectModelDTO.electrified = false;
                            gameObject.powerPlantId = 0;
                            if (gameObjectBusinessDTO.powerSource != 0) //if power source of current plant is ZERO the plant is at destroy process
                            {
                                break;
                            }
                        }

                    }
                }
            }

            gameObjectBusinessDTO.powerTarget = powerConsume;
            if (gameObjectBusinessDTO.powerTarget <= gameObjectBusinessDTO.powerSource)
            {
                gameObjectBusinessDTO.electrified = gameObjectBusinessDTO.gameObjectModelDTO.electrified = true;
            }
            else
            {
                gameObjectBusinessDTO.electrified = gameObjectBusinessDTO.gameObjectModelDTO.electrified = false;
            }

        }

        public void PowerTargetDestroy(GameObjectBusinessDTO gameObjectBusinessDTO)
        {

            List<GameObjectBusinessDTO> currentList = FindAllPowerNeighbours(null, gameObjectBusinessDTO);

            foreach (GameObjectBusinessDTO gameObject in currentList)
            {
                if (gameObject.powerSource == 0) //the target object is not power plant 
                {
                    // if (gameObject.powerPlantId == gameObjectBusinessDTO.powerPlantId)
                    {
                        gameObject.electrified = gameObject.gameObjectModelDTO.electrified = false;
                        gameObject.powerPlantId = 0;
                    }
                }
                else
                {
                    int storePowerSource = gameObjectBusinessDTO.powerSource;
                    gameObjectBusinessDTO.powerSource = 0;
                    //   PowerTargetDestroy(gameObject);
                    gameObjectBusinessDTO.powerSource = storePowerSource;
                    gameObjectBusinessDTO.electrified = true;
                }
            }
        }

        public override void LifeCycle(GameObjectBusinessDTO gameObjectBusinessDTO)
        {
            CalculatePower(gameObjectBusinessDTO);


        }

    }
}
