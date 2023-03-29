using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using CityGame.Interfaces;
using CityGame.Models.Interfaces;
using System;
using System.Windows;

namespace CityGame.Models
{
    public class ResidentModel : GameObjectModel, IGameObjectModel
    {
        protected override string _GroupName { get; set; } = SpritesGroupEnum.resident0;
        public override string GroupName => _GroupName;        
        public ResidentModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {            
            startingGroup = spriteBusiness.GetGroupByName(GroupName);
        }
        protected override void LiveCycle(GameObjectModelDTO gameObjectModelDTO)
        {
            gameObjectModelDTO.timeLive++;

            gameObjectModelDTO.Group = spriteBusiness.GetGroupByName(SpritesGroupEnum.residentBase + gameObjectModelDTO.level);
            if (gameObjectModelDTO.Group == null)
            {
                gameObjectModelDTO.level = 0;
                gameObjectModelDTO.Group = startingGroup;
            }
            gameObjectModelDTO.animationFrame = 0;


            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!Canceled)
                {
                    if ((gameObjectModelDTO != null) && (gameObjectModelDTO.Group != null) && (gameObjectModelDTO.positionDTO != null))
                    {
                        terrainModel.BuildObject(gameObjectModelDTO.positionDTO.x, gameObjectModelDTO.positionDTO.y, gameObjectModelDTO.Group, gameObjectModelDTO.animationFrame);
                    }
                }
            });

            base.LiveCycle(gameObjectModelDTO);

            /*
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
            */
        }
    }
}
