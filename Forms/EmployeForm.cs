using GestionEmployes.Models;
using GestionEmployes.Services;
using GestionEmployes.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestionEmployes.Forms
{
    public partial class EmployeForm : Form
    {
        private readonly IEmployeService _employeService;
        private List<Employe> _employes;
        private Employe _selectedEmploye;

        // Déclaration des contrôles
        private DataGridView dgvEmployes;
        private TextBox txtCin;
        private TextBox txtNom;
        private TextBox txtPrenom;
        private TextBox txtUtilisateur;
        private TextBox txtMotDePasse;
        private TextBox txtSalaire;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClear;
        private Button btnRefresh;
        private Label lblCin;
        private Label lblNom;
        private Label lblPrenom;
        private Label lblUtilisateur;
        private Label lblMotDePasse;
        private Label lblSalaire;

        public EmployeForm(IEmployeService employeService)
        {
            _employeService = employeService;
            InitializeComponent();
            Theme.Apply(this);
            SetupForm();
            LoadEmployesAsync();
        }
        // Événements pour notifier les autres formulaires
        public event EventHandler<EmployeEventArgs> EmployeCreated;
        public event EventHandler<EmployeEventArgs> EmployeUpdated;
        public event EventHandler<EmployeEventArgs> EmployeDeleted;

        // Classe pour transporter les données d'événement
        public class EmployeEventArgs : EventArgs
        {
            public Employe Employe { get; }
            public EmployeEventArgs(Employe employe)
            {
                Employe = employe;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Création des contrôles
            CreateControls();

            // Configuration du formulaire
            this.Text = "Gestion des Employés";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Theme.BackgroundColor;
            this.Padding = new Padding(20);

            this.ResumeLayout(false);
        }

        private void CreateControls()
        {
            // Header Panel
            var headerPanel = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(1140, 60),
                BackColor = Theme.CardBackground,
                Padding = new Padding(20, 15, 20, 15)
            };
            var headerLabel = new Label
            {
                Text = "👥 Gestion des Employés",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Theme.PrimaryColor,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            headerPanel.Controls.Add(headerLabel);
            this.Controls.Add(headerPanel);

            // DataGridView Card
            var gridCard = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(1140, 320),
                BackColor = Theme.CardBackground,
                Padding = new Padding(15)
            };
            
            dgvEmployes = new DataGridView
            {
                Location = new Point(15, 15),
                Size = new Size(1110, 290),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            gridCard.Controls.Add(dgvEmployes);
            this.Controls.Add(gridCard);

            // Form Card for inputs
            var formCard = new Panel
            {
                Location = new Point(20, 440),
                Size = new Size(1140, 250),
                BackColor = Theme.CardBackground,
                Padding = new Padding(30, 25, 30, 25)
            };

            var formTitle = new Label
            {
                Text = "Informations de l'employé",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Theme.TextColor,
                Location = new Point(30, 25),
                AutoSize = true
            };
            formCard.Controls.Add(formTitle);

            // Column 1
            int col1X = 30;
            int col2X = 400;
            int col3X = 770;
            int startY = 70;
            int rowHeight = 65;

            // CIN
            lblCin = new Label { Text = "CIN", Location = new Point(col1X, startY), Size = new Size(300, 20), Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Theme.TextColor };
            txtCin = new TextBox { Location = new Point(col1X, startY + 25), Size = new Size(320, 35), Font = new Font("Segoe UI", 10F) };

            // Nom
            lblNom = new Label { Text = "Nom", Location = new Point(col2X, startY), Size = new Size(300, 20), Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Theme.TextColor };
            txtNom = new TextBox { Location = new Point(col2X, startY + 25), Size = new Size(320, 35), Font = new Font("Segoe UI", 10F) };

            // Prénom
            lblPrenom = new Label { Text = "Prénom", Location = new Point(col3X, startY), Size = new Size(300, 20), Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Theme.TextColor };
            txtPrenom = new TextBox { Location = new Point(col3X, startY + 25), Size = new Size(320, 35), Font = new Font("Segoe UI", 10F) };

            // Row 2
            startY += rowHeight;

            // Utilisateur
            lblUtilisateur = new Label { Text = "Nom d'utilisateur", Location = new Point(col1X, startY), Size = new Size(300, 20), Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Theme.TextColor };
            txtUtilisateur = new TextBox { Location = new Point(col1X, startY + 25), Size = new Size(320, 35), Font = new Font("Segoe UI", 10F) };

            // Mot de passe
            lblMotDePasse = new Label { Text = "Mot de passe", Location = new Point(col2X, startY), Size = new Size(300, 20), Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Theme.TextColor };
            txtMotDePasse = new TextBox { Location = new Point(col2X, startY + 25), Size = new Size(320, 35), UseSystemPasswordChar = true, Font = new Font("Segoe UI", 10F) };

            // Salaire
            lblSalaire = new Label { Text = "Salaire (DH)", Location = new Point(col3X, startY), Size = new Size(300, 20), Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Theme.TextColor };
            txtSalaire = new TextBox { Location = new Point(col3X, startY + 25), Size = new Size(320, 35), Font = new Font("Segoe UI", 10F) };

            // Buttons
            int btnY = startY + 75;
            btnAdd = new Button { Text = "➕ Ajouter", Name = "btnAdd", Location = new Point(col1X, btnY), Size = new Size(140, 45) };
            btnUpdate = new Button { Text = "✏️ Modifier", Name = "btnUpdate", Location = new Point(col1X + 155, btnY), Size = new Size(140, 45) };
            btnDelete = new Button { Text = "🗑️ Supprimer", Name = "btnDelete", Location = new Point(col1X + 310, btnY), Size = new Size(140, 45) };
            btnClear = new Button { Text = "🧹 Vider", Name = "btnClear", Location = new Point(col1X + 465, btnY), Size = new Size(140, 45) };
            btnRefresh = new Button { Text = "🔄 Actualiser", Name = "btnRefresh", Location = new Point(col1X + 620, btnY), Size = new Size(140, 45) };

            // Add all controls to form card
            formCard.Controls.AddRange(new Control[]
            {
                lblCin, txtCin, lblNom, txtNom, lblPrenom, txtPrenom,
                lblUtilisateur, txtUtilisateur, lblMotDePasse, txtMotDePasse, lblSalaire, txtSalaire,
                btnAdd, btnUpdate, btnDelete, btnClear, btnRefresh
            });

            this.Controls.Add(formCard);
        }

        private void SetupForm()
        {
            // Setup DataGridView
            dgvEmployes.AutoGenerateColumns = false;
            dgvEmployes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEmployes.MultiSelect = false;
            dgvEmployes.ReadOnly = true;

            // Add columns
            dgvEmployes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cin",
                HeaderText = "CIN",
                DataPropertyName = "Cin",
                Width = 100
            });

            dgvEmployes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nom",
                HeaderText = "Nom",
                DataPropertyName = "Nom",
                Width = 150
            });

            dgvEmployes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Prenom",
                HeaderText = "Prénom",
                DataPropertyName = "Prenom",
                Width = 150
            });

            dgvEmployes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Utilisateur",
                HeaderText = "Utilisateur",
                DataPropertyName = "Utilisateur",
                Width = 120
            });

            dgvEmployes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Salaire",
                HeaderText = "Salaire",
                DataPropertyName = "Salaire",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            // Event handlers
            dgvEmployes.SelectionChanged += DgvEmployes_SelectionChanged;
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += BtnClear_Click;
            btnRefresh.Click += BtnRefresh_Click;

            // Text changed events for validation
            txtCin.TextChanged += (s, e) => UpdateButtonStates();
            txtNom.TextChanged += (s, e) => UpdateButtonStates();
            txtPrenom.TextChanged += (s, e) => UpdateButtonStates();
            txtUtilisateur.TextChanged += (s, e) => UpdateButtonStates();
            txtMotDePasse.TextChanged += (s, e) => UpdateButtonStates();

            // Initial state
            UpdateButtonStates();
        }

        private async Task LoadEmployesAsync()
        {
            try
            {
                _employes = await _employeService.GetAllEmployesAsync();
                dgvEmployes.DataSource = _employes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des employés: {ex.Message}\n{ex.InnerException?.Message}", "Erreur",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvEmployes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEmployes.SelectedRows.Count > 0)
            {
                _selectedEmploye = dgvEmployes.SelectedRows[0].DataBoundItem as Employe;
                FillFormWithEmploye(_selectedEmploye);
                UpdateButtonStates();
            }
        }

        private void FillFormWithEmploye(Employe employe)
        {
            if (employe == null) return;

            txtCin.Text = employe.Cin;
            txtNom.Text = employe.Nom;
            txtPrenom.Text = employe.Prenom;
            txtUtilisateur.Text = employe.Utilisateur;
            txtSalaire.Text = employe.Salaire?.ToString("N2") ?? "";

            txtCin.Enabled = false; // Disable CIN when updating
        }

        private void ClearForm()
        {
            txtCin.Clear();
            txtNom.Clear();
            txtPrenom.Clear();
            txtUtilisateur.Clear();
            txtMotDePasse.Clear();
            txtSalaire.Clear();

            txtCin.Enabled = true;
            _selectedEmploye = null;
            dgvEmployes.ClearSelection();
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = _selectedEmploye != null;
            bool hasValidData = !string.IsNullOrWhiteSpace(txtCin.Text) &&
                               !string.IsNullOrWhiteSpace(txtNom.Text) &&
                               !string.IsNullOrWhiteSpace(txtPrenom.Text) &&
                               !string.IsNullOrWhiteSpace(txtUtilisateur.Text) &&
                               !string.IsNullOrWhiteSpace(txtMotDePasse.Text);

            btnAdd.Enabled = hasValidData && !hasSelection;
            btnUpdate.Enabled = hasValidData && hasSelection;
            btnDelete.Enabled = hasSelection;
            btnClear.Enabled = true;
            btnRefresh.Enabled = true;
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var employe = CreateEmployeFromForm();
                var created = await _employeService.CreateEmployeAsync(employe);
                LoadEmployesAsync();
                ClearForm();

                // Notifier les autres formulaires
                try
                {
                    EmployeCreated?.Invoke(this, new EmployeEventArgs(created));
                }
                catch { /* Ignorer les erreurs de notification pour ne pas perturber l'UI */ }

                MessageBox.Show("Employé ajouté avec succès!", "Succès",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                var employe = CreateEmployeFromForm();
                employe.Cin = _selectedEmploye.Cin; // Keep original CIN
                var updated = await _employeService.UpdateEmployeAsync(employe);
                LoadEmployesAsync();

                // Notifier les autres formulaires
                try
                {
                    EmployeUpdated?.Invoke(this, new EmployeEventArgs(updated));
                }
                catch { /* Ignorer les erreurs de notification pour ne pas perturber l'UI */ }

                MessageBox.Show("Employé modifié avec succès!", "Succès",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedEmploye == null) return;

            var result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer l'employé {_selectedEmploye.Nom} {_selectedEmploye.Prenom}?",
                "Confirmation de suppression",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var employeToDelete = _selectedEmploye; // Garder une référence

                    // Notifier les autres formulaires avant la suppression
                    try
                    {
                        EmployeDeleted?.Invoke(this, new EmployeEventArgs(employeToDelete));
                    }
                    catch { /* Ignorer les erreurs de notification */ }

                    await _employeService.DeleteEmployeAsync(employeToDelete.Cin);
                    LoadEmployesAsync();
                    ClearForm();
                    MessageBox.Show("Employé supprimé avec succès!", "Succès",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur: {ex.Message}", "Erreur",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
      

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadEmployesAsync();
            MessageBox.Show("Tableau actualisé", "Information",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private Employe CreateEmployeFromForm()
        {
            if (!int.TryParse(txtMotDePasse.Text.Trim(), out int motDePasse))
            {
                throw new ArgumentException("Le mot de passe doit être un nombre valide.");
            }

            decimal? salaire = null;
            if (!string.IsNullOrWhiteSpace(txtSalaire.Text))
            {
                if (!decimal.TryParse(txtSalaire.Text.Trim(), out decimal salaireValue))
                {
                    throw new ArgumentException("Le salaire doit être un nombre valide.");
                }
                salaire = salaireValue;
            }

            return new Employe(
                txtCin.Text.Trim(),
                txtNom.Text.Trim(),
                txtPrenom.Text.Trim(),
                txtUtilisateur.Text.Trim(),
                salaire
            );
        }
    }
}