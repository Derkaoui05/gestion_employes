using GestionEmployes.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionEmployes.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base(new SQLiteConnection("Data Source=GestionEmployes.db;"), true)
        {
            // CONFIGURATIONS IMPORTANTES POUR ÉVITER LES CONFLITS
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.AutoDetectChangesEnabled = true;

            // Log EF SQL to debug output for diagnostics
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

            // Désactiver les migrations automatiques - utiliser plutôt des migrations explicites
            Database.SetInitializer<ApplicationDbContext>(null);

            // Ou si vous voulez garder la création automatique :
            // Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationDbContext>());

            // Désactiver les migrations automatiques - utiliser plutôt des migrations explicites
            Database.SetInitializer<ApplicationDbContext>(null);

            // Ou si vous voulez garder la création automatique :
            // Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationDbContext>());
        }

        public DbSet<Employe> Employes { get; set; }
        public DbSet<Avance> Avances { get; set; }
        public DbSet<Absence> Absences { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // DÉSACTIVER la suppression en cascade pour éviter les problèmes
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            // Configure Employe entity
            modelBuilder.Entity<Employe>()
                .HasKey(e => e.Cin)
                .Property(e => e.Cin)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<Employe>()
                .Property(e => e.Nom)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Employe>()
                .Property(e => e.Prenom)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Employe>()
                .Property(e => e.Utilisateur)
                .HasMaxLength(50)
                .IsRequired();

            // CORRECTION: Utiliser decimal(18,2) au lieu de (10,2)
            modelBuilder.Entity<Employe>()
                .Property(e => e.Salaire)
                .HasPrecision(18, 2); // Changé de 10,2 à 18,2

            // Configure Avance entity
            modelBuilder.Entity<Avance>()
                .HasKey(a => a.Id);

            // CORRECTION: Utiliser decimal(18,2) au lieu de (10,2)
            modelBuilder.Entity<Avance>()
                .Property(a => a.Montant)
                .HasPrecision(18, 2); // Changé de 10,2 à 18,2

            modelBuilder.Entity<Avance>()
                .HasRequired(a => a.Employe)
                .WithMany(e => e.Avances)
                .HasForeignKey(a => a.EmployeCin)
                .WillCascadeOnDelete(false); // Changé de true à false

            // Configure Absence entity
            modelBuilder.Entity<Absence>()
                .HasKey(a => a.Id);

            // CORRECTION: Utiliser decimal(18,2) au lieu de (10,2)
            modelBuilder.Entity<Absence>()
                .Property(a => a.Penalite)
                .HasPrecision(18, 2); // Changé de 10,2 à 18,2

            modelBuilder.Entity<Absence>()
                .HasRequired(a => a.Employe)
                .WithMany(e => e.Absences)
                .HasForeignKey(a => a.EmployeCin)
                .WillCascadeOnDelete(false); // Changé de true à false

            base.OnModelCreating(modelBuilder);
        }
    }
}