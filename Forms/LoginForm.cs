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
            this.lblActivationInfo = new LinkLabel();
            this.lblSupport = new Label();

            this.SuspendLayout();

            // LoginForm
            this.ClientSize = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Connexion - Gestion des Employés";
            this.BackColor = Color.White;

            // pnlContainer
            this.pnlContainer.Dock = DockStyle.Fill;
            this.pnlContainer.BackColor = Color.White;
            this.pnlContainer.Padding = new Padding(30);
            this.Controls.Add(this.pnlContainer);

            // lblTitle
            this.lblTitle.Text = "Gestion des Employés";
            this.lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblTitle.AutoSize = false;
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Location = new Point(50, 30);
            this.lblTitle.Size = new Size(400, 40);
            this.pnlContainer.Controls.Add(this.lblTitle);

            var lblSubtitle = new Label
            {
                Text = "Système de gestion du personnel",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(50, 75),
                Size = new Size(400, 25)
            };
            this.pnlContainer.Controls.Add(lblSubtitle);

            int startY = 120;

            // lblUsername
            this.lblUsername.Text = "Nom d'utilisateur";
            this.lblUsername.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblUsername.ForeColor = Color.Black;
            this.lblUsername.Location = new Point(50, startY);
            this.lblUsername.AutoSize = true;
            this.pnlContainer.Controls.Add(this.lblUsername);

            // txtUsername
            this.txtUsername.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.txtUsername.Location = new Point(50, startY + 25);
            this.txtUsername.Size = new Size(400, 30);
            this.txtUsername.BorderStyle = BorderStyle.FixedSingle;
            this.txtUsername.BackColor = Color.White;
            this.txtUsername.ForeColor = Color.Black;
            this.pnlContainer.Controls.Add(this.txtUsername);

            // lblPassword
            this.lblPassword.Text = "Mot de passe";
            this.lblPassword.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblPassword.ForeColor = Color.Black;
            this.lblPassword.Location = new Point(50, startY + 80);
            this.lblPassword.AutoSize = true;
            this.pnlContainer.Controls.Add(this.lblPassword);

            // txtPassword
            this.txtPassword.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.txtPassword.Location = new Point(50, startY + 105);
            this.txtPassword.Size = new Size(400, 30);
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.BorderStyle = BorderStyle.FixedSingle;
            this.txtPassword.BackColor = Color.White;
            this.txtPassword.ForeColor = Color.Black;
            this.pnlContainer.Controls.Add(this.txtPassword);

            // lblError
            this.lblError.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.lblError.ForeColor = Color.Red;
            this.lblError.Location = new Point(50, startY + 150);
            this.lblError.Size = new Size(400, 20);
            this.lblError.TextAlign = ContentAlignment.MiddleCenter;
            this.lblError.Visible = false;
            this.pnlContainer.Controls.Add(this.lblError);

            // btnLogin
            this.btnLogin.Text = "Se connecter";
            this.btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnLogin.BackColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnLogin.ForeColor = Color.White;
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.Location = new Point(50, startY + 180);
            this.btnLogin.Size = new Size(400, 40);
            this.btnLogin.Cursor = Cursors.Hand;
            this.pnlContainer.Controls.Add(this.btnLogin);

            // lblActivationInfo
            this.lblActivationInfo.Text = "🔑 Procédure d'activation";
            this.lblActivationInfo.Font = new Font("Segoe UI", 8F, FontStyle.Regular);
            this.lblActivationInfo.LinkColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblActivationInfo.Location = new Point(50, startY + 240);
            this.lblActivationInfo.Size = new Size(400, 20);
            this.lblActivationInfo.TextAlign = ContentAlignment.MiddleCenter;
            this.lblActivationInfo.Click += LblActivationInfo_Click;
            this.pnlContainer.Controls.Add(this.lblActivationInfo);

            // lblSupport
            this.lblSupport.Text = $"📞 Support: {LicenseManager.GetSupportPhone()}";
            this.lblSupport.Font = new Font("Segoe UI", 8F, FontStyle.Regular);
            this.lblSupport.ForeColor = Color.Gray;
            this.lblSupport.Location = new Point(50, startY + 265);
            this.lblSupport.Size = new Size(400, 20);
            this.lblSupport.TextAlign = ContentAlignment.MiddleCenter;
            this.pnlContainer.Controls.Add(this.lblSupport);

            this.ResumeLayout(false);
        }

        private void SetupForm()
        {
            // Gestionnaires d'événements
            this.btnLogin.Click += BtnLogin_Click;
            this.txtPassword.KeyPress += TxtPassword_KeyPress;

            // Champs vides
            this.txtUsername.Text = "";
            this.txtPassword.Text = "";
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

                // ✅ Validation simplifiée - vérifie seulement l'activation
                if (LicenseManager.ValidateCredentials(username, password))
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
        }

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnLogin_Click(sender, e);
                e.Handled = true;
            }
        }

        private void LblActivationInfo_Click(object sender, EventArgs e)
        {
            string activationInfo = LicenseManager.GetLicenseInfo();
            MessageBox.Show(activationInfo, "PROCÉDURE D'ACTIVATION",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task LoadMainScreenAsync()
        {
            try
            {
                var mainForm = new MainForm();
                mainForm.FormClosed += (s, e) => this.Close();
                this.Hide();
                mainForm.Show();
            }
            catch (Exception ex)
            {
                ShowError($"Erreur lors du chargement: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }
    }
}