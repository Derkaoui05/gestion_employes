using System;
using System.Windows.Forms;

namespace GestionEmployes.Utils
{
    public class LicenseManager
    {
        private const string SUPPORT_PHONE = "0669286543";

        public static bool ValidateCredentials(string username, string password)
        {
            try
            {
                Console.WriteLine($"=== Validation des identifiants ===");
                Console.WriteLine($"Username saisi: '{username}'");
                Console.WriteLine($"Password saisi: '{password}'");

                // ✅ VALIDATION AVEC LE SYSTÈME D'ACTIVATION WINDOWS
                // Vérifier d'abord si l'application est activée
                if (!ActivationManager.IsActivated())
                {
                    MessageBox.Show($"❌ Application non activée.\n\n" +
                                  $"📋 Procédez à l'activation :\n" +
                                  $"1. Obtenez votre Machine ID\n" +
                                  $"2. Envoyez-le au support\n" +
                                  $"3. Recevez votre clé d'activation\n\n" +
                                  $"📞 Support: {SUPPORT_PHONE}",
                                  "Activation Requise",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                    return false;
                }

                // ✅ SI ACTIVÉE, ACCEPTER TOUS LES IDENTIFIANTS
                // (ou vous pouvez mettre une logique spécifique ici)
                Console.WriteLine("✅ Application activée - Connexion autorisée");
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur validation: {ex.Message}");
                MessageBox.Show($"❌ Erreur technique: {ex.Message}\n\n📞 Contactez le support: {SUPPORT_PHONE}",
                              "Erreur",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
                return false;
            }
        }

        public static string GetCustomerName()
        {
            // Retourner un nom générique
            return "Client GestionEmployes";
        }

        public static string GetLicenseUsername()
        {
            // ✅ Retourner vide - l'utilisateur saisira ses identifiants
            return "";
        }

        public static string GetSupportPhone()
        {
            return SUPPORT_PHONE;
        }

        public static bool CheckLicenseExists()
        {
            // ✅ Retourner true car on utilise le système d'activation Windows
            return true;
        }

        public static string GetLicenseInfo()
        {
            return $"🔑 SYSTÈME D'ACTIVATION\n\n" +
                   $"📋 PROCÉDURE :\n" +
                   $"1. Obtenez votre Machine ID\n" +
                   $"2. Envoyez-le au support: {SUPPORT_PHONE}\n" +
                   $"3. Recevez votre clé d'activation\n" +
                   $"4. Activez l'application\n" +
                   $"5. Utilisez n'importe quels identifiants pour vous connecter\n\n" +
                   $"📞 Support: {SUPPORT_PHONE}";
        }
    }
}