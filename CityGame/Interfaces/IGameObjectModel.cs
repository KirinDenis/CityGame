using CityGame.Data.DTO;
using CityGame.Interfaces;

namespace CityGame.Models.Interfaces
{

    public class CheckPositionDTO
    {
        public bool IsPossible { get; set; }
        public bool[,]? Colisions { get; set; }
    }

    internal interface IGameObjectModel: IGameObject
    {
        public GroupDTO? startingGroup { get; set; }
    }
}
