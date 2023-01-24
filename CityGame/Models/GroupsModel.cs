using CityGame.DTOs;
using CityGame.DTOs.Enum;
using CityGame.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace CityGame.Models
{
    public class GroupsModel
    {
        /*
        private List<SpriteDTO>? waterGroup;
        private List<SpriteDTO>? forestGroup;
        private List<SpriteDTO>? roadGroup;
        private List<SpriteDTO>? railGroup;
        private List<SpriteDTO>? wireGroup;
        */

        private Dictionary<string, List<SpriteDTO>?> Groups = new Dictionary<string, List<SpriteDTO>?>();

        //TODO: injection
        private SpriteBusiness spriteBusiness = new SpriteBusiness();

        public GroupsModel()
        {
            foreach (string groupName in SpritesGroupEnum.groups)
            {
                Groups.Add(groupName, spriteBusiness.GetSpriteByGroupName(groupName));
            }
        }

        public List<SpriteDTO>? GetGroup(string groupName)
        {
            return Groups.FirstOrDefault(g => g.Key.Equals(groupName)).Value;
        }

    }
}
