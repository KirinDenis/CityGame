using CityGame.DTOs.Enum;

namespace CityGame.Models
{
    internal class RoadGameObjectModel : NetworkGameObjectModel
    {
        public RoadGameObjectModel(TerrainModel terrainModel, ObjectType networkType) : base(terrainModel, networkType)
        {
        }
    }
}
