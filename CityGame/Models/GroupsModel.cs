using CityGame.DataModels;
using CityGame.DataModels.Enum;
using CityGame.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace CityGame.Models
{
    public class GroupsModel
    {
        /*
        private List<SpriteModel>? waterGroup;
        private List<SpriteModel>? forestGroup;
        private List<SpriteModel>? roadGroup;
        private List<SpriteModel>? railGroup;
        private List<SpriteModel>? wireGroup;
        */

        private Dictionary<string, List<SpriteModel>?> Groups = new Dictionary<string, List<SpriteModel>?>();

        //TODO: injection
        private SpriteBusiness spriteBusiness = new SpriteBusiness();

        public GroupsModel()
        {
            foreach (string groupName in SpritesGroupEnum.groups)
            {
                Groups.Add(groupName, spriteBusiness.GetSpriteByGroupName(groupName));
            }
        }

        public List<SpriteModel>? GetGroup(string groupName)
        {
            return Groups.FirstOrDefault(g => g.Key.Equals(groupName)).Value;
        }

    }
}
