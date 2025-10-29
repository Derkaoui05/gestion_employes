using System;
using System.Data.Entity;
using System.Linq;

namespace GestionEmployes.Data
{
    public class DatabaseHelper
    {
        public static ApplicationDbContext CreateNewContext()
        {
            return new ApplicationDbContext();
        }

        public static void EnsureDatabaseCreated()
        {
            try
            {
                Console.WriteLine("   ⏳ Initialisation de la base de données...");

                var dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GestionEmploye.db");

                bool needsRecreate = false;
                bool exists;

                using (var context = CreateNewContext())
                {
                    exists = context.Database.Exists();

                    if (!exists)
                    {
                        Console.WriteLine("   ⏳ Création de la base de données...");
                        needsRecreate = true;
                    }
                    else
                    {
                        Console.WriteLine("   ✅ Base de données existe déjà");

                        bool hasEmploye = false, hasAvance = false, hasAbsence = false,
                             hasSupplier = false, hasFacture = false, hasTransaction = false;

                        try
                        {
                            hasEmploye = context.Database.SqlQuery<int>(
                                "SELECT 1 FROM sqlite_master WHERE type='table' AND name='Employe'").FirstOrDefault() == 1;
                            hasAvance = context.Database.SqlQuery<int>(
                                "SELECT 1 FROM sqlite_master WHERE type='table' AND name='Avance'").FirstOrDefault() == 1;
                            hasAbsence = context.Database.SqlQuery<int>(
                                "SELECT 1 FROM sqlite_master WHERE type='table' AND name='Absence'").FirstOrDefault() == 1;
                            hasSupplier = context.Database.SqlQuery<int>(
                                "SELECT 1 FROM sqlite_master WHERE type='table' AND name='Supplier'").FirstOrDefault() == 1;
                            hasFacture = context.Database.SqlQuery<int>(
                                "SELECT 1 FROM sqlite_master WHERE type='table' AND name='Facture'").FirstOrDefault() == 1;
                            hasTransaction = context.Database.SqlQuery<int>(
                                "SELECT 1 FROM sqlite_master WHERE type='table' AND name='Transaction'").FirstOrDefault() == 1;

                            Console.WriteLine($"   📊 Tables détectées:");
                            Console.WriteLine($"      - Employe: {(hasEmploye ? "✅" : "❌")}");
                            Console.WriteLine($"      - Avance: {(hasAvance ? "✅" : "❌")}");
                            Console.WriteLine($"      - Absence: {(hasAbsence ? "✅" : "❌")}");
                            Console.WriteLine($"      - Supplier: {(hasSupplier ? "✅" : "❌")}");
                            Console.WriteLine($"      - Facture: {(hasFacture ? "✅" : "❌")}");
                            Console.WriteLine($"      - Transaction: {(hasTransaction ? "✅" : "❌")}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"   ⚠️ Erreur vérification tables: {ex.Message}");
                        }

                        if (!(hasEmploye && hasAvance && hasAbsence && hasSupplier && hasFacture && hasTransaction))
                        {
                            Console.WriteLine("   🔧 Schéma incomplet détecté. Création des tables manquantes...");
                            try
                            {
                                context.Database.ExecuteSqlCommand("PRAGMA foreign_keys = ON;");

                                // Employe
                                context.Database.ExecuteSqlCommand(@"CREATE TABLE IF NOT EXISTS Employe (
                                    Cin TEXT NOT NULL PRIMARY KEY,
                                    Nom TEXT NOT NULL,
                                    Prenom TEXT NOT NULL,
                                    Utilisateur TEXT NOT NULL,
                                    Salaire NUMERIC NULL
                                );");

                                // Avance
                                context.Database.ExecuteSqlCommand(@"CREATE TABLE IF NOT EXISTS Avance (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Montant NUMERIC NOT NULL,
                                    DateAvance TEXT NOT NULL,
                                    EmployeCin TEXT NOT NULL,
                                    FOREIGN KEY(EmployeCin) REFERENCES Employe(Cin) ON DELETE NO ACTION ON UPDATE NO ACTION
                                );");

                                // Absence
                                context.Database.ExecuteSqlCommand(@"CREATE TABLE IF NOT EXISTS Absence (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Penalite NUMERIC NOT NULL,
                                    DateAbsence TEXT NOT NULL,
                                    EmployeCin TEXT NOT NULL,
                                    FOREIGN KEY(EmployeCin) REFERENCES Employe(Cin) ON DELETE NO ACTION ON UPDATE NO ACTION
                                );");

                                // Supplier - CORRIGÉ (Supplier au lieu de Suppliers)
                                context.Database.ExecuteSqlCommand(@"CREATE TABLE IF NOT EXISTS Supplier (
                                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Name TEXT NOT NULL,
                                    Contact TEXT,
                                    Phone TEXT,
                                    Email TEXT,
                                    Address TEXT,
                                    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                                    IsActive BOOLEAN DEFAULT 1
                                );");

                                // Facture
                                context.Database.ExecuteSqlCommand(@"CREATE TABLE IF NOT EXISTS Facture (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Number TEXT NOT NULL UNIQUE,
                                    SupplierId INTEGER NOT NULL,
                                    Amount DECIMAL(15,2) NOT NULL,
                                    Advance DECIMAL(15,2) DEFAULT 0,
                                    InvoiceDate DATETIME NOT NULL,
                                    DueDate DATETIME NOT NULL,
                                    Notes TEXT,
                                    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                                    FOREIGN KEY (SupplierId) REFERENCES Supplier(ID) ON DELETE NO ACTION ON UPDATE NO ACTION
                                );");

                                // Transaction - Renommé en PaymentTransaction
                                context.Database.ExecuteSqlCommand(@"CREATE TABLE IF NOT EXISTS PaymentTransaction (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    FactureId INTEGER NOT NULL,
                                    EmployeeCin TEXT,
                                    Type TEXT NOT NULL,
                                    Amount DECIMAL(15,2) NOT NULL,
                                    TransactionDate DATETIME NOT NULL,
                                    Description TEXT,
                                    PaymentMethod TEXT,
                                    Reference TEXT,
                                    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                                    FOREIGN KEY (FactureId) REFERENCES Facture(Id) ON DELETE NO ACTION ON UPDATE NO ACTION,
                                    FOREIGN KEY (EmployeeCin) REFERENCES Employe(Cin) ON DELETE NO ACTION ON UPDATE NO ACTION
                                );");

                                Console.WriteLine("   ✅ Tables manquantes créées avec succès");
                            }
                            catch (Exception exCreate)
                            {
                                Console.WriteLine($"   ⚠️ Erreur création tables: {exCreate.Message}");
                                needsRecreate = true;
                            }
                        }
                        else
                        {
                            Console.WriteLine("   ✅ Schéma complet détecté");
                        }
                    }
                }

                if (needsRecreate)
                {
                    try
                    {
                        Console.WriteLine("   🔄 Recréation complète de la base de données...");

                        // Pour SQLite, on ne peut pas utiliser Database.Delete()
                        // On supprime simplement le fichier
                        if (System.IO.File.Exists(dbPath))
                        {
                            try
                            {
                                System.IO.File.Delete(dbPath);
                                Console.WriteLine("   🗑️ Fichier SQLite supprimé");
                            }
                            catch (Exception exFile)
                            {
                                Console.WriteLine($"   ⚠️ Impossible de supprimer le fichier: {exFile.Message}");
                            }
                        }

                        // Créer une nouvelle base de données
                        using (var ctxCreate = CreateNewContext())
                        {
                            ctxCreate.Database.CreateIfNotExists();
                            ctxCreate.Database.ExecuteSqlCommand("PRAGMA foreign_keys = ON;");

                            // Créer toutes les tables
                            ctxCreate.Database.ExecuteSqlCommand(@"CREATE TABLE IF NOT EXISTS Employe (
                                Cin TEXT NOT NULL PRIMARY KEY,
                                Nom TEXT NOT NULL,
                                Prenom TEXT NOT NULL,
                                Utilisateur TEXT NOT NULL,
                                Salaire NUMERIC NULL
                            );");

                            ctxCreate.Database.ExecuteSqlCommand(@"CREATE TABLE IF NOT EXISTS Avance (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Montant NUMERIC NOT NULL,
                                DateAvance TEXT NOT NULL,
                                EmployeCin TEXT NOT NULL,
                                FOREIGN KEY(EmployeCin) REFERENCES Employe(Cin) ON DELETE NO ACTION ON UPDATE NO ACTION
                            );");

                            ctxCreate.Database.ExecuteSqlCommand(@"CREATE TABLE IF NOT EXISTS Absence (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Penalite NUMERIC NOT NULL,
                                DateAbsence TEXT NOT NULL,
                                EmployeCin TEXT NOT NULL,
                                FOREIGN KEY(EmployeCin) REFERENCES Employe(Cin) ON DELETE NO ACTION ON UPDATE NO ACTION
                            );");

                            ctxCreate.Database.ExecuteSqlCommand(@"CREATE TABLE IF NOT EXISTS Supplier (
                                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                Name TEXT NOT NULL,
                                Contact TEXT,
                                Phone TEXT,
                                Email TEXT,
                                Address TEXT,
                                CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                                IsActive BOOLEAN DEFAULT 1
                            );");

                            ctxCreate.Database.ExecuteSqlCommand(@"CREATE TABLE IF NOT EXISTS Facture (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Number TEXT NOT NULL UNIQUE,
                                SupplierId INTEGER NOT NULL,
                                Amount DECIMAL(15,2) NOT NULL,
                                Advance DECIMAL(15,2) DEFAULT 0,
                                InvoiceDate DATETIME NOT NULL,
                                DueDate DATETIME NOT NULL,
                                Notes TEXT,
                                CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                                FOREIGN KEY (SupplierId) REFERENCES Supplier(ID) ON DELETE NO ACTION ON UPDATE NO ACTION
                            );");

                            // Transaction - Renommé en PaymentTransaction pour éviter le mot réservé
                            ctxCreate.Database.ExecuteSqlCommand(@"CREATE TABLE IF NOT EXISTS PaymentTransaction (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    FactureId INTEGER NOT NULL,
                                    EmployeeCin TEXT,
                                    Type TEXT NOT NULL,
                                    Amount DECIMAL(15,2) NOT NULL,
                                    TransactionDate DATETIME NOT NULL,
                                    Description TEXT,
                                    PaymentMethod TEXT,
                                    Reference TEXT,
                                    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                                    FOREIGN KEY (FactureId) REFERENCES Facture(Id) ON DELETE NO ACTION ON UPDATE NO ACTION,
                                    FOREIGN KEY (EmployeeCin) REFERENCES Employe(Cin) ON DELETE NO ACTION ON UPDATE NO ACTION
                                );");

                            Console.WriteLine("   ✅ Base de données recréée avec succès");
                        }
                    }
                    catch (Exception exRecreate)
                    {
                        Console.WriteLine($"❌ Erreur recréation: {exRecreate.Message}");
                        throw;
                    }
                }

                Console.WriteLine("   ✅ Initialisation complète réussie!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur création BD: {ex.Message}");
                throw;
            }
        }

        public static void CloseContext(ApplicationDbContext context)
        {
            try
            {
                context?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Erreur fermeture contexte: {ex.Message}");
            }
        }
    }
}