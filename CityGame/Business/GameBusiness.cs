using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using CityGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace CityGame.Business
{
    public class GameBusiness : CityGameEngine
    {
        public event EventHandler? BudgetChanged = null;

        protected long __budget = 20000;
        private long _budget
        {
            get { return __budget; }
            set
            {
                __budget = value;
                BudgetChanged?.Invoke(this, new EventArgs());
            }
        }
        public long budget {
            get 
            { 
                return _budget;
            }
        }

        public EcosystemItemDTO[,] ecosystem;

        public GameBusiness(string cityName, int size = 100) : base(cityName, size)
        {
            ecosystem = new EcosystemItemDTO[size, size];
        }

        public override bool BuildObject(PositionDTO position, GroupDTO? group)
        {
            if ((group == null) || string.IsNullOrEmpty(group.Name))    
            {
                return false;
            }

            int objectPrice = -1;

            switch (SpritesGroupEnum.GetObjectTypeByGroupName(group.Name))
            {
                case ObjectType.network: objectPrice = 10; break;
                case ObjectType.garden: objectPrice = 5; break;
                case ObjectType.building:
                    switch (group.Name)
                    {
                        case SpritesGroupEnum.firedepartment: objectPrice = 500; break;
                        case SpritesGroupEnum.policedepartment: objectPrice = 500; break;
                        case SpritesGroupEnum.coalpowerplant: objectPrice = 1500; break;
                        default: objectPrice = 100; break;
                    }
                    break;                
            }

            if ((objectPrice <= 0) || (objectPrice > budget))
            {
                return false;
            }

            if (base.BuildObject(position, group))
            {
                _budget -= objectPrice;
                return true;
            }

            return false;
        }
    }
}
