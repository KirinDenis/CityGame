using CityGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGame.Data.DTO
{
    public class GameObjectBusinessDTO
    {
        public EcosystemItemDTO EcosystemItem { get; set; } = new EcosystemItemDTO();

        public GameObjectModelDTO gameObjectModelDTO { get; set; }
    }
}
