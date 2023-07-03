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
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}
