


using System.Drawing;

namespace MobileTag.Models
{
    public class Team
    {
        public int ID { get; set; }
        public string TeamName { get; set; }
        //public Color TeamColor { get; set; }

        //public Team(int id, string teamName, Color teamColor)
        //{
        //    ID = id;
        //    TeamName = teamName;
        //    TeamColor = teamColor;
        //}

        //Constructor that does not initialize Color
        public Team(int id, string teamName)
        {
            ID = id;
            TeamName = teamName;
        }
    }
}