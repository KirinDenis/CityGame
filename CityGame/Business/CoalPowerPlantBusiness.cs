using CityGame.Data.DTO;
using CityGame.Models;
using System;
using System.Collections.Generic;

namespace CityGame.Business
{
    public class CoalPowerPlantBusiness : GameObjectBusiness
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

        private int PowerNeighbours(GameObjectBusinessDTO coalPowerPlantBusinessDTO, GameObjectBusinessDTO selectedObject, int powerConsume)
        {
            List<GameObjectBusinessDTO> gameObjects = gameBusiness.GetNeighbours(selectedObject);
            foreach (GameObjectBusinessDTO gameObject in gameObjects)
            {
                if (gameObject.powerSource == 0) //the target object is not power plant 
                {

                    if ((gameObject.powerPlantId == 0) || (gameObject.powerPlantId == coalPowerPlantBusinessDTO.powerPlantId))
                    {
                        if (gameObject.powerPlantSession != coalPowerPlantBusinessDTO.powerPlantSession)
                        {
                            if (gameObject.powerSource == 0) //the target object is not power plant 
                            {
                                gameObject.powerPlantSession = coalPowerPlantBusinessDTO.powerPlantSession;
                                if (powerConsume + gameObject.powerTarget < coalPowerPlantBusinessDTO.powerSource)
                                {
                                    gameObject.electrified = true;
                                    gameObject.powerPlantId = coalPowerPlantBusinessDTO.powerPlantId;
                                }
                                else
                                {
                                    gameObject.electrified = false;
                                    gameObject.powerPlantId = 0;
                                }

                                powerConsume += gameObject.powerTarget;
                                gameObject.gameObjectModelDTO.electrified = gameObject.electrified;
                                powerConsume = PowerNeighbours(coalPowerPlantBusinessDTO, gameObject, powerConsume);
                            }
                        }
                        
                    }
                }
                else
                {
                    if (gameObject.powerPlantId != coalPowerPlantBusinessDTO.powerPlantId) //it is not current power plant
                    {
                       // powerConsume = PowerNeighbours(coalPowerPlantBusinessDTO, gameObject, powerConsume);
                    }
                }
                
            }
            return powerConsume;
        }



        public override void LifeCycle(GameObjectBusinessDTO gameObjectBusinessDTO)
        {

            gameObjectBusinessDTO.powerPlantSession++;
            int powerConsume = PowerNeighbours(gameObjectBusinessDTO, gameObjectBusinessDTO, 0);

            if (powerConsume > gameObjectBusinessDTO.powerSource)
            {
                gameObjectBusinessDTO.electrified = false;
            }
            else
            {
                gameObjectBusinessDTO.electrified = true;
            }
            gameObjectBusinessDTO.gameObjectModelDTO.electrified = gameObjectBusinessDTO.electrified;

            /*
                List<GameObjectBusinessDTO> gameObjects = gameBusiness.GetNeighbours(gameObjectBusinessDTO);
                foreach(GameObjectBusinessDTO  gameObject in gameObjects)
                {
                    if (gameObject.powerSource == 0)
                    {
                        if (powerConsume + gameObject.powerTarget < gameObjectBusinessDTO.powerSource)
                        {                        
                            gameObject.electrified = true;
                        }
                        else
                        {
                            gameObject.electrified = false;
                        }
                        powerConsume += gameObject.powerTarget;
                        gameObject.gameObjectModelDTO.electrified = gameObject.electrified;                    
                    }
                }    
                if (powerConsume > gameObjectBusinessDTO.powerSource)
                {
                    gameObjectBusinessDTO.electrified = false;
                }
                else 
                {
                    gameObjectBusinessDTO.electrified = true;
                }
                gameObjectBusinessDTO.gameObjectModelDTO.electrified = gameObjectBusinessDTO.electrified;
            */
        }
    }
}
