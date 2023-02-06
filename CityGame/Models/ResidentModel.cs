using CityGame.Data.DTO;
using CityGame.Graphics;
using System.Threading.Tasks;
using System.Windows;

namespace CityGame.Models
{
    internal class ResidentModel : GameObjectModel
    {
        private SpriteBusiness spriteBusiness = new SpriteBusiness();
        private TerrainModel terrainModel;

        private int animationFrame = 1;

        public ResidentModel(TerrainModel terrainModel) : base(terrainModel)
        {
            this.terrainModel = terrainModel;

        }

        public void Animate()
        {
            Task t = Task.Run(async delegate
            {
                if (Group?.Sprites.Count > 1)
                {
                    while (true)
                    {

                        if (animationFrame >= Group.Sprites.Count)
                        {
                            animationFrame = 1;
                        }
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            {
                                Put(positionDTO.x, positionDTO.y, Group, animationFrame);
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



        public void Put(ushort x, ushort y, GroupDTO group, int frame = 0)
        {


            if (group == null)
            {
                return;
            }

            if (group?.Sprites.Count - 1 < frame)
            {
                return;
            }

            if ((x > terrainModel.terrainSize - group?.Width) || (y > terrainModel.terrainSize - group?.Height))
            {
                return;
            }

            positionDTO = new PositionDTO()
            {
                x = x,
                y = y
            };


            for (int sx = 0; sx < group?.Width; sx++)
            {
                for (int sy = 0; sy < group?.Height; sy++)
                {

                    if (group?.Sprites[frame].Sprites[sx, sy] != null)
                    {
                        terrainModel.terrain[x + sx, y + sy] = group?.Sprites[frame].Sprites[sx, sy];
                        terrainModel.PutImage(x + sx, y + sy, group?.Sprites[frame].Sprites[sx, sy].x, group?.Sprites[frame].Sprites[sx, sy].y);
                    }
                }
            }


        }

    }
}
