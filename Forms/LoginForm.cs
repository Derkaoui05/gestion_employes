using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using GestionEmployes.Utils;

namespace GestionEmployes.Forms
{
    public partial class LoginForm : Form
    {
        private Panel pnlContainer;
        private Label lblTitle;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnCancel;
        private Label lblError;

        public LoginForm()
        {
            InitializeComponent();
            Theme.Apply(this);
            SetupForm();
        }

        private void InitializeComponent()
        {
            this.pnlContainer = new Panel();
            this.lblTitle = new Label();
            this.lblUsername = new Label();
            this.txtUsername = new TextBox();
            this.lblPassword = new Label();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.btnCancel = new Button();
            this.lblError = new Label();

            this.SuspendLayout();

            // LoginForm - Modern gradient background
            this.ClientSize = new Size(550, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Connexion - Système de Gestion";
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Theme.PrimaryColor;

            // pnlContainer - Main background
            this.pnlContainer.Dock = DockStyle.Fill;
            this.pnlContainer.BackColor = Theme.BackgroundColor;
            this.pnlContainer.Padding = new Padding(40);
            this.Controls.Add(this.pnlContainer);

            // Create a centered card panel
            var cardPanel = new Panel
            {
                Size = new Size(420, 480),
                Location = new Point(65, 85),
                BackColor = Theme.CardBackground,
                Padding = new Padding(40, 30, 40, 30)
            };
            this.pnlContainer.Controls.Add(cardPanel);

            // Logo/Icon area (decorative)
            var iconPanel = new Panel
            {
                Size = new Size(80, 80),
                Location = new Point(170, 30),
                BackColor = Theme.PrimaryColor
            };
            var iconLabel = new Label
            {
                Text = "🔐",
                Font = new Font("Segoe UI", 32F),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            iconPanel.Controls.Add(iconLabel);
            cardPanel.Controls.Add(iconPanel);

            // lblTitle - Modern title
            this.lblTitle.Text = "Bienvenue";
            this.lblTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            this.lblTitle.ForeColor = Theme.TextColor;
            this.lblTitle.AutoSize = false;
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Location = new Point(40, 125);
            this.lblTitle.Size = new Size(340, 40);
            cardPanel.Controls.Add(this.lblTitle);

            var lblSubtitle = new Label
            {
                Text = "Connectez-vous à votre compte",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Theme.TextSecondary,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 165),
                Size = new Size(340, 25)
            };
            cardPanel.Controls.Add(lblSubtitle);

            int startY = 210;
            int spacing = 90;

            // lblUsername - Modern label
            this.lblUsername.Text = "Nom d'utilisateur";
            this.lblUsername.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblUsername.ForeColor = Theme.TextColor;
            this.lblUsername.Location = new Point(40, startY);
            this.lblUsername.AutoSize = true;
            cardPanel.Controls.Add(this.lblUsername);

            // txtUsername - Modern textbox with better styling
            this.txtUsername.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            this.txtUsername.Location = new Point(40, startY + 28);
            this.txtUsername.Size = new Size(340, 35);
            this.txtUsername.BorderStyle = BorderStyle.FixedSingle;
            this.txtUsername.BackColor = Color.White;
            this.txtUsername.ForeColor = Theme.TextColor;
            cardPanel.Controls.Add(this.txtUsername);

            // lblPassword - Modern label
            this.lblPassword.Text = "Mot de passe";
            this.lblPassword.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblPassword.ForeColor = Theme.TextColor;
            this.lblPassword.Location = new Point(40, startY + spacing);
            this.lblPassword.AutoSize = true;
            cardPanel.Controls.Add(this.lblPassword);

            // txtPassword - Modern password field
            this.txtPassword.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            this.txtPassword.Location = new Point(40, startY + spacing + 28);
            this.txtPassword.Size = new Size(340, 35);
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.BorderStyle = BorderStyle.FixedSingle;
            this.txtPassword.BackColor = Color.White;
            this.txtPassword.ForeColor = Theme.TextColor;
            cardPanel.Controls.Add(this.txtPassword);

            // lblError - Modern error message
            this.lblError.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.lblError.ForeColor = Theme.DangerColor;
            this.lblError.Location = new Point(40, startY + spacing + 70);
            this.lblError.Size = new Size(340, 20);
            this.lblError.TextAlign = ContentAlignment.MiddleCenter;
            this.lblError.Visible = false;
            cardPanel.Controls.Add(this.lblError);

            // btnLogin - Modern primary button
            this.btnLogin.Text = "Se connecter";
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.btnLogin.BackColor = Theme.PrimaryColor;
            this.btnLogin.ForeColor = Color.White;
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.Location = new Point(40, startY + spacing + 100);
            this.btnLogin.Size = new Size(340, 45);
            this.btnLogin.Cursor = Cursors.Hand;
            cardPanel.Controls.Add(this.btnLogin);


            // Footer text
            var lblFooter = new Label
            {
                Text = "© 2025 Système de Gestion des Employés",
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                ForeColor = Theme.TextSecondary,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(65, 580),
                Size = new Size(420, 20)
            };
            this.pnlContainer.Controls.Add(lblFooter);

            this.ResumeLayout(false);
        }


        private void SetupForm()
        {
            // Ajouter les gestionnaires d'événements
            this.btnLogin.Click += BtnLogin_Click;
            this.btnCancel.Click += BtnCancel_Click;
            this.txtPassword.KeyPress += TxtPassword_KeyPress;

            // Définir les valeurs par défaut pour le débogage
            this.txtUsername.Text = "admin";
            this.txtPassword.Text = "2025";
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var username = txtUsername.Text.Trim();
                var password = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ShowError("Veuillez remplir tous les champs");
                    return;
                }

                Console.WriteLine($"Tentative de connexion: {username}");

                // Validate credentials using LicenseManager
                if (LicenseManager.ValidateCredentials(username, password))
                {
                    await LoadMainScreenAsync();
                }
                else
                {
                    ShowError("Identifiants invalides ou licence non trouvée");
                    txtPassword.Clear();
                    txtUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Erreur lors de la connexion: {ex.Message}");
                Console.WriteLine($"Erreur Login: {ex}");
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnLogin_Click(sender, e);
                e.Handled = true;
            }
        }

        private async Task LoadMainScreenAsync()
        {
            try
            {
                var mainForm = new MainForm();
                // When MainForm closes, also close the hidden LoginForm to end the message loop
                mainForm.FormClosed += (s, e) => this.Close();
                this.Hide();
                mainForm.Show();
            }
            catch (Exception ex)
            {
                ShowError($"Erreur lors du chargement de l'application: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }
    }
}