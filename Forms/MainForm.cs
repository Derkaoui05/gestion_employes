using GestionEmployes.Data;
using GestionEmployes.Services;
using GestionEmployes.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GestionEmployes.Forms
{
    public partial class MainForm : Form
    {
        private IEmployeService _employeService;
        private IAvanceService _avanceService;
        private IAbsenceService _absenceService;
        private ReportService _reportService;
        private SupplierService _supplierService;
        private FactureService _factureService;

        public MainForm()
        {
            InitializeComponent();
            Theme.Apply(this);
            SetupForm();
            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                InitializeServices();
                SetupTabPages();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeServices()
        {
            _employeService = new EmployeService(DatabaseHelper.CreateNewContext());
            _avanceService = new AvanceService(DatabaseHelper.CreateNewContext());
            _absenceService = new AbsenceService(DatabaseHelper.CreateNewContext());
            _reportService = new ReportService(
                 DatabaseHelper.CreateNewContext(),
                 new EmployeService(DatabaseHelper.CreateNewContext()), // Nouvelle instance
                 new AvanceService(DatabaseHelper.CreateNewContext()),  // Nouvelle instance
                 new AbsenceService(DatabaseHelper.CreateNewContext())  // Nouvelle instance
             );

            // 🔴 NOUVEL AJOUT - Services pour Suppliers et Factures
            _supplierService = new SupplierService();
            _factureService = new FactureService();
        }

        private void SetupForm()
        {
            this.Text = $"Système de Gestion des Employés - {LicenseManager.GetCustomerName()}";
            this.Size = new Size(1600, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Theme.BackgroundColor;

            Theme.StyleTabControl(tabControl);
            tabControl.BackColor = Theme.BackgroundColor;
            tabControl.Dock = DockStyle.Fill;
        }

        private void SetupTabPages()
        {
            if (tabControl == null) return;

            tabControl.TabPages.Clear();

            // ==================== TAB 1: UNIFIED EMPLOYEE MANAGEMENT ====================
            var unifiedEmployeeTab = new TabPage("👥 Gestion des Employés");
            try
            {
                var unifiedForm = new UnifiedEmployeeForm(_employeService, _avanceService, _absenceService)
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
                unifiedEmployeeTab.Controls.Add(unifiedForm);
                unifiedForm.Show();
            }
            catch (Exception ex)
            {
                unifiedEmployeeTab.Text = "Gestion Employés (Erreur)";
                AddErrorLabel(unifiedEmployeeTab, ex.Message);
            }

            tabControl.TabPages.Add(unifiedEmployeeTab);

            // ==================== TAB 2: UNIFIED SUPPLIER & FACTURE MANAGEMENT ====================
            var supplierDetailsTab = new TabPage("🏢 Fournisseurs & Factures");
            try
            {
                var supplierDetailsForm = new SupplierDetailsForm()
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
                supplierDetailsTab.Controls.Add(supplierDetailsForm);
                supplierDetailsForm.Show();
            }
            catch (Exception ex)
            {
                supplierDetailsTab.Text = "Fournisseurs & Factures (Erreur)";
                AddErrorLabel(supplierDetailsTab, ex.Message);
            }

            tabControl.TabPages.Add(supplierDetailsTab);

            // ==================== TAB 3: REPORTS ====================
            var rapportTab = new TabPage("📊 Rapports");
            try
            {
                var reportForm = new ReportForm(new ReportService(
                        DatabaseHelper.CreateNewContext(),
                        new EmployeService(DatabaseHelper.CreateNewContext()),
                        new AvanceService(DatabaseHelper.CreateNewContext()),
                        new AbsenceService(DatabaseHelper.CreateNewContext())
                    ))
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
                rapportTab.Controls.Add(reportForm);
                reportForm.Show();
            }
            catch (Exception ex)
            {
                rapportTab.Text = "Rapports (Erreur)";
                AddErrorLabel(rapportTab, ex.Message);
            }

            tabControl.TabPages.Add(rapportTab);


            var dashboardTab = new TabPage("📊 Tableau de Bord");
            try
            {
                var dashboardForm = new DashboardForm(_employeService, _avanceService, _absenceService, _supplierService, _factureService)
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
                dashboardTab.Controls.Add(dashboardForm);
                dashboardForm.Show();
            }
            catch (Exception ex)
            {
                dashboardTab.Text = "Tableau de Bord (Erreur)";
                AddErrorLabel(dashboardTab, ex.Message);
            }

            tabControl.TabPages.Add(dashboardTab);
        }

        private void AddErrorLabel(TabPage tab, string errorMessage)
        {
            var label = new Label
            {
                Text = $"Erreur de chargement:\n\n{errorMessage}",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            tab.Controls.Add(label);
        }
    }
}