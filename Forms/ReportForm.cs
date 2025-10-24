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
    public partial class ReportForm : Form
    {
        private readonly ReportService _reportService;
        private List<WeeklyReport> _currentReports;

        public ReportForm(ReportService reportService)
        {
            _reportService = reportService;
            InitializeComponent();
            Theme.Apply(this);
            SetupForm();
            LoadCurrentWeekReport();
        }

        public void SetupForm()
        {
            // Setup DataGridView
            dgvReports.AutoGenerateColumns = false;
            dgvReports.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReports.MultiSelect = false;
            dgvReports.ReadOnly = true;

            // Add columns
            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cin",
                HeaderText = "CIN",
                DataPropertyName = "Cin",
                Width = 100
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nom",
                HeaderText = "Nom",
                DataPropertyName = "Nom",
                Width = 120
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Prenom",
                HeaderText = "Prénom",
                DataPropertyName = "Prenom",
                Width = 120
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Salaire",
                HeaderText = "Salaire",
                DataPropertyName = "Salaire",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalAvances",
                HeaderText = "Avances",
                DataPropertyName = "TotalAvances",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalPenalites",
                HeaderText = "Pénalités",
                DataPropertyName = "TotalPenalites",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NombreAbsences",
                HeaderText = "Absences",
                DataPropertyName = "NombreAbsences",
                Width = 80
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SalaireNet",
                HeaderText = "Salaire Net",
                DataPropertyName = "SalaireNet",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });
            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "WeekStart",
                HeaderText = "Début Semaine",
                DataPropertyName = "WeekStart",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "WeekEnd",
                HeaderText = "Fin Semaine",
                DataPropertyName = "WeekEnd",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });

            // Setup DatePickers
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Today;

            // Event handlers
            btnCurrentWeek.Click += BtnCurrentWeek_Click;
            btnCustomPeriod.Click += BtnCustomPeriod_Click;
            btnExportExcel.Click += BtnExportExcel_Click;
            btnRefresh.Click += BtnRefresh_Click;

            // Initial state
            UpdateButtonStates();
        }

        private async Task LoadCurrentWeekReport()
        {
            try
            {
                lblStatus.Text = "Chargement du rapport de la semaine courante...";
                _currentReports = await _reportService.GenerateCurrentWeekReportAsync();
                dgvReports.DataSource = _currentReports;
                UpdateSummaryLabels();
                lblStatus.Text = $"Rapport chargé: {_currentReports.Count} employés";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du rapport: {ex.Message}", "Erreur",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Erreur lors du chargement";
            }
        }

        private async Task LoadCustomPeriodReport()
        {
            try
            {
                var startDate = dtpStartDate.Value.Date;
                var endDate = dtpEndDate.Value.Date;

                if (startDate > endDate)
                {
                    MessageBox.Show("La date de début doit être antérieure à la date de fin", "Erreur",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                lblStatus.Text = "Chargement du rapport personnalisé...";
                _currentReports = await _reportService.GenerateWeeklyReportAsync(startDate, endDate);
                dgvReports.DataSource = _currentReports;
                UpdateSummaryLabels();
                lblStatus.Text = $"Rapport chargé: {_currentReports.Count} employés ({startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du rapport: {ex.Message}", "Erreur",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Erreur lors du chargement";
            }
        }
        private void UpdateSummaryLabels()
        {
            if (_currentReports == null || _currentReports.Count == 0)
            {
                lblTotalEmployees.Text = "Total employés: 0";
                lblTotalAdvances.Text = "Total avances: 0,00 DH";
                lblTotalPenalties.Text = "Total pénalités: 0,00 DH";
                lblTotalNetSalary.Text = "Total salaires nets: 0,00 DH";
                return;
            }

            int totalEmployees = _currentReports.Count;
            decimal totalAdvances = _currentReports.Sum(r => r.TotalAvances ?? 0);
            decimal totalPenalties = _currentReports.Sum(r => r.TotalPenalites ?? 0);
            decimal totalNetSalary = _currentReports.Sum(r => r.SalaireNet ?? 0);

            lblTotalEmployees.Text = $"Total employés: {totalEmployees}";
            lblTotalAdvances.Text = $"Total avances: {totalAdvances:N2} DH";
            lblTotalPenalties.Text = $"Total pénalités: {totalPenalties:N2} DH";
            lblTotalNetSalary.Text = $"Total salaires nets: {totalNetSalary:N2} DH";
        }

        private void UpdateButtonStates()
        {
            bool hasReports = _currentReports != null && _currentReports.Count > 0;
            btnExportExcel.Enabled = hasReports;
        }

        private async void BtnCurrentWeek_Click(object sender, EventArgs e)
        {
            await LoadCurrentWeekReport();
            UpdateButtonStates();
        }

        private async void BtnCustomPeriod_Click(object sender, EventArgs e)
        {
            await LoadCustomPeriodReport();
            UpdateButtonStates();
        }
        private async void BtnExportExcel_Click(object sender, EventArgs e)
        {
            if (_currentReports == null || _currentReports.Count == 0)
            {
                MessageBox.Show("Aucun rapport à exporter", "Information",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Fichiers Excel (*.xlsx)|*.xlsx|Tous les fichiers (*.*)|*.*";
                saveDialog.FilterIndex = 1;
                saveDialog.FileName = $"Rapport_Employes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        lblStatus.Text = "Export en cours...";
                        btnExportExcel.Enabled = false;

                        bool success = await _reportService.ExportToExcelAsync(_currentReports, saveDialog.FileName);

                        if (success)
                        {
                            MessageBox.Show($"Rapport exporté avec succès vers:\n{saveDialog.FileName}", "Succès",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lblStatus.Text = "Export terminé avec succès";
                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de l'export du rapport", "Erreur",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                            lblStatus.Text = "Erreur lors de l'export";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de l'export: {ex.Message}", "Erreur",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                        lblStatus.Text = "Erreur lors de l'export";
                    }
                    finally
                    {
                        btnExportExcel.Enabled = true;
                    }
                }
            }
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadCurrentWeekReport();
            UpdateButtonStates();
            MessageBox.Show("Rapport actualisé", "Information",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        // Event handlers for date validation
        private void DtpStartDate_ValueChanged(object sender, EventArgs e)
        {
            // Ensure start date is not after end date
            if (dtpStartDate.Value > dtpEndDate.Value)
            {
                dtpEndDate.Value = dtpStartDate.Value;
            }
        }

        private void DtpEndDate_ValueChanged(object sender, EventArgs e)
        {
            // Ensure end date is not before start date
            if (dtpEndDate.Value < dtpStartDate.Value)
            {
                dtpStartDate.Value = dtpEndDate.Value;
            }
        }
    }
}
