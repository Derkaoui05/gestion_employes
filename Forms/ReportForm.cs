using GestionEmployes.Data;
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
        private ReportService _reportService;
        private List<WeeklyReport> _currentReports;

        // Handlers pour les événements (nécessaires pour la désinscription)
        private EventHandler<EmployeEventArgs> _employeAddedHandler;
        private EventHandler<EmployeEventArgs> _employeUpdatedHandler;
        private EventHandler<EmployeEventArgs> _employeDeletedHandler;
        private EventHandler<AvanceEventArgs> _avanceAddedHandler;
        private EventHandler<AvanceEventArgs> _avanceUpdatedHandler;
        private EventHandler<AvanceEventArgs> _avanceDeletedHandler;
        private EventHandler<AbsenceEventArgs> _absenceAddedHandler;
        private EventHandler<AbsenceEventArgs> _absenceUpdatedHandler;
        private EventHandler<AbsenceEventArgs> _absenceDeletedHandler;
        private EventHandler<GenericEventArgs> _dataChangedHandler;

        public ReportForm(ReportService reportService)
        {
            _reportService = reportService;
            InitializeComponent();
            ApplyCustomTheme();
            SetupForm();
            SubscribeToEvents();
            LoadCurrentWeekReport();
        }

        private void ApplyCustomTheme()
        {
            this.BackColor = Color.FromArgb(240, 245, 249);
            this.ForeColor = Color.FromArgb(33, 37, 41);
        }

        public void SetupForm()
        {
            this.Dock = DockStyle.Fill;
            this.AutoSize = false;

            // Configuration du DataGridView
            dgvReports.AutoGenerateColumns = false;
            dgvReports.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReports.MultiSelect = false;
            dgvReports.ReadOnly = true;
            dgvReports.Dock = DockStyle.Fill;
            dgvReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReports.BackgroundColor = Color.White;
            dgvReports.BorderStyle = BorderStyle.Fixed3D;
            dgvReports.GridColor = Color.FromArgb(229, 229, 229);

            // Style des en-têtes de colonnes
            dgvReports.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5)
            };

            dgvReports.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvReports.ColumnHeadersHeight = 50;

            // Style des cellules
            dgvReports.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(33, 37, 41),
                SelectionBackColor = Color.FromArgb(220, 237, 255),
                SelectionForeColor = Color.Black,
                Padding = new Padding(3)
            };

            // Style des lignes alternées
            dgvReports.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Font = new Font("Segoe UI", 10F)
            };

            dgvReports.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvReports.RowHeadersVisible = false;

            // Colonnes
            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cin",
                HeaderText = "CIN",
                DataPropertyName = "Cin",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nom",
                HeaderText = "NOM",
                DataPropertyName = "Nom",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft }
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Prenom",
                HeaderText = "PRENOM",
                DataPropertyName = "Prenom",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft }
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Salaire",
                HeaderText = "SALAIRE",
                DataPropertyName = "Salaire",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    ForeColor = Color.FromArgb(46, 125, 50)
                }
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalAvances",
                HeaderText = "AVANCES",
                DataPropertyName = "TotalAvances",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    ForeColor = Color.FromArgb(198, 40, 40)
                }
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalPenalites",
                HeaderText = "PENALITES",
                DataPropertyName = "TotalPenalites",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    ForeColor = Color.FromArgb(198, 40, 40)
                }
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NombreAbsences",
                HeaderText = "ABSENCES",
                DataPropertyName = "NombreAbsences",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    ForeColor = Color.FromArgb(198, 40, 40)
                }
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SalaireNet",
                HeaderText = "SALAIRE NET",
                DataPropertyName = "SalaireNet",
                Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 81, 40)
                }
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "WeekStart",
                HeaderText = "DEBUT SEMAINE",
                DataPropertyName = "WeekStart",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy",
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "WeekEnd",
                HeaderText = "FIN SEMAINE",
                DataPropertyName = "WeekEnd",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy",
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvReports.RowTemplate.Height = 40;
            dgvReports.Font = new Font("Segoe UI", 10.5F);

            // Style des boutons sans icônes
            ApplyButtonStyle(btnCurrentWeek, Color.FromArgb(41, 128, 185), "Semaine Courante");
            ApplyButtonStyle(btnExportExcel, Color.FromArgb(39, 174, 96), "Exporter Excel");
            ApplyButtonStyle(btnRefresh, Color.FromArgb(52, 152, 219), "Actualiser");

            // Style des labels de résumé
            ApplySummaryLabelStyle(lblTotalEmployees, Color.FromArgb(52, 152, 219));
            ApplySummaryLabelStyle(lblTotalAdvances, Color.FromArgb(231, 76, 60));
            ApplySummaryLabelStyle(lblTotalPenalties, Color.FromArgb(230, 126, 34));
            ApplySummaryLabelStyle(lblTotalNetSalary, Color.FromArgb(46, 204, 113));

            // Event handlers
            btnCurrentWeek.Click += BtnCurrentWeek_Click;
            btnExportExcel.Click += BtnExportExcel_Click;
            btnRefresh.Click += BtnRefresh_Click;

            // Alignement des boutons côte à côte
            ArrangeButtons();

            UpdateButtonStates();
        }

        private void ArrangeButtons()
        {
            // Positionner les boutons côte à côte avec espacement
            int buttonWidth = 180;
            int buttonHeight = 50;
            int spacing = 15;
            int startX = 20;
            int yPosition = 10;

            btnCurrentWeek.Location = new Point(startX, yPosition);
            btnCurrentWeek.Size = new Size(buttonWidth, buttonHeight);

            btnExportExcel.Location = new Point(startX + buttonWidth + spacing, yPosition);
            btnExportExcel.Size = new Size(buttonWidth, buttonHeight);

            btnRefresh.Location = new Point(startX + (buttonWidth + spacing) * 2, yPosition);
            btnRefresh.Size = new Size(buttonWidth, buttonHeight);
        }

        private void ApplyButtonStyle(Button button, Color backgroundColor, string text = null)
        {
            if (!string.IsNullOrEmpty(text))
                button.Text = text;

            button.BackColor = backgroundColor;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backgroundColor);
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backgroundColor);
            button.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.Size = new Size(180, 50);
            button.Padding = new Padding(10, 5, 10, 5);
        }

        private void ApplySummaryLabelStyle(Label label, Color color)
        {
            label.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label.ForeColor = color;
            label.BackColor = Color.Transparent;
            label.AutoSize = true;
            label.Padding = new Padding(5);
        }

        private async Task LoadCurrentWeekReport()
        {
            try
            {
                // Mettre à jour le statut sur le thread UI
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        lblStatus.Text = "Chargement du rapport de la semaine courante...";
                        lblStatus.ForeColor = Color.FromArgb(52, 152, 219);
                    }));
                }
                else
                {
                    lblStatus.Text = "Chargement du rapport de la semaine courante...";
                    lblStatus.ForeColor = Color.FromArgb(52, 152, 219);
                }

                // Créer un nouveau ReportService avec de nouveaux contextes pour s'assurer d'avoir les données fraîches
                // Cela évite les problèmes de cache d'Entity Framework
                _reportService = new ReportService(
                    DatabaseHelper.CreateNewContext(),
                    new EmployeService(DatabaseHelper.CreateNewContext()),
                    new AvanceService(DatabaseHelper.CreateNewContext()),
                    new AbsenceService(DatabaseHelper.CreateNewContext())
                );

                // Charger les données (peut être fait sur n'importe quel thread)
                _currentReports = await _reportService.GenerateCurrentWeekReportAsync();
                
                // Mettre à jour l'UI sur le thread UI
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        dgvReports.DataSource = _currentReports;
                        dgvReports.AutoResizeColumns();
                        UpdateSummaryLabels();
                        UpdateButtonStates();
                        lblStatus.Text = $"Rapport chargé: {_currentReports.Count} employés";
                        lblStatus.ForeColor = Color.FromArgb(39, 174, 96);
                    }));
                }
                else
                {
                    dgvReports.DataSource = _currentReports;
                    dgvReports.AutoResizeColumns();
                    UpdateSummaryLabels();
                    UpdateButtonStates();
                    lblStatus.Text = $"Rapport chargé: {_currentReports.Count} employés";
                    lblStatus.ForeColor = Color.FromArgb(39, 174, 96);
                }
            }
            catch (Exception ex)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show($"Erreur lors du chargement du rapport: {ex.Message}", "Erreur",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                        lblStatus.Text = "Erreur lors du chargement";
                        lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                    }));
                }
                else
                {
                    MessageBox.Show($"Erreur lors du chargement du rapport: {ex.Message}", "Erreur",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblStatus.Text = "Erreur lors du chargement";
                    lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                }
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

            if (!hasReports)
            {
                btnExportExcel.BackColor = Color.FromArgb(149, 165, 166);
                btnExportExcel.FlatAppearance.MouseOverBackColor = Color.FromArgb(149, 165, 166);
            }
            else
            {
                btnExportExcel.BackColor = Color.FromArgb(39, 174, 96);
                btnExportExcel.FlatAppearance.MouseOverBackColor = ControlPaint.Light(Color.FromArgb(39, 174, 96));
            }
        }

        private async void BtnCurrentWeek_Click(object sender, EventArgs e)
        {
            await LoadCurrentWeekReport();
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
                        lblStatus.ForeColor = Color.FromArgb(52, 152, 219);
                        btnExportExcel.Enabled = false;

                        bool success = await _reportService.ExportToExcelAsync(_currentReports, saveDialog.FileName);

                        if (success)
                        {
                            MessageBox.Show($"Rapport exporté avec succès vers:\n{saveDialog.FileName}", "Succès",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lblStatus.Text = "Export terminé avec succès";
                            lblStatus.ForeColor = Color.FromArgb(39, 174, 96);
                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de l'export du rapport", "Erreur",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                            lblStatus.Text = "Erreur lors de l'export";
                            lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de l'export: {ex.Message}", "Erreur",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                        lblStatus.Text = "Erreur lors de l'export";
                        lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                    }
                    finally
                    {
                        btnExportExcel.Enabled = true;
                        UpdateButtonStates();
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (dgvReports != null && dgvReports.Columns.Count > 0)
            {
                dgvReports.AutoResizeColumns();
            }
        }

        private void SubscribeToEvents()
        {
            // Créer les handlers une seule fois
            _employeAddedHandler = (s, e) => RefreshReport();
            _employeUpdatedHandler = (s, e) => RefreshReport();
            _employeDeletedHandler = (s, e) => RefreshReport();
            _avanceAddedHandler = (s, e) => RefreshReport();
            _avanceUpdatedHandler = (s, e) => RefreshReport();
            _avanceDeletedHandler = (s, e) => RefreshReport();
            _absenceAddedHandler = (s, e) => RefreshReport();
            _absenceUpdatedHandler = (s, e) => RefreshReport();
            _absenceDeletedHandler = (s, e) => RefreshReport();
            _dataChangedHandler = (s, e) => RefreshReport();

            // S'abonner à tous les événements de modification des employés, avances et absences
            EventBus.EmployeAdded += _employeAddedHandler;
            EventBus.EmployeUpdated += _employeUpdatedHandler;
            EventBus.EmployeDeleted += _employeDeletedHandler;

            EventBus.AvanceAdded += _avanceAddedHandler;
            EventBus.AvanceUpdated += _avanceUpdatedHandler;
            EventBus.AvanceDeleted += _avanceDeletedHandler;

            EventBus.AbsenceAdded += _absenceAddedHandler;
            EventBus.AbsenceUpdated += _absenceUpdatedHandler;
            EventBus.AbsenceDeleted += _absenceDeletedHandler;

            // S'abonner à l'événement générique
            EventBus.DataChanged += _dataChangedHandler;
        }

        private void UnsubscribeFromEvents()
        {
            // Se désabonner de tous les événements
            if (_employeAddedHandler != null) EventBus.EmployeAdded -= _employeAddedHandler;
            if (_employeUpdatedHandler != null) EventBus.EmployeUpdated -= _employeUpdatedHandler;
            if (_employeDeletedHandler != null) EventBus.EmployeDeleted -= _employeDeletedHandler;

            if (_avanceAddedHandler != null) EventBus.AvanceAdded -= _avanceAddedHandler;
            if (_avanceUpdatedHandler != null) EventBus.AvanceUpdated -= _avanceUpdatedHandler;
            if (_avanceDeletedHandler != null) EventBus.AvanceDeleted -= _avanceDeletedHandler;

            if (_absenceAddedHandler != null) EventBus.AbsenceAdded -= _absenceAddedHandler;
            if (_absenceUpdatedHandler != null) EventBus.AbsenceUpdated -= _absenceUpdatedHandler;
            if (_absenceDeletedHandler != null) EventBus.AbsenceDeleted -= _absenceDeletedHandler;

            if (_dataChangedHandler != null) EventBus.DataChanged -= _dataChangedHandler;
        }

        private async void RefreshReport()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(RefreshReport));
                return;
            }

            try
            {
                // Mettre à jour le statut pour indiquer le rafraîchissement automatique
                if (lblStatus != null)
                {
                    lblStatus.Text = "Mise à jour automatique...";
                    lblStatus.ForeColor = Color.FromArgb(52, 152, 219);
                }

                await LoadCurrentWeekReport();
            }
            catch (Exception ex)
            {
                // Log l'erreur silencieusement pour ne pas perturber l'utilisateur
                Console.WriteLine($"Erreur rafraîchissement automatique du rapport: {ex.Message}");
                if (lblStatus != null)
                {
                    lblStatus.Text = "Erreur lors de la mise à jour automatique";
                    lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            UnsubscribeFromEvents();
            base.OnFormClosed(e);
        }
    }
}