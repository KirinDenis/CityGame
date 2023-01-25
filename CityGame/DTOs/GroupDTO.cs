using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CityGame.DTOs
{
    public class GroupSpritesDTO
    {
        public PositionDTO[,] Sprites = new PositionDTO[7, 7];
    }

    public class GroupDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ushort Width { get; set; } = 3;
        public ushort Height { get; set; } = 3;

        public ushort CenterX { get; set; } = 1;
        public ushort CenterY { get; set; } = 1;

        public List<GroupSpritesDTO> Sprites { get; set; } = new List<GroupSpritesDTO>();
        
    }
}
