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
    public partial class AbsenceForm : Form
    {
        private readonly IAbsenceService _absenceService;
        private readonly IEmployeService _employeService;
        private List<Absence> _absences;
        private List<Employe> _employes;
        private Absence _selectedAbsence;
        public AbsenceForm(IAbsenceService absenceService, IEmployeService employeService)
        {
            _absenceService = absenceService;
            _employeService = employeService;
            InitializeComponent();
            Theme.Apply(this);
            SetupForm();
            LoadDataAsync();
        }

        private void SetupForm()
        {
            // Setup DataGridView
            dgvAbsences.AutoGenerateColumns = false;
            dgvAbsences.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAbsences.MultiSelect = false;
            dgvAbsences.ReadOnly = true;

            // Add columns
            dgvAbsences.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                DataPropertyName = "Id",
                Width = 60
            });

            dgvAbsences.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EmployeNom",
                HeaderText = "Employé",
                DataPropertyName = "Employe",
                Width = 200
            });

            dgvAbsences.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EmployeCin",
                HeaderText = "CIN",
                DataPropertyName = "EmployeCin",
                Width = 100
            });

            dgvAbsences.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Penalite",
                HeaderText = "Pénalité",
                DataPropertyName = "Penalite",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            dgvAbsences.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DateAbsence",
                HeaderText = "Date",
                DataPropertyName = "DateAbsence",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });

            // Setup ComboBox
            cmbEmploye.DisplayMember = "ToString";
            cmbEmploye.ValueMember = "Cin";

            // Event handlers
            dgvAbsences.SelectionChanged += DgvAbsences_SelectionChanged;
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += BtnClear_Click;
            btnRefresh.Click += BtnRefresh_Click;
            // Enable/disable buttons when inputs change
            cmbEmploye.SelectedIndexChanged += CmbEmploye_SelectedIndexChanged;
            txtPenalite.TextChanged += TxtPenalite_TextChanged;

            // Initial state
            UpdateButtonStates();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                _absences = await _absenceService.GetAllAbsencesAsync();
                _employes = await _employeService.GetAllEmployesAsync();

                dgvAbsences.DataSource = _absences;
                cmbEmploye.DataSource = _employes;

                // Select first employee by default if available
                if (_employes != null && _employes.Count > 0)
                {
                    cmbEmploye.SelectedIndex = 0;
                }

                // Re-evaluate buttons now that data is loaded
                UpdateButtonStates();

                UpdateLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des données: {ex.Message}", "Erreur",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvAbsences_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAbsences.SelectedRows.Count > 0)
            {
                _selectedAbsence = dgvAbsences.SelectedRows[0].DataBoundItem as Absence;
                FillFormWithAbsence(_selectedAbsence);
                UpdateSelectedDetails(_selectedAbsence);
                UpdateButtonStates();
            }
        }

        private void FillFormWithAbsence(Absence absence)
        {
            if (absence == null) return;

            cmbEmploye.SelectedValue = absence.EmployeCin;
            txtPenalite.Text = absence.Penalite.ToString("N2");
            dtpDateAbsence.Value = absence.DateAbsence;
        }

        private void UpdateSelectedDetails(Absence absence)
        {
            if (absence == null)
            {
                lblSelectedId.Text = "-";
                lblSelectedPenalite.Text = "-";
                lblSelectedDate.Text = "-";
                return;
            }

            lblSelectedId.Text = absence.Id.ToString();
            lblSelectedPenalite.Text = absence.Penalite.ToString("N2") + " DH";
            lblSelectedDate.Text = absence.DateAbsence.ToString("dd/MM/yyyy");
        }

        private void ClearForm()
        {
            cmbEmploye.SelectedIndex = -1;
            txtPenalite.Clear();
            dtpDateAbsence.Value = DateTime.Today;
            _selectedAbsence = null;
            dgvAbsences.ClearSelection();
            UpdateButtonStates();
            UpdateSelectedDetails(null);
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = _selectedAbsence != null;
            bool hasValidData = cmbEmploye.SelectedItem != null &&
                               !string.IsNullOrWhiteSpace(txtPenalite.Text);

            btnAdd.Enabled = hasValidData && !hasSelection;
            btnUpdate.Enabled = hasValidData && hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private void UpdateLabels()
        {
            decimal total = _absences?.Sum(a => a.Penalite) ?? 0;
            int count = _absences?.Count ?? 0;

            lblTotal.Text = $"Total des pénalités: {total:N2} DH";
            lblCount.Text = $"Nombre d'absences: {count}";
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var absence = CreateAbsenceFromForm();
                await _absenceService.CreateAbsenceAsync(absence);
                await LoadDataAsync();
                ClearForm();
                MessageBox.Show("Absence ajoutée avec succès!", "Succès",
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
            if (_selectedAbsence == null) return;

            try
            {
                var absence = CreateAbsenceFromForm();
                absence.Id = _selectedAbsence.Id;
                await _absenceService.UpdateAbsenceAsync(absence);
                await LoadDataAsync();
                ClearForm();
                MessageBox.Show("Absence modifiée avec succès!", "Succès",
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
            if (_selectedAbsence == null) return;

            var result = MessageBox.Show(
                "Êtes-vous sûr de vouloir supprimer cette absence?",
                "Confirmation de suppression",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _absenceService.DeleteAbsenceAsync(_selectedAbsence.Id);
                    await LoadDataAsync();
                    ClearForm();
                    MessageBox.Show("Absence supprimée avec succès!", "Succès",
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
            await LoadDataAsync();
            MessageBox.Show("Données actualisées", "Information",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private Absence CreateAbsenceFromForm()
        {
            var selectedEmploye = cmbEmploye.SelectedItem as Employe;
            if (selectedEmploye == null)
            {
                throw new ArgumentException("Veuillez sélectionner un employé");
            }

            if (!decimal.TryParse(txtPenalite.Text.Trim(), out decimal penalite))
            {
                throw new ArgumentException("La pénalité doit être un nombre valide");
            }

            if (penalite < 0)
            {
                throw new ArgumentException("La pénalité ne peut pas être négative");
            }

            return new Absence(penalite, dtpDateAbsence.Value.Date, selectedEmploye);
        }

        // Event handlers for form validation
        private void CmbEmploye_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void TxtPenalite_TextChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }
    }
}
