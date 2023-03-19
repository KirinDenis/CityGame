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

        public List<GameObjectDTO> gameObjects = new List<GameObjectDTO>();

        public GroupDTO? startingGroup { get; set; }

        public GameObjectModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel)
        {
            this.spriteBusiness = spriteBusiness;
            this.terrainModel = terrainModel;
            Live();
        }

        public virtual bool Build(PositionDTO positionDTO)
        {
            if (startingGroup == null)
            {
                return false;
            }

            GameObjectDTO gameObject = new GameObjectDTO()
            {
                positionDTO = positionDTO,
                Group = this.startingGroup
            };
            terrainModel.BuildObject(positionDTO.x, positionDTO.y, startingGroup, 0);

            gameObjects.Add(gameObject);

            return true;
        }

        public void Destroy(GameObjectDTO gameObjectDTO)
        {

            gameObjects.Remove(gameObjectDTO);
            byte destroyStep = 0;
            GroupDTO? destroyGroup = spriteBusiness.GetGroupByName(SpritesGroupEnum.explosion);
            Task destroyTask = Task.Run(async delegate
            {
                while (destroyStep < 7)
                {
                    destroyStep++;
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        if (!Canceled)
                        {
                            //TEMP
                            for (ushort sx = 0; sx < gameObjectDTO.Group.Width; sx++)
                            {
                                for (ushort sy = 0; sy < gameObjectDTO.Group.Height; sy++)
                                {

                                    if (destroyGroup?.Sprites[destroyStep].Sprites[0, 0] != null)
                                    {
                                        //terrainModel.terrain[gameObjectDTO.positionDTO.x + sx, gameObjectDTO.positionDTO.y + sy] = destroyGroup.Sprites[destroyStep].Sprites[0, 0];
                                        //terrainModel.PutSprite((ushort)(gameObjectDTO.positionDTO.x + sx), (ushort)(gameObjectDTO.positionDTO.y + sy), destroyGroup.Sprites[destroyStep].Sprites[0, 0].x, destroyGroup.Sprites[destroyStep].Sprites[0, 0].y);
                                        terrainModel.PutSprite((ushort)(gameObjectDTO.positionDTO.x + sx), (ushort)(gameObjectDTO.positionDTO.y + sy), destroyGroup, destroyStep, 0, 0);
                                    }
                                }
                            }

                            if ((gameObjectDTO != null) && (destroyGroup != null) && (gameObjectDTO.positionDTO != null))
                            {
                                terrainModel.BuildObject(gameObjectDTO.positionDTO.x, gameObjectDTO.positionDTO.y, destroyGroup, destroyStep);
                            }
                        }
                    });
                    await Task.Delay(300);
                }
            });

        }

        private void Live()
        {

            liveTask = Task.Run(async delegate
            {
                while (!Canceled)
                {
                    foreach (GameObjectDTO gameObject in gameObjects.ToArray())
                    {
                        if (gameObject.Group?.Sprites.Count > 1)
                        {
                            if (gameObject.animationFrame >= gameObject.Group.Sprites.Count)
                            {
                                gameObject.animationFrame = 1;
                            }
                            _ = Application.Current.Dispatcher.Invoke(() =>
                            {
                                if (!Canceled)
                                {
                                    terrainModel.BuildObject(gameObject.positionDTO.x, gameObject.positionDTO.y, gameObject.Group, gameObject.animationFrame);
                                }

                                return Task.CompletedTask;
                            });
                            gameObject.animationFrame++;

                        }
                        LiveCycle(gameObject);

                    }
                    await Task.Delay(300);
                }
            });
        }

        protected virtual void LiveCycle(GameObjectDTO gameObject)
        {

        }

        public void Dispose()
        {
            Canceled = true;
        }
    }
}
