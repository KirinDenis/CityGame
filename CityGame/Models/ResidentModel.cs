﻿using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using System;
using System.Windows;

namespace CityGame.Models
{
    internal class ResidentModel : GameObjectModel
    {
        private readonly Random random = new();
        public ResidentModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {
            startingGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.resident0);

        }
        protected override void LiveCycle(GameObjectDTO gameObject)
        {
            gameObject.timeLive++;
            if (gameObject.timeLive > random.Next(10))
            {
                gameObject.timeLive = 0;
                gameObject.level++;
                gameObject.Group = spriteBusiness.GetGroupByName(SpritesGroupEnum.residentBase + gameObject.level);
                if (gameObject.Group == null)
                {
                    gameObject.level = 0;
                    gameObject.Group = startingGroup;
                }
                gameObject.animationFrame = 0;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (!Canceled)
                    {
                        if ((gameObject != null) && (gameObject.Group != null) && (gameObject.positionDTO != null))
                        {
                            terrainModel.BuildObject(gameObject.positionDTO.x, gameObject.positionDTO.y, gameObject.Group, gameObject.animationFrame);
                        }
                    }
                });
            }
        }
    }
}
