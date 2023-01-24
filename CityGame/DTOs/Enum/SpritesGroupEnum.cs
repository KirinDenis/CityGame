namespace CityGame.DTOs.Enum
{
    /// <summary>
    /// Game object sprite's groups enum
    /// NOTE: all lowercase 
    /// WARNING: don't change the order, add all new items to the end of the enumirated list
    /// </summary>
    public static class SpritesGroupEnum
    {
        public const string nogroup = "nogroup";
        public const string water = "water";
        public const string forest = "forest";
        public const string road = "road";
        public const string rail = "rail";
        public const string wire = "wire";
        public const string resident0 = "resident0";
        public const string industrial1 = "industrial1";
        public const string policedepartment = "policedepartment";

        public static string[] groups = { water , forest, road, rail, wire, resident0, industrial1, policedepartment };

        public static string ByObjectType(ObjectType objectType)
        {
            
            switch (objectType)
            {
                case ObjectType.road: return road;
                case ObjectType.rail: return rail;
                case ObjectType.wire: return wire;
                case ObjectType.resident: return resident0;
                case ObjectType.industrial: return industrial1;
                case ObjectType.policeDepartment: return policedepartment;
                default: return road;
            }
        }

        public static bool CheckGroupName(string groupName)
        {
            foreach(string group in groups)
            {
                if (group.ToLower().Equals(groupName.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
