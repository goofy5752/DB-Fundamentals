using System.Collections.Generic;

namespace P03_FootballBetting.Data.Models
{
    public class Color
    {
        public Color()
        {
            this.HomeTeam = new List<Team>();

            this.AwayTeam = new List<Team>();
        }

        public int ColorId { get; set; }

        public string Name { get; set; }

        public ICollection<Team> HomeTeam { get; set; }

        public ICollection<Team> AwayTeam { get; set; }
    }
}