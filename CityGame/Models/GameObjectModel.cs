using CityGame.Data.DTO;
using CityGame.Graphics;
using CityGame.Models.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace CityGame.Models
{
    public class GameObjectModel : BaseModel, IGameObjectModel, IDisposable
    {

        protected SpriteBusiness spriteBusiness = new SpriteBusiness();
        protected TerrainModel terrainModel;

        protected PositionDTO positionDTO;

        private int animationFrame = 1;

        private Task t;
        private bool myCancelation = false;


        public GroupDTO? Group { get; set; }

        public GameObjectModel(TerrainModel terrainModel)
        {
            this.terrainModel = terrainModel;
        }

        public GameObjectModel(TerrainModel terrainModel, PositionDTO positionDTO, GroupDTO group)
        {
            this.terrainModel = terrainModel;

            this.positionDTO = positionDTO;
            this.Group = group;
            terrainModel.BuildObject(positionDTO.x, positionDTO.y, Group, 0);
            Live();
        }

        public virtual bool Build(PositionDTO positionDTO)
        {
            return false;
        }

        private void Live()
        {

            t = Task.Run(async delegate
            {
                if (Group?.Sprites.Count > 1)
                {
                    while (!myCancelation)
                    {



                        if (animationFrame >= Group.Sprites.Count)
                        {
                            animationFrame = 1;
                        }
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            {
                                if (!myCancelation)
                                {
                                    terrainModel.BuildObject(positionDTO.x, positionDTO.y, Group, animationFrame);
                                }
                            }
                        });
                        animationFrame++;

                        await Task.Delay(300);
                    }
                }
            });



            //TimeSpan ts = TimeSpan.FromMilliseconds(150);
            //if (!t.Wait(ts))
            //    Console.WriteLine("The timeout interval elapsed.");

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
