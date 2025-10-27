using System;
using System.Windows.Forms;
using GestionEmployes.Utils;

namespace GestionEmployes.Forms
{
    public partial class ActivationForm : Form
    {
        public ActivationForm()
        {
            InitializeComponent();
        }

        private void ActivationForm_Load(object sender, EventArgs e)
        {
            LoadMachineId();
        }

        private void LoadMachineId()
        {
            try
            {
                string machineId = ActivationManager.GetMachineId();

                if (string.IsNullOrEmpty(machineId) || machineId.StartsWith("ERROR_"))
                {
                    lblMachineId.Text = "ID non disponible - Contactez le support: 0669286543";
                    lblMachineId.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblMachineId.Text = machineId;
                    lblMachineId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));

                    // Copie automatique
                    try
                    {
                        Clipboard.SetText(machineId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erreur copie automatique: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                lblMachineId.Text = "Erreur: " + ex.Message;
                lblMachineId.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            string key = txtActivationKey.Text.Trim();

            if (string.IsNullOrEmpty(key))
            {
                MessageBox.Show("Veuillez entrer une clé d'activation", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ActivationManager.Activate(key))
            {
                MessageBox.Show("Application activée avec succès!", "Succès",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Clé d'activation invalide!", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtActivationKey.Focus();
                txtActivationKey.SelectAll();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}