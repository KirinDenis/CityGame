using CityGame.Data.DTO;
using CityGame.DTOs;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CityGame.Models
{
    public class CityGameEngine
    {

        private TerrainModel terrainModel;
        private RoadGameObjectModel roadGameObjectModel;
        private RailGameObjectModel railGameObjectModel;
        private ResidentModel residentModel;
        


        public event EventHandler RenderCompleted;


        public CityGameEngine(string cityName, int size = 100)
        {        
            terrainModel = new TerrainModel(size);
            roadGameObjectModel = new RoadGameObjectModel(terrainModel);
            railGameObjectModel = new RailGameObjectModel(terrainModel);
            

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

        public void PutObject(ushort x, ushort y, GroupDTO? group)
        {
            switch (group?.Name)
            {
                case SpritesGroupEnum.road : roadGameObjectModel.Put(x, y); break;
                case SpritesGroupEnum.rail: railGameObjectModel.Put(x, y); break;
                case SpritesGroupEnum.wire: railGameObjectModel.Put(x, y); break;
                default:
                    residentModel = new ResidentModel(terrainModel);
                    residentModel.Group = group;
                    residentModel.Put(x, y, group);
                    residentModel.Animate();
                    break;

            }
        }

        public ObjectType[,] TestPosition(GroupDTO? group, int x, int y)
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
