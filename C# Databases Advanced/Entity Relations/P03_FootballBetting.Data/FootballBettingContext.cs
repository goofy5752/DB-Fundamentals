namespace P03_FootballBetting.Data
{
    using Microsoft.EntityFrameworkCore;
    using P03_FootballBetting.Data.Models;

    public class FootballBettingContext : DbContext
    {
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
        }

        private static void ConfigTeamEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                            .Entity<Team>(entity =>
                            {
                                entity
                        .HasKey(t => t.TeamId);

                                entity
                        .Property(t => t.Name)
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .IsRequired();
                            });
        }
    }
}