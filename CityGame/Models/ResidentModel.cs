using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using CityGame.Interfaces;
using CityGame.Models.Interfaces;
using System;
using System.Windows;

namespace CityGame.Models
{
    public enum ResidentMode
    {
        zero, 
        basic,
        standart, 
        hospital,
        church
    };

    public class ResidentModel : GameObjectModel, IGameObjectModel
    {
        protected override string _GroupName { get; set; } = SpritesGroupEnum.resident0;
        public override string GroupName => _GroupName;
       
        public const string zeroLevelGroupName = SpritesGroupEnum.residentBase + "0";
        public const string basicLevelGroupName = SpritesGroupEnum.residentBase + "19";
        public const string hospitalGroupName = SpritesGroupEnum.residentBase + "17";
        public const string churchGroupName = SpritesGroupEnum.residentBase + "18";


        public ResidentModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel) : base(spriteBusiness, terrainModel)
        {            
            startingGroup = spriteBusiness.GetGroupByName(GroupName);            
        }
        protected override void LiveCycle(GameObjectModelDTO gameObjectModelDTO)
        {
            gameObjectModelDTO.timeLive++;

            switch (gameObjectModelDTO.residentMode)
            {
                case ResidentMode.zero: gameObjectModelDTO.Group = startingGroup; break;
                case ResidentMode.basic: gameObjectModelDTO.Group = spriteBusiness.GetGroupByName(basicLevelGroupName); break;
                case ResidentMode.hospital: gameObjectModelDTO.Group = spriteBusiness.GetGroupByName(hospitalGroupName); break;
                case ResidentMode.church: gameObjectModelDTO.Group = spriteBusiness.GetGroupByName(churchGroupName); break;
                default:
                    gameObjectModelDTO.Group = spriteBusiness.GetGroupByName(SpritesGroupEnum.residentBase + gameObjectModelDTO.level); break;
            }
            
            if (gameObjectModelDTO.Group == null)
            {
                gameObjectModelDTO.level = 0;
                gameObjectModelDTO.Group = startingGroup;
            }

            if ((gameObjectModelDTO.residentMode != ResidentMode.basic) && (this != null))
            {
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
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (!Canceled)
                    {
                        if ((gameObjectModelDTO != null) && (gameObjectModelDTO.Group != null) && (gameObjectModelDTO.positionDTO != null))
                        {
                            for(int bx=0; bx<3; bx++)
                            {
                                for (int by = 0; by < 3; by++)
                                {
                                    if (gameObjectModelDTO.basicHouses[bx, by] != null)
                                    {
                                        terrainModel.PutSprite(
                                            (ushort)(gameObjectModelDTO.positionDTO.x + bx),
                                            (ushort)(gameObjectModelDTO.positionDTO.y + by),
                                            gameObjectModelDTO.Group,
                                            gameObjectModelDTO.basicHouses[bx, by]);
                                    }
                                    else 
                                    {
                                        terrainModel.PutSprite(
                                            (ushort)(gameObjectModelDTO.positionDTO.x + bx),
                                            (ushort)(gameObjectModelDTO.positionDTO.y + by),
                                            startingGroup,
                                            0,
                                            bx,
                                            by);

                                    }
                                }
                            }
                        }
                    }
                });
            }               
            base.DrawElectrified(gameObjectModelDTO);
        }
    }
}
