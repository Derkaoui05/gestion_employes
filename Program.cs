using GestionEmployes.Data;
using GestionEmployes.Forms;
using GestionEmployes.Utils;
using System;
using System.IO;
using System.Windows.Forms;

namespace GestionEmployes
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Pas de console en production

            try
            {
                Console.WriteLine("=== Démarrage Application ===\n");

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // ÉTAPE 2: Vérifier/créer la base de données
                Console.WriteLine("🔧 Vérification de la base de données...");
                Console.WriteLine("   ⏳ Cela peut prendre quelques secondes la première fois...");
                try
                {
                    var startTime = DateTime.Now;
                    DatabaseHelper.EnsureDatabaseCreated();
                    var elapsed = DateTime.Now - startTime;
                    Console.WriteLine($"✅ Base de données prête (durée: {elapsed.TotalSeconds:F1}s)\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ ERREUR BD: {ex.Message}");
                    Console.WriteLine($"   Détails: {ex.InnerException?.Message}");
                    Console.WriteLine($"   Stack: {ex.StackTrace}\n");
                    MessageBox.Show($"Erreur base de données:\n{ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine("❌ Application fermée - Erreur BD");
                    return;
                }

                // ÉTAPE 3: Gérer la license
                var licensePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "license.dat");
                Console.WriteLine("📄 Gestion de la license...");
                try
                {
                    if (!File.Exists(licensePath))
                    {
                        Console.WriteLine("   📝 Création de la license...");
                        LicenseManager.GenerateLicense("Admin Company", "admin", "2025");
                        Console.WriteLine("   ✅ License créée avec admin/2025");
                    }
                    else
                    {
                        Console.WriteLine("   ✅ License existante détectée");
                        bool licenseValide = LicenseManager.ValidateCredentials("admin", "2025");
                        Console.WriteLine($"   🔐 Test license: {(licenseValide ? "✅ OK" : "❌ INVALIDE")}");
                        if (!licenseValide)
                        {
                            Console.WriteLine("   🔄 Recréation de la license...");
                            File.Delete(licensePath);
                            LicenseManager.GenerateLicense("Admin Company", "admin", "2025");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ❌ Erreur license: {ex.Message}");
                }

                Console.WriteLine();

                // ÉTAPE 4: Vérifier l'activation (clé universelle, une seule fois)
                Console.WriteLine("🔐 Vérification de l'activation...");
                if (!ActivationManager.CheckActivation())
                {
                    Console.WriteLine("❌ Application non activée");
                    return;
                }

                // ÉTAPE 5: Lancer l'application
                Console.WriteLine("🚀 Lancement de l'application...\n");
                Application.Run(new LoginForm());
                Console.WriteLine("👋 Application fermée");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n💥 ERREUR CRITIQUE:");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"\nErreur interne:");
                    Console.WriteLine($"Message: {ex.InnerException.Message}");
                }

                MessageBox.Show(
                    $"Erreur critique:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                // Pas d'attente clavier en mode sans console
            }
            finally
            {
                Console.WriteLine("\n🧹 Nettoyage des ressources...");
                Console.WriteLine("✅ Nettoyage terminé");
                Console.WriteLine("\n👋 Au revoir!");
            }
        }

    }
}