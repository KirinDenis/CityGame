namespace CityGame.DataModels
{

    /// <summary>
    /// Resource sprite item model
    /// </summary>
    public class SpriteModel
    {
        /// <summary>
        /// sprite position inside resource (image)
        /// </summary>
        public PositionModel position { get; set; } = new PositionModel();

        /// <summary>
        /// sprite name for human
        /// </summary>
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// sprite is part of sprite's group, null by default -> not in group 
        /// </summary>
        public int groupId { get; set; } = 0;

        /// <summary>
        /// sprite location inside group, null by default
        /// </summary>
        public PositionModel? groupPosition { get; set; }

        /// <summary>
        /// Sprite animation frame index (if 0 - it is not animation frame)
        /// </summary>
        public int? animationFrame { get; set; } = 0;
    }
}
