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

                // ✅ VALIDATION AVEC IDENTIFIANTS GÉNÉRIQUES
                const string GENERIC_USERNAME = "admin";
                const string GENERIC_PASSWORD = "12345";

                bool isValid = username.Equals(GENERIC_USERNAME, StringComparison.OrdinalIgnoreCase) &&
                              password == GENERIC_PASSWORD;

                if (!isValid)
                {
                    MessageBox.Show($"❌ Identifiants incorrects.\n\n" +
                                  $"💡 Identifiants par défaut :\n" +
                                  $"Nom d'utilisateur: {GENERIC_USERNAME}\n" +
                                  $"Mot de passe: {GENERIC_PASSWORD}\n\n" +
                                  $"📞 Support: {SUPPORT_PHONE}",
                                  "Identifiants Incorrects",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                    return false;
                }

                Console.WriteLine("✅ Identifiants valides - Connexion autorisée");
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
            return "Administrateur";
        }

        public static string GetLicenseUsername()
        {
            return "admin";
        }

        public static string GetSupportPhone()
        {
            return SUPPORT_PHONE;
        }

        public static bool CheckLicenseExists()
        {
            // ✅ Retourner true car on utilise des identifiants fixes
            return true;
        }

        public static string GetLicenseInfo()
        {
            return $"🔑 IDENTIFIANTS PAR DÉFAUT\n\n" +
                   $"📋 INFORMATIONS DE CONNEXION :\n" +
                   $"Nom d'utilisateur: admin\n" +
                   $"Mot de passe: 12345\n\n" +
                   $"💡 Vous pouvez modifier ces identifiants dans le code\n" +
                   $"📞 Support: {SUPPORT_PHONE}";
        }
    }
}