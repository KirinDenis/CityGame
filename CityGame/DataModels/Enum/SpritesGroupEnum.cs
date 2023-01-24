﻿namespace CityGame.DataModels.Enum
{
    public static class SpritesGroupEnum
    {
        public const string water = "water";
        public const string forest = "forest";
        public const string road = "road";
        public const string rail = "rail";
        public const string wire = "wire";
        public const string resident0 = "Resident0";
        public const string industrial1 = "Industrial1";
        public const string policeDepartment = "PoliceDepartment";

        public static string[] groups = { water , forest, road, rail, wire, resident0, industrial1, policeDepartment };

        public static string ByObjectType(ObjectType objectType)
        {
            switch (objectType)
            {
                case ObjectType.road: return road;
                case ObjectType.rail: return rail;
                case ObjectType.wire: return wire;
                case ObjectType.resident: return resident0;
                case ObjectType.industrial: return industrial1;
                case ObjectType.policeDepartment: return policeDepartment;
                default: return road;
            }
        }
    }
}
