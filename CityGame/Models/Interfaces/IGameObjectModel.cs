using CityGame.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGame.Models.Interfaces
{

    public class CheckPositionDTO
    {
        public bool IsPossible { get; set; }
        public bool[,] Colisions { get; set; }
    }

    internal interface IGameObjectModel
    {
        public GroupDTO? Group { get; set; }

    public CheckPositionDTO CheckPosition(ushort x, ushort y);

        public bool Put(ushort x, ushort y);
    }
}
