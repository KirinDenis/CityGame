using CityGame.DTOs.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGame.Data.DTO
{
    public class TestPositionDTO
    {
        public bool CanBuild { get; set; } = false;
        public ObjectType[,] PositionArea { get; set; } = null;
    }
}
