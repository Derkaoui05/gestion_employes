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
    public partial class FactureForm : Form
    {
        private FactureService _factureService;
        private SupplierService _supplierService;
        private List<Facture> _factures;
        private List<Supplier> _suppliers;
        private Facture _selectedFacture;

        // Controls
        private DataGridView dgvFactures;
        private ComboBox cmbSupplier;
        private TextBox txtNumber;
        private TextBox txtAmount;
        private TextBox txtAdvance;
        private DateTimePicker dtpInvoiceDate;
        private DateTimePicker dtpDueDate;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClear;
        private Label lblRemaining;
        private Label lblCount;

        // ✅ CLASSE D'AFFICHAGE POUR LES FACTURES
        public class FactureDisplay
        {
            public int Id { get; set; }
            public string Number { get; set; }
            public string SupplierName { get; set; }
            public decimal Amount { get; set; }
            public decimal Advance { get; set; }
            public decimal Remaining { get; set; }
            public string Status { get; set; }
            public DateTime InvoiceDate { get; set; }
            public DateTime DueDate { get; set; }
        }

        public FactureForm()
        {
            _factureService = new FactureService();
            _supplierService = new SupplierService();

            this.Text = "Gestion des Factures";
            this.Size = new Size(1500, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Theme.BackgroundColor;
            this.Padding = new Padding(15);

            Theme.Apply(this);
            CreateAllControls();
            this.Load += FactureForm_Load;

            // 🔴 S'ABONNER AUX ÉVÉNEMENTS
            SubscribeToEvents();
        }

        // 🔴 NOUVELLE MÉTHODE POUR S'ABONNER
        private void SubscribeToEvents()
        {
            EventBus.SupplierAdded += (s, e) => RefreshSuppliersAsync();
            EventBus.SupplierUpdated += (s, e) => RefreshSuppliersAsync();
            EventBus.SupplierDeleted += (s, e) => RefreshSuppliersAsync();
        }

        // 🔴 NOUVELLE MÉTHODE POUR RECHARGER INTELLIGEMMENT
        private void RefreshSuppliersAsync()
        {
            try
            {
                // Sauvegarder la sélection actuelle
                int selectedId = (cmbSupplier.SelectedValue != null) ? (int)cmbSupplier.SelectedValue : -1;
                string selectedName = cmbSupplier.SelectedItem?.ToString() ?? "";

                // Créer une nouvelle instance pour lire les données fraîches
                _supplierService = new SupplierService();
                _suppliers = _supplierService.GetAllSuppliers();

                // Réinitialiser le ComboBox avec les nouvelles données
                cmbSupplier.DataSource = null;
                cmbSupplier.DataSource = new List<Supplier>(_suppliers);
                cmbSupplier.DisplayMember = "Name";
                cmbSupplier.ValueMember = "ID";

                // Restaurer la sélection si elle existe toujours
                if (selectedId > 0 && _suppliers.Any(s => s.ID == selectedId))
                {
                    cmbSupplier.SelectedValue = selectedId;
                }
                else if (_suppliers.Count > 0)
                {
                    cmbSupplier.SelectedIndex = 0;
                }

                Console.WriteLine($"✅ Fournisseurs rechargés: {_suppliers.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur RefreshSuppliers: {ex.Message}");
            }
        }

        private void CreateAllControls()
        {
            // ==================== MAIN CARD ====================
            var mainCard = CreateCardPanel(15, 15, 1450, 800, "📋 Gestion des Factures");

            // ==================== DATAGRIDVIEW ====================
            dgvFactures = new DataGridView
            {
                Location = new Point(20, 50),
                Size = new Size(1410, 250),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false
            };

            // ✅ COLONNES CORRIGÉES AVEC LES BONNES DataPropertyName
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn { Name = "ID", HeaderText = "ID", DataPropertyName = "Id", Width = 50 });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Number", HeaderText = "N° Facture", DataPropertyName = "Number", Width = 100 });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn { Name = "SupplierName", HeaderText = "Fournisseur", DataPropertyName = "SupplierName", Width = 150 }); // ✅ CORRIGÉ
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Amount", HeaderText = "Montant", DataPropertyName = "Amount", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Advance", HeaderText = "Avance", DataPropertyName = "Advance", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Remaining", HeaderText = "Reste", DataPropertyName = "Remaining", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Statut", DataPropertyName = "Status", Width = 80 });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn { Name = "InvoiceDate", HeaderText = "Date Facture", DataPropertyName = "InvoiceDate", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn { Name = "DueDate", HeaderText = "Date Échéance", DataPropertyName = "DueDate", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });

            dgvFactures.SelectionChanged += (s, e) => DgvFactures_SelectionChanged();
            mainCard.Controls.Add(dgvFactures);

            // ==================== INPUT SECTION ====================
            int inputY = 320;
            int col1X = 20, col2X = 480, col3X = 940;

            // Supplier
            var lblSupplier = CreateLabel("Fournisseur", col1X, inputY);
            cmbSupplier = new ComboBox
            {
                Location = new Point(col1X, inputY + 25),
                Size = new Size(430, 35),
                DisplayMember = "Name",
                ValueMember = "ID",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                FormattingEnabled = true
            };
            mainCard.Controls.Add(lblSupplier);
            mainCard.Controls.Add(cmbSupplier);

            // Invoice Number
            var lblNumber = CreateLabel("N° Facture", col2X, inputY);
            txtNumber = CreateTextBox(col2X, inputY + 25, 430, 35);
            mainCard.Controls.Add(lblNumber);
            mainCard.Controls.Add(txtNumber);

            // Amount
            var lblAmount = CreateLabel("Montant (DH)", col3X, inputY);
            txtAmount = CreateTextBox(col3X, inputY + 25, 350, 35);
            mainCard.Controls.Add(lblAmount);
            mainCard.Controls.Add(txtAmount);

            inputY += 75;

            // Invoice Date
            var lblInvoiceDate = CreateLabel("Date Facture", col1X, inputY);
            dtpInvoiceDate = new DateTimePicker
            {
                Location = new Point(col1X, inputY + 25),
                Size = new Size(430, 35),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                Font = new Font("Segoe UI", 10F)
            };
            mainCard.Controls.Add(lblInvoiceDate);
            mainCard.Controls.Add(dtpInvoiceDate);

            // Due Date
            var lblDueDate = CreateLabel("Date Échéance", col2X, inputY);
            dtpDueDate = new DateTimePicker
            {
                Location = new Point(col2X, inputY + 25),
                Size = new Size(430, 35),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddDays(30),
                Font = new Font("Segoe UI", 10F)
            };
            mainCard.Controls.Add(lblDueDate);
            mainCard.Controls.Add(dtpDueDate);

            // Advance
            var lblAdvance = CreateLabel("Avance (DH)", col3X, inputY);
            txtAdvance = CreateTextBox(col3X, inputY + 25, 350, 35);
            txtAdvance.Text = "0";
            mainCard.Controls.Add(lblAdvance);
            mainCard.Controls.Add(txtAdvance);

            inputY += 75;

            inputY += 100;

            // ==================== BUTTONS ====================
            btnAdd = CreateButton("➕ Ajouter", col1X, inputY, Theme.SuccessColor);
            btnUpdate = CreateButton("✏️ Modifier", col1X + 165, inputY, Theme.InfoColor);
            btnDelete = CreateButton("🗑️ Supprimer", col1X + 330, inputY, Theme.DangerColor);
            btnClear = CreateButton("🧹 Effacer", col1X + 495, inputY, Theme.WarningColor);

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += BtnClear_Click;

            mainCard.Controls.Add(btnAdd);
            mainCard.Controls.Add(btnUpdate);
            mainCard.Controls.Add(btnDelete);
            mainCard.Controls.Add(btnClear);

            lblRemaining = new Label
            {
                Location = new Point(col1X + 660, inputY + 8),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Theme.InfoColor,
                Text = "Reste: 0.00 DH"
            };
            mainCard.Controls.Add(lblRemaining);

            lblCount = new Label
            {
                Location = new Point(col1X + 660, inputY + 35),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Theme.PrimaryColor,
                Text = "Factures: 0"
            };
            mainCard.Controls.Add(lblCount);

            this.Controls.Add(mainCard);
        }

        private Panel CreateCardPanel(int x, int y, int width, int height, string title)
        {
            var card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Theme.CardBackground,
                Padding = new Padding(15),
                BorderStyle = BorderStyle.None
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Theme.PrimaryColor,
                Location = new Point(15, 10),
                AutoSize = true
            };
            card.Controls.Add(titleLabel);

            return card;
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(430, 20),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Theme.TextColor,
                AutoSize = false
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
                BackColor = Color.White,
                ForeColor = Theme.TextColor
            };
        }

        private Button CreateButton(string text, int x, int y, Color backColor)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(155, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand
            };
        }

        private async void FactureForm_Load(object sender, EventArgs e)
        {
            try
            {
                await LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ✅ MÉTHODE LoadData CORRIGÉE
        private async System.Threading.Tasks.Task LoadData()
        {
            try
            {
                _suppliers = _supplierService.GetAllSuppliers();
                _factures = _factureService.GetAllFactures();

                cmbSupplier.DataSource = new List<Supplier>(_suppliers);
                cmbSupplier.DisplayMember = "Name";
                cmbSupplier.ValueMember = "ID";

                // ✅ CRÉER UNE LISTE POUR L'AFFICHAGE AVEC LE NOM DU FOURNISSEUR
                var displayList = new List<FactureDisplay>();

                foreach (var facture in _factures)
                {
                    decimal remaining = facture.Amount - facture.Advance;

                    displayList.Add(new FactureDisplay
                    {
                        Id = facture.Id,
                        Number = facture.Number,
                        SupplierName = facture.Supplier?.Name ?? "Inconnu", // ✅ NOM DU FOURNISSEUR
                        Amount = facture.Amount,
                        Advance = facture.Advance,
                        Remaining = remaining,
                        Status = remaining <= 0 ? "Payée" : "En cours",
                        InvoiceDate = facture.InvoiceDate,
                        DueDate = facture.DueDate
                    });
                }

                dgvFactures.DataSource = displayList;
                UpdateLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur LoadData: {ex.Message}\n{ex.InnerException?.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ✅ MÉTHODE DgvFactures_SelectionChanged CORRIGÉE
        private void DgvFactures_SelectionChanged()
        {
            if (dgvFactures.SelectedRows.Count > 0 && dgvFactures.SelectedRows[0].DataBoundItem is FactureDisplay displayItem)
            {
                // ✅ TROUVER LA FACTURE ORIGINALE PAR ID
                _selectedFacture = _factures.FirstOrDefault(f => f.Id == displayItem.Id);
                FillForm(_selectedFacture);
            }
        }

        private void FillForm(Facture facture)
        {
            if (facture == null) return;

            cmbSupplier.SelectedValue = facture.SupplierId;
            txtNumber.Text = facture.Number;
            txtAmount.Text = facture.Amount.ToString("N2");
            txtAdvance.Text = facture.Advance.ToString("N2");
            dtpInvoiceDate.Value = facture.InvoiceDate;
            dtpDueDate.Value = facture.DueDate;

            decimal remaining = facture.Amount - facture.Advance;
            lblRemaining.Text = $"Reste: {remaining:N2} DH";
        }

        private void ClearForm()
        {
            cmbSupplier.SelectedIndex = -1;
            txtNumber.Text = "";
            txtAmount.Text = "";
            txtAdvance.Text = "0";
            dtpInvoiceDate.Value = DateTime.Today;
            dtpDueDate.Value = DateTime.Today.AddDays(30);
            lblRemaining.Text = "Reste: 0.00 DH";
            _selectedFacture = null;
            dgvFactures.ClearSelection();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbSupplier.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez sélectionner un fournisseur", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNumber.Text))
                {
                    MessageBox.Show("Le numéro de facture est obligatoire", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Le montant doit être un nombre positif", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAdvance.Text, out decimal advance))
                    advance = 0;

                var facture = new Facture
                {
                    Number = txtNumber.Text,
                    SupplierId = (int)cmbSupplier.SelectedValue,
                    Amount = amount,
                    Advance = advance,
                    InvoiceDate = dtpInvoiceDate.Value,
                    DueDate = dtpDueDate.Value,
                    CreatedDate = DateTime.Now
                };

                if (_factureService.AddFacture(facture))
                {
                    LoadData();
                    ClearForm();
                    MessageBox.Show("Facture ajoutée avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedFacture == null)
                {
                    MessageBox.Show("Veuillez sélectionner une facture", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbSupplier.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez sélectionner un fournisseur", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNumber.Text))
                {
                    MessageBox.Show("Le numéro de facture est obligatoire", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Le montant doit être un nombre positif", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAdvance.Text, out decimal advance))
                    advance = 0;

                _selectedFacture.Number = txtNumber.Text;
                _selectedFacture.SupplierId = (int)cmbSupplier.SelectedValue;
                _selectedFacture.Amount = amount;
                _selectedFacture.Advance = advance;
                _selectedFacture.InvoiceDate = dtpInvoiceDate.Value;
                _selectedFacture.DueDate = dtpDueDate.Value;

                if (_factureService.UpdateFacture(_selectedFacture))
                {
                    LoadData();
                    ClearForm();
                    MessageBox.Show("Facture modifiée avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedFacture == null)
                {
                    MessageBox.Show("Veuillez sélectionner une facture", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer la facture {_selectedFacture.Number}?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (_factureService.DeleteFacture(_selectedFacture.Id))
                    {
                        LoadData();
                        ClearForm();
                        MessageBox.Show("Facture supprimée avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void UpdateLabels()
        {
            lblCount.Text = $"Factures: {_factures?.Count ?? 0}";
        }

        // 🔴 SE DÉSABONNER LORS DE LA FERMETURE
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            EventBus.SupplierAdded -= (s, args) => RefreshSuppliersAsync();
            EventBus.SupplierUpdated -= (s, args) => RefreshSuppliersAsync();
            EventBus.SupplierDeleted -= (s, args) => RefreshSuppliersAsync();
            base.OnFormClosing(e);
        }
    }
}