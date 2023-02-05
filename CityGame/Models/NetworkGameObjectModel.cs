using CityGame.Data.DTO;
using CityGame.DTOs.Enum;

namespace CityGame.Models
{
    internal class NetworkGameObjectModel : GameObjectModel
    {
        public NetworkGameObjectModel(TerrainModel terrainModel) : base(terrainModel)
        {
        }

        public bool[,] BuildNetworkItem(int x, int y)
        {
            bool[,] FS = new bool[3, 3]; //FS means friendly sprites, the sprites of selected network item type
            int ox = 0;
            for (int tx = CLeft(x); tx < CRight(x) + 1; tx++, ox++)
            {
                int oy = 0;
                for (int ty = CLeft(y); ty < CRight(y) + 1; ty++, oy++)
                {

                    FS[ox, oy] = false;
                    foreach (PositionDTO? spritePosition in Group?.Sprites[0].Sprites)
                    {
                        if (terrainModel.terrain[tx, ty] == spritePosition)
                        {
                            FS[ox, oy] = true;
                            break;
                        }
                    }
                }
            }

            terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[4, 0];

            int l = 0;
            int c = 1;
            int r = 2;
            int t = 0;
            int b = 2;

            //Central cross of 4 roads
            if (FS[c, t] & FS[c, b] & FS[l, c] & FS[r, c])
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[1, 1];
            }
            //Left Right Top cross of 3 roads  
            else
            if (FS[c, t] & FS[l, c] & FS[r, c])
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[1, 2];
            }
            //Left Right Bottom cross of 3 roads 
            else
            if (FS[c, b] & FS[l, c] & FS[r, c])
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[1, 0];
            }
            //Left Top Bottom cross of 3 roads 
            else
            if (FS[c, t] & FS[c, b] & FS[l, c])
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[2, 1];
            }
            //Right Top Bottom cross of 3 roads 
            else
            if (FS[c, t] & FS[c, b] & FS[r, c])
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[0, 1];
            }
            //Right Top turn of 2 roads 
            else
            if (FS[c, b] & FS[r, c])
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[0, 0];
            }
            //Left Top turn of 2 roads 
            else
            if (FS[c, b] & FS[l, c])
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[2, 0];
            }
            //Right Bottom turn of 2 roads 
            else
            if (FS[c, t] & FS[r, c])
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[0, 2];
            }
            //Left Bottom turn of 2 roads 
            else
            if (FS[c, t] & FS[l, c])
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[2, 2];
            }
            //Horisontal road
            else
            if (FS[r, c] | FS[l, c])
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[3, 0];
            }
            //Vertical road
            else
            if (FS[c, t] | FS[c, b])
            {
                terrainModel.terrain[x, y] = Group.Sprites[0].Sprites[4, 0];
            }
            //else default single road

            terrainModel.PutImage(x, y, terrainModel.terrain[x, y].x, terrainModel.terrain[x, y].y);

            return FS;
        }

        public override bool Put(ushort x, ushort y)
        {
            bool[,] FS = BuildNetworkItem(x, y);

            //Rebuild near roads            
            int ox = 0;
            for (int tx = CLeft(x); tx < CRight(x) + 1; tx++, ox++)
            {
                int oy = 0;
                for (int ty = CLeft(y); ty < CRight(y) + 1; ty++, oy++)
                {
                    if (FS[ox, oy])
                    {
                        BuildNetworkItem(tx, ty);
                    }
                }
            }
            return true;
        }
    }
}
