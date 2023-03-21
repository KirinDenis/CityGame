using CityGame.DTOs.Const;
using System.Collections.Generic;

namespace CityGame.Data.DTO
{
    public class GroupSpritesDTO
    {
        public PositionDTO?[,]? Sprites = new PositionDTO[GameConsts.GroupSize, GameConsts.GroupSize];
    }

    public class GroupDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public ushort Width { get; set; } = 3;
        public ushort Height { get; set; } = 3;

        public ushort CenterX { get; set; } = 1;
        public ushort CenterY { get; set; } = 1;

        public List<GroupSpritesDTO> Frames { get; set; } = new List<GroupSpritesDTO>();

    }
}
