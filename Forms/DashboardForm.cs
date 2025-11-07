using GestionEmployes.Models;
using GestionEmployes.Services;
using GestionEmployes.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestionEmployes.Forms
{
    public partial class DashboardForm : Form
    {
        private readonly IEmployeService _employeService;
        private readonly IAvanceService _avanceService;
        private readonly IAbsenceService _absenceService;
        private readonly SupplierService _supplierService;
        private readonly FactureService _factureService;
        private readonly DashboardService _dashboardService;

        private List<Employe> _employes;
        private List<Supplier> _suppliers;
        private List<Avance> _avances;
        private List<Absence> _absences;
        private List<Facture> _factures;

        // Stored event handlers to ensure proper subscribe/unsubscribe and live refresh
        private EventHandler<EmployeEventArgs> _employeAddedHandler;
        private EventHandler<EmployeEventArgs> _employeUpdatedHandler;
        private EventHandler<EmployeEventArgs> _employeDeletedHandler;

        private EventHandler<AvanceEventArgs> _avanceAddedHandler;
        private EventHandler<AvanceEventArgs> _avanceUpdatedHandler;
        private EventHandler<AvanceEventArgs> _avanceDeletedHandler;

        private EventHandler<AbsenceEventArgs> _absenceAddedHandler;
        private EventHandler<AbsenceEventArgs> _absenceUpdatedHandler;
        private EventHandler<AbsenceEventArgs> _absenceDeletedHandler;

        private EventHandler<SupplierEventArgs> _supplierAddedHandler;
        private EventHandler<SupplierEventArgs> _supplierUpdatedHandler;
        private EventHandler<SupplierEventArgs> _supplierDeletedHandler;

        private EventHandler<FactureEventArgs> _factureAddedHandler;
        private EventHandler<FactureEventArgs> _factureUpdatedHandler;
        private EventHandler<FactureEventArgs> _factureDeletedHandler;

        private EventHandler<GenericEventArgs> _dataChangedHandler;

        private TabControl tabControl;
        private DataGridView dgvEmployesDashboard;
        private DataGridView dgvSuppliersDashboard;

        // Controls de filtrage Employés
        private TextBox txtFilterEmployeName;
        private DateTimePicker dtpEmployeFrom;
        private DateTimePicker dtpEmployeTo;
        private Button btnFilterEmployes;
        private Button btnResetEmployesFilter;

        // Controls de filtrage Fournisseurs
        private TextBox txtFilterSupplierName;
        private DateTimePicker dtpSupplierFrom;
        private DateTimePicker dtpSupplierTo;
        private Button btnFilterSuppliers;
        private Button btnResetSuppliersFilter;

        // Couleurs personnalisées
        private Color PrimaryColor = Color.FromArgb(41, 128, 185);
        private Color SuccessColor = Color.FromArgb(39, 174, 96);
        private Color DangerColor = Color.FromArgb(231, 76, 60);
        private Color WarningColor = Color.FromArgb(230, 126, 34);
        private Color CardBackground = Color.FromArgb(248, 249, 250);
        private Color HeaderColor = Color.FromArgb(52, 73, 94);
        private Color FilterPanelColor = Color.FromArgb(236, 240, 241);

        public DashboardForm(IEmployeService employeService, IAvanceService avanceService,
                               IAbsenceService absenceService, SupplierService supplierService,
                               FactureService factureService, DashboardService dashboardService)
        {
            _employeService = employeService;
            _avanceService = avanceService;
            _absenceService = absenceService;
            _supplierService = supplierService;
            _factureService = factureService;
            _dashboardService = dashboardService;

            InitializeDashboard();
            SubscribeToEvents();
            LoadData();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Embedded forms sometimes don't trigger Shown; ensure a refresh on Load
            RefreshData();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // Ensure fresh data after app startup or when the form is first displayed
            RefreshData();
        }

        private void SubscribeToEvents()
        {
            _employeAddedHandler = (s, e) => RefreshData();
            _employeUpdatedHandler = (s, e) => RefreshData();
            _employeDeletedHandler = (s, e) => RefreshData();
            EventBus.EmployeAdded += _employeAddedHandler;
            EventBus.EmployeUpdated += _employeUpdatedHandler;
            EventBus.EmployeDeleted += _employeDeletedHandler;

            _avanceAddedHandler = (s, e) => RefreshData();
            _avanceUpdatedHandler = (s, e) => RefreshData();
            _avanceDeletedHandler = (s, e) => RefreshData();
            EventBus.AvanceAdded += _avanceAddedHandler;
            EventBus.AvanceUpdated += _avanceUpdatedHandler;
            EventBus.AvanceDeleted += _avanceDeletedHandler;

            _absenceAddedHandler = (s, e) => RefreshData();
            _absenceUpdatedHandler = (s, e) => RefreshData();
            _absenceDeletedHandler = (s, e) => RefreshData();
            EventBus.AbsenceAdded += _absenceAddedHandler;
            EventBus.AbsenceUpdated += _absenceUpdatedHandler;
            EventBus.AbsenceDeleted += _absenceDeletedHandler;

            _supplierAddedHandler = (s, e) => RefreshData();
            _supplierUpdatedHandler = (s, e) => RefreshData();
            _supplierDeletedHandler = (s, e) => RefreshData();
            EventBus.SupplierAdded += _supplierAddedHandler;
            EventBus.SupplierUpdated += _supplierUpdatedHandler;
            EventBus.SupplierDeleted += _supplierDeletedHandler;

            _factureAddedHandler = (s, e) => RefreshData();
            _factureUpdatedHandler = (s, e) => RefreshData();
            _factureDeletedHandler = (s, e) => RefreshData();
            EventBus.FactureAdded += _factureAddedHandler;
            EventBus.FactureUpdated += _factureUpdatedHandler;
            EventBus.FactureDeleted += _factureDeletedHandler;

            _dataChangedHandler = (s, e) => RefreshData();
            EventBus.DataChanged += _dataChangedHandler;
        }

        private void UnsubscribeFromEvents()
        {
            if (_employeAddedHandler != null) EventBus.EmployeAdded -= _employeAddedHandler;
            if (_employeUpdatedHandler != null) EventBus.EmployeUpdated -= _employeUpdatedHandler;
            if (_employeDeletedHandler != null) EventBus.EmployeDeleted -= _employeDeletedHandler;

            if (_avanceAddedHandler != null) EventBus.AvanceAdded -= _avanceAddedHandler;
            if (_avanceUpdatedHandler != null) EventBus.AvanceUpdated -= _avanceUpdatedHandler;
            if (_avanceDeletedHandler != null) EventBus.AvanceDeleted -= _avanceDeletedHandler;

            if (_absenceAddedHandler != null) EventBus.AbsenceAdded -= _absenceAddedHandler;
            if (_absenceUpdatedHandler != null) EventBus.AbsenceUpdated -= _absenceUpdatedHandler;
            if (_absenceDeletedHandler != null) EventBus.AbsenceDeleted -= _absenceDeletedHandler;

            if (_supplierAddedHandler != null) EventBus.SupplierAdded -= _supplierAddedHandler;
            if (_supplierUpdatedHandler != null) EventBus.SupplierUpdated -= _supplierUpdatedHandler;
            if (_supplierDeletedHandler != null) EventBus.SupplierDeleted -= _supplierDeletedHandler;

            if (_factureAddedHandler != null) EventBus.FactureAdded -= _factureAddedHandler;
            if (_factureUpdatedHandler != null) EventBus.FactureUpdated -= _factureUpdatedHandler;
            if (_factureDeletedHandler != null) EventBus.FactureDeleted -= _factureDeletedHandler;

            if (_dataChangedHandler != null) EventBus.DataChanged -= _dataChangedHandler;
        }

        private async void RefreshData()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(RefreshData));
                return;
            }

            try
            {
                await LoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur rafraîchissement automatique: {ex.Message}");
            }
        }

        private void InitializeDashboard()
        {
            this.Text = "Tableau de Bord - Gestion Employés & Fournisseurs";
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 245, 249);

            // Tab Control Principal
            tabControl = new TabControl
            {
                Location = new Point(10, 10),
                Size = new Size(1360, 740),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ItemSize = new Size(250, 35),
                Appearance = TabAppearance.FlatButtons
            };

            // Onglet Employés
            var tabEmployes = new TabPage("📊 TABLEAU DE BORD EMPLOYÉS");
            CreateEmployesDashboard(tabEmployes);
            tabControl.Controls.Add(tabEmployes);

            // Onglet Fournisseurs
            var tabSuppliers = new TabPage("🏢 TABLEAU DE BORD FOURNISSEURS");
            CreateSuppliersDashboard(tabSuppliers);
            tabControl.Controls.Add(tabSuppliers);

            this.Controls.Add(tabControl);
        }

        private void CreateEmployesDashboard(TabPage tabPage)
        {
            tabPage.BackColor = Color.FromArgb(240, 245, 249);
            tabPage.Padding = new Padding(15);

            // Header
            var headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1330, 80),
                BackColor = HeaderColor,
                Dock = DockStyle.Top
            };

            var titleLabel = new Label
            {
                Text = "GESTION DES EMPLOYÉS - VUE D'ENSEMBLE",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };
            headerPanel.Controls.Add(titleLabel);
            tabPage.Controls.Add(headerPanel);

            // Panel de Filtres
            var filterPanel = new Panel
            {
                Location = new Point(0, 80),
                Size = new Size(1330, 90),
                BackColor = FilterPanelColor,
                Dock = DockStyle.Top,
                BorderStyle = BorderStyle.FixedSingle
            };

            CreateEmployeFilters(filterPanel);
            tabPage.Controls.Add(filterPanel);

            // Cartes de statistiques
            CreateEmployeStatsCards(tabPage);

            // DataGridView des employés
            dgvEmployesDashboard = new DataGridView
            {
                Location = new Point(15, 300),
                Size = new Size(1310, 400),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                Font = new Font("Segoe UI", 10F),
                RowHeadersVisible = false
            };

            // Style du DataGridView
            dgvEmployesDashboard.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            dgvEmployesDashboard.ColumnHeadersHeight = 45;
            dgvEmployesDashboard.EnableHeadersVisualStyles = false;

            // Colonnes
            dgvEmployesDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cin",
                HeaderText = "CIN",
                DataPropertyName = "Cin",
                Width = 140,
                HeaderCell = new DataGridViewColumnHeaderCell { Style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } }
            });

            dgvEmployesDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nom",
                HeaderText = "NOM",
                DataPropertyName = "Nom",
                Width = 180,
                HeaderCell = new DataGridViewColumnHeaderCell { Style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } }
            });

            dgvEmployesDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Prenom",
                HeaderText = "PRÉNOM",
                DataPropertyName = "Prenom",
                Width = 180,
                HeaderCell = new DataGridViewColumnHeaderCell { Style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } }
            });

            dgvEmployesDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Salaire",
                HeaderText = "SALAIRE",
                DataPropertyName = "Salaire",
                Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvEmployesDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalAvances",
                HeaderText = "AVANCES TOTALES",
                DataPropertyName = "TotalAvances",
                Width = 170,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvEmployesDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalAbsences",
                HeaderText = "PÉNALITÉS TOTALES",
                DataPropertyName = "TotalAbsences",
                Width = 190,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvEmployesDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SalaireNet",
                HeaderText = "SALAIRE NET",
                DataPropertyName = "SalaireNet",
                Width = 160,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = PrimaryColor
                }
            });

            dgvEmployesDashboard.CellDoubleClick += DgvEmployesDashboard_CellDoubleClick;
            tabPage.Controls.Add(dgvEmployesDashboard);
        }

        private void CreateEmployeFilters(Panel panel)
        {
            int startY = 25;
            int controlWidth = 200;
            int controlHeight = 35;
            int spacing = 20;

            // Label Filtre Nom
            var lblName = new Label
            {
                Text = "NOM EMPLOYÉ:",
                Location = new Point(20, startY),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = HeaderColor
            };
            panel.Controls.Add(lblName);

            // TextBox Filtre Nom - PLUS GRAND
            txtFilterEmployeName = new TextBox
            {
                Location = new Point(150, startY - 5),
                Size = new Size(controlWidth, controlHeight),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            panel.Controls.Add(txtFilterEmployeName);

            // Label Date De
            var lblDateFrom = new Label
            {
                Text = "DU:",
                Location = new Point(370, startY),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = HeaderColor
            };
            panel.Controls.Add(lblDateFrom);

            // DateTimePicker De - PLUS GRAND
            dtpEmployeFrom = new DateTimePicker
            {
                Location = new Point(410, startY - 5),
                Size = new Size(controlWidth, controlHeight),
                Font = new Font("Segoe UI", 11F),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddMonths(-1)
            };
            panel.Controls.Add(dtpEmployeFrom);

            // Label Date À
            var lblDateTo = new Label
            {
                Text = "AU:",
                Location = new Point(630, startY),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = HeaderColor
            };
            panel.Controls.Add(lblDateTo);

            // DateTimePicker À - PLUS GRAND
            dtpEmployeTo = new DateTimePicker
            {
                Location = new Point(670, startY - 5),
                Size = new Size(controlWidth, controlHeight),
                Font = new Font("Segoe UI", 11F),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };
            panel.Controls.Add(dtpEmployeTo);

            // Bouton Filtrer - PLUS GRAND
            btnFilterEmployes = new Button
            {
                Text = "APPLIQUER FILTRES",
                Location = new Point(890, startY - 5),
                Size = new Size(150, controlHeight),
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat
            };
            btnFilterEmployes.FlatAppearance.BorderSize = 0;
            btnFilterEmployes.Click += BtnFilterEmployes_Click;
            panel.Controls.Add(btnFilterEmployes);

            // Bouton Réinitialiser - PLUS GRAND
            btnResetEmployesFilter = new Button
            {
                Text = "RÉINITIALISER",
                Location = new Point(1050, startY - 5),
                Size = new Size(150, controlHeight),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat
            };
            btnResetEmployesFilter.FlatAppearance.BorderSize = 0;
            btnResetEmployesFilter.Click += BtnResetEmployesFilter_Click;
            panel.Controls.Add(btnResetEmployesFilter);
        }

        private void CreateEmployeStatsCards(TabPage tabPage)
        {
            int startX = 15;
            int cardWidth = 320;
            int cardHeight = 100;
            int spacing = 15;

            // Carte Nombre d'employés
            var cardEmployes = CreateStatsCard("NOMBRE D'EMPLOYÉS", "0", PrimaryColor, startX, 180, cardWidth, cardHeight);
            cardEmployes.Name = "cardEmployesCount";
            tabPage.Controls.Add(cardEmployes);

            // Carte Total Avances
            var cardAvances = CreateStatsCard("TOTAL AVANCES", "0 DH", DangerColor, startX + cardWidth + spacing, 180, cardWidth, cardHeight);
            cardAvances.Name = "cardTotalAvances";
            tabPage.Controls.Add(cardAvances);

            // Carte Total Pénalités
            var cardPenalites = CreateStatsCard("TOTAL PÉNALITÉS", "0 DH", WarningColor, startX + (cardWidth + spacing) * 2, 180, cardWidth, cardHeight);
            cardPenalites.Name = "cardTotalPenalites";
            tabPage.Controls.Add(cardPenalites);

            // Carte Total Salaires Nets
            var cardSalaires = CreateStatsCard("TOTAL SALAIRES NETS", "0 DH", SuccessColor, startX + (cardWidth + spacing) * 3, 180, cardWidth, cardHeight);
            cardSalaires.Name = "cardTotalSalaires";
            tabPage.Controls.Add(cardSalaires);
        }

        private void CreateSuppliersDashboard(TabPage tabPage)
        {
            tabPage.BackColor = Color.FromArgb(240, 245, 249);
            tabPage.Padding = new Padding(15);

            // Header
            var headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1330, 80),
                BackColor = HeaderColor,
                Dock = DockStyle.Top
            };

            var titleLabel = new Label
            {
                Text = "GESTION DES FOURNISSEURS - VUE D'ENSEMBLE",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };
            headerPanel.Controls.Add(titleLabel);
            tabPage.Controls.Add(headerPanel);

            // Panel de Filtres
            var filterPanel = new Panel
            {
                Location = new Point(0, 80),
                Size = new Size(1330, 90),
                BackColor = FilterPanelColor,
                Dock = DockStyle.Top,
                BorderStyle = BorderStyle.FixedSingle
            };

            CreateSupplierFilters(filterPanel);
            tabPage.Controls.Add(filterPanel);

            // Cartes de statistiques
            CreateSupplierStatsCards(tabPage);

            // DataGridView des fournisseurs
            dgvSuppliersDashboard = new DataGridView
            {
                Location = new Point(15, 300),
                Size = new Size(1310, 400),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                Font = new Font("Segoe UI", 10F),
                RowHeadersVisible = false
            };

            // Style du DataGridView
            dgvSuppliersDashboard.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            dgvSuppliersDashboard.ColumnHeadersHeight = 45;
            dgvSuppliersDashboard.EnableHeadersVisualStyles = false;

            // Colonnes
            dgvSuppliersDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ID",
                HeaderText = "ID",
                DataPropertyName = "ID",
                Width = 80,
                HeaderCell = new DataGridViewColumnHeaderCell { Style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } }
            });

            dgvSuppliersDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "NOM DU FOURNISSEUR",
                DataPropertyName = "Name",
                Width = 280,
                HeaderCell = new DataGridViewColumnHeaderCell { Style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } }
            });

            dgvSuppliersDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Phone",
                HeaderText = "TÉLÉPHONE",
                DataPropertyName = "Phone",
                Width = 180,
                HeaderCell = new DataGridViewColumnHeaderCell { Style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } }
            });

            dgvSuppliersDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalFactures",
                HeaderText = "NOMBRE FACTURES",
                DataPropertyName = "TotalFactures",
                Width = 180,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvSuppliersDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalMontant",
                HeaderText = "MONTANT TOTAL",
                DataPropertyName = "TotalMontant",
                Width = 180,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvSuppliersDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalRestant",
                HeaderText = "RESTE À PAYER",
                DataPropertyName = "TotalRestant",
                Width = 180,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvSuppliersDashboard.CellDoubleClick += DgvSuppliersDashboard_CellDoubleClick;
            tabPage.Controls.Add(dgvSuppliersDashboard);
        }

        private void CreateSupplierFilters(Panel panel)
        {
            int startY = 25;
            int controlWidth = 200;
            int controlHeight = 35;
            int spacing = 20;

            // Label Filtre Nom
            var lblName = new Label
            {
                Text = "NOM FOURNISSEUR:",
                Location = new Point(20, startY),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = HeaderColor
            };
            panel.Controls.Add(lblName);

            // TextBox Filtre Nom - PLUS GRAND
            txtFilterSupplierName = new TextBox
            {
                Location = new Point(170, startY - 5),
                Size = new Size(controlWidth, controlHeight),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            panel.Controls.Add(txtFilterSupplierName);

            // Label Date De
            var lblDateFrom = new Label
            {
                Text = "DU:",
                Location = new Point(390, startY),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = HeaderColor
            };
            panel.Controls.Add(lblDateFrom);

            // DateTimePicker De - PLUS GRAND
            dtpSupplierFrom = new DateTimePicker
            {
                Location = new Point(430, startY - 5),
                Size = new Size(controlWidth, controlHeight),
                Font = new Font("Segoe UI", 11F),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddMonths(-1)
            };
            panel.Controls.Add(dtpSupplierFrom);

            // Label Date À
            var lblDateTo = new Label
            {
                Text = "AU:",
                Location = new Point(650, startY),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = HeaderColor
            };
            panel.Controls.Add(lblDateTo);

            // DateTimePicker À - PLUS GRAND
            dtpSupplierTo = new DateTimePicker
            {
                Location = new Point(690, startY - 5),
                Size = new Size(controlWidth, controlHeight),
                Font = new Font("Segoe UI", 11F),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };
            panel.Controls.Add(dtpSupplierTo);

            // Bouton Filtrer - PLUS GRAND
            btnFilterSuppliers = new Button
            {
                Text = "APPLIQUER FILTRES",
                Location = new Point(910, startY - 5),
                Size = new Size(150, controlHeight),
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat
            };
            btnFilterSuppliers.FlatAppearance.BorderSize = 0;
            btnFilterSuppliers.Click += BtnFilterSuppliers_Click;
            panel.Controls.Add(btnFilterSuppliers);

            // Bouton Réinitialiser - PLUS GRAND
            btnResetSuppliersFilter = new Button
            {
                Text = "RÉINITIALISER",
                Location = new Point(1070, startY - 5),
                Size = new Size(150, controlHeight),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat
            };
            btnResetSuppliersFilter.FlatAppearance.BorderSize = 0;
            btnResetSuppliersFilter.Click += BtnResetSuppliersFilter_Click;
            panel.Controls.Add(btnResetSuppliersFilter);
        }

        private void CreateSupplierStatsCards(TabPage tabPage)
        {
            int startX = 15;
            int cardWidth = 320;
            int cardHeight = 100;
            int spacing = 15;

            // Carte Nombre de fournisseurs
            var cardSuppliers = CreateStatsCard("NOMBRE DE FOURNISSEURS", "0", PrimaryColor, startX, 180, cardWidth, cardHeight);
            cardSuppliers.Name = "cardSuppliersCount";
            tabPage.Controls.Add(cardSuppliers);

            // Carte Total Factures
            var cardFactures = CreateStatsCard("NOMBRE DE FACTURES", "0", SuccessColor, startX + cardWidth + spacing, 180, cardWidth, cardHeight);
            cardFactures.Name = "cardTotalFactures";
            tabPage.Controls.Add(cardFactures);

            // Carte Montant Total
            var cardMontant = CreateStatsCard("MONTANT TOTAL FACTURES", "0 DH", WarningColor, startX + (cardWidth + spacing) * 2, 180, cardWidth, cardHeight);
            cardMontant.Name = "cardTotalMontant";
            tabPage.Controls.Add(cardMontant);

            // Carte Reste à Payer
            var cardReste = CreateStatsCard("RESTE À PAYER", "0 DH", DangerColor, startX + (cardWidth + spacing) * 3, 180, cardWidth, cardHeight);
            cardReste.Name = "cardTotalReste";
            tabPage.Controls.Add(cardReste);
        }

        private Panel CreateStatsCard(string title, string value, Color color, int x, int y, int width, int height)
        {
            var card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = CardBackground,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(10, 10),
                AutoSize = true
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(10, 35),
                AutoSize = true
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);

            return card;
        }

        // Les méthodes restantes (LoadData, UpdateEmployesDashboard, UpdateSuppliersDashboard, etc.)
        // restent identiques à votre code précédent...
        private async System.Threading.Tasks.Task LoadData()
        {
            try
            {
                _employes = await _employeService.GetAllEmployesAsync() ?? new List<Employe>();
                _suppliers = _supplierService.GetAllSuppliers() ?? new List<Supplier>();
                _avances = await _avanceService.GetAllAvancesAsync() ?? new List<Avance>();
                _absences = await _absenceService.GetAllAbsencesAsync() ?? new List<Absence>();
                _factures = _factureService.GetAllFactures() ?? new List<Facture>();

                UpdateEmployesDashboard();
                UpdateSuppliersDashboard();
                UpdateStatsCards();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des données: {ex.Message}");
            }
        }

        private async Task UpdateEmployesDashboard()
        {
            try
            {
                var employeData = new List<dynamic>();
                DateTime? startDate = dtpEmployeFrom.Checked ? dtpEmployeFrom.Value : (DateTime?)null;
                DateTime? endDate = dtpEmployeTo.Checked ? dtpEmployeTo.Value : (DateTime?)null;

                foreach (var employe in _employes)
                {
                    decimal totalAvances = 0;
                    decimal totalAbsences = 0;

                    if (startDate.HasValue && endDate.HasValue)
                    {
                        totalAvances = await _avanceService.GetTotalAvancesByEmployeAndDateRangeAsync(
                            employe.Cin, startDate.Value, endDate.Value);
                        totalAbsences = await _absenceService.GetTotalPenalitesByEmployeAndDateRangeAsync(
                            employe.Cin, startDate.Value, endDate.Value);
                    }
                    else
                    {
                        totalAvances = _avances.Where(a => a.EmployeCin == employe.Cin).Sum(a => a.Montant);
                        totalAbsences = _absences.Where(a => a.EmployeCin == employe.Cin).Sum(a => a.Penalite);
                    }

                    decimal salaireNet = (employe.Salaire ?? 0) - totalAvances - totalAbsences;

                    employeData.Add(new
                    {
                        employe.Cin,
                        employe.Nom,
                        employe.Prenom,
                        Salaire = employe.Salaire ?? 0,
                        TotalAvances = totalAvances,
                        TotalAbsences = totalAbsences,
                        SalaireNet = salaireNet
                    });
                }

                if (!string.IsNullOrEmpty(txtFilterEmployeName.Text))
                {
                    employeData = employeData.Where(e =>
                        e.Nom.ToLower().Contains(txtFilterEmployeName.Text.ToLower()) ||
                        e.Prenom.ToLower().Contains(txtFilterEmployeName.Text.ToLower())
                    ).ToList();
                }

                dgvEmployesDashboard.DataSource = employeData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour du dashboard employés: {ex.Message}");
            }
        }

        private void UpdateSuppliersDashboard()
        {
            try
            {
                var supplierData = new List<dynamic>();
                DateTime? startDate = dtpSupplierFrom.Checked ? dtpSupplierFrom.Value : (DateTime?)null;
                DateTime? endDate = dtpSupplierTo.Checked ? dtpSupplierTo.Value : (DateTime?)null;

                foreach (var supplier in _suppliers)
                {
                    var facturesSupplier = _factures.Where(f => f.SupplierId == supplier.ID).ToList();

                    if (startDate.HasValue && endDate.HasValue)
                    {
                        facturesSupplier = facturesSupplier.Where(f =>
                            f.InvoiceDate >= startDate.Value && f.InvoiceDate <= endDate.Value
                        ).ToList();
                    }

                    int totalFactures = facturesSupplier.Count;
                    decimal totalMontant = facturesSupplier.Sum(f => f.Amount);
                    decimal totalRestant = facturesSupplier.Sum(f => f.Remaining);

                    supplierData.Add(new
                    {
                        supplier.ID,
                        supplier.Name,
                        supplier.Phone,
                        TotalFactures = totalFactures,
                        TotalMontant = totalMontant,
                        TotalRestant = totalRestant
                    });
                }

                if (!string.IsNullOrEmpty(txtFilterSupplierName.Text))
                {
                    supplierData = supplierData.Where(s =>
                        s.Name.ToLower().Contains(txtFilterSupplierName.Text.ToLower())
                    ).ToList();
                }

                dgvSuppliersDashboard.DataSource = supplierData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour du dashboard fournisseurs: {ex.Message}");
            }
        }

        private async void UpdateStatsCards()
        {
            await UpdateEmployeStatsCards();
            UpdateSupplierStatsCards();
        }

        private async System.Threading.Tasks.Task UpdateEmployeStatsCards()
        {
            try
            {
                var employesTab = tabControl.TabPages[0];
                DateTime? startDate = dtpEmployeFrom.Checked ? dtpEmployeFrom.Value : (DateTime?)null;
                DateTime? endDate = dtpEmployeTo.Checked ? dtpEmployeTo.Value : (DateTime?)null;

                var filteredEmployes = _employes.AsEnumerable();
                if (!string.IsNullOrEmpty(txtFilterEmployeName.Text))
                {
                    filteredEmployes = filteredEmployes.Where(e =>
                        e.Nom.ToLower().Contains(txtFilterEmployeName.Text.ToLower()) ||
                        e.Prenom.ToLower().Contains(txtFilterEmployeName.Text.ToLower())
                    );
                }

                decimal totalAvances = 0;
                decimal totalPenalites = 0;

                if (startDate.HasValue && endDate.HasValue)
                {
                    foreach (var employe in filteredEmployes)
                    {
                        totalAvances += await _avanceService.GetTotalAvancesByEmployeAndDateRangeAsync(
                            employe.Cin, startDate.Value, endDate.Value);
                        totalPenalites += await _absenceService.GetTotalPenalitesByEmployeAndDateRangeAsync(
                            employe.Cin, startDate.Value, endDate.Value);
                    }
                }
                else
                {
                    var filteredEmployesList = filteredEmployes.ToList();
                    var employeCins = filteredEmployesList.Select(e => e.Cin).ToList();

                    totalAvances = _avances
                        .Where(a => employeCins.Contains(a.EmployeCin))
                        .Sum(a => a.Montant);

                    totalPenalites = _absences
                        .Where(a => employeCins.Contains(a.EmployeCin))
                        .Sum(a => a.Penalite);
                }

                decimal totalSalaires = filteredEmployes.Sum(e => e.Salaire ?? 0);
                decimal totalNets = totalSalaires - totalAvances - totalPenalites;

                UpdateCardValue(employesTab, "cardEmployesCount", filteredEmployes.Count().ToString());
                UpdateCardValue(employesTab, "cardTotalAvances", totalAvances.ToString("N2") + " DH");
                UpdateCardValue(employesTab, "cardTotalPenalites", totalPenalites.ToString("N2") + " DH");
                UpdateCardValue(employesTab, "cardTotalSalaires", totalNets.ToString("N2") + " DH");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour des statistiques employés: {ex.Message}");
            }
        }

        private void UpdateSupplierStatsCards()
        {
            try
            {
                var suppliersTab = tabControl.TabPages[1];
                DateTime? startDate = dtpSupplierFrom.Checked ? dtpSupplierFrom.Value : (DateTime?)null;
                DateTime? endDate = dtpSupplierTo.Checked ? dtpSupplierTo.Value : (DateTime?)null;

                var filteredSuppliers = _suppliers.AsEnumerable();
                var filteredFactures = _factures.AsEnumerable();

                if (!string.IsNullOrEmpty(txtFilterSupplierName.Text))
                {
                    filteredSuppliers = filteredSuppliers.Where(s =>
                        s.Name.ToLower().Contains(txtFilterSupplierName.Text.ToLower())
                    );
                    var filteredSupplierIds = filteredSuppliers.Select(s => s.ID).ToList();
                    filteredFactures = filteredFactures.Where(f => filteredSupplierIds.Contains(f.SupplierId));
                }

                if (startDate.HasValue && endDate.HasValue)
                {
                    filteredFactures = filteredFactures.Where(f =>
                        f.InvoiceDate >= startDate.Value && f.InvoiceDate <= endDate.Value
                    );
                }

                var filteredFacturesList = filteredFactures.ToList();

                int suppliersCount = filteredSuppliers.Count();
                int facturesCount = filteredFacturesList.Count();
                decimal totalMontant = filteredFacturesList.Sum(f => f.Amount);
                decimal totalReste = filteredFacturesList.Sum(f => f.Remaining);

                UpdateCardValue(suppliersTab, "cardSuppliersCount", suppliersCount.ToString());
                UpdateCardValue(suppliersTab, "cardTotalFactures", facturesCount.ToString());
                UpdateCardValue(suppliersTab, "cardTotalMontant", totalMontant.ToString("N2") + " DH");
                UpdateCardValue(suppliersTab, "cardTotalReste", totalReste.ToString("N2") + " DH");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour des statistiques fournisseurs: {ex.Message}");
            }
        }

        private void UpdateCardValue(TabPage tabPage, string cardName, string value)
        {
            var card = tabPage.Controls.OfType<Panel>().FirstOrDefault(p => p.Name == cardName);
            if (card != null && card.Controls.Count > 1)
            {
                card.Controls[1].Text = value;
            }
        }

        // Événements de filtrage
        private async void BtnFilterEmployes_Click(object sender, EventArgs e)
        {
            await UpdateEmployesDashboard();
            await UpdateEmployeStatsCards();
        }

        private async void BtnResetEmployesFilter_Click(object sender, EventArgs e)
        {
            txtFilterEmployeName.Text = string.Empty;
            dtpEmployeFrom.Value = DateTime.Today.AddMonths(-1);
            dtpEmployeTo.Value = DateTime.Today;
            dtpEmployeFrom.Checked = false;
            dtpEmployeTo.Checked = false;

            await UpdateEmployesDashboard();
            await UpdateEmployeStatsCards();
        }

        private void BtnFilterSuppliers_Click(object sender, EventArgs e)
        {
            UpdateSuppliersDashboard();
            UpdateSupplierStatsCards();
        }

        private void BtnResetSuppliersFilter_Click(object sender, EventArgs e)
        {
            txtFilterSupplierName.Text = string.Empty;
            dtpSupplierFrom.Value = DateTime.Today.AddMonths(-1);
            dtpSupplierTo.Value = DateTime.Today;
            dtpSupplierFrom.Checked = false;
            dtpSupplierTo.Checked = false;

            UpdateSuppliersDashboard();
            UpdateSupplierStatsCards();
        }

        private void DgvEmployesDashboard_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var cin = dgvEmployesDashboard.Rows[e.RowIndex].Cells["Cin"].Value.ToString();
                var employe = _employes.FirstOrDefault(emp => emp.Cin == cin);

                if (employe != null)
                {
                    var transactionsForm = new EmployeTransactionsForm(employe, _avances, _absences);
                    transactionsForm.ShowDialog();
                }
            }
        }

        private void DgvSuppliersDashboard_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var supplierId = (int)dgvSuppliersDashboard.Rows[e.RowIndex].Cells["ID"].Value;
                var supplier = _suppliers.FirstOrDefault(s => s.ID == supplierId);

                if (supplier != null)
                {
                    var facturesForm = new SupplierFacturesForm(supplier, _factures);
                    facturesForm.ShowDialog();
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