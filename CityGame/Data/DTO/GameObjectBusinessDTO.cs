namespace CityGame.Data.DTO
{
    public class GameObjectBusinessDTO
    {
        public EcosystemItemDTO EcosystemItem { get; set; } = new EcosystemItemDTO();
        
        public bool electrified { get; set; } = false;

        public int powerSource { get; set; } = 0;

        public int powerTarget { get; set; } = 0;

        public int powerPlantId { get; set; } = 0;

        public int powerPlantSession { get; set; } = 0;

        public GameObjectModelDTO gameObjectModelDTO { get; set; }
    }
}
