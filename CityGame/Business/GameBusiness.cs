using CityGame.Data.DTO;
using CityGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

        public event EventHandler<DebugMessageDTO> DebugMessage;

        public EcosystemItemDTO[,] ecosystem;

        public List<GameObjectBusiness> gameObjects = new List<GameObjectBusiness>();

        private CoalPowerPlantBusiness coalPowerPlantBusiness;

        public long gameDay = 0;

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


            gameObjects.Add(new ResidetBusiness(this, NewGameObjectModel(typeof(ResidentModel))));
            coalPowerPlantBusiness = new CoalPowerPlantBusiness(this, NewGameObjectModel(typeof(CoalPowerPlantModel))); //need for build and destroy objects
            gameObjects.Add(coalPowerPlantBusiness);
            gameObjects.Add(new WireBusiness(this, NewGameObjectModel(typeof(WireGameObjectModel))));


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

        private DateTime DaysToDate(double days)
        {
            // Дата 1 января 1900 года
            DateTime baseDate = new DateTime(1900, 1, 1, 0, 0, 0);

            // Добавляем указанное количество часов
            TimeSpan time = TimeSpan.FromDays(days);

            // Возвращаем дату и время в формате DateTime
            return baseDate + time;
        }



        private void Timer_Tick(object? sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                while (true)
                {
                    gameDay++;

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
                        bool xOverlap = currentModel.positionDTO.x + currentModel.Group.Width == selectedModel.positionDTO.x || selectedModel.positionDTO.x + selectedModel.Group.Width == currentModel.positionDTO.x;

                        bool yOverlap = currentModel.positionDTO.y + currentModel.Group.Height == selectedModel.positionDTO.y || selectedModel.positionDTO.y + selectedModel.Group.Height == currentModel.positionDTO.y;


                        if (!(xOverlap && yOverlap))
                        {


                            if ((currentModel.positionDTO.x + currentModel.Group.Width == selectedModel.positionDTO.x || selectedModel.positionDTO.x + selectedModel.Group.Width == currentModel.positionDTO.x) &&
                                (currentModel.positionDTO.y >= selectedModel.positionDTO.y && currentModel.positionDTO.y <= selectedModel.positionDTO.y + selectedModel.Group.Height ||
                                selectedModel.positionDTO.y >= currentModel.positionDTO.y && selectedModel.positionDTO.y <= currentModel.positionDTO.y + currentModel.Group.Height))
                            {
                                list.Add(gameObjectBusinessDTO);
                            }
                            if ((currentModel.positionDTO.y + currentModel.Group.Height == selectedModel.positionDTO.y || selectedModel.positionDTO.y + selectedModel.Group.Height == currentModel.positionDTO.y) &&
                                (currentModel.positionDTO.x >= selectedModel.positionDTO.x && currentModel.positionDTO.x <= selectedModel.positionDTO.x + selectedModel.Group.Width ||
                                selectedModel.positionDTO.x >= currentModel.positionDTO.x && selectedModel.positionDTO.x <= currentModel.positionDTO.x + currentModel.Group.Width))
                            {
                                list.Add(gameObjectBusinessDTO);
                            }

                            /*
                            if ((Math.Abs(currentModel.positionDTO.x - selectedModel.positionDTO.x + selectedModel.Group.Width) == currentModel.Group.Width 
                                || 
                                Math.Abs(currentModel.positionDTO.x + currentModel.Group.Width - selectedModel.positionDTO.x) == currentModel.Group.Width) 
                                ||
                                (Math.Abs(currentModel.positionDTO.y - selectedModel.positionDTO.y + selectedModel.Group.Height) == currentModel.Group.Height
                                ||
                                Math.Abs(currentModel.positionDTO.y + currentModel.Group.Height - selectedModel.positionDTO.y) == currentModel.Group.Height))                             {
                                {
                                    list.Add(gameObjectBusinessDTO);
                                }
                            }
                            */
                                

                        }
                    }
                }
            }
            return list;
        }


        public GameObjectBusiness? GetGameObjectBusiness(PositionDTO position)
        {
            if (position == null)
            {
                return null;
            }

            foreach (GameObjectBusiness gameObjectBusiness in gameObjects.ToArray())
            {


                GameObjectBusinessDTO? gameObjectBusinessDTO = gameObjectBusiness.gameObjectBusinessDTOs.Where(o =>
                     o.gameObjectModelDTO.positionDTO != null
                     &&
                     o.gameObjectModelDTO.Group != null
                     &&
                     (o.gameObjectModelDTO.positionDTO.x <= position?.x && o.gameObjectModelDTO.positionDTO.y <= position.y
                     &&
                     o.gameObjectModelDTO.positionDTO.x + o.gameObjectModelDTO.Group.Width - 1 >= position.x
                     &&
                     o.gameObjectModelDTO.positionDTO.y + o.gameObjectModelDTO.Group.Height - 1>= position.y
                     )).FirstOrDefault();

                if (gameObjectBusinessDTO != null)
                {
                    return gameObjectBusiness;
                }
            }
            return null;
        }

        public GameObjectBusinessDTO? GetGameObjectBusinessDTO(PositionDTO position)
        {
            if (position == null)
            {
                return null;
            }

            foreach (GameObjectBusiness gameObjectBusiness in gameObjects.ToArray())
            {


                GameObjectBusinessDTO? gameObjectBusinessDTO = gameObjectBusiness.gameObjectBusinessDTOs.Where(o =>
                     o.gameObjectModelDTO.positionDTO != null
                     &&
                     o.gameObjectModelDTO.Group != null
                     &&
                     (o.gameObjectModelDTO.positionDTO.x <= position?.x && o.gameObjectModelDTO.positionDTO.y <= position.y
                     &&
                     o.gameObjectModelDTO.positionDTO.x + o.gameObjectModelDTO.Group.Width - 1>= position.x
                     &&
                     o.gameObjectModelDTO.positionDTO.y + o.gameObjectModelDTO.Group.Height - 1 >= position.y
                     )).FirstOrDefault();

                if (gameObjectBusinessDTO != null)
                {
                    return gameObjectBusinessDTO;
                }
            }
            return null;
        }

        
        public bool DestroyObjectAtPosition(PositionDTO position)
        {

            if (position != null)
            {
                GameObjectBusiness? gameObjectBusiness = GetGameObjectBusiness(position);
                GameObjectBusinessDTO? gameObjectBusinessDTO = GetGameObjectBusinessDTO(position);

                if ((gameObjectBusiness != null) && (gameObjectBusinessDTO != null))
                {
                    if ((gameObjectBusiness.GetType() != typeof(CoalPowerPlantBusiness)) && (gameObjectBusinessDTO.powerPlantId != 0))
                    {
                        GameObjectBusinessDTO? connectedCoalPowerPlant =
                            coalPowerPlantBusiness.gameObjectBusinessDTOs.Where(c => c.powerPlantId == gameObjectBusinessDTO.powerPlantId).FirstOrDefault();
                        if (connectedCoalPowerPlant != null)
                        {
                            coalPowerPlantBusiness.PowerTargetDestroy(connectedCoalPowerPlant);
                        }
                    }
                    gameObjectBusiness.Destroy(gameObjectBusinessDTO);
                    return true;
                }
            }

            return false;
        }

        public void OnDebugMessage(DebugMessageDTO debugMessageDTO)
        {
            DebugMessage?.Invoke(this, debugMessageDTO);
        }
    }

}
