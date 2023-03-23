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
        
        public ResidetBusiness(GameObjectModel gameObjectModel) : base(gameObjectModel)
        {
            this.gameObjectModel = gameObjectModel;
            cost = 100;
        }

        public override void LifeCycle(GameObjectBusinessDTO gameObjectBusinessDTO)
        {
            gameObjectBusinessDTO.EcosystemItem.Polution++;
            if (gameObjectBusinessDTO.EcosystemItem.Polution > 2 * gameObjectBusinessDTO.gameObjectModelDTO.level)
            {
                gameObjectBusinessDTO.gameObjectModelDTO.level++;

                //gameObjectBusinessDTO.gameObjectModelDTO.animationFrame++;
            }    
        }
    }
}
