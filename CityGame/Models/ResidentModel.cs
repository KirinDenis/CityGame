using CityGame.Data.DTO;
using CityGame.DTOs;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGame.Models
{
    internal class ResidentModel
    {
        private SpriteBusiness spriteBusiness = new SpriteBusiness();
        private TerrainModel terrainModel;
        

        public ResidentModel(TerrainModel terrainModel)
        {            
            this.terrainModel = terrainModel;
        }

        public void Put(ushort x, ushort y, GroupDTO group)
        {
            
            
            if (group == null) 
            { 
                return; 
            }


            for (int sx = 0; sx < group?.Width; sx++)
            {
                for (int sy = 0; sy < group?.Height; sy++)
                { 
                
                if (group?.Sprites[0].Sprites[sx,sy] != null)
                {
                    terrainModel.terrain[x + sx, y + sy] = group?.Sprites[0].Sprites[sx, sy];
                    terrainModel.PutImage(x + sx, y + sy, group?.Sprites[0].Sprites[sx, sy].x, group?.Sprites[0].Sprites[sx, sy].y) ;
                }
            }
            }
            

        }

    }
}
