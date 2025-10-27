using GestionEmployes.Data;
using GestionEmployes.Forms;
using GestionEmployes.Utils;
using System;
using System.Windows.Forms;

namespace GestionEmployes
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Initialisation base de données
                DatabaseHelper.EnsureDatabaseCreated();

                // Vérification activation logiciel
                if (ActivationManager.CheckActivation())
                {
                    // Lancement application (login utilisateur)
                    Application.Run(new LoginForm());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}