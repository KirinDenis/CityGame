using CityGame.DataModels;
using CityGame.DataModels.Enum;
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
        private NetworkModel networkModel;
        private GroupsModel groupsModel;


        public event EventHandler RenderCompleted;


        public CityGameEngine(string cityName, int size = 400)
        {
            groupsModel = new GroupsModel();
            terrainModel = new TerrainModel(groupsModel, size);
            networkModel = new NetworkModel(groupsModel, terrainModel);

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

        public void PutNetworkItem(int x, int y, NetworkType networkType)
        {
            networkModel.PutNetworkItem(x, y, networkType);
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
