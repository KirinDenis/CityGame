using CityGame.DataModels;
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
        public int? GroupId { get; set; }

        public List<SpriteModel>? SpritesGroup { get; set; }

    public CheckPositionDTO CheckPosition(ushort x, ushort y);

        public bool Put(ushort x, ushort y);
    }
}
