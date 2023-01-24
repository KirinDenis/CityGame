using CityGame.DTOs.Enum;

namespace CityGame.Models
{
    internal class RoadGameObjectModel : NetworkGameObjectModel
    {
        public RoadGameObjectModel(GroupsModel groupsModel, TerrainModel terrainModel, ObjectType networkType) : base(groupsModel, terrainModel, networkType)
        {
        }
    }
}
