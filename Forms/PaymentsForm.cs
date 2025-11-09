using GestionEmployes.Models;
using GestionEmployes.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GestionEmployes.Forms
{
    public partial class PaymentsForm : Form
    {
        private TransactionService _transactionService;
        private FactureService _factureService;
        private Facture _facture;
        private List<Transaction> _transactions;
        private Transaction _selectedTransaction;

        // Controls
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblFactureInfo;

        private DataGridView dgvTransactions;

        private Panel pnlForm;
        private TextBox txtAmount;
        private DateTimePicker dtpTransactionDate;

        private Button btnAddPayment;
        private Button btnUpdatePayment;
        private Button btnDeletePayment;
        private Button btnClear;
        private Button btnClose;

        private Panel pnlSummary;
        private Label lblTotalAmount;
        private Label lblTotalPaid;
        private Label lblRemaining;

        private Label lblFactureSoldee;

        // Couleurs
        private Color PrimaryColor = Color.FromArgb(41, 128, 185);
        private Color SuccessColor = Color.FromArgb(39, 174, 96);
        private Color DangerColor = Color.FromArgb(231, 76, 60);
        private Color WarningColor = Color.FromArgb(230, 126, 34);
        private Color InfoColor = Color.FromArgb(52, 152, 219);
        private Color HeaderColor = Color.FromArgb(52, 73, 94);
        private Color CardBackground = Color.FromArgb(248, 249, 250);
        private Color TextColor = Color.FromArgb(33, 37, 41);

        public PaymentsForm(Facture facture)
        {
            if (facture == null)
                throw new ArgumentNullException(nameof(facture));

            _facture = facture;
            _transactionService = new TransactionService();
            _factureService = new FactureService();

            InitializeForm();
            CreateControls();
            LoadTransactions();
        }

        private void InitializeForm()
        {
            this.Text = $"Paiements - Facture {_facture.Number}";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 245, 249);
            this.Padding = new Padding(20);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void CreateControls()
        {
            // ==================== HEADER ====================
            pnlHeader = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(1140, 120),
                BackColor = HeaderColor,
                Padding = new Padding(20)
            };

            lblTitle = new Label
            {
                Text = $"HISTORIQUE DES PAIEMENTS",
                Location = new Point(20, 15),
                Size = new Size(800, 30),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false
            };

            lblFactureInfo = new Label
            {
                Text = $"Facture N° {_facture.Number} | Fournisseur: {_facture.Supplier?.Name ?? "N/A"}\n" +
                       $"Date: {_facture.InvoiceDate:dd/MM/yyyy} | Échéance: {_facture.DueDate:dd/MM/yyyy}",
                Location = new Point(20, 50),
                Size = new Size(1000, 50),
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.White,
                AutoSize = false
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblFactureInfo);
            this.Controls.Add(pnlHeader);

            // ==================== SUMMARY PANEL ====================
            pnlSummary = new Panel
            {
                Location = new Point(20, 160),
                Size = new Size(1140, 80),
                BackColor = CardBackground,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblTotalAmount = CreateSummaryLabel("Montant Total:", _facture.Amount, 20, 20, PrimaryColor);
            lblTotalPaid = CreateSummaryLabel("Total Payé:", _facture.Advance, 400, 20, SuccessColor);
            lblRemaining = CreateSummaryLabel("Reste à Payer:", _facture.Remaining, 780, 20,
                _facture.Remaining > 0 ? DangerColor : SuccessColor);

            pnlSummary.Controls.Add(lblTotalAmount);
            pnlSummary.Controls.Add(lblTotalPaid);
            pnlSummary.Controls.Add(lblRemaining);
            this.Controls.Add(pnlSummary);

            // ==================== DATAGRIDVIEW ====================
            dgvTransactions = new DataGridView
            {
                Location = new Point(20, 260),
                Size = new Size(1140, 250),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                RowHeadersVisible = false
            };

            dgvTransactions.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            dgvTransactions.ColumnHeadersHeight = 40;
            dgvTransactions.EnableHeadersVisualStyles = false;

            dgvTransactions.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.White,
                ForeColor = TextColor,
                SelectionBackColor = Color.FromArgb(220, 237, 255),
                SelectionForeColor = TextColor
            };

            dgvTransactions.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 250, 252)
            };

            // Colonnes
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                DataPropertyName = "Id",
                Width = 60
            });
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TransactionDate",
                HeaderText = "DATE",
                DataPropertyName = "TransactionDate",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "MONTANT (DH)",
                DataPropertyName = "Amount",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvTransactions.SelectionChanged += DgvTransactions_SelectionChanged;
            this.Controls.Add(dgvTransactions);

            // ==================== FORM PANEL ====================
            pnlForm = new Panel
            {
                Location = new Point(20, 530),
                Size = new Size(1140, 180),
                BackColor = CardBackground,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            // Ligne 1: Montant + Date
            var lblAmount = CreateLabel("Montant (DH) *", 20, 20);
            txtAmount = CreateTextBox(20, 45, 200, 35);

            var lblDate = CreateLabel("Date *", 250, 20);
            dtpTransactionDate = new DateTimePicker
            {
                Location = new Point(250, 45),
                Width = 200,
                Height = 35,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                Font = new Font("Segoe UI", 10F)
            };


            pnlForm.Controls.Add(lblAmount);
            pnlForm.Controls.Add(txtAmount);
            pnlForm.Controls.Add(lblDate);
            pnlForm.Controls.Add(dtpTransactionDate);

            // Boutons à droite du panel
            int btnX = 900;
            int btnY = 20;
            int btnWidth = 200;
            int btnHeight = 40;
            int btnSpacing = 50;

            btnAddPayment = CreateButton("➕ AJOUTER", btnX, btnY, btnWidth, btnHeight, SuccessColor);
            btnUpdatePayment = CreateButton("✏️ MODIFIER", btnX, btnY + btnSpacing, btnWidth, btnHeight, InfoColor);
            btnDeletePayment = CreateButton("🗑️ SUPPRIMER", btnX, btnY + (btnSpacing * 2), btnWidth, btnHeight, DangerColor);

            btnAddPayment.Click += BtnAddPayment_Click;
            btnUpdatePayment.Click += BtnUpdatePayment_Click;
            btnDeletePayment.Click += BtnDeletePayment_Click;

            pnlForm.Controls.Add(btnAddPayment);
            pnlForm.Controls.Add(btnUpdatePayment);
            pnlForm.Controls.Add(btnDeletePayment);

            this.Controls.Add(pnlForm);

            // ==================== BOTTOM BUTTONS ====================
            btnClear = new Button
            {
                Text = "🔄 EFFACER",
                Location = new Point(20, 730),
                Size = new Size(150, 40),
                BackColor = WarningColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.Click += BtnClear_Click;

            btnClose = new Button
            {
                Text = "✖️ FERMER",
                Location = new Point(1010, 730),
                Size = new Size(150, 40),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(btnClear);
            this.Controls.Add(btnClose);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = HeaderColor,
                AutoSize = true
            };
        }

        private TextBox CreateTextBox(int x, int y, int width, int height)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
        }

        private Button CreateButton(string text, int x, int y, int width, int height, Color backColor)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = {
                    BorderSize = 0,
                    MouseOverBackColor = ControlPaint.Light(backColor),
                    MouseDownBackColor = ControlPaint.Dark(backColor)
                },
                Cursor = Cursors.Hand
            };
        }

        private Label CreateSummaryLabel(string title, decimal value, int x, int y, Color color)
        {
            var container = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(350, 50),
                BackColor = Color.Transparent
            };

            var lblTitle = new Label
            {
                Text = title,
                Location = new Point(0, 0),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = TextColor,
                AutoSize = true
            };

            var lblValue = new Label
            {
                Text = value.ToString("N2") + " DH",
                Location = new Point(0, 25),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = color,
                AutoSize = true
            };

            container.Controls.Add(lblTitle);
            container.Controls.Add(lblValue);

            return lblValue; // On retourne le label de valeur pour le mettre à jour
        }

        private void LoadTransactions()
        {
            try
            {
                _transactions = _transactionService.GetTransactionsByFacture(_facture.Id);
                dgvTransactions.DataSource = new List<Transaction>(_transactions);

                // Rafraîchir les informations de la facture
                RefreshFactureInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des paiements: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshFactureInfo()
        {
            try
            {
                // Recharger la facture depuis la base de données
                _facture = _factureService.GetFactureById(_facture.Id);
                if (_facture == null) return;

                // Mettre à jour le résumé
                lblTotalAmount.Text = _facture.Amount.ToString("N2") + " DH";
                lblTotalPaid.Text = _facture.Advance.ToString("N2") + " DH";
                lblRemaining.Text = _facture.Remaining.ToString("N2") + " DH";

                // Changer la couleur du reste selon le statut
                if (_facture.Remaining <= 0)
                    lblRemaining.ForeColor = SuccessColor;
                else if (_facture.Remaining < _facture.Amount)
                    lblRemaining.ForeColor = WarningColor;
                else
                    lblRemaining.ForeColor = DangerColor;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur RefreshFactureInfo: {ex.Message}");
            }
        }

        private void DgvTransactions_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTransactions.SelectedRows.Count > 0)
            {
                _selectedTransaction = dgvTransactions.SelectedRows[0].DataBoundItem as Transaction;
                if (_selectedTransaction != null)
                {
                    FillForm(_selectedTransaction);
                }
            }
        }

        private void FillForm(Transaction transaction)
        {
            if (transaction == null) return;

            txtAmount.Text = transaction.Amount.ToString("N2");
            dtpTransactionDate.Value = transaction.TransactionDate;
        }

        private void ClearForm()
        {
            txtAmount.Text = "";
            dtpTransactionDate.Value = DateTime.Today;
            _selectedTransaction = null;
            dgvTransactions.ClearSelection();
        }

        private void BtnAddPayment_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    MessageBox.Show("Le montant est obligatoire", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Le montant doit être positif", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Vérifier que le montant ne dépasse pas le reste à payer
                if (amount > _facture.Remaining)
                {
                    var result = MessageBox.Show(
                        $"Le montant ({amount:N2} DH) dépasse le reste à payer ({_facture.Remaining:N2} DH).\n\n" +
                        $"Voulez-vous ajuster le montant automatiquement à {_facture.Remaining:N2} DH ?",
                        "Montant supérieur",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        amount = _facture.Remaining;
                        txtAmount.Text = amount.ToString("N2");
                    }
                    else
                    {
                        return;
                    }
                }

                // Créer la transaction
                var transaction = new Transaction
                {
                    FactureId = _facture.Id,
                    Type = "Paiement",
                    Amount = amount,
                    TransactionDate = dtpTransactionDate.Value,
                    CreatedDate = DateTime.Now
                };

                if (_transactionService.AddTransaction(transaction))
                {
                    LoadTransactions();
                    ClearForm();
                    MessageBox.Show("Paiement enregistré avec succès!", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'enregistrement du paiement", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdatePayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedTransaction == null)
                {
                    MessageBox.Show("Veuillez sélectionner un paiement", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    MessageBox.Show("Le montant est obligatoire", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Le montant doit être positif", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Calculer le reste à payer en excluant le paiement actuel
                decimal remainingWithoutCurrent = _facture.Remaining + _selectedTransaction.Amount;

                if (amount > remainingWithoutCurrent)
                {
                    MessageBox.Show(
                        $"Le nouveau montant ({amount:N2} DH) dépasse le reste à payer ({remainingWithoutCurrent:N2} DH)",
                        "Montant invalide",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                _selectedTransaction.Amount = amount;
                _selectedTransaction.TransactionDate = dtpTransactionDate.Value;

                if (_transactionService.UpdateTransaction(_selectedTransaction))
                {
                    LoadTransactions();
                    ClearForm();
                    MessageBox.Show("Paiement modifié avec succès!", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Erreur lors de la modification du paiement", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeletePayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedTransaction == null)
                {
                    MessageBox.Show("Veuillez sélectionner un paiement", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer ce paiement de {_selectedTransaction.Amount:N2} DH ?",
                    "Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (_transactionService.DeleteTransaction(_selectedTransaction.Id))
                    {
                        LoadTransactions();
                        ClearForm();
                        MessageBox.Show("Paiement supprimé avec succès!", "Succès",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la suppression du paiement", "Erreur",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
    }
}