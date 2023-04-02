using CityGame.Data.DTO;
using CityGame.DTOs.Const;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CityGame.Models
{
    public class CityGameEngine
    {
        private string _cityName = "Where the streets have no name";
        public string cityName
        { get { return _cityName; } set { _cityName = value; } }

        private int _size = GameConsts.DefaultTerrainSize;
        public int size { get { return _size; } set { _size = value; } }

        private int lifeCycleTime = 100;
        private SpriteBusiness spriteBusiness = new SpriteBusiness();
        private TerrainModel terrainModel;

        //private List<GameObjectModel> gameObjectModels = new List<GameObjectModel>();

        public event EventHandler? TerrainRenderCompleted = null;
        public event EventHandler? MapRenderCompleted = null;

        public CityGameEngine(string cityName, int size = GameConsts.DefaultTerrainSize)
        {
            _cityName = cityName;
            _size = size;
            terrainModel = new TerrainModel(size);
            /*
            gameObjectModels.Add(new RoadGameObjectModel(spriteBusiness, terrainModel));
            gameObjectModels.Add(new RailGameObjectModel(spriteBusiness, terrainModel));
            gameObjectModels.Add(new WireGameObjectModel(spriteBusiness, terrainModel));
            gameObjectModels.Add(new GardenModel(spriteBusiness, terrainModel));
            gameObjectModels.Add(new ResidentModel(spriteBusiness, terrainModel));
            gameObjectModels.Add(new IndustrialModel(spriteBusiness, terrainModel));
            gameObjectModels.Add(new ComercialModel(spriteBusiness, terrainModel));
            gameObjectModels.Add(new PoliceDepartmentModel(spriteBusiness, terrainModel));
            gameObjectModels.Add(new FireDepartmentModel(spriteBusiness, terrainModel));
            gameObjectModels.Add(new StadiumModel(spriteBusiness, terrainModel));
            gameObjectModels.Add(new CoalPowerPlantModel(spriteBusiness, terrainModel));
            gameObjectModels.Add(new NuclearPowerPlantModel(spriteBusiness, terrainModel));
            gameObjectModels.Add(new SeaPortModel(spriteBusiness, terrainModel));
            gameObjectModels.Add(new AirPortModel(spriteBusiness, terrainModel));
            */
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(lifeCycleTime);
            timer.Tick += Timer_Tick; 
            timer.Start();
        }

        public GameObjectModel NewGameObjectModel(Type gameObjectModelType)
        {
            return Activator.CreateInstance(gameObjectModelType, spriteBusiness, terrainModel) as GameObjectModel;
        }

        public int GetTerrainSize()
        {
            return size;
        }

        public WriteableBitmap? GetTerrainBitmap()
        {
            if ((terrainModel != null) && (terrainModel.terrainBitmap != null))
            {
                return terrainModel.terrainBitmap;
            }
            else
            {
                return null;
            }

        }

        public WriteableBitmap? GetMapBitmap()
        {
            if ((terrainModel != null) && (terrainModel.mapBitmap != null))
            {
                return terrainModel.mapBitmap;
            }
            else
            {
                return null;
            }
        }

        public void GenerateTerrain()
        {
            terrainModel.GenerateNewTerrain();
        }

        public virtual bool BuildObject(PositionDTO position, GameObjectModel gameObjectModel)
        {
            if (TestPosition(gameObjectModel.startingGroup, position).CanBuild)
            {
                        gameObjectModel.Build(position);
                        return true;
            }
            return false;
        }

        public TestPositionDTO TestPosition(GroupDTO? group, PositionDTO position)
        {
            return this.terrainModel.TestPosition(group, position);
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {
            OnTerrainRenderCompleted(EventArgs.Empty);
            OnMapRenderCompleted(EventArgs.Empty);
        }

        protected virtual void OnTerrainRenderCompleted(EventArgs e)
        {
            TerrainRenderCompleted?.Invoke(this, e);
        }

        protected virtual void OnMapRenderCompleted(EventArgs e)
        {
            MapRenderCompleted?.Invoke(this, e);
        }

    }
}
