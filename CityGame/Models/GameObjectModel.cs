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
    class DestroyAnimation
    {
        public byte step = 0;

        public byte time = 0;
    }

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

        private Random random = new Random();


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

            GroupDTO? destroyGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.explosion);
            Task destroyTask = Task.Run(async delegate
            {
             
                DestroyAnimation[,] destroyAnimations = new DestroyAnimation[(int)gameObjectModelDTO?.Group?.Width, (int)gameObjectModelDTO?.Group?.Height];
                for (ushort sx = 0; sx < gameObjectModelDTO?.Group?.Width; sx++)
                {
                    for (ushort sy = 0; sy < gameObjectModelDTO?.Group?.Height; sy++)
                    {
                        destroyAnimations[sx, sy] = new DestroyAnimation();
                        destroyAnimations[sx, sy].time = (byte)random.Next(4);
                    }
                }

                while (true)
                {
                    int doneCount = 0;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (!Canceled)
                        {

                            for (ushort sx = 0; sx < gameObjectModelDTO?.Group?.Width; sx++)
                            {
                                for (ushort sy = 0; sy < gameObjectModelDTO?.Group?.Height; sy++)
                                {
                                    if (destroyAnimations[sx, sy].time > 0)
                                    {
                                        destroyAnimations[sx, sy].time--;
                                        continue;
                                    }
                                    destroyAnimations[sx, sy].time = (byte)random.Next(4);

                                    if (destroyAnimations[sx, sy].step < 7)
                                    {
                                        destroyAnimations[sx, sy].step++;
                                        if (destroyGroup?.Frames?[destroyAnimations[sx, sy].step]?.Sprites?[0, 0] != null)
                                        {
                                            if ((gameObjectModelDTO != null) && (destroyGroup != null) && (gameObjectModelDTO.positionDTO != null))
                                            {
                                                terrainModel.PutSprite((ushort)(gameObjectModelDTO.positionDTO.x + sx), (ushort)(gameObjectModelDTO.positionDTO.y + sy), destroyGroup, destroyAnimations[sx, sy].step, 0, 0);

                                            }
                                        }
                                    }
                                    else
                                    {
                                        doneCount++;
                                    }
                                }
                            }

                            //if ((gameObjectModelDTO != null) && (destroyGroup != null) && (gameObjectModelDTO.positionDTO != null))
                           //{
                              //  terrainModel.BuildObject(gameObjectModelDTO.positionDTO.x, gameObjectModelDTO.positionDTO.y, destroyGroup, 0);
                           // }
                        }
                    });

                    if (doneCount >= gameObjectModelDTO?.Group?.Width + gameObjectModelDTO?.Group?.Height)
                    {
                        for (ushort sx = 0; sx < gameObjectModelDTO?.Group?.Width; sx++)
                        {
                            for (ushort sy = 0; sy < gameObjectModelDTO?.Group?.Height; sy++)
                            {
                                if ((gameObjectModelDTO != null) && (destroyGroup != null) && (gameObjectModelDTO.positionDTO != null))
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        terrainModel.PutSprite((ushort)(gameObjectModelDTO.positionDTO.x + sx), (ushort)(gameObjectModelDTO.positionDTO.y + sy), destroyGroup, 7, random.Next(3), random.Next(3));
                                    });

                                }
                            }
                        }
                        break;
                    }
                    
                    await Task.Delay(40);
                }

            });

        }



        public void Dispose()
        {
            Canceled = true;
        }
    }
}
