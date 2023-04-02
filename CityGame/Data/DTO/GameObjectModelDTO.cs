namespace CityGame.Data.DTO
{
    public class GameObjectModelDTO
    {
        public GroupDTO? Group { get; set; }
        public PositionDTO? positionDTO { get; set; }

        public PositionDTO? centerPosition { get; set; } = new PositionDTO()
        {
            x = 0,
            y = 0
        };
        public byte animationFrame { get; set; } = 1;

        public byte level = 0;

        public uint timeLive = 0;

        
        public bool electrified { get; set; } = false;

    }
}
