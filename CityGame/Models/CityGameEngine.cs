using CityGame.Data.DTO;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CityGame.Models
{
    public class CityGameEngine
    {
        private SpriteBusiness spriteBusiness = new SpriteBusiness();
        private TerrainModel terrainModel;
        

        private RoadGameObjectModel roadGameObjectModel;
        private RailGameObjectModel railGameObjectModel;
        private WireGameObjectModel wireGameObjectModel;
        private GardenModel gardenModel;
        private ResidentModel residentModel;
        private IndustrialModel industrialModel;
        private ComercialModel comercialModel;

        private PoliceDepartmentModel policeDepartmentModel;
        private FireDepartmentModel fireDepartmentModel;
        private StadiumModel stadiumModel;
        private CoalPowerPlantModel coalPowerPlantModel;
        private NuclearPowerPlantModel nuclearPowerPlantModel;
        private SeaPortModel seaPortModel;
        private AirPortModel airPortModel;




        public event EventHandler RenderCompleted;


        public CityGameEngine(string cityName, int size = 100)
        {
            terrainModel = new TerrainModel(size);
            roadGameObjectModel = new RoadGameObjectModel(spriteBusiness, terrainModel);
            railGameObjectModel = new RailGameObjectModel(spriteBusiness, terrainModel);
            wireGameObjectModel = new WireGameObjectModel(spriteBusiness, terrainModel);
            gardenModel = new GardenModel(spriteBusiness, terrainModel);
            residentModel = new ResidentModel(spriteBusiness, terrainModel);
            industrialModel = new IndustrialModel(spriteBusiness, terrainModel);
            comercialModel = new ComercialModel(spriteBusiness, terrainModel);
            policeDepartmentModel = new PoliceDepartmentModel(spriteBusiness, terrainModel);
            fireDepartmentModel = new FireDepartmentModel(spriteBusiness, terrainModel);
            stadiumModel = new  StadiumModel(spriteBusiness, terrainModel);
            coalPowerPlantModel = new CoalPowerPlantModel(spriteBusiness, terrainModel);
            nuclearPowerPlantModel = new  NuclearPowerPlantModel(spriteBusiness, terrainModel);
            seaPortModel = new  SeaPortModel(spriteBusiness, terrainModel);
            airPortModel = new AirPortModel(spriteBusiness, terrainModel);


            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick; ;
            timer.Start();
        }

        public int GetTerrainSize()
        {
            return terrainModel.terrainSize;
        }

        public WriteableBitmap GetTerrainBitmap()
        {
            return terrainModel.bitmapSource;
        }

        public void GenerateTerrain()
        {
            terrainModel.GenerateNewTerrain();
        }

        public bool BuildObject(PositionDTO position, GroupDTO? group)
        {
            if (TestPosition(group, position.x, position.y).CanBuild)
            {
                switch (group?.Name)
                {
                    case SpritesGroupEnum.road: roadGameObjectModel.Build(position); return true;
                    case SpritesGroupEnum.rail: railGameObjectModel.Build(position); return true;
                    case SpritesGroupEnum.wire: wireGameObjectModel.Build(position); return true;
                    case SpritesGroupEnum.garden: gardenModel.Build(position); return true;
                    case SpritesGroupEnum.resident0: residentModel.Build(position); return true;
                    case SpritesGroupEnum.industrial0: industrialModel.Build(position); return true;
                    case SpritesGroupEnum.comercial0: comercialModel.Build(position); return true;
                    case SpritesGroupEnum.policedepartment: policeDepartmentModel.Build(position); return true;
                    case SpritesGroupEnum.firedepartment: fireDepartmentModel.Build(position); return true;
                    case SpritesGroupEnum.stadium: stadiumModel.Build(position); return true;
                    case SpritesGroupEnum.coalpowerplant: coalPowerPlantModel.Build(position); return true;
                    case SpritesGroupEnum.nuclearpowerplant: nuclearPowerPlantModel.Build(position); return true;
                    case SpritesGroupEnum.seaport: seaPortModel.Build(position); return true;
                    case SpritesGroupEnum.airport: airPortModel.Build(position); return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        public TestPositionDTO TestPosition(GroupDTO? group, int x, int y)
        {
            return this.terrainModel.TestPosition(group, x, y);
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {
            OnRenderCompleted(EventArgs.Empty);
        }

        protected virtual void OnRenderCompleted(EventArgs e)
        {
            RenderCompleted?.Invoke(this, e);
        }





    }
}
