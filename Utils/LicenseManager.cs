using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GestionEmployes.Utils
{
    public class LicenseManager
    {
        // Clé fixe de 16 caractères pour AES-128
        private const string SECRET_KEY = "MySecretKey123456"; // 16 chars exactement
        private const string ALGORITHM = "AES";

        public static bool ValidateCredentials(string username, string password)
        {
            try
            {
                {
                    Console.WriteLine($"=== Validation des identifiants ===");
                    Console.WriteLine($"Username saisi: '{username}'");
                    Console.WriteLine($"Password saisi: '{password}'");

                    var license = LoadLicense();
                    if (license == null)
                    {
                        Console.WriteLine("❌ Aucun fichier license trouvé");
                        return false;
                    }

                    var storedUsername = license["username"];
                    var encryptedPassword = license["password"];

                    Console.WriteLine($"Username stocké: '{storedUsername}'");
                    Console.WriteLine($"Password chiffré: '{encryptedPassword}'");

                    if (string.IsNullOrEmpty(storedUsername) || string.IsNullOrEmpty(encryptedPassword))
                    {
                        Console.WriteLine("❌ Données license invalides");
                        return false;
                    }

                    // Déchiffrer le mot de passe stocké
                    var decryptedPassword = Decrypt(encryptedPassword);
                    Console.WriteLine($"Password déchiffré: '{decryptedPassword}'");

                    // Valider les identifiants
                    bool usernameMatch = username.Equals(storedUsername);
                    bool passwordMatch = password.Equals(decryptedPassword);

                    Console.WriteLine($"Username match: {usernameMatch}");
                    Console.WriteLine($"Password match: {passwordMatch}");

                    return usernameMatch && passwordMatch;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur validation: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                return false;
            }
        }

        public static string GetCustomerName()
        {
            try
            {
                var license = LoadLicense();
                if (license != null)
                {
                    return license["customer_name"] ?? "Client";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lecture nom client: {ex.Message}");
            }
            return "Client";
        }

        private static System.Collections.Specialized.NameValueCollection LoadLicense()
        {
            try
            {
                var licensePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "license.dat");
                Console.WriteLine($"📁 Chemin license: {licensePath}");

                if (!File.Exists(licensePath))
                {
                    Console.WriteLine("❌ Fichier license.dat non trouvé!");
                    return null;
                }

                // Lire et afficher le contenu brut
                var rawContent = File.ReadAllText(licensePath);
                Console.WriteLine($"📄 Contenu brut du fichier:");
                Console.WriteLine(rawContent);

                var license = new System.Collections.Specialized.NameValueCollection();
                var lines = File.ReadAllLines(licensePath);

                foreach (var line in lines)
                {
                    Console.WriteLine($"📝 Ligne: '{line}'");
                    if (line.Contains("="))
                    {
                        // Prendre seulement la première occurrence de '='
                        int equalsIndex = line.IndexOf('=');
                        if (equalsIndex > 0)
                        {
                            string key = line.Substring(0, equalsIndex).Trim();
                            string value = line.Substring(equalsIndex + 1).Trim();
                            license[key] = value;
                            Console.WriteLine($"   ➤ {key} = {value}");
                        }
                    }
                }

                return license;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur chargement license: {ex.Message}");
                return null;
            }
        }

        public static string Encrypt(string plainText)
        {
            try
            {
                Console.WriteLine($"🔒 Chiffrement de: '{plainText}'");

                using (var aes = Aes.Create())
                {
                    // S'assurer que la clé fait exactement 16 bytes
                    byte[] keyBytes = Encoding.UTF8.GetBytes(SECRET_KEY);
                    if (keyBytes.Length != 16)
                    {
                        Array.Resize(ref keyBytes, 16);
                    }

                    aes.Key = keyBytes;
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var encryptor = aes.CreateEncryptor())
                    {
                        var plainBytes = Encoding.UTF8.GetBytes(plainText);
                        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                        string result = Convert.ToBase64String(encryptedBytes);
                        Console.WriteLine($"🔒 Résultat chiffré: '{result}'");
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur chiffrement: {ex.Message}");
                throw;
            }
        }

        private static string Decrypt(string encryptedText)
        {
            try
            {
                Console.WriteLine($"🔓 Déchiffrement de: '{encryptedText}'");

                using (var aes = Aes.Create())
                {
                    // S'assurer que la clé fait exactement 16 bytes
                    byte[] keyBytes = Encoding.UTF8.GetBytes(SECRET_KEY);
                    if (keyBytes.Length != 16)
                    {
                        Array.Resize(ref keyBytes, 16);
                    }

                    aes.Key = keyBytes;
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor())
                    {
                        var encryptedBytes = Convert.FromBase64String(encryptedText);
                        var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        string result = Encoding.UTF8.GetString(decryptedBytes);
                        Console.WriteLine($"🔓 Résultat déchiffré: '{result}'");
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur déchiffrement: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                throw;
            }
        }

        public static void GenerateLicense(string customerName, string username, string password)
        {
            try
            {
                Console.WriteLine($"=== Génération de la license ===");
                Console.WriteLine($"Customer: {customerName}");
                Console.WriteLine($"Username: {username}");
                Console.WriteLine($"Password: {password}");

                var licensePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "license.dat");

                // Chiffrer le mot de passe
                string encryptedPassword = Encrypt(password);
                Console.WriteLine($"Password chiffré: {encryptedPassword}");

                var content = new StringBuilder();
                content.AppendLine($"customer_name={customerName}");
                content.AppendLine($"username={username}");
                content.AppendLine($"password={encryptedPassword}");
                content.AppendLine($"license_date={DateTime.Now:yyyy-MM-dd}");

                File.WriteAllText(licensePath, content.ToString());

                Console.WriteLine($"✅ License générée: {licensePath}");

                // Vérifier la license
                if (File.Exists(licensePath))
                {
                    var verifyContent = File.ReadAllText(licensePath);
                    Console.WriteLine($"✅ Contenu vérifié:");
                    Console.WriteLine(verifyContent);

                    // Tester immédiatement la validation
                    Console.WriteLine($"🧪 Test de validation...");
                    bool test = ValidateCredentials(username, password);
                    Console.WriteLine($"🧪 Résultat test: {test}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur génération license: {ex.Message}");
                throw;
            }
        }

        // Méthode utilitaire pour recréer la license si nécessaire
        public static void RecreateLicense()
        {
            try
            {
                var licensePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "license.dat");
                if (File.Exists(licensePath))
                {
                    File.Delete(licensePath);
                    Console.WriteLine("🗑 Ancienne license supprimée");
                }

                GenerateLicense("Admin Company", "admin", "2025");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur recréation: {ex.Message}");
            }
        }
    }
}