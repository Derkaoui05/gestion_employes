using GestionEmployes.Models;
using GestionEmployes.Services;
using GestionEmployes.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        private List<Employe> _employes;
        private List<Supplier> _suppliers;
        private List<Avance> _avances;
        private List<Absence> _absences;
        private List<Facture> _factures;

        private TabControl tabControl;
        private DataGridView dgvEmployesDashboard;
        private DataGridView dgvSuppliersDashboard;

        // Couleurs personnalisées
        private Color PrimaryColor = Color.FromArgb(41, 128, 185);
        private Color SuccessColor = Color.FromArgb(39, 174, 96);
        private Color DangerColor = Color.FromArgb(231, 76, 60);
        private Color WarningColor = Color.FromArgb(230, 126, 34);
        private Color CardBackground = Color.FromArgb(248, 249, 250);

        public DashboardForm(IEmployeService employeService, IAvanceService avanceService,
                               IAbsenceService absenceService, SupplierService supplierService,
                               FactureService factureService)
        {
            _employeService = employeService;
            _avanceService = avanceService;
            _absenceService = absenceService;
            _supplierService = supplierService;
            _factureService = factureService;

            InitializeDashboard();
            SubscribeToEvents();
            LoadData();
        }

        private void SubscribeToEvents()
        {
            // S'abonner à tous les événements de modification
            EventBus.EmployeAdded += (s, e) => RefreshData();
            EventBus.EmployeUpdated += (s, e) => RefreshData();
            EventBus.EmployeDeleted += (s, e) => RefreshData();

            EventBus.AvanceAdded += (s, e) => RefreshData();
            EventBus.AvanceDeleted += (s, e) => RefreshData();

            EventBus.AbsenceAdded += (s, e) => RefreshData();
            EventBus.AbsenceDeleted += (s, e) => RefreshData();

            EventBus.SupplierAdded += (s, e) => RefreshData();
            EventBus.SupplierUpdated += (s, e) => RefreshData();
            EventBus.SupplierDeleted += (s, e) => RefreshData();

            EventBus.FactureAdded += (s, e) => RefreshData();
            EventBus.FactureUpdated += (s, e) => RefreshData();
            EventBus.FactureDeleted += (s, e) => RefreshData();

            // S'abonner à l'événement générique
            EventBus.DataChanged += (s, e) => RefreshData();
        }

        private void UnsubscribeFromEvents()
        {
            // Se désabonner de tous les événements
            EventBus.EmployeAdded -= (s, e) => RefreshData();
            EventBus.EmployeUpdated -= (s, e) => RefreshData();
            EventBus.EmployeDeleted -= (s, e) => RefreshData();

            EventBus.AvanceAdded -= (s, e) => RefreshData();
            EventBus.AvanceDeleted -= (s, e) => RefreshData();

            EventBus.AbsenceAdded -= (s, e) => RefreshData();
            EventBus.AbsenceDeleted -= (s, e) => RefreshData();

            EventBus.SupplierAdded -= (s, e) => RefreshData();
            EventBus.SupplierUpdated -= (s, e) => RefreshData();
            EventBus.SupplierDeleted -= (s, e) => RefreshData();

            EventBus.FactureAdded -= (s, e) => RefreshData();
            EventBus.FactureUpdated -= (s, e) => RefreshData();
            EventBus.FactureDeleted -= (s, e) => RefreshData();

            EventBus.DataChanged -= (s, e) => RefreshData();
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
                Font = new Font("Segoe UI", 10F),
                ItemSize = new Size(200, 30)
            };

            // Onglet Employés
            var tabEmployes = new TabPage("📊 Tableau de Bord Employés");
            CreateEmployesDashboard(tabEmployes);
            tabControl.Controls.Add(tabEmployes);

            // Onglet Fournisseurs
            var tabSuppliers = new TabPage("🏢 Tableau de Bord Fournisseurs");
            CreateSuppliersDashboard(tabSuppliers);
            tabControl.Controls.Add(tabSuppliers);

            this.Controls.Add(tabControl);
        }

        private void CreateEmployesDashboard(TabPage tabPage)
        {
            tabPage.BackColor = Color.FromArgb(240, 245, 249);
            tabPage.Padding = new Padding(10);

            // Titre
            var titleLabel = new Label
            {
                Text = "Gestion des Employés - Vue d'Ensemble",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(10, 10),
                AutoSize = true
            };
            tabPage.Controls.Add(titleLabel);

            // Cartes de statistiques
            CreateEmployeStatsCards(tabPage);

            // DataGridView des employés
            dgvEmployesDashboard = new DataGridView
            {
                Location = new Point(10, 150),
                Size = new Size(1320, 500),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                Font = new Font("Segoe UI", 10F)
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

            // Colonnes
            dgvEmployesDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cin",
                HeaderText = "CIN",
                DataPropertyName = "Cin",
                Width = 120
            });

            dgvEmployesDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nom",
                HeaderText = "NOM",
                DataPropertyName = "Nom",
                Width = 150
            });

            dgvEmployesDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Prenom",
                HeaderText = "PRENOM",
                DataPropertyName = "Prenom",
                Width = 150
            });

            dgvEmployesDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Salaire",
                HeaderText = "SALAIRE",
                DataPropertyName = "Salaire",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvEmployesDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalAvances",
                HeaderText = "AVANCES TOTALES",
                DataPropertyName = "TotalAvances",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvEmployesDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalAbsences",
                HeaderText = "PÉNALITÉS TOTALES",
                DataPropertyName = "TotalAbsences",
                Width = 170,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvEmployesDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SalaireNet",
                HeaderText = "SALAIRE NET",
                DataPropertyName = "SalaireNet",
                Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                }
            });

            dgvEmployesDashboard.CellDoubleClick += DgvEmployesDashboard_CellDoubleClick;
            tabPage.Controls.Add(dgvEmployesDashboard);
        }

        private void CreateEmployeStatsCards(TabPage tabPage)
        {
            int startX = 10;
            int cardWidth = 320;
            int cardHeight = 80;
            int spacing = 10;

            // Carte Nombre d'employés
            var cardEmployes = CreateStatsCard("Nombre d'Employés", "0", PrimaryColor, startX, 60, cardWidth, cardHeight);
            tabPage.Controls.Add(cardEmployes);

            // Carte Total Avances
            var cardAvances = CreateStatsCard("Total Avances", "0 DH", DangerColor, startX + cardWidth + spacing, 60, cardWidth, cardHeight);
            tabPage.Controls.Add(cardAvances);

            // Carte Total Pénalités
            var cardPenalites = CreateStatsCard("Total Pénalités", "0 DH", WarningColor, startX + (cardWidth + spacing) * 2, 60, cardWidth, cardHeight);
            tabPage.Controls.Add(cardPenalites);

            // Carte Total Salaires Nets
            var cardSalaires = CreateStatsCard("Total Salaires Nets", "0 DH", SuccessColor, startX + (cardWidth + spacing) * 3, 60, cardWidth, cardHeight);
            tabPage.Controls.Add(cardSalaires);
        }

        private void CreateSuppliersDashboard(TabPage tabPage)
        {
            tabPage.BackColor = Color.FromArgb(240, 245, 249);
            tabPage.Padding = new Padding(10);

            // Titre
            var titleLabel = new Label
            {
                Text = "Gestion des Fournisseurs - Vue d'Ensemble",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(10, 10),
                AutoSize = true
            };
            tabPage.Controls.Add(titleLabel);

            // Cartes de statistiques
            CreateSupplierStatsCards(tabPage);

            // DataGridView des fournisseurs
            dgvSuppliersDashboard = new DataGridView
            {
                Location = new Point(10, 150),
                Size = new Size(1320, 500),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                Font = new Font("Segoe UI", 10F)
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

            // Colonnes
            dgvSuppliersDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ID",
                HeaderText = "ID",
                DataPropertyName = "ID",
                Width = 80
            });

            dgvSuppliersDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "NOM",
                DataPropertyName = "Name",
                Width = 250
            });

            dgvSuppliersDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Phone",
                HeaderText = "TÉLÉPHONE",
                DataPropertyName = "Phone",
                Width = 150
            });

            dgvSuppliersDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalFactures",
                HeaderText = "NOMBRE FACTURES",
                DataPropertyName = "TotalFactures",
                Width = 160,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvSuppliersDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalMontant",
                HeaderText = "MONTANT TOTAL",
                DataPropertyName = "TotalMontant",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvSuppliersDashboard.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalRestant",
                HeaderText = "RESTE À PAYER",
                DataPropertyName = "TotalRestant",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvSuppliersDashboard.CellDoubleClick += DgvSuppliersDashboard_CellDoubleClick;
            tabPage.Controls.Add(dgvSuppliersDashboard);
        }

        private void CreateSupplierStatsCards(TabPage tabPage)
        {
            int startX = 10;
            int cardWidth = 320;
            int cardHeight = 80;
            int spacing = 10;

            // Carte Nombre de fournisseurs
            var cardSuppliers = CreateStatsCard("Nombre de Fournisseurs", "0", PrimaryColor, startX, 60, cardWidth, cardHeight);
            tabPage.Controls.Add(cardSuppliers);

            // Carte Total Factures
            var cardFactures = CreateStatsCard("Nombre de Factures", "0", SuccessColor, startX + cardWidth + spacing, 60, cardWidth, cardHeight);
            tabPage.Controls.Add(cardFactures);

            // Carte Montant Total
            var cardMontant = CreateStatsCard("Montant Total Factures", "0 DH", WarningColor, startX + (cardWidth + spacing) * 2, 60, cardWidth, cardHeight);
            tabPage.Controls.Add(cardMontant);

            // Carte Reste à Payer
            var cardReste = CreateStatsCard("Reste à Payer", "0 DH", DangerColor, startX + (cardWidth + spacing) * 3, 60, cardWidth, cardHeight);
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
                Padding = new Padding(10)
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
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(10, 35),
                AutoSize = true
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);

            return card;
        }

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
                // On évite les MessageBox pour le rafraîchissement automatique
                Console.WriteLine($"Erreur lors du chargement des données: {ex.Message}");
            }
        }

        private void UpdateEmployesDashboard()
        {
            var employeData = new List<dynamic>();

            foreach (var employe in _employes)
            {
                decimal totalAvances = _avances.Where(a => a.EmployeCin == employe.Cin).Sum(a => a.Montant);
                decimal totalAbsences = _absences.Where(a => a.EmployeCin == employe.Cin).Sum(a => a.Penalite);
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

            dgvEmployesDashboard.DataSource = employeData;
        }

        private void UpdateSuppliersDashboard()
        {
            var supplierData = new List<dynamic>();

            foreach (var supplier in _suppliers)
            {
                var facturesSupplier = _factures.Where(f => f.SupplierId == supplier.ID).ToList();
                int totalFactures = facturesSupplier.Count;
                decimal totalMontant = facturesSupplier.Sum(f => f.Amount);
                decimal totalRestant = facturesSupplier.Sum(f => f.Amount - f.Advance);

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

            dgvSuppliersDashboard.DataSource = supplierData;
        }

        private void UpdateStatsCards()
        {
            // Mettre à jour les cartes Employés
            UpdateEmployeStatsCards();
            // Mettre à jour les cartes Fournisseurs
            UpdateSupplierStatsCards();
        }

        private void UpdateEmployeStatsCards()
        {
            var employesTab = tabControl.TabPages[0];

            // Nombre d'employés
            var cardEmployes = employesTab.Controls.OfType<Panel>().FirstOrDefault(p => p.Controls[0].Text == "Nombre d'Employés");
            if (cardEmployes != null)
                cardEmployes.Controls[1].Text = _employes.Count.ToString();

            // Total avances
            var cardAvances = employesTab.Controls.OfType<Panel>().FirstOrDefault(p => p.Controls[0].Text == "Total Avances");
            if (cardAvances != null)
                cardAvances.Controls[1].Text = _avances.Sum(a => a.Montant).ToString("N2") + " DH";

            // Total pénalités
            var cardPenalites = employesTab.Controls.OfType<Panel>().FirstOrDefault(p => p.Controls[0].Text == "Total Pénalités");
            if (cardPenalites != null)
                cardPenalites.Controls[1].Text = _absences.Sum(a => a.Penalite).ToString("N2") + " DH";

            // Total salaires nets
            var cardSalaires = employesTab.Controls.OfType<Panel>().FirstOrDefault(p => p.Controls[0].Text == "Total Salaires Nets");
            if (cardSalaires != null)
            {
                decimal totalSalaires = _employes.Sum(e => e.Salaire ?? 0);
                decimal totalAvances = _avances.Sum(a => a.Montant);
                decimal totalPenalites = _absences.Sum(a => a.Penalite);
                decimal totalNets = totalSalaires - totalAvances - totalPenalites;
                cardSalaires.Controls[1].Text = totalNets.ToString("N2") + " DH";
            }
        }

        private void UpdateSupplierStatsCards()
        {
            var suppliersTab = tabControl.TabPages[1];

            // Nombre de fournisseurs
            var cardSuppliers = suppliersTab.Controls.OfType<Panel>().FirstOrDefault(p => p.Controls[0].Text == "Nombre de Fournisseurs");
            if (cardSuppliers != null)
                cardSuppliers.Controls[1].Text = _suppliers.Count.ToString();

            // Nombre de factures
            var cardFactures = suppliersTab.Controls.OfType<Panel>().FirstOrDefault(p => p.Controls[0].Text == "Nombre de Factures");
            if (cardFactures != null)
                cardFactures.Controls[1].Text = _factures.Count.ToString();

            // Montant total
            var cardMontant = suppliersTab.Controls.OfType<Panel>().FirstOrDefault(p => p.Controls[0].Text == "Montant Total Factures");
            if (cardMontant != null)
                cardMontant.Controls[1].Text = _factures.Sum(f => f.Amount).ToString("N2") + " DH";

            // Reste à payer
            var cardReste = suppliersTab.Controls.OfType<Panel>().FirstOrDefault(p => p.Controls[0].Text == "Reste à Payer");
            if (cardReste != null)
            {
                decimal totalReste = _factures.Sum(f => f.Amount - f.Advance);
                cardReste.Controls[1].Text = totalReste.ToString("N2") + " DH";
            }
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