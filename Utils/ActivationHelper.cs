using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GestionEmployes.Utils
{
    public class ActivationHelper
    {
        private const string SECRET_SALT = "GestionEmployesSecret2024"; // CHANGE THIS!
        private const string REGISTRY_PATH = @"Software\GestionEmployes";

        public static string GetMachineId()
        {
            try
            {
                var macAddress = NetworkInterface
                    .GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .FirstOrDefault();

                return macAddress ?? "000000000000";
            }
            catch
            {
                return "000000000000";
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

        public static bool Activate(string activationKey)
        {
            try
            {
                string machineId = GetMachineId();
                string expectedKey = GenerateKey(machineId);

                if (activationKey == expectedKey)
                {
                    SaveKey(activationKey);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static void Deactivate()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(REGISTRY_PATH))
                {
                    key.DeleteValue("ActivationKey", false);
                }
            }
            catch { }
        }

        // For you to generate keys
        public static string GenerateKey(string machineId)
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
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(REGISTRY_PATH))
            {
                key.SetValue("ActivationKey", activationKey);
            }
        }

        private static string GetStoredKey()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH))
            {
                return key?.GetValue("ActivationKey") as string;
            }
        }

    }
}
