using CityGame.Data.DTO;
using CityGame.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media.Media3D;
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
        public long budget
        {
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
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    ecosystem[x, y] = new EcosystemItemDTO();
                }
            }

            gameObjects.Add(new WireBusiness(this, NewGameObjectModel(typeof(WireGameObjectModel))));
            gameObjects.Add(new ResidetBusiness(this, NewGameObjectModel(typeof(ResidentModel))));
            gameObjects.Add(new CoalPowerPlantBusiness(this, NewGameObjectModel(typeof(CoalPowerPlantModel))));

            //DispatcherTimer timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromMilliseconds(300);
            //timer.Tick += Timer_Tick;
            //timer.Start();
            Timer_Tick(null, null);
        }

        public void FillCircle(EcosystemItemDTO[,] _ecosystem, int centerX, int centerY, int radius, int weight)
        {
            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                for (int x = centerX - radius; x <= centerX + radius; x++)
                {
                    if (Math.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY)) <= radius)
                    {
                        double dist = Math.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                        int currentWeight = (int)Math.Max(0, weight - dist);

                        if ((x >= 0) && (x < size) && (y >= 0) && (y < size) && (_ecosystem[x, y].Population + (byte)currentWeight < 255))
                        {
                            _ecosystem[x, y].Population += (byte)currentWeight;
                        }
                    }
                }
            }
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                while (true)
                {


                    EcosystemItemDTO[,] _ecosystem = new EcosystemItemDTO[ecosystem.GetLength(0), ecosystem.GetLength(1)];

                    for (int x = 0; x < size; x++)
                    {
                        for (int y = 0; y < size; y++)
                        {
                            _ecosystem[x, y] = new EcosystemItemDTO();
                            ecosystem[x, y].Population = 0;
                        }
                    }



                    foreach (GameObjectBusiness gameObjectBusiness in gameObjects.ToArray())
                    {
                        foreach (GameObjectBusinessDTO gameObjectBusinessDTO in gameObjectBusiness.gameObjectBusinessDTOs.ToArray())
                        {

                            ecosystem[gameObjectBusinessDTO.gameObjectModelDTO.centerPosition.x, gameObjectBusinessDTO.gameObjectModelDTO.centerPosition.y].Population = gameObjectBusinessDTO.EcosystemItem.Population;
                        }
                    }

                    for (int centerX = 2; centerX < size - 2; centerX++)
                    {
                        for (int centerY = 2; centerY < size - 2; centerY++)
                        {
                            if (ecosystem[centerX, centerY].Population > 0)
                            {
                                //  _ecosystem[x, y] = ecosystem[x, y];
                                //FillCircle(_ecosystem, x, y, 3 * 3, ecosystem[x, y].Population / 10);

                                int radius = 3 * 3;

                                for (int y = centerY - radius; y <= centerY + radius; y++)
                                {
                                    for (int x = centerX - radius; x <= centerX + radius; x++)
                                    {
                                        if (Math.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY)) <= radius)
                                        {
                                            double dist = Math.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                                            int currentWeight = (int)Math.Max(0, ecosystem[centerX, centerY].Population / 10 - dist);

                                            if ((x >= 0) && (x < size) && (y >= 0) && (y < size) && (_ecosystem[x, y].Population + (byte)currentWeight < 255))
                                            {
                                                _ecosystem[x, y].Population += (byte)currentWeight;
                                            }
                                        }
                                    }
                                }


                                //if (_ecosystem[x, y].Population > 255)
                                // {
                                //     _ecosystem[x, y].Population = 255;
                                //  }
                            }
                        }
                    }
                    ecosystem = _ecosystem;
                    Thread.Sleep(1000);
                }
            });
            thread.Start();
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

        public List<GameObjectBusinessDTO> GetNeighbours(GameObjectBusinessDTO selectedGameObjectBusinessDTO)
        {
            List<GameObjectBusinessDTO> list = new List<GameObjectBusinessDTO>();

            GameObjectModelDTO selectedModel = selectedGameObjectBusinessDTO.gameObjectModelDTO;

            foreach (GameObjectBusiness gameObjectBusiness in gameObjects.ToArray())
            {
                foreach (GameObjectBusinessDTO gameObjectBusinessDTO in gameObjectBusiness.gameObjectBusinessDTOs.ToArray())
                {
                    if (gameObjectBusinessDTO != selectedGameObjectBusinessDTO)
                    {
                        GameObjectModelDTO currentModel = gameObjectBusinessDTO.gameObjectModelDTO;

                        if (((currentModel.positionDTO.x >= selectedModel.positionDTO.x - selectedModel.Group.Width)
                          &&
                            (currentModel.positionDTO.x <= selectedModel.positionDTO.x + selectedModel.Group.Width))
                            &&
                            ((currentModel.positionDTO.y >= selectedModel.positionDTO.y - selectedModel.Group.Height)
                          &&
                            (currentModel.positionDTO.y <= selectedModel.positionDTO.y + selectedModel.Group.Height)))
                        {
                            list.Add(gameObjectBusinessDTO);
                        }
                    }
                }
            }
            return list;
        }       
    }
}
