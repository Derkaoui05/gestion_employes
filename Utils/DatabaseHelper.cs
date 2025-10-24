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

        public static void EnsureDatabaseCreated(){
            try
            {
                Console.WriteLine("   ⏳ Initialisation de la base de données...");

                // Chemin physique du fichier SQLite (résout les problèmes AnyCPU/working dir)
                var dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GestionEmployes.db");

                bool needsRecreate = false;
                bool exists;

                // Ouvrir un contexte uniquement pour l'inspection
                using (var context = CreateNewContext())
                {
                    exists = context.Database.Exists();

                    if (!exists)
                    {
                        Console.WriteLine("   ⏳ Création de la base de données...");
                        // Aucune base, on créera après avoir fermé le contexte
                        needsRecreate = true; // utiliser le même flux de création hors connexion
                    }
                    else
                    {
                        Console.WriteLine("   ✅ Base de données existe déjà");

                        // Vérifier les tables essentielles
                        bool hasEmploye = false, hasAvance = false, hasAbsence = false;
                        try
                        {
                            hasEmploye = context.Database.SqlQuery<int>(
                                "SELECT 1 FROM sqlite_master WHERE type='table' AND name='Employe'").FirstOrDefault() == 1;
                            hasAvance = context.Database.SqlQuery<int>(
                                "SELECT 1 FROM sqlite_master WHERE type='table' AND name='Avance'").FirstOrDefault() == 1;
                            hasAbsence = context.Database.SqlQuery<int>(
                                "SELECT 1 FROM sqlite_master WHERE type='table' AND name='Absence'").FirstOrDefault() == 1;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"   ⚠️ Erreur vérification tables: {ex.Message}");
                        }

                        if (!(hasEmploye && hasAvance && hasAbsence))
                        {
                            Console.WriteLine("   🔧 Schéma incomplet détecté (tables manquantes). Création des tables manquantes...");
                            try
                            {
                                // Activer les clés étrangères
                                context.Database.ExecuteSqlCommand("PRAGMA foreign_keys = ON;");

                                // Créer Employe si nécessaire
                                context.Database.ExecuteSqlCommand(@"CREATE TABLE IF NOT EXISTS Employe (
                                    Cin TEXT NOT NULL PRIMARY KEY,
                                    Nom TEXT NOT NULL,
                                    Prenom TEXT NOT NULL,
                                    Utilisateur TEXT NOT NULL,
                                    MotDePasse INTEGER NOT NULL,
                                    Salaire NUMERIC NULL
                                );");

                                // Créer Avance si nécessaire
                                context.Database.ExecuteSqlCommand(@"CREATE TABLE IF NOT EXISTS Avance (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Montant NUMERIC NOT NULL,
                                    DateAvance TEXT NOT NULL,
                                    EmployeCin TEXT NOT NULL,
                                    FOREIGN KEY(EmployeCin) REFERENCES Employe(Cin) ON DELETE NO ACTION ON UPDATE NO ACTION
                                );");

                                // Créer Absence si nécessaire
                                context.Database.ExecuteSqlCommand(@"CREATE TABLE IF NOT EXISTS Absence (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Penalite NUMERIC NOT NULL,
                                    DateAbsence TEXT NOT NULL,
                                    EmployeCin TEXT NOT NULL,
                                    FOREIGN KEY(EmployeCin) REFERENCES Employe(Cin) ON DELETE NO ACTION ON UPDATE NO ACTION
                                );");

                                hasEmploye = true; hasAvance = true; hasAbsence = true;
                                Console.WriteLine("   ✅ Tables manquantes créées");
                            }
                            catch (Exception exCreate)
                            {
                                Console.WriteLine($"   ⚠️ Création directe des tables a échoué: {exCreate.Message}. On forcera une recréation complète.");
                                needsRecreate = true;
                            }
                        }
                        else
                        {
                            Console.WriteLine("   ✅ Schéma OK (Employe, Avance, Absence)");
                        }
                    }
                } // IMPORTANT: sortir du using pour libérer la connexion avant delete/create

                if (needsRecreate)
                {
                    try
                    {
                        // Fermer toute connexion résiduelle puis supprimer via EF
                        using (var ctxToDelete = CreateNewContext())
                        {
                            try { ctxToDelete.Database.Connection.Close(); } catch { }
                            if (ctxToDelete.Database.Exists())
                            {
                                ctxToDelete.Database.Delete();
                                Console.WriteLine("   🗑️ Ancienne base supprimée (EF)");
                            }
                        }

                        // File system fallback si le fichier existe toujours
                        if (System.IO.File.Exists(dbPath))
                        {
                            System.IO.File.Delete(dbPath);
                            Console.WriteLine("   🗑️ Fichier SQLite supprimé (FS)");
                        }
                    }
                    catch (Exception exDel)
                    {
                        Console.WriteLine($"   ⚠️ Échec suppression: {exDel.Message}");
                    }

                    // Recréer avec un nouveau contexte propre
                    using (var ctxCreate = CreateNewContext())
                    {
                        // Crée le fichier si absent
                        ctxCreate.Database.CreateIfNotExists();
                        // S'assurer que les tables existent
                        ctxCreate.Database.ExecuteSqlCommand("PRAGMA foreign_keys = ON;");
                        ctxCreate.Database.ExecuteSqlCommand(@"CREATE TABLE IF NOT EXISTS Employe (
                                    Cin TEXT NOT NULL PRIMARY KEY,
                                    Nom TEXT NOT NULL,
                                    Prenom TEXT NOT NULL,
                                    Utilisateur TEXT NOT NULL,
                                    MotDePasse INTEGER NOT NULL,
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
                        Console.WriteLine("   ✅ Base (re)créée avec les tables du modèle");
                    }
                }
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