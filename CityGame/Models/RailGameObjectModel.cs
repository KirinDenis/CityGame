using CityGame.DataModels.Enum;

namespace CityGame.Models
{
    internal class RailGameObjectModel : NetworkGameObjectModel
    {
        public RailGameObjectModel(GroupsModel groupsModel, TerrainModel terrainModel, ObjectType networkType) : base(groupsModel, terrainModel, networkType)
        {
        }
    }
}
