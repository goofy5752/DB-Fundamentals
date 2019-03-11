namespace P01_HospitalDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class HospitalContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }

        public DbSet<Visitation> Visitations { get; set; }

        public DbSet<Diagnose> Diagnoses { get; set; }

        public DbSet<Medicament> Medicaments { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<PatientMedicament> PatientMedicaments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Config.SqlConnection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigurePatientEntity(modelBuilder);

            ConfigureVisitationEntity(modelBuilder);

            ConfigureDiagnoseEntity(modelBuilder);

            ConfigureMedicamentEntity(modelBuilder);

            ConfigurePatientMedicamentEntity(modelBuilder);

            ConfigureDoctorEntity(modelBuilder);
        }

        private void ConfigureDoctorEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Doctor>()
                .HasKey(x => x.DoctorId);

            modelBuilder
                .Entity<Doctor>()
                .HasMany(d => d.Visitations)
                .WithOne(v => v.Doctor);

            modelBuilder
                .Entity<Doctor>()
                .Property(x => x.Name)
                .HasMaxLength(100)
                .IsUnicode();

            modelBuilder
                .Entity<Doctor>()
                .Property(x => x.Speciality)
                .HasMaxLength(100)
                .IsUnicode();
        }

        private void ConfigurePatientMedicamentEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PatientMedicament>()
                .HasKey(x => new { x.PatientId, x.MedicamentId });

            modelBuilder
                .Entity<PatientMedicament>()
                .HasOne(pm => pm.Patient)
                .WithMany(p => p.Prescriptions)
                .HasForeignKey(pm => pm.PatientId);

            modelBuilder
                .Entity<PatientMedicament>()
                .HasOne(pm => pm.Medicament)
                .WithMany(m => m.Prescriptions)
                .HasForeignKey(pm => pm.MedicamentId);
        }

        private void ConfigureMedicamentEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Medicament>()
                .HasKey(x => x.MedicamentId);

            modelBuilder
                .Entity<Medicament>()
                .Property(x => x.Name)
                .HasMaxLength(50)
                .IsUnicode();
        }

        private void ConfigureDiagnoseEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Diagnose>()
                .HasKey(x => x.DiagnoseId);

            modelBuilder
                .Entity<Diagnose>()
                .Property(x => x.Name)
                .HasMaxLength(50)
                .IsUnicode();

            modelBuilder
                .Entity<Diagnose>()
                .Property(x => x.Comments)
                .HasMaxLength(250)
                .IsUnicode();
        }

        private void ConfigureVisitationEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Visitation>()
                .HasKey(x => x.VisitationId);

            modelBuilder
                .Entity<Visitation>()
                .Property(x => x.Comments)
                .HasMaxLength(250)
                .IsUnicode();
        }

        private void ConfigurePatientEntity(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Patient>()
                .HasKey(x => x.PatientId);

            modelBuilder
                .Entity<Patient>()
                .HasMany(p => p.Visitations)
                .WithOne(v => v.Patient);

            modelBuilder
                .Entity<Patient>()
                .HasMany(p => p.Diagnoses)
                .WithOne(d => d.Patient);

            modelBuilder
                .Entity<Patient>()
                .Property(x => x.FirstName)
                .HasMaxLength(50)
                .IsUnicode();

            modelBuilder
                .Entity<Patient>()
                .Property(x => x.LastName)
                .HasMaxLength(50)
                .IsUnicode();

            modelBuilder
                .Entity<Patient>()
                .Property(x => x.Address)
                .HasMaxLength(250)
                .IsUnicode();

            modelBuilder
                .Entity<Patient>()
                .Property(x => x.Email)
                .HasMaxLength(80)
                .IsUnicode(false);
        }
    }
}
