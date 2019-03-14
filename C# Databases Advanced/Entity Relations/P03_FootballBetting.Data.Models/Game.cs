using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    public class Game
    {
        public Game()
        {
            this.PlayerStatistics = new List<PlayerStatistic>();
            this.Bets = new List<Bet>();
            this.Result = $"{this.HomeTeamGoals}:{this.AwayTeamGoals}";
        }

        public int GameId { get; set; }

        public int HomeTeamId { get; set; }
        [NotMapped]
        public Team HomeTeam { get; set; }

        public int AwayTeamId { get; set; }
        [NotMapped]
        public Team AwayTeam { get; set; }

        public int HomeTeamGoals { get; set; }

        public int AwayTeamGoals { get; set; }

        public DateTime DateTime { get; set; }

        public double HomeTeamBetRate { get; set; }

        public double AwayTeamBetRate { get; set; }

        public double DrawBetRate { get; set; }

        public string Result { get; set; }

        public ICollection<PlayerStatistic> PlayerStatistics { get; set; }

        public ICollection<Bet> Bets { get; set; }
    }
}