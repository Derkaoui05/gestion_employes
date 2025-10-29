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
        private Label lblError;
        private LinkLabel lblActivationInfo;
        private Label lblSupport;

        public LoginForm()
        {
            InitializeComponent();
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
            this.lblError = new Label();
            //this.lblActivationInfo = new LinkLabel();
            this.lblSupport = new Label();

            this.SuspendLayout();

            // LoginForm - Taille réduite et centrage automatique
            this.ClientSize = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Connexion - Gestion des Employés";
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.Sizable; // Permet de redimensionner et voir les boutons
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.ShowIcon = true;

            // pnlContainer - Centré dans le formulaire
            this.pnlContainer.Size = new Size(350, 450);
            this.pnlContainer.Location = new Point(
                (this.ClientSize.Width - this.pnlContainer.Width) / 2,
                (this.ClientSize.Height - this.pnlContainer.Height) / 2
            );
            this.pnlContainer.BackColor = Color.White;
            this.pnlContainer.Anchor = AnchorStyles.None; // Reste centré
            this.Controls.Add(this.pnlContainer);

            // Title Section
            var titlePanel = new Panel
            {
                Location = new Point(0, 20),
                Size = new Size(pnlContainer.Width, 80),
                BackColor = Color.Transparent
            };
            this.pnlContainer.Controls.Add(titlePanel);

            // lblTitle
            this.lblTitle.Text = "Gestion des Employés";
            this.lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTitle.ForeColor = Theme.PrimaryColor;
            this.lblTitle.AutoSize = false;
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Location = new Point(0, 0);
            this.lblTitle.Size = new Size(titlePanel.Width, 40);
            titlePanel.Controls.Add(this.lblTitle);

            var lblSubtitle = new Label
            {
                Text = "Système de gestion du personnel",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 45),
                Size = new Size(titlePanel.Width, 25)
            };
            titlePanel.Controls.Add(lblSubtitle);

            // Form Container
            var formPanel = new Panel
            {
                Location = new Point(0, 110),
                Size = new Size(pnlContainer.Width, 280),
                BackColor = Color.Transparent
            };
            this.pnlContainer.Controls.Add(formPanel);

            int startY = 0;

            // lblUsername
            this.lblUsername.Text = "Nom d'utilisateur";
            this.lblUsername.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblUsername.ForeColor = Color.FromArgb(60, 60, 60);
            this.lblUsername.Location = new Point(0, startY);
            this.lblUsername.Size = new Size(formPanel.Width, 20);
            this.lblUsername.TextAlign = ContentAlignment.MiddleLeft;
            formPanel.Controls.Add(this.lblUsername);

            // txtUsername
            this.txtUsername.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.txtUsername.Location = new Point(0, startY + 25);
            this.txtUsername.Size = new Size(formPanel.Width, 35);
            this.txtUsername.BorderStyle = BorderStyle.FixedSingle;
            this.txtUsername.BackColor = Color.White;
            this.txtUsername.ForeColor = Color.FromArgb(40, 40, 40);
            this.txtUsername.Padding = new Padding(8);
            formPanel.Controls.Add(this.txtUsername);

            // lblPassword
            this.lblPassword.Text = "Mot de passe";
            this.lblPassword.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblPassword.ForeColor = Color.FromArgb(60, 60, 60);
            this.lblPassword.Location = new Point(0, startY + 80);
            this.lblPassword.Size = new Size(formPanel.Width, 20);
            this.lblPassword.TextAlign = ContentAlignment.MiddleLeft;
            formPanel.Controls.Add(this.lblPassword);

            // txtPassword
            this.txtPassword.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.txtPassword.Location = new Point(0, startY + 105);
            this.txtPassword.Size = new Size(formPanel.Width, 35);
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.BorderStyle = BorderStyle.FixedSingle;
            this.txtPassword.BackColor = Color.White;
            this.txtPassword.ForeColor = Color.FromArgb(40, 40, 40);
            this.txtPassword.Padding = new Padding(8);
            formPanel.Controls.Add(this.txtPassword);

            // lblError
            this.lblError.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.lblError.ForeColor = Theme.DangerColor;
            this.lblError.Location = new Point(0, startY + 155);
            this.lblError.Size = new Size(formPanel.Width, 20);
            this.lblError.TextAlign = ContentAlignment.MiddleCenter;
            this.lblError.Visible = false;
            formPanel.Controls.Add(this.lblError);

            // btnLogin
            this.btnLogin.Text = "Se connecter";
            this.btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnLogin.BackColor = Theme.PrimaryColor;
            this.btnLogin.ForeColor = Color.White;
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.Location = new Point(0, startY + 185);
            this.btnLogin.Size = new Size(formPanel.Width, 40);
            this.btnLogin.Cursor = Cursors.Hand;
            this.btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 110, 184);
            this.btnLogin.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 95, 165);
            formPanel.Controls.Add(this.btnLogin);

            // Footer Section
            var footerPanel = new Panel
            {
                Location = new Point(0, 400),
                Size = new Size(pnlContainer.Width, 60),
                BackColor = Color.Transparent
            };
            this.pnlContainer.Controls.Add(footerPanel);

            //// lblActivationInfo
            //this.lblActivationInfo.Text = "🔑 Procédure d'activation";
            //this.lblActivationInfo.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            //this.lblActivationInfo.LinkColor = Theme.PrimaryColor;
            //this.lblActivationInfo.ActiveLinkColor = Color.FromArgb(0, 95, 165);
            //this.lblActivationInfo.VisitedLinkColor = Theme.PrimaryColor;
            //this.lblActivationInfo.Location = new Point(0, 5);
            //this.lblActivationInfo.Size = new Size(footerPanel.Width, 20);
            //this.lblActivationInfo.TextAlign = ContentAlignment.MiddleCenter;
            ////this.lblActivationInfo.Click += LblActivationInfo_Click;
            //footerPanel.Controls.Add(this.lblActivationInfo);

            // lblSupport
            this.lblSupport.Text = $"📞 Support: {LicenseManager.GetSupportPhone()}";
            this.lblSupport.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.lblSupport.ForeColor = Color.FromArgb(120, 120, 120);
            this.lblSupport.Location = new Point(0, 30);
            this.lblSupport.Size = new Size(footerPanel.Width, 20);
            this.lblSupport.TextAlign = ContentAlignment.MiddleCenter;
            footerPanel.Controls.Add(this.lblSupport);

            this.ResumeLayout(false);
        }

        private void SetupForm()
        {
            // Gestionnaires d'événements
            this.btnLogin.Click += BtnLogin_Click;
            this.txtPassword.KeyPress += TxtPassword_KeyPress;

            // Gestion des bordures avec événements
            this.txtUsername.Enter += (s, e) =>
            {
                this.txtUsername.BackColor = Color.FromArgb(248, 248, 248);
            };
            this.txtUsername.Leave += (s, e) =>
            {
                this.txtUsername.BackColor = Color.White;
            };
            this.txtPassword.Enter += (s, e) =>
            {
                this.txtPassword.BackColor = Color.FromArgb(248, 248, 248);
            };
            this.txtPassword.Leave += (s, e) =>
            {
                this.txtPassword.BackColor = Color.White;
            };

            // Champs vides
            this.txtUsername.Text = "";
            this.txtPassword.Text = "";

            // Focus sur le premier champ
            this.txtUsername.Focus();
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                // Afficher l'état de chargement
                btnLogin.Text = "Connexion...";
                btnLogin.Enabled = false;
                lblError.Visible = false;

                var username = txtUsername.Text.Trim();
                var password = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ShowError("Veuillez remplir tous les champs");
                    return;
                }

                // ✅ Validation simplifiée  avec identifiants génériques
                if (ValidateGenericCredentials(username, password))
                {
                    await LoadMainScreenAsync();
                }
                else
                {
                    // Le message d'erreur est géré dans LicenseManager
                }
            }
            catch (Exception ex)
            {
                ShowError($"Erreur lors de la connexion: {ex.Message}");
            }
            finally
            {
                // Restaurer le bouton
                btnLogin.Text = "Se connecter";
                btnLogin.Enabled = true;
            }
        }
        private bool ValidateGenericCredentials(string username, string password)
        {
            // Identifiants fixes
            const string GENERIC_USERNAME = "admin";
            const string GENERIC_PASSWORD = "12345";

            bool isValid = username.Equals(GENERIC_USERNAME, StringComparison.OrdinalIgnoreCase) &&
                           password == GENERIC_PASSWORD;

            if (!isValid)
            {
                ShowError("Nom d'utilisateur ou mot de passe incorrect");
            }

            return isValid;
        }

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnLogin_Click(sender, e);
                e.Handled = true;
            }
        }

        //private void LblActivationInfo_Click(object sender, EventArgs e)
        //{
        //    string activationInfo = LicenseManager.GetLicenseInfo();
        //    MessageBox.Show(activationInfo, "PROCÉDURE D'ACTIVATION",
        //        MessageBoxButtons.OK, MessageBoxIcon.Information);
        //}

        private async Task LoadMainScreenAsync()
        {
            try
            {
                // Ajouter un effet de transition
                this.Opacity = 0.9;
                await Task.Delay(50);

                var mainForm = new MainForm();
                mainForm.FormClosed += (s, e) => this.Close();
                this.Hide();
                mainForm.Show();
            }
            catch (Exception ex)
            {
                ShowError($"Erreur lors du chargement: {ex.Message}");
                this.Opacity = 1.0;
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;

            // Animation de shake pour attirer l'attention
            var originalLocation = pnlContainer.Location;
            for (int i = 0; i < 3; i++)
            {
                pnlContainer.Location = new Point(originalLocation.X + 3, originalLocation.Y);
                System.Threading.Thread.Sleep(50);
                Application.DoEvents();
                pnlContainer.Location = new Point(originalLocation.X - 3, originalLocation.Y);
                System.Threading.Thread.Sleep(50);
                Application.DoEvents();
            }
            pnlContainer.Location = originalLocation;
        }

        // Gestion du redimensionnement pour garder le contenu centré
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (pnlContainer != null)
            {
                pnlContainer.Location = new Point(
                    (this.ClientSize.Width - pnlContainer.Width) / 2,
                    (this.ClientSize.Height - pnlContainer.Height) / 2
                );
            }
        }
    }
}