﻿using CityGame.Data.DTO;
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
        private ResidentModel residentModel;

        public event EventHandler RenderCompleted;


        public CityGameEngine(string cityName, int size = 100)
        {
            terrainModel = new TerrainModel(size);
            roadGameObjectModel = new RoadGameObjectModel(spriteBusiness, terrainModel);
            railGameObjectModel = new RailGameObjectModel(spriteBusiness, terrainModel);
            wireGameObjectModel = new WireGameObjectModel(spriteBusiness, terrainModel);
            residentModel = new ResidentModel(spriteBusiness, terrainModel);


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

        public void BuildObject(PositionDTO position, GroupDTO? group)
        {
            switch (group?.Name)
            {
                case SpritesGroupEnum.road: roadGameObjectModel.Build(position); break;
                case SpritesGroupEnum.rail: railGameObjectModel.Build(position); break;
                case SpritesGroupEnum.wire: wireGameObjectModel.Build(position); break;
                default:
                    residentModel.Build(position);                    
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
