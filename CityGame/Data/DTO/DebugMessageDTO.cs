using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGame.Data.DTO
{
    public class DebugMessageDTO
    { 
        public PositionDTO? Position { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
