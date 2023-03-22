using CityGame.Data.DTO;
using CityGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGame.Business
{
    public class GameObjectBusiness
    {
        private GameObjectModel gameObjectModel { get; set; }

        public EcosystemItemDTO ecosystem = new EcosystemItemDTO();

        public GameObjectBusiness(GameObjectModel gameObjectModel)
        {
            this.gameObjectModel = gameObjectModel;
        }
    }
}
