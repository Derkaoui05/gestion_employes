using Microsoft.Win32;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace GestionEmployes.Utils
{
    public static class ActivationManager
    {
        // ⚠️ IMPORTANT: This must EXACTLY match the Secret Salt in Key Manager!
        private const string SECRET_SALT = "OthmaneEmploye";
        private const string REGISTRY_PATH = @"Software\GestionEmployes";

        public static bool CheckActivation()
        {
            try
            {
                if (IsActivated())
                {
                    return true;
                }

                // Show activation form
                using (var activationForm = new Forms.ActivationForm())
                {
                    if (activationForm.ShowDialog() == DialogResult.OK)
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("L'application doit être activée pour fonctionner.",
                                      "Activation Requise",
                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur d'activation: {ex.Message}", "Erreur",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool IsActivated()
        {
            try
            {
                string storedKey = GetStoredKey();
                if (string.IsNullOrEmpty(storedKey))
                    return false;

                string machineId = GetMachineId();
                string expectedKey = GenerateKey(machineId);

                return storedKey == expectedKey;
            }
            catch
            {
                return false;
            }
        }

        public static string GetMachineId()
        {
            try
            {
                // Method 1: Try MAC Address
                string macAddress = GetMacAddress();
                if (!string.IsNullOrEmpty(macAddress) && macAddress != "000000000000")
                {
                    return macAddress;
                }

                // Method 2: Fallback - Computer name + User name
                string computerInfo = Environment.MachineName + "_" + Environment.UserName;

                // Create a consistent hash
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(computerInfo));
                    return BitConverter.ToString(hash).Replace("-", "").Substring(0, 16);
                }
            }
            catch (Exception ex)
            {
                // Ultimate fallback
                return "ERROR_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            }
        }

        private static string GetMacAddress()
        {
            try
            {
                var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                    .FirstOrDefault(nic => nic.OperationalStatus == OperationalStatus.Up &&
                                          nic.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                if (networkInterface != null)
                {
                    string mac = networkInterface.GetPhysicalAddress().ToString();
                    return !string.IsNullOrEmpty(mac) ? mac : "000000000000";
                }
                return "000000000000";
            }
            catch
            {
                return "000000000000";
            }
        }

        public static bool Activate(string activationKey)
        {
            try
            {
                string machineId = GetMachineId();
                string expectedKey = GenerateKey(machineId);

                // Debug: Show what's being compared
                MessageBox.Show($"Machine ID: {machineId}\nExpected Key: {expectedKey}\nYour Key: {activationKey}",
                              "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (activationKey == expectedKey)
                {
                    SaveKey(activationKey);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'activation: {ex.Message}", "Erreur",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // ✅ CETTE MÉTHODE DOIT ÊTRE IDENTIQUE À KeyGeneratorService
        private static string GenerateKey(string machineId)
        {
            string data = machineId + SECRET_SALT;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                string base64Hash = Convert.ToBase64String(hash);

                string rawKey = base64Hash.Replace("=", "").Replace("+", "").Replace("/", "");
                rawKey = rawKey.Substring(0, 20);

                return $"{rawKey.Substring(0, 4)}-{rawKey.Substring(4, 4)}-{rawKey.Substring(8, 4)}-{rawKey.Substring(12, 4)}";
            }
        }

        private static void SaveKey(string activationKey)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(REGISTRY_PATH))
                {
                    key.SetValue("ActivationKey", activationKey);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur sauvegarde clé: {ex.Message}", "Erreur",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static string GetStoredKey()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH))
                {
                    return key?.GetValue("ActivationKey") as string;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}