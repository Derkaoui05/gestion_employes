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
    public partial class AvanceForm : Form
    {
        private readonly IAvanceService _avanceService;
        private readonly IEmployeService _employeService;
        private List<Avance> _avances;
        private List<Employe> _employes;
        private Avance _selectedAvance;

        public AvanceForm(IAvanceService avanceService, IEmployeService employeService)
        {
            _avanceService = avanceService;
            _employeService = employeService;
            InitializeComponent();
            Theme.Apply(this);
            SetupForm();
            LoadDataAsync();
        }
     
        private void SetupForm()
        {
            // Setup DataGridView
            dgvAvances.AutoGenerateColumns = false;
            dgvAvances.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAvances.MultiSelect = false;
            dgvAvances.ReadOnly = true;

            // Add columns
            dgvAvances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                DataPropertyName = "Id",
                Width = 60
            });

            dgvAvances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EmployeNom",
                HeaderText = "Employé",
                DataPropertyName = "Employe",
                Width = 200
            });

            dgvAvances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EmployeCin",
                HeaderText = "CIN",
                DataPropertyName = "EmployeCin",
                Width = 100
            });

            dgvAvances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Montant",
                HeaderText = "Montant",
                DataPropertyName = "Montant",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            dgvAvances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DateAvance",
                HeaderText = "Date",
                DataPropertyName = "DateAvance",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });

            // Setup ComboBox
            cmbEmploye.DisplayMember = "ToString";
            cmbEmploye.ValueMember = "Cin";

            // Event handlers
            dgvAvances.SelectionChanged += DgvAvances_SelectionChanged;
            cmbEmploye.SelectedIndexChanged += CmbEmploye_SelectedIndexChanged;
            txtMontant.TextChanged += TxtMontant_TextChanged;
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += BtnClear_Click;
            btnRefresh.Click += BtnRefresh_Click;

            // Initial state
            UpdateButtonStates();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                _avances = await _avanceService.GetAllAvancesAsync();
                _employes = await _employeService.GetAllEmployesAsync();

                dgvAvances.DataSource = _avances;
                cmbEmploye.DataSource = _employes;

                UpdateTotalLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des données: {ex.Message}\n{ex.InnerException?.Message}", "Erreur",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvAvances_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAvances.SelectedRows.Count > 0)
            {
                _selectedAvance = dgvAvances.SelectedRows[0].DataBoundItem as Avance;
                FillFormWithAvance(_selectedAvance);
                UpdateSelectedDetails(_selectedAvance);
                UpdateButtonStates();
            }
        }

        private void FillFormWithAvance(Avance avance)
        {
            if (avance == null) return;

            cmbEmploye.SelectedValue = avance.EmployeCin;
            txtMontant.Text = avance.Montant.ToString("N2");
            dtpDateAvance.Value = avance.DateAvance;
        }

        private void UpdateSelectedDetails(Avance avance)
        {
            if (avance == null)
            {
                lblSelectedId.Text = "-";
                lblSelectedMontant.Text = "-";
                lblSelectedDate.Text = "-";
                return;
            }

            lblSelectedId.Text = avance.Id.ToString();
            lblSelectedMontant.Text = avance.Montant.ToString("N2") + " DH";
            lblSelectedDate.Text = avance.DateAvance.ToString("dd/MM/yyyy");
        }

        private void ClearForm()
        {
            cmbEmploye.SelectedIndex = -1;
            txtMontant.Clear();
            dtpDateAvance.Value = DateTime.Today;
            _selectedAvance = null;
            dgvAvances.ClearSelection();
            UpdateButtonStates();
            UpdateSelectedDetails(null);
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = _selectedAvance != null;
            bool hasValidData = cmbEmploye.SelectedItem != null &&
                               !string.IsNullOrWhiteSpace(txtMontant.Text);

            btnAdd.Enabled = hasValidData && !hasSelection;
            btnUpdate.Enabled = hasValidData && hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private void UpdateTotalLabel()
        {
            decimal total = _avances?.Sum(a => a.Montant) ?? 0;
            lblTotal.Text = $"Total des avances: {total:N2} DH";
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var avance = CreateAvanceFromForm();
                await _avanceService.CreateAvanceAsync(avance);
                await LoadDataAsync();
                ClearForm();
                MessageBox.Show("Avance ajoutée avec succès!", "Succès",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}\n{ex.InnerException?.Message}", "Erreur",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedAvance == null) return;

            try
            {
                var avance = CreateAvanceFromForm();
                avance.Id = _selectedAvance.Id;
                await _avanceService.UpdateAvanceAsync(avance);
                await LoadDataAsync();
                ClearForm();
                MessageBox.Show("Avance modifiée avec succès!", "Succès",
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
            if (_selectedAvance == null) return;

            var result = MessageBox.Show(
                "Êtes-vous sûr de vouloir supprimer cette avance?",
                "Confirmation de suppression",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _avanceService.DeleteAvanceAsync(_selectedAvance.Id);
                    await LoadDataAsync();
                    ClearForm();
                    MessageBox.Show("Avance supprimée avec succès!", "Succès",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur: {ex.Message}\n{ex.InnerException?.Message}", "Erreur",
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
            await LoadDataAsync();
            MessageBox.Show("Données actualisées", "Information",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private Avance CreateAvanceFromForm()
        {
            var selectedEmploye = cmbEmploye.SelectedItem as Employe;
            if (selectedEmploye == null)
            {
                throw new ArgumentException("Veuillez sélectionner un employé");
            }

            if (!decimal.TryParse(txtMontant.Text.Trim(), out decimal montant))
            {
                throw new ArgumentException("Le montant doit être un nombre valide");
            }

            if (montant <= 0)
            {
                throw new ArgumentException("Le montant doit être positif");
            }

            return new Avance(montant, dtpDateAvance.Value.Date, selectedEmploye);
        }

        // Event handlers for form validation
        private void CmbEmploye_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void TxtMontant_TextChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }
    }
}
