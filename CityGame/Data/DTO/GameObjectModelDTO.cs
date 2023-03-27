﻿namespace CityGame.Data.DTO
{
    public class GameObjectModelDTO
    {
        public GroupDTO? Group { get; set; }
        public PositionDTO? positionDTO { get; set; }

        public PositionDTO? centerPosition { get; set; }
        public byte animationFrame { get; set; } = 1;

        public byte level = 0;

        public uint timeLive = 0;

    }
}
