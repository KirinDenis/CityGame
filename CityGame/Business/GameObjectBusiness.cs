using CityGame.Data.DTO;
using CityGame.Interfaces;
using CityGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGame.Business
{
    public class GameObjectBusiness: IGameObject
    {
        public GameObjectModel gameObjectModel { get; set; }

        //public EcosystemItemDTO ecosystem = new EcosystemItemDTO();

        public List<GameObjectBusinessDTO> gameObjectBusinessDTOs = new List<GameObjectBusinessDTO>();

        protected virtual string _GroupName { get; set; }

        public virtual string GroupName { get { return _GroupName; } }

        public int cost { get; set; }

        protected Task? liveTask = null;

        protected bool Canceled = false;

        public GameObjectBusiness(GameObjectModel gameObjectModel)
        {
            this.gameObjectModel = gameObjectModel;
            Live();
        }

        public virtual bool Build(PositionDTO positionDTO)
        {
            GameObjectModelDTO gameObjectModelDTO = gameObjectModel.Build(positionDTO);
            if (gameObjectModelDTO != null)
            {
                GameObjectBusinessDTO gameObjectBusinessDTO = new GameObjectBusinessDTO();
                gameObjectBusinessDTO.gameObjectModelDTO = gameObjectModelDTO;
                gameObjectBusinessDTOs.Add(gameObjectBusinessDTO);
                return true;
            }

            return false;
        }


        private void Live()
        {

            liveTask = Task.Run(async delegate
            {
                while (!Canceled)
                {
                    foreach (GameObjectBusinessDTO gameObjectBusinessDTO in gameObjectBusinessDTOs)
                    {
                        LifeCycle(gameObjectBusinessDTO);
                    }
                    await Task.Delay(300);
                }
            });
        }


        public virtual void LifeCycle(GameObjectBusinessDTO gameObjectBusinessDTO)
        {

        }
    }
}
