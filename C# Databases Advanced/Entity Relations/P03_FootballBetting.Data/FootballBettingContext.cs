namespace P03_FootballBetting.Data
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using P03_FootballBetting.Data.Models;

    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        {

        }

        public FootballBettingContext(DbContextOptions options)
            :base(options)
        {

        }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Color> Colors { get; set; }

        public DbSet<Town> Towns { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<Position> Positions { get; set; }

        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Bet> Bets { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Config.SqlConnect);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigTeamEntity(modelBuilder);

            ConfigColorEntity(modelBuilder);

            ConfigTownEntity(modelBuilder);

            ConfigCountryEntity(modelBuilder);

            ConfigPlayerEntity(modelBuilder);

            ConfigPositionEntity(modelBuilder);

            ConfigPlayerStatisticEntity(modelBuilder);

            ConfigGameEntity(modelBuilder);

            ConfigBetEntity(modelBuilder);

            ConfigUserEntity(modelBuilder);
        }

        private static void ConfigUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>(entity =>
                {
                    entity
                        .HasKey(u => u.UserId);

                    entity
                        .Property(u => u.Username)
                        .HasMaxLength(60)
                        .IsRequired();

                    entity
                        .Property(u => u.Password)
                        .HasMaxLength(12)
                        .IsRequired();

                    entity
                        .Property(u => u.Email)
                        .HasMaxLength(80)
                        .IsRequired();

                    entity
                        .Property(u => u.Name)
                        .HasMaxLength(50)
                        .IsRequired();
                });
        }

        private static void ConfigBetEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Bet>(entity =>
                {
                    entity
                        .HasKey(b => b.BetId);

                    entity
                        .HasOne(b => b.Game)
                        .WithMany(g => g.Bets);

                    entity
                        .HasOne(b => b.User)
                        .WithMany(u => u.Bets);

                    entity
                        .Property(b => b.Prediction)
                        .IsRequired();

                    entity
                        .Property(b => b.UserId)
                        .IsRequired();

                    entity
                        .Property(b => b.GameId)
                        .IsRequired();
                });
        }

        private static void ConfigGameEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Game>(entity =>
                {
                    entity
                        .HasKey(g => g.GameId);
                });
        }

        private static void ConfigPlayerStatisticEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PlayerStatistic>()
                .HasKey(ps => new { ps.GameId, ps.PlayerId });

            modelBuilder.Entity<PlayerStatistic>()
                .HasOne(ps => ps.Game)
                .WithMany(g => g.PlayerStatistics)
                .HasForeignKey(ps => ps.GameId);

            modelBuilder.Entity<PlayerStatistic>()
                .HasOne(ps => ps.Player)
                .WithMany(p => p.PlayerStatistics)
                .HasForeignKey(ps => ps.PlayerId);
        }

        private static void ConfigPositionEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Position>(entity =>
                {
                    entity
                        .HasKey(p => p.PositionId);

                    entity
                        .Property(p => p.Name)
                        .HasMaxLength(50)
                        .IsRequired();
                });
        }

        private static void ConfigPlayerEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Player>(entity =>
                {
                    entity
                        .HasKey(p => p.PlayerId);

                    entity
                        .HasOne(p => p.Team)
                        .WithMany(t => t.Players);

                    entity
                        .HasOne(p => p.Position)
                        .WithMany(p => p.Players);

                    entity
                        .Property(p => p.Name)
                        .HasMaxLength(50)
                        .IsRequired()
                        .IsUnicode();

                    entity
                        .Property(p => p.SquadNumber)
                        .IsRequired();
                });
        }

        private static void ConfigCountryEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Country>(entity =>
                {
                    entity
                        .HasKey(c => c.CountryId);

                    entity
                        .Property(c => c.Name)
                        .HasMaxLength(30)
                        .IsRequired();
                });
        }

        private static void ConfigTownEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Town>(entity =>
                {
                    entity
                        .HasKey(t => t.TownId);

                    entity
                        .HasOne(t => t.Country)
                        .WithMany(c => c.Towns);

                    entity
                        .Property(t => t.Name)
                        .HasMaxLength(50)
                        .IsRequired();
                });
        }

        private static void ConfigColorEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Color>(entity =>
                {
                    entity
                        .HasKey(c => c.ColorId);

                    entity
                        .Property(c => c.Name)
                        .HasMaxLength(20)
                        .IsRequired();
                });
        }

        private static void ConfigTeamEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Team>(entity =>
                {
                    entity
                        .HasKey(t => t.TeamId);

                    entity
                        .HasOne(t => t.Town)
                        .WithMany(t => t.Teams);

                    entity
                        .HasOne(t => t.PrimaryKitColor)
                        .WithMany(pk => pk.PrimaryKitTeams)
                        .HasForeignKey(t => t.PrimaryKitColorId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity
                        .HasOne(t => t.SecondaryKitColor)
                        .WithMany(pk => pk.SecondaryKitTeams)
                        .HasForeignKey(t => t.SecondaryKitColorId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity
                        .Property(t => t.Name)
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .IsRequired();

                    entity
                        .Property(t => t.LogoUrl)
                        .IsRequired();

                    entity
                        .Property(t => t.Initials)
                        .HasColumnType("CHAR(3)")
                        .IsRequired();
                });
        }
    }
}