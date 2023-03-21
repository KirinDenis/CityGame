using CityGame.Data.DTO;

namespace CityGame.Models.Interfaces
{

    public class CheckPositionDTO
    {
        public bool IsPossible { get; set; }
        public bool[,]? Colisions { get; set; }
    }

    internal interface IGameObjectModel
    {
        public GroupDTO? startingGroup { get; set; }
    }
}
