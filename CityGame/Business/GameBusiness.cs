using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using CityGame.Models;
using CityGame.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

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

        List<GameObjectBusiness> gameObjects = new List<GameObjectBusiness>();

        public GameBusiness(string cityName, int size = 100) : base(cityName, size)
        {
            ecosystem = new EcosystemItemDTO[size, size];            
            gameObjects.Add(new ResidetBusiness(NewGameObjectModel(typeof(ResidentModel))));

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            //foreach (GameObjectBusiness gameObjectBusiness in gameObjects)
            //{
              //  gameObjectBusiness.LifeCycle();
            //}
        }

        public bool BuildObject(PositionDTO position, GroupDTO? group)
        {
            if ((group == null) || string.IsNullOrEmpty(group.Name))    
            {
                return false;
            }

            foreach (GameObjectBusiness gameObjectBusiness in gameObjects)
            {
                //if ((gameObjectModel == null) || (gameObjectModel.startingGroup == null) || (string.IsNullOrEmpty(gameObjectModel.startingGroup.Name)))
                //{
                  //  continue;
                //}

                if (gameObjectBusiness.gameObjectModel.startingGroup.Name.Equals(group?.Name))
                {
                    if (base.TestPosition(gameObjectBusiness.gameObjectModel.startingGroup, position).CanBuild)
                    {
                        if (gameObjectBusiness.Build(position))
                        {
                            _budget -= gameObjectBusiness.cost;
                            return true;
                        }
                    }
                    return false;
                }
            }

            return false;
        }
    }
}
