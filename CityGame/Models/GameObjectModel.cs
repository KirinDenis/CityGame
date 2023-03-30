using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using CityGame.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace CityGame.Models
{
    public class GameObjectModel : BaseModel, IGameObjectModel, IDisposable
    {
        protected SpriteBusiness spriteBusiness;
        protected TerrainModel terrainModel;

        protected Task? liveTask = null;

        protected bool Canceled = false;

        public List<GameObjectModelDTO> gameObjectModelDTOs = new List<GameObjectModelDTO>();

        public GroupDTO? startingGroup { get; set; }

        public GroupDTO? electricGroup { get; set; }

        public DateTime electricBlink { get; set; } = DateTime.Now;

        protected virtual string _GroupName { get; set; }

        public virtual string GroupName { get { return _GroupName; } }




        //  protected virtual string _GroupName { get; set; }

        public GameObjectModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel)
        {
            this.spriteBusiness = spriteBusiness;
            this.terrainModel = terrainModel;
            electricGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.electrified);
            Live();
        }

        public virtual GameObjectModelDTO Build(PositionDTO positionDTO)
        {
            if (startingGroup == null)
            {
                return null;
            }

            GameObjectModelDTO gameObjectModelDTO = new GameObjectModelDTO()
            {
                positionDTO = positionDTO,
                centerPosition = new PositionDTO()
                {
                    x = (ushort)(positionDTO.x + this.startingGroup.CenterX),
                    y = (ushort)(positionDTO.y + this.startingGroup.CenterY),
                },
                Group = this.startingGroup
            };
            terrainModel.BuildObject(positionDTO.x, positionDTO.y, startingGroup, 0);

            gameObjectModelDTOs.Add(gameObjectModelDTO);

            return gameObjectModelDTO;
        }

        private void Live()
        {

            liveTask = Task.Run(async delegate
            {
                while (!Canceled)
                {
                    foreach (GameObjectModelDTO gameObjectModelDTO in gameObjectModelDTOs.ToArray())
                    {
                        LiveCycle(gameObjectModelDTO);

                    }

                    if ((DateTime.Now - electricBlink).TotalMilliseconds > 1000)
                    {
                        electricBlink = DateTime.Now;
                    }
                    await Task.Delay(300);
                }
            });
        }

        protected virtual void LiveCycle(GameObjectModelDTO gameObjectModelDTO)
        {

            if (gameObjectModelDTO.Group?.Frames.Count > 1)
            {
                if (gameObjectModelDTO.animationFrame >= gameObjectModelDTO.Group.Frames.Count)
                {
                    gameObjectModelDTO.animationFrame = 1;
                }
                _ = Application.Current.Dispatcher.Invoke(() =>
                {
                    if (!Canceled)
                    {
                        if (gameObjectModelDTO.positionDTO != null)
                        {
                            terrainModel.BuildObject(gameObjectModelDTO.positionDTO.x, gameObjectModelDTO.positionDTO.y, gameObjectModelDTO.Group, gameObjectModelDTO.animationFrame);
                        }
                    }

                    return Task.CompletedTask;
                });
                gameObjectModelDTO.animationFrame++;
            }


        }

        public void DrawElectrified(GameObjectModelDTO gameObjectModelDTO)
        {
            if ((DateTime.Now - electricBlink).TotalMilliseconds > 1000)
            {

                if (!gameObjectModelDTO.electrified)
                {
                    if (gameObjectModelDTO.positionDTO != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (!Canceled)
                            {

                                terrainModel.BuildObject(gameObjectModelDTO.centerPosition.x, gameObjectModelDTO.centerPosition.y, electricGroup, 0);

                            }

                        });
                    }
                }
            }

        }

        public void Destroy(GameObjectModelDTO gameObjectModelDTO)
        {

            gameObjectModelDTOs.Remove(gameObjectModelDTO);
            byte destroyStep = 0;
            GroupDTO? destroyGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.explosion);
            Task destroyTask = Task.Run(async delegate
            {
                while (destroyStep < 7)
                {
                    destroyStep++;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (!Canceled)
                        {
                            for (ushort sx = 0; sx < gameObjectModelDTO?.Group?.Width; sx++)
                            {
                                for (ushort sy = 0; sy < gameObjectModelDTO?.Group?.Height; sy++)
                                {
                                    if (destroyGroup?.Frames?[destroyStep]?.Sprites?[0, 0] != null)
                                    {
                                        if ((gameObjectModelDTO != null) && (destroyGroup != null) && (gameObjectModelDTO.positionDTO != null))
                                        {
                                            terrainModel.PutSprite((ushort)(gameObjectModelDTO.positionDTO.x + sx), (ushort)(gameObjectModelDTO.positionDTO.y + sy), destroyGroup, destroyStep, 0, 0);
                                        }
                                    }
                                }
                            }
                            if ((gameObjectModelDTO != null) && (destroyGroup != null) && (gameObjectModelDTO.positionDTO != null))
                            {
                                terrainModel.BuildObject(gameObjectModelDTO.positionDTO.x, gameObjectModelDTO.positionDTO.y, destroyGroup, destroyStep);
                            }
                        }
                    });
                    await Task.Delay(300);
                }
            });

        }



        public void Dispose()
        {
            Canceled = true;
        }
    }
}
