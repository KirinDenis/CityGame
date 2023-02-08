namespace CityGame.Data.DTO
{
    public class GameObjectDTO
    {
        public GroupDTO? Group { get; set; }
        public PositionDTO positionDTO { get; set; }
        public int animationFrame { get; set; } = 1;

    }
}
