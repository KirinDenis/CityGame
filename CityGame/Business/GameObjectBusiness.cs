using CityGame.Data.DTO;
using CityGame.Data.Enum;
using CityGame.Interfaces;
using CityGame.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Threading.Tasks;

namespace CityGame.Business
{
    public class GameObjectBusiness : IGameObject
    {
        protected GameBusiness gameBusiness;
        public GameObjectModel gameObjectModel { get; set; }

        //public EcosystemItemDTO ecosystem = new EcosystemItemDTO();

        public List<GameObjectBusinessDTO> gameObjectBusinessDTOs = new List<GameObjectBusinessDTO>();

        protected virtual string _GroupName { get; set; }

        public virtual string GroupName { get { return _GroupName; } }


        protected Task? liveTask = null;

        protected bool Canceled = false;

        public int cost { get; set; } = 0;

        //public GameObjectBusinessDTO defaultGameObjectBusinessDTO = new GameObjectBusinessDTO();

        public GameObjectBusiness(GameBusiness gameBusiness, GameObjectModel gameObjectModel)
        {
            this.gameBusiness = gameBusiness;
            this.gameObjectModel = gameObjectModel;
            Live();
        }

        public bool Build(PositionDTO positionDTO)
        {
            GameObjectModelDTO gameObjectModelDTO = gameObjectModel.Build(positionDTO);
            if (gameObjectModelDTO != null)
            {
                GameObjectBusinessDTO gameObjectBusinessDTO = new GameObjectBusinessDTO();
                //       gameObjectBusinessDTO.cost = defaultGameObjectBusinessDTO.cost;
                //       gameObjectBusinessDTO.electrified = defaultGameObjectBusinessDTO.electrified;
                gameObjectBusinessDTO.gameObjectModelDTO = gameObjectModelDTO;

                gameObjectBusinessDTO = BuildDeligate(gameObjectBusinessDTO);


                gameObjectBusinessDTOs.Add(gameObjectBusinessDTO);



                return true;
            }

            return false;
        }

        public virtual GameObjectBusinessDTO BuildDeligate(GameObjectBusinessDTO gameObjectBusinessDTO)
        {
            return gameObjectBusinessDTO;
        }

        public virtual void Destroy(GameObjectBusinessDTO gameObjectBusinessDTO)
        {
            gameObjectModel.Destroy(gameObjectBusinessDTO.gameObjectModelDTO);
            gameObjectBusinessDTOs.Remove(gameObjectBusinessDTO);
        }


        [STAThread]
        private void Live()
        {

            liveTask = Task.Run(async delegate
            {
                while (true)
                {
                    try
                    {

                        foreach (GameObjectBusinessDTO gameObjectBusinessDTO in gameObjectBusinessDTOs.ToArray())
                        {
                            LifeCycle(gameObjectBusinessDTO);

                            DebugMessageDTO debugMessage = new DebugMessageDTO();
                            debugMessage.Position = gameObjectBusinessDTO?.gameObjectModelDTO?.positionDTO;                            
                            debugMessage.Properties.Add(ObjectProperties.Day, gameObjectBusinessDTO?.lastDay.ToString());
                            debugMessage.Properties.Add(ObjectProperties.Population, gameObjectBusinessDTO?.EcosystemItem.Population.ToString());
                            debugMessage.Properties.Add(ObjectProperties.Electrified, gameObjectBusinessDTO?.electrified.ToString());
                            debugMessage.Properties.Add(ObjectProperties.PowerPlantId, gameObjectBusinessDTO?.powerPlantId.ToString());
                            debugMessage.Properties.Add(ObjectProperties.PowerTarget, gameObjectBusinessDTO?.powerTarget.ToString());
                            debugMessage.Properties.Add(ObjectProperties.PowerSource, gameObjectBusinessDTO?.powerSource.ToString());

                            gameBusiness.OnDebugMessage(debugMessage);

                        }
                        await Task.Delay(300);
                    }
                    catch (Exception e)
                    {
                        await Task.Delay(100);
                    }

                }                
            });
        }


        public virtual void LifeCycle(GameObjectBusinessDTO gameObjectBusinessDTO)
        {

        }
    }
}
