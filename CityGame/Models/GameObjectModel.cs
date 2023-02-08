using CityGame.Data.DTO;
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

        protected PositionDTO positionDTO;

        private int animationFrame = 1;

        private Task liveTask;
        private bool Canceled = false;

        private List<GameObjectModel> familyModels = new List<GameObjectModel>();
        private List<PositionDTO> positions;


        public GroupDTO? Group { get; set; }

        public GameObjectModel(SpriteBusiness spriteBusiness, TerrainModel terrainModel)
        {
            this.spriteBusiness = spriteBusiness;
            this.terrainModel = terrainModel;
            positions = new List<PositionDTO>();
            Live();
        }

        public virtual bool Build(PositionDTO positionDTO)
        {
            this.positionDTO = positionDTO;
            terrainModel.BuildObject(positionDTO.x, positionDTO.y, Group, 0);
            familyModels.Add(this);
            positions.Add(this.positionDTO);
            return true;
        }

        private void Live()
        {

            liveTask = Task.Run(async delegate
            {
                while (!Canceled)
                {
                    foreach (PositionDTO position in positions.ToArray())
                    {
                        if (Group?.Sprites.Count > 1)
                        {
                            if (animationFrame >= Group.Sprites.Count)
                            {
                                animationFrame = 1;
                            }
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                if (!Canceled)
                                {
                                    terrainModel.BuildObject(position.x, position.y, Group, animationFrame);
                                }
                            });
                            animationFrame++;
                            
                        }

                        LiveCycle();
                    }                
                    await Task.Delay(300);
                }
            });
        }

        protected virtual void LiveCycle()
        {

        }

        public void Dispose()
        {
            Canceled = true;
        }
    }
}
