using System;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace GestionEmployes.Utils
{
    public class ActivationManager
    {
        private const string MasterKey = "KEY-GESTIONS-EMPLOYES";
        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GestionEmployes",
            "config.ini"
        );

        public static string GetMacAddress()
        {
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    if (adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        return adapter.GetPhysicalAddress().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur MAC: " + ex.Message);
            }
            return "UNKNOWN";
        }

        public static string GenerateSoftwareLicense(string appName, string version)
        {
            string input = $"{appName}_{version}_{DateTime.Now:yyyy}";
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hash).Substring(0, 24).ToUpper();
            }
        }

        public static string GenerateActivationKey(string macAddress, string softwareLicense)
        {
            string input = $"{macAddress}_{softwareLicense}";
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                string hashString = Convert.ToBase64String(hash)
                    .Replace("/", "").Replace("+", "").Substring(0, 32);
                return FormatKey(hashString);
            }
        }

        private static string FormatKey(string key)
        {
            StringBuilder formatted = new StringBuilder();
            for (int i = 0; i < key.Length; i += 4)
            {
                if (i > 0) formatted.Append("-");
                formatted.Append(key.Substring(i, Math.Min(4, key.Length - i)));
            }
            return formatted.ToString();
        }

        public static void SaveConfiguration(string macAddress, string softwareLicense, string activationKey)
        {
            try
            {
                string dir = Path.GetDirectoryName(ConfigPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                string config = $"SOFTWARE_LICENSE={softwareLicense}\n" +
                              $"ACTIVATION_KEY={activationKey}\n" +
                              $"MAC_ADDRESS={macAddress}\n" +
                              $"INSTALLATION_DATE={DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                              $"ACTIVATED=1";

                File.WriteAllText(ConfigPath, config);
                Console.WriteLine("✅ Configuration sauvegardée: " + ConfigPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Erreur sauvegarde: " + ex.Message);
            }
        }

        public static (string softwareLicense, string activationKey, string macAddress) LoadConfiguration()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string[] lines = File.ReadAllLines(ConfigPath);
                    string softwareLicense = "", activationKey = "", macAddress = "";

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("SOFTWARE_LICENSE=")) softwareLicense = line.Split('=')[1];
                        if (line.StartsWith("ACTIVATION_KEY=")) activationKey = line.Split('=')[1];
                        if (line.StartsWith("MAC_ADDRESS=")) macAddress = line.Split('=')[1];
                    }

                    return (softwareLicense, activationKey, macAddress);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Erreur chargement: " + ex.Message);
            }
            return ("", "", "");
        }

        // Générer la clé attendue pour cet ordinateur
        public static string GetExpectedActivationKey()
        {
            string macAddress = GetMacAddress();
            string softwareLicense = GenerateSoftwareLicense("GestionEmployes", "1.0");
            return GenerateActivationKey(macAddress, softwareLicense);
        }

        // Valider une clé saisie
        public static bool ValidateActivationKey(string enteredKey)
        {
            if (string.IsNullOrWhiteSpace(enteredKey)) return false;
            return enteredKey.Equals(MasterKey, StringComparison.OrdinalIgnoreCase);
        }

        // Première installation: demander la clé
        public static bool ShowActivationForm()
        {
            Form activationForm = new Form
            {
                Text = "Activation - GestionEmployes",
                Width = 500,
                Height = 280,
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ControlBox = false
            };

            Label labelInfo = new Label
            {
                Text = "Bienvenue! Veuillez entrer votre clé d'activation:",
                Location = new System.Drawing.Point(20, 20),
                Width = 450,
                Height = 40,
                AutoSize = false
            };

            Label labelKey = new Label
            {
                Text = "Clé d'activation:",
                Location = new System.Drawing.Point(20, 70),
                Width = 150
            };

            TextBox txtKey = new TextBox
            {
                Location = new System.Drawing.Point(20, 100),
                Width = 450,
                Font = new System.Drawing.Font("Courier New", 12)
            };

            Button btnActivate = new Button
            {
                Text = "Activer",
                Location = new System.Drawing.Point(200, 200),
                Width = 100,
                Height = 40
            };

            Button btnCancel = new Button
            {
                Text = "Annuler",
                Location = new System.Drawing.Point(310, 200),
                Width = 100,
                Height = 40
            };

            bool isActivated = false;

            btnActivate.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtKey.Text))
                {
                    MessageBox.Show("Veuillez entrer une clé d'activation!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (ValidateActivationKey(txtKey.Text))
                {
                    // Sauvegarder l'état activé (on peut stocker des placeholders pour compatibilité)
                    SaveConfiguration("UNIVERSAL", "UNIVERSAL", MasterKey);
                    MessageBox.Show("✅ Activation réussie!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    isActivated = true;
                    activationForm.Close();
                }
                else
                {
                    MessageBox.Show("❌ Clé d'activation invalide!", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtKey.Clear();
                    txtKey.Focus();
                }
            };

            btnCancel.Click += (s, e) =>
            {
                activationForm.Close();
            };

            activationForm.Controls.Add(labelInfo);
            activationForm.Controls.Add(labelKey);
            activationForm.Controls.Add(txtKey);
            activationForm.Controls.Add(btnActivate);
            activationForm.Controls.Add(btnCancel);

            activationForm.ShowDialog();
            return isActivated;
        }

        // Vérifier la configuration au démarrage
        public static bool CheckActivation()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var content = File.ReadAllText(ConfigPath);
                    if (content.Contains("ACTIVATED=1"))
                    {
                        Console.WriteLine("✅ Activation déjà effectuée");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ Erreur lecture activation: " + ex.Message);
            }

            Console.WriteLine("📝 Première installation - Demande d'activation (clé universelle)");
            return ShowActivationForm();
        }
    }
}