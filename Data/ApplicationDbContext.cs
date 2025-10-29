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
        public ApplicationDbContext() : base(new SQLiteConnection("Data Source=GestionEmploye.db;"), true)
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.AutoDetectChangesEnabled = true;

            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

            Database.SetInitializer<ApplicationDbContext>(null);
        }

        public DbSet<Employe> Employes { get; set; }
        public DbSet<Avance> Avances { get; set; }
        public DbSet<Absence> Absences { get; set; }

        // ✅ Nouvelles entités
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Facture> Factures { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            // ---------------- EMPLOYE ----------------
            modelBuilder.Entity<Employe>()
                .HasKey(e => e.Cin)
                .Property(e => e.Cin)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<Employe>()
                .Property(e => e.Salaire)
                .HasPrecision(18, 2);

            // ---------------- AVANCE ----------------
            modelBuilder.Entity<Avance>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Avance>()
                .Property(a => a.Montant)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Avance>()
                .HasRequired(a => a.Employe)
                .WithMany(e => e.Avances)
                .HasForeignKey(a => a.EmployeCin)
                .WillCascadeOnDelete(false);

            // ---------------- ABSENCE ----------------
            modelBuilder.Entity<Absence>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Absence>()
                .Property(a => a.Penalite)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Absence>()
                .HasRequired(a => a.Employe)
                .WithMany(e => e.Absences)
                .HasForeignKey(a => a.EmployeCin)
                .WillCascadeOnDelete(false);

            // ---------------- SUPPLIER ----------------
            modelBuilder.Entity<Supplier>()
                .HasKey(s => s.ID)
                .ToTable("Supplier");  // Explicitement "Supplier" (singulier)

            modelBuilder.Entity<Supplier>()
                .Property(s => s.Name)
                .HasMaxLength(150)
                .IsRequired();

            // ---------------- FACTURE ----------------
            modelBuilder.Entity<Facture>()
                .HasKey(f => f.Id)
                .ToTable("Facture");

            modelBuilder.Entity<Facture>()
                .Property(f => f.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Facture>()
                .Property(f => f.Advance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Facture>()
                .HasRequired(f => f.Supplier)
                .WithMany(s => s.Factures)
                .HasForeignKey(f => f.SupplierId)
                .WillCascadeOnDelete(false);

            // ---------------- TRANSACTION ----------------
            // Mapped to PaymentTransaction table (évite le mot réservé SQL)
            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.Id)
                .ToTable("PaymentTransaction");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .HasRequired(t => t.Facture)
                .WithMany(f => f.Transactions)
                .HasForeignKey(t => t.FactureId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Transaction>()
                .HasOptional(t => t.Employee)
                .WithMany()
                .HasForeignKey(t => t.EmployeeCin)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}