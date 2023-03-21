using CityGame.DTOs.Enum;

namespace CityGame.Data.DTO
{
    public class TestPositionDTO
    {
        public bool CanBuild { get; set; } = false;
        public ObjectType[,]? PositionArea { get; set; } = null;
    }
}
