namespace CityGame.Data.DTO
{
    public class GameObjectBusinessDTO
    {
        public EcosystemItemDTO EcosystemItem { get; set; } = new EcosystemItemDTO();

        public int cost { get; set; }
        public bool electrified { get; set; } = false;

        public int powerSource { get; set; } = 0;

        public int powerTarget { get; set; } = 0;

        public GameObjectModelDTO gameObjectModelDTO { get; set; }
    }
}
