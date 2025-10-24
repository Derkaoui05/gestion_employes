using GestionEmployes.Data;
using GestionEmployes.Services;
using GestionEmployes.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestionEmployes.Forms
{
    public partial class MainForm : Form
    {
        // SUPPRIMER le contexte partagé - chaque form aura son propre contexte
        // private ApplicationDbContext _context;

        // Garder les services mais ils seront créés avec leurs propres contextes
        private IEmployeService _employeService;
        private IAvanceService _avanceService;
        private IAbsenceService _absenceService;
        private ReportService _reportService;

        public MainForm()
        {
            Console.WriteLine("🚀 Début construction MainForm");

            InitializeComponent();
            Console.WriteLine("✅ InitializeComponent terminé");

            Theme.Apply(this);

            SetupForm();
            Console.WriteLine("✅ SetupForm terminé");

            // Utiliser l'événement Load pour initialiser les services
            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Console.WriteLine("📥 Début MainForm_Load");

            try
            {
                // Debug
                Console.WriteLine($"tabControl est null: {tabControl == null}");
                Console.WriteLine($"Nombre d'onglets initiaux: {tabControl?.TabPages?.Count ?? 0}");

                // Initialiser les services (version synchrone maintenant)
                InitializeServices();

                // Configurer les onglets
                SetupTabPages();

                Console.WriteLine($"✅ Nombre d'onglets finaux: {tabControl?.TabPages?.Count ?? 0}");
                Console.WriteLine("🎉 MainForm chargé avec succès");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERREUR dans MainForm_Load: {ex.Message}");
                MessageBox.Show($"Erreur lors du chargement: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeServices()
        {
            try
            {
                Console.WriteLine("🔧 Début InitializeServices");

                // Chaque service utilise son propre DbContext
                // On crée juste des instances, pas de connexion immédiate à la BD
                _employeService = new EmployeService(DatabaseHelper.CreateNewContext());
                _avanceService = new AvanceService(DatabaseHelper.CreateNewContext());
                _absenceService = new AbsenceService(DatabaseHelper.CreateNewContext());
                _reportService = new ReportService(
                    DatabaseHelper.CreateNewContext(),
                    new EmployeService(DatabaseHelper.CreateNewContext()), // Nouvelle instance
                    new AvanceService(DatabaseHelper.CreateNewContext()),  // Nouvelle instance
                    new AbsenceService(DatabaseHelper.CreateNewContext())  // Nouvelle instance
                );

                Console.WriteLine("✅ InitializeServices terminé - Services créés avec DbContext séparés");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur InitializeServices: {ex.Message}");
                throw;
            }
        }

        private void SetupForm()
        {
            Console.WriteLine("🎨 Configuration du formulaire...");

            this.Text = $"Système de Gestion des Employés - {LicenseManager.GetCustomerName()}";
            this.Size = new Size(1400, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Theme.BackgroundColor;

            // Style the tab control
            Theme.StyleTabControl(tabControl);
            tabControl.BackColor = Theme.BackgroundColor;

            Console.WriteLine("✅ Configuration du formulaire terminée");
        }

        private void SetupTabPages()
        {
            try
            {
                Console.WriteLine("📑 Début SetupTabPages");

                // Vérifier que le tabControl existe
                if (tabControl == null)
                {
                    Console.WriteLine("❌ tabControl est null!");
                    return;
                }

                // Chaque form crée ses propres services avec leurs propres DbContext
                var employeTab = CreateTabPage("👤 Employés",
                    () => new EmployeForm(new EmployeService(DatabaseHelper.CreateNewContext())));

                var avanceTab = CreateTabPage("💰 Avances",
                    () => new AvanceForm(
                        new AvanceService(DatabaseHelper.CreateNewContext()),
                        new EmployeService(DatabaseHelper.CreateNewContext()) // Nouvelle instance
                    ));

                var absenceTab = CreateTabPage("📅 Absences",
                    () => new AbsenceForm(
                        new AbsenceService(DatabaseHelper.CreateNewContext()),
                        new EmployeService(DatabaseHelper.CreateNewContext()) // Nouvelle instance
                    ));

                var rapportTab = CreateTabPage("📊 Rapports",
                    () => new ReportForm(new ReportService(
                        DatabaseHelper.CreateNewContext(),
                        new EmployeService(DatabaseHelper.CreateNewContext()),
                        new AvanceService(DatabaseHelper.CreateNewContext()),
                        new AbsenceService(DatabaseHelper.CreateNewContext())
                    )));

                // Onglet Quitter
                var quitTab = CreateQuitTab();

                // Add tabs to tab control
                tabControl.TabPages.Clear();
                tabControl.TabPages.AddRange(new TabPage[] { employeTab, avanceTab, absenceTab, rapportTab, quitTab });

                Console.WriteLine($"✅ {tabControl.TabPages.Count} onglets créés avec DbContext séparés");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur SetupTabPages: {ex.Message}");
                MessageBox.Show($"Erreur lors de la création des onglets: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private TabPage CreateTabPage(string title, Func<Form> formFactory)
        {
            var tabPage = new TabPage(title);

            try
            {
                var form = formFactory();

                // Configurer le form comme contenu de l'onglet
                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;
                form.Dock = DockStyle.Fill;

                // Ajouter le form à l'onglet
                tabPage.Controls.Add(form);
                form.Show();

                Console.WriteLine($"✅ Onglet '{title}' créé");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur création onglet '{title}': {ex.Message}");
                // Créer un onglet vide avec un message d'erreur
                tabPage.Text = $"{title} (Erreur)";
                var label = new Label
                {
                    Text = $"Erreur de chargement: {ex.Message}",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.Red
                };
                tabPage.Controls.Add(label);
            }

            return tabPage;
        }
        
        private TabPage CreateQuitTab()
        {
            var tabPage = new TabPage("🚪 Quitter");

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 3
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));

            var btnQuit = new Button
            {
                Text = "Quitter l'application",
                AutoSize = true,
                Anchor = AnchorStyles.None,
                Padding = new Padding(12)
            };
            btnQuit.Click += (s, e) =>
            {
                var result = MessageBox.Show(
                    "Êtes-vous sûr de vouloir quitter ?",
                    "Quitter",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            };

            var lbl = new Label
            {
                Text = "Voulez-vous quitter l'application ?",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };

            layout.Controls.Add(lbl, 0, 0);
            layout.SetColumnSpan(lbl, 3);
            layout.Controls.Add(btnQuit, 1, 1);

            tabPage.Controls.Add(layout);
            return tabPage;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                Console.WriteLine("🔚 Fermeture de l'application...");

                // NE PAS appeler DatabaseHelper.Close() - elle n'existe plus
                // Chaque DbContext se ferme automatiquement quand il n'est plus utilisé

                Console.WriteLine("✅ Fermeture MainForm terminée");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Erreur fermeture: {ex.Message}");
            }
            base.OnFormClosing(e);
        }

        // Méthode utilitaire pour tester un service rapidement
        private async Task TestServiceConnection()
        {
            try
            {
                Console.WriteLine("🔍 Test de connexion rapide...");

                // Créer un contexte temporaire juste pour le test
                using (var testContext = DatabaseHelper.CreateNewContext())
                {
                    var testService = new EmployeService(testContext);
                    var result = await testService.GetAllEmployesAsync();
                    Console.WriteLine($"✅ Test réussi: {result.Count} employé(s)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test échoué: {ex.Message}");
            }
        }
    }
}