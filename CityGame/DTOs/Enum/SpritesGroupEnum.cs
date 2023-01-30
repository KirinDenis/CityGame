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
        public const string resident1 = "resident1";
        public const string resident2 = "resident2";
        public const string resident3 = "resident3";
        public const string resident4 = "resident4";
        public const string resident5 = "resident5";
        public const string resident6 = "resident6";
        public const string resident7 = "resident7";
        public const string resident8 = "resident8";
        public const string resident9 = "resident9";

        public const string comercial0 = "comercial0";
        public const string comercial1 = "comercial1";
        public const string comercial2 = "comercial2";
        public const string comercial3 = "comercial3";
        public const string comercial4 = "comercial4";
        public const string comercial5 = "comercial5";
        public const string comercial6 = "comercial6";
        public const string comercial7 = "comercial7";
        public const string comercial8 = "comercial8";
        public const string comercial9 = "comercial9";

        public const string industrial0 = "industrial0";
        public const string industrial1 = "industrial1";
        public const string industrial2 = "industrial2";
        public const string industrial3 = "industrial3";
        public const string industrial4 = "industrial4";
        public const string industrial5 = "industrial5";
        public const string industrial6 = "industrial6";
        public const string industrial7 = "industrial7";
        public const string industrial8 = "industrial8";
        public const string industrial9 = "industrial9";
        
        public const string policedepartment = "policedepartment";
        public const string firedepartment = "firedepartment";

        public const string coalpowerplant = "coalpowerplant";
        public const string nuclearpowerplant = "nuclearpowerplant";

        public const string seaport = "seaport";
        public const string airport = "airport";

        public const string garden = "garden";

        public const string stadium = "stadium";

        public const string strange0 = "strange0";
        public const string strange1 = "strange1";
        public const string strange2 = "strange2";
        public const string strange3 = "strange3";
        public const string strange4 = "strange4";
        public const string strange5 = "strange5";
        public const string strange6 = "strange6";
        public const string strange7 = "strange7";

        public static string[] groups = { water , forest, road, rail, wire,
            resident0,
            resident1,
            resident2,
            resident3,
            resident4,
            resident5,
            resident6,
            resident7,
            resident8,
            resident9,

            comercial0,
            comercial1,
            comercial2,
            comercial3,
            comercial4,
            comercial5,
            comercial6,
            comercial7,
            comercial8,
            comercial9,

            industrial0,
            industrial1,
            industrial2,
            industrial3,
            industrial4,
            industrial5,
            industrial6,
            industrial7,
            industrial8,
            industrial9,

            policedepartment,
            firedepartment,

            coalpowerplant,
            nuclearpowerplant,

            seaport,
            airport,

            garden,

            stadium,

            strange0,
            strange1,
            strange2,
            strange3,
            strange4,
            strange5,
            strange6,
            strange7 }; 



        public static string ByObjectType(ObjectType objectType)
        {
            
            switch (objectType)
            {
                case ObjectType.road: return road;
                case ObjectType.rail: return rail;
                case ObjectType.wire: return wire;
                case ObjectType.resident: return resident0;
                case ObjectType.comercial: return comercial0;
                case ObjectType.industrial: return industrial0;
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
