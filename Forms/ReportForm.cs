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

        // Modern Colors (matching other forms)
        private Color PrimaryColor = Color.FromArgb(41, 128, 185);
        private Color SecondaryColor = Color.FromArgb(52, 152, 219);
        private Color SuccessColor = Color.FromArgb(39, 174, 96);
        private Color DangerColor = Color.FromArgb(231, 76, 60);
        private Color WarningColor = Color.FromArgb(230, 126, 34);
        private Color InfoColor = Color.FromArgb(52, 152, 219);
        private Color CardBackground = Color.FromArgb(248, 249, 250);
        private Color TextColor = Color.FromArgb(33, 37, 41);
        private Color HeaderColor = Color.FromArgb(52, 73, 94);
        private Color FilterPanelColor = Color.FromArgb(236, 240, 241);

        public ReportForm(ReportService reportService)
        {
            _reportService = reportService;
            
            this.Text = "Rapports Hebdomadaires";
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(240, 245, 249);
            this.Padding = new Padding(15);

            CreateModernControls();
            SubscribeToEvents();
            
            // Load data silently without showing messages during startup
            _ = LoadCurrentWeekReportSilent();
        }

        private void CreateModernControls()
        {
            // ==================== HEADER PANEL ====================
            var headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(this.Width, 70),
                BackColor = HeaderColor,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            var titleLabel = new Label
            {
                Text = "RAPPORTS HEBDOMADAIRES",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };
            headerPanel.Controls.Add(titleLabel);
            this.Controls.Add(headerPanel);

            // ==================== CONTROL PANEL ====================
            var controlPanel = new Panel
            {
                Location = new Point(15, 80),
                Size = new Size(this.Width - 30, 80),
                BackColor = FilterPanelColor,
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Create modern buttons
            btnCurrentWeek = CreateModernButton("📊 SEMAINE COURANTE", SuccessColor);
            btnCurrentWeek.Location = new Point(20, 20);
            btnCurrentWeek.Click += BtnCurrentWeek_Click;

            btnExportExcel = CreateModernButton("📈 EXPORTER EXCEL", InfoColor);
            btnExportExcel.Location = new Point(220, 20);
            btnExportExcel.Click += BtnExportExcel_Click;

            btnRefresh = CreateModernButton("🔄 ACTUALISER", WarningColor);
            btnRefresh.Location = new Point(420, 20);
            btnRefresh.Click += BtnRefresh_Click;

            controlPanel.Controls.Add(btnCurrentWeek);
            controlPanel.Controls.Add(btnExportExcel);
            controlPanel.Controls.Add(btnRefresh);
            this.Controls.Add(controlPanel);

            // ==================== SUMMARY PANEL ====================
            var summaryPanel = new Panel
            {
                Location = new Point(15, 170),
                Size = new Size(this.Width - 30, 60),
                BackColor = CardBackground,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            lblTotalEmployees = CreateSummaryLabel("Total employés: 0", PrimaryColor);
            lblTotalEmployees.Location = new Point(20, 15);

            lblTotalAdvances = CreateSummaryLabel("Total avances: 0,00 DH", DangerColor);
            lblTotalAdvances.Location = new Point(250, 15);

            lblTotalPenalties = CreateSummaryLabel("Total pénalités: 0,00 DH", WarningColor);
            lblTotalPenalties.Location = new Point(500, 15);

            lblTotalNetSalary = CreateSummaryLabel("Total salaires nets: 0,00 DH", SuccessColor);
            lblTotalNetSalary.Location = new Point(750, 15);

            summaryPanel.Controls.Add(lblTotalEmployees);
            summaryPanel.Controls.Add(lblTotalAdvances);
            summaryPanel.Controls.Add(lblTotalPenalties);
            summaryPanel.Controls.Add(lblTotalNetSalary);
            this.Controls.Add(summaryPanel);

            // ==================== STATUS PANEL ====================
            var statusPanel = new Panel
            {
                Location = new Point(15, 240),
                Size = new Size(this.Width - 30, 30),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            lblStatus = new Label
            {
                Text = "Prêt",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = InfoColor,
                Location = new Point(5, 5),
                AutoSize = true
            };
            statusPanel.Controls.Add(lblStatus);
            this.Controls.Add(statusPanel);

            // ==================== DATAGRIDVIEW ====================
            SetupModernDataGridView();
        }

        private void SetupModernDataGridView()
        {
            dgvReports = new DataGridView
            {
                Location = new Point(15, 280),
                Size = new Size(this.Width - 30, this.Height - 320),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                GridColor = Color.FromArgb(229, 229, 229),
                AllowUserToAddRows = false,
                RowHeadersVisible = false
            };

            this.Controls.Add(dgvReports);

            // Style des en-têtes de colonnes
            dgvReports.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5)
            };

            dgvReports.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvReports.ColumnHeadersHeight = 45;
            dgvReports.EnableHeadersVisualStyles = false;

            // Style des cellules
            dgvReports.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                ForeColor = TextColor,
                SelectionBackColor = Color.FromArgb(220, 237, 255),
                SelectionForeColor = TextColor,
                Padding = new Padding(3)
            };

            // Style des lignes alternées
            dgvReports.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Font = new Font("Segoe UI", 10F)
            };

            dgvReports.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvReports.RowTemplate.Height = 35;

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

            UpdateButtonStates();
        }

        private Button CreateModernButton(string text, Color backgroundColor)
        {
            return new Button
            {
                Text = text,
                Size = new Size(180, 40),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                BackColor = backgroundColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = {
                    BorderSize = 0,
                    MouseOverBackColor = ControlPaint.Light(backgroundColor),
                    MouseDownBackColor = ControlPaint.Dark(backgroundColor)
                },
                Cursor = Cursors.Hand
            };
        }

        private Label CreateSummaryLabel(string text, Color color)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = color,
                BackColor = Color.Transparent,
                AutoSize = true
            };
        }

        // Silent loading method to prevent startup messages
        private async Task LoadCurrentWeekReportSilent()
        {
            try
            {
                if (lblStatus != null)
                {
                    lblStatus.Text = "Chargement en cours...";
                    lblStatus.ForeColor = InfoColor;
                }

                _reportService = new ReportService(
                    DatabaseHelper.CreateNewContext(),
                    new EmployeService(DatabaseHelper.CreateNewContext()),
                    new AvanceService(DatabaseHelper.CreateNewContext()),
                    new AbsenceService(DatabaseHelper.CreateNewContext())
                );

                _currentReports = await _reportService.GenerateCurrentWeekReportAsync();
                
                if (dgvReports != null && dgvReports.IsHandleCreated)
                {
                    dgvReports.DataSource = _currentReports;
                    UpdateSummaryLabels();
                    UpdateButtonStates();
                    
                    if (lblStatus != null)
                    {
                        lblStatus.Text = $"Rapport chargé: {_currentReports?.Count ?? 0} employés";
                        lblStatus.ForeColor = SuccessColor;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log silently without showing message box on startup
                Console.WriteLine($"Erreur chargement initial: {ex.Message}");
                if (lblStatus != null)
                {
                    lblStatus.Text = "Erreur lors du chargement initial";
                    lblStatus.ForeColor = DangerColor;
                }
            }
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
                if (lblStatus != null)
                {
                    lblStatus.Text = "Chargement du rapport de la semaine courante...";
                    lblStatus.ForeColor = InfoColor;
                }

                _reportService = new ReportService(
                    DatabaseHelper.CreateNewContext(),
                    new EmployeService(DatabaseHelper.CreateNewContext()),
                    new AvanceService(DatabaseHelper.CreateNewContext()),
                    new AbsenceService(DatabaseHelper.CreateNewContext())
                );

                _currentReports = await _reportService.GenerateCurrentWeekReportAsync();
                
                if (dgvReports != null)
                {
                    dgvReports.DataSource = _currentReports;
                    UpdateSummaryLabels();
                    UpdateButtonStates();
                    
                    if (lblStatus != null)
                    {
                        lblStatus.Text = $"Rapport chargé: {_currentReports?.Count ?? 0} employés";
                        lblStatus.ForeColor = SuccessColor;
                    }
                }
            }
            catch (Exception ex)
            {
                // Only show message for user-initiated actions, not startup
                MessageBox.Show($"Erreur lors du chargement du rapport: {ex.Message}", "Erreur",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                              
                if (lblStatus != null)
                {
                    lblStatus.Text = "Erreur lors du chargement";
                    lblStatus.ForeColor = DangerColor;
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