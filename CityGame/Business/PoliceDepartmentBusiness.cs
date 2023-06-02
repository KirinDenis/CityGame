﻿using CityGame.Data.DTO;
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
    public class PoliceDepartmentBusiness : GameObjectBusiness
    {
        
        public PoliceDepartmentBusiness(GameBusiness gameBusiness, GameObjectModel gameObjectModel) : base(gameBusiness, gameObjectModel)
        {
            this.gameObjectModel = gameObjectModel;
            cost = 500;
        }

        public override GameObjectBusinessDTO BuildDeligate(GameObjectBusinessDTO gameObjectBusinessDTO)
        {            
            gameObjectBusinessDTO.powerTarget = 0;

            return gameObjectBusinessDTO;
        }


        public override void LifeCycle(GameObjectBusinessDTO gameObjectBusinessDTO)
        {
        }
    }
}
