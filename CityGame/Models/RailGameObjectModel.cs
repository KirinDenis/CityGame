using CityGame.DTOs.Enum;

namespace CityGame.Models
{
    internal class RailGameObjectModel : NetworkGameObjectModel
    {
        public RailGameObjectModel(TerrainModel terrainModel, ObjectType networkType) : base(terrainModel, networkType)
        {
        }
    }
}
