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
    public partial class SupplierDetailsForm : Form
    {
        private SupplierService _supplierService;
        private FactureService _factureService;
        private List<Supplier> _suppliers;
        private List<Facture> _factures;
        private Supplier _selectedSupplier;
        private Facture _selectedFacture;

        // Controls - LEFT PANEL: SUPPLIERS
        private DataGridView dgvSuppliers;
        private TextBox txtName;
        private TextBox txtPhone;
        private Button btnAddSupplier;
        private Button btnUpdateSupplier;
        private Button btnDeleteSupplier;
        private Button btnClearSupplier;

        // Controls - RIGHT PANEL: FACTURES
        private DataGridView dgvFactures;
        private ComboBox cmbSupplier;
        private TextBox txtFactureNumber;
        private TextBox txtFactureAmount;
        private TextBox txtFactureAdvance;
        private DateTimePicker dtpInvoiceDate;
        private DateTimePicker dtpDueDate;
        private Button btnAddFacture;
        private Button btnUpdateFacture;
        private Button btnDeleteFacture;
        private Button btnClearFacture;

        // Nouvelles couleurs personnalisées
        private Color PrimaryColor = Color.FromArgb(41, 128, 185);    // Bleu principal
        private Color SecondaryColor = Color.FromArgb(52, 152, 219);  // Bleu secondaire
        private Color SuccessColor = Color.FromArgb(39, 174, 96);     // Vert succès
        private Color DangerColor = Color.FromArgb(231, 76, 60);      // Rouge danger
        private Color WarningColor = Color.FromArgb(230, 126, 34);    // Orange avertissement
        private Color InfoColor = Color.FromArgb(52, 152, 219);       // Bleu info
        private Color CardBackground = Color.FromArgb(248, 249, 250); // Fond carte
        private Color TextColor = Color.FromArgb(33, 37, 41);         // Texte principal

        public SupplierDetailsForm()
        {
            _supplierService = new SupplierService();
            _factureService = new FactureService();

            this.Text = "Gestion Fournisseurs & Factures";
            this.Size = new Size(1800, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(240, 245, 249); // Fond principal léger
            this.Padding = new Padding(15);

            CreateAllControls();
            this.Load += SupplierDetailsForm_Load;

            // S'abonner aux événements
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            EventBus.SupplierAdded += (s, e) => LoadSuppliersAsync();
            EventBus.SupplierUpdated += (s, e) => LoadSuppliersAsync();
            EventBus.SupplierDeleted += (s, e) => LoadSuppliersAsync();
        }

        private void UnsubscribeFromEvents()
        {
            EventBus.SupplierAdded -= (s, e) => LoadSuppliersAsync();
            EventBus.SupplierUpdated -= (s, e) => LoadSuppliersAsync();
            EventBus.SupplierDeleted -= (s, e) => LoadSuppliersAsync();
        }

        private void CreateAllControls()
        {
            // ==================== LEFT PANEL: SUPPLIERS ====================
            var leftPanel = new Panel
            {
                Location = new Point(15, 15),
                Size = new Size(880, 850),
                BackColor = CardBackground,
                Padding = new Padding(15),
                BorderStyle = BorderStyle.FixedSingle
            };

            var leftTitle = new Label
            {
                Text = "Fournisseurs",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(15, 10),
                AutoSize = true
            };
            leftPanel.Controls.Add(leftTitle);

            // ==================== SUPPLIERS DATAGRIDVIEW ====================
            dgvSuppliers = new DataGridView
            {
                Location = new Point(15, 50),
                Size = new Size(850, 250),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false
            };

            // Style amélioré pour le DataGridView
            dgvSuppliers.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            dgvSuppliers.ColumnHeadersHeight = 45;

            dgvSuppliers.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                ForeColor = TextColor,
                SelectionBackColor = Color.FromArgb(220, 237, 255),
                SelectionForeColor = TextColor
            };

            dgvSuppliers.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 250, 252)
            };

            // Colonnes sans email
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { Name = "ID", HeaderText = "ID", DataPropertyName = "ID", Width = 60 });
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "NOM", DataPropertyName = "Name", Width = 400 });
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Phone", HeaderText = "TELEPHONE", DataPropertyName = "Phone", Width = 200 });

            dgvSuppliers.SelectionChanged += (s, e) => DgvSuppliers_SelectionChanged();
            leftPanel.Controls.Add(dgvSuppliers);

            // ==================== SUPPLIERS INPUT SECTION ====================
            int inputY = 320;
            int col1X = 15;

            // Name
            var lblName = CreateLabel("Nom *", col1X, inputY);
            txtName = CreateTextBox(col1X, inputY + 25, 820, 40);
            leftPanel.Controls.Add(lblName);
            leftPanel.Controls.Add(txtName);

            inputY += 80;

            // Phone
            var lblPhone = CreateLabel("Téléphone", col1X, inputY);
            txtPhone = CreateTextBox(col1X, inputY + 25, 820, 40);

            leftPanel.Controls.Add(lblPhone);
            leftPanel.Controls.Add(txtPhone);

            inputY += 80;

            // ==================== SUPPLIERS BUTTONS - 2 PAR LIGNE ====================
            // Première ligne de boutons
            btnAddSupplier = CreateButton("Ajouter", col1X, inputY, SuccessColor);
            btnUpdateSupplier = CreateButton("Modifier", col1X + 210, inputY, InfoColor);

            // Deuxième ligne de boutons
            inputY += 70;
            btnDeleteSupplier = CreateButton("Supprimer", col1X, inputY, DangerColor);
            btnClearSupplier = CreateButton("Effacer", col1X + 210, inputY, WarningColor);

            btnAddSupplier.Click += BtnAddSupplier_Click;
            btnUpdateSupplier.Click += BtnUpdateSupplier_Click;
            btnDeleteSupplier.Click += BtnDeleteSupplier_Click;
            btnClearSupplier.Click += BtnClearSupplier_Click;

            leftPanel.Controls.Add(btnAddSupplier);
            leftPanel.Controls.Add(btnUpdateSupplier);
            leftPanel.Controls.Add(btnDeleteSupplier);
            leftPanel.Controls.Add(btnClearSupplier);

            this.Controls.Add(leftPanel);

            // ==================== RIGHT PANEL: FACTURES ====================
            var rightPanel = new Panel
            {
                Location = new Point(915, 15),
                Size = new Size(880, 850),
                BackColor = CardBackground,
                Padding = new Padding(15),
                BorderStyle = BorderStyle.FixedSingle
            };

            var rightTitle = new Label
            {
                Text = "Factures",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(15, 10),
                AutoSize = true
            };
            rightPanel.Controls.Add(rightTitle);

            // ==================== FACTURES DATAGRIDVIEW ====================
            dgvFactures = new DataGridView
            {
                Location = new Point(15, 50),
                Size = new Size(850, 250),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false
            };

            // Style amélioré pour le DataGridView des factures
            dgvFactures.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            dgvFactures.ColumnHeadersHeight = 45;

            dgvFactures.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                ForeColor = TextColor,
                SelectionBackColor = Color.FromArgb(220, 237, 255),
                SelectionForeColor = TextColor
            };

            dgvFactures.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 250, 252)
            };

            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn { Name = "ID", HeaderText = "ID", DataPropertyName = "Id", Width = 50 });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Number", HeaderText = "N° FACTURE", DataPropertyName = "Number", Width = 120 });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn { Name = "SupplierName", HeaderText = "FOURNISSEUR", DataPropertyName = "SupplierName", Width = 150 });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Amount", HeaderText = "MONTANT", DataPropertyName = "Amount", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Advance", HeaderText = "AVANCE", DataPropertyName = "Advance", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Remaining", HeaderText = "RESTE", DataPropertyName = "Remaining", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "STATUT", DataPropertyName = "Status", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });

            dgvFactures.SelectionChanged += (s, e) => DgvFactures_SelectionChanged();
            rightPanel.Controls.Add(dgvFactures);

            // ==================== FACTURES INPUT SECTION ====================
            inputY = 320;
            col1X = 15;

            // Fournisseur ComboBox - Ligne 1
            var lblSupplier = CreateLabel("Fournisseur *", col1X, inputY);
            cmbSupplier = new ComboBox
            {
                Location = new Point(col1X, inputY + 25),
                Size = new Size(400, 40),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Name",
                ValueMember = "ID",
                BackColor = Color.White,
                ForeColor = TextColor
            };
            cmbSupplier.SelectedIndexChanged += (s, e) => CmbSupplier_SelectedIndexChanged();

            // Numéro de facture - Ligne 1
            var lblNumber = CreateLabel("N° Facture *", col1X + 420, inputY);
            txtFactureNumber = CreateTextBox(col1X + 420, inputY + 25, 400, 40);

            rightPanel.Controls.Add(lblSupplier);
            rightPanel.Controls.Add(cmbSupplier);
            rightPanel.Controls.Add(lblNumber);
            rightPanel.Controls.Add(txtFactureNumber);

            inputY += 80;

            // Montant - Ligne 2
            var lblAmount = CreateLabel("Montant (DH) *", col1X, inputY);
            txtFactureAmount = CreateTextBox(col1X, inputY + 25, 400, 40);

            // Avance - Ligne 2
            var lblAdvance = CreateLabel("Avance (DH)", col1X + 420, inputY);
            txtFactureAdvance = CreateTextBox(col1X + 420, inputY + 25, 400, 40);
            txtFactureAdvance.Text = "0";

            rightPanel.Controls.Add(lblAmount);
            rightPanel.Controls.Add(txtFactureAmount);
            rightPanel.Controls.Add(lblAdvance);
            rightPanel.Controls.Add(txtFactureAdvance);

            inputY += 80;

            // Date Facture - Ligne 3
            var lblInvoiceDate = CreateLabel("Date Facture *", col1X, inputY);
            dtpInvoiceDate = new DateTimePicker
            {
                Location = new Point(col1X, inputY + 25),
                Size = new Size(400, 35),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                ForeColor = TextColor
            };

            // Date Échéance - Ligne 3
            var lblDueDate = CreateLabel("Échéance *", col1X + 420, inputY);
            dtpDueDate = new DateTimePicker
            {
                Location = new Point(col1X + 420, inputY + 25),
                Size = new Size(320, 35),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddDays(30),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                ForeColor = TextColor
            };

            rightPanel.Controls.Add(lblInvoiceDate);
            rightPanel.Controls.Add(dtpInvoiceDate);
            rightPanel.Controls.Add(lblDueDate);
            rightPanel.Controls.Add(dtpDueDate);

            inputY += 80;

            // ==================== FACTURES BUTTONS - 2 PAR LIGNE ====================
            // Première ligne de boutons
            btnAddFacture = CreateButton("Ajouter", col1X, inputY, SuccessColor);
            btnUpdateFacture = CreateButton("Modifier", col1X + 210, inputY, InfoColor);

            // Deuxième ligne de boutons
            inputY += 70;
            btnDeleteFacture = CreateButton("Supprimer", col1X, inputY, DangerColor);
            btnClearFacture = CreateButton("Effacer", col1X + 210, inputY, WarningColor);

            btnAddFacture.Click += BtnAddFacture_Click;
            btnUpdateFacture.Click += BtnUpdateFacture_Click;
            btnDeleteFacture.Click += BtnDeleteFacture_Click;
            btnClearFacture.Click += BtnClearFacture_Click;

            rightPanel.Controls.Add(btnAddFacture);
            rightPanel.Controls.Add(btnUpdateFacture);
            rightPanel.Controls.Add(btnDeleteFacture);
            rightPanel.Controls.Add(btnClearFacture);

            this.Controls.Add(rightPanel);
        }

        private void CmbSupplier_SelectedIndexChanged()
        {
            if (cmbSupplier.SelectedItem is Supplier selectedSupplier)
            {
                _selectedSupplier = selectedSupplier;
            }
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = TextColor,
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
                ForeColor = TextColor
            };
        }

        private Button CreateButton(string text, int x, int y, Color backColor)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(200, 50),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
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

        private async void SupplierDetailsForm_Load(object sender, EventArgs e)
        {
            try
            {
                await LoadSuppliersAsync();
                await LoadFacturesAsync();
                await LoadSuppliersComboBoxAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de chargement: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadSuppliersAsync()
        {
            try
            {
                _suppliers = _supplierService.GetAllSuppliers();
                dgvSuppliers.DataSource = new List<Supplier>(_suppliers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur LoadSuppliers: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadSuppliersComboBoxAsync()
        {
            try
            {
                cmbSupplier.DataSource = null;
                cmbSupplier.DataSource = _suppliers;
                cmbSupplier.DisplayMember = "Name";
                cmbSupplier.ValueMember = "ID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur LoadSuppliersComboBox: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadFacturesAsync()
        {
            try
            {
                _factures = _factureService.GetAllFactures();

                var displayList = new List<dynamic>();
                foreach (var facture in _factures)
                {
                    decimal remaining = facture.Amount - facture.Advance;
                    displayList.Add(new
                    {
                        Id = facture.Id,
                        Number = facture.Number,
                        SupplierName = facture.Supplier?.Name ?? "Inconnu",
                        Amount = facture.Amount,
                        Advance = facture.Advance,
                        Remaining = remaining,
                        Status = remaining <= 0 ? "Payée" : "En cours",
                        InvoiceDate = facture.InvoiceDate,
                        DueDate = facture.DueDate,
                        FactureObj = facture
                    });
                }

                dgvFactures.DataSource = displayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur LoadFactures: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== SUPPLIERS EVENTS ====================
        private void DgvSuppliers_SelectionChanged()
        {
            if (dgvSuppliers.SelectedRows.Count > 0)
            {
                _selectedSupplier = dgvSuppliers.SelectedRows[0].DataBoundItem as Supplier;
                FillSupplierForm(_selectedSupplier);

                // Mettre à jour le ComboBox dans la partie Facture
                if (_selectedSupplier != null)
                {
                    cmbSupplier.SelectedValue = _selectedSupplier.ID;
                }
            }
        }

        private void FillSupplierForm(Supplier supplier)
        {
            if (supplier == null) return;

            txtName.Text = supplier.Name ?? "";
            txtPhone.Text = supplier.Phone ?? "";
        }

        private void ClearSupplierForm()
        {
            txtName.Text = "";
            txtPhone.Text = "";
            _selectedSupplier = null;
            dgvSuppliers.ClearSelection();
        }

        private async void BtnAddSupplier_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Le nom est obligatoire", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var supplier = new Supplier
                {
                    Name = txtName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Contact = "",
                    Email = "",
                    Address = "",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                if (_supplierService.AddSupplier(supplier))
                {
                    await LoadSuppliersAsync();
                    await LoadSuppliersComboBoxAsync();
                    ClearSupplierForm();

                    // Déclencher les événements pour le dashboard
                    EventBus.OnSupplierAdded(this, supplier.ID, supplier.Name);
                    EventBus.OnDataChanged(this, "Nouveau fournisseur ajouté");

                    MessageBox.Show("Fournisseur ajouté avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnUpdateSupplier_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedSupplier == null)
                {
                    MessageBox.Show("Veuillez sélectionner un fournisseur", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Le nom est obligatoire", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _selectedSupplier.Name = txtName.Text.Trim();
                _selectedSupplier.Phone = txtPhone.Text.Trim();

                if (_supplierService.UpdateSupplier(_selectedSupplier))
                {
                    await LoadSuppliersAsync();
                    await LoadSuppliersComboBoxAsync();

                    if (_selectedSupplier != null && !string.IsNullOrEmpty(_selectedSupplier.Name))
                    {
                        // Déclencher les événements pour le dashboard
                        EventBus.OnSupplierUpdated(this, _selectedSupplier.ID, _selectedSupplier.Name);
                        EventBus.OnDataChanged(this, "Fournisseur modifié");
                    }

                    ClearSupplierForm();
                    MessageBox.Show("Fournisseur modifié avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDeleteSupplier_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedSupplier == null)
                {
                    MessageBox.Show("Veuillez sélectionner un fournisseur", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Vérifier s'il y a des factures liées à ce fournisseur
                bool hasFactures = _factures?.Any(f => f.SupplierId == _selectedSupplier.ID) ?? false;

                if (hasFactures)
                {
                    MessageBox.Show($"Impossible de supprimer {_selectedSupplier.Name} car il a des factures associées. Supprimez d'abord les factures liées.",
                                  "Suppression impossible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer {_selectedSupplier.Name}?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int supplierId = _selectedSupplier.ID;
                    string supplierName = _selectedSupplier.Name;

                    if (_supplierService.DeleteSupplier(supplierId))
                    {
                        await LoadSuppliersAsync();
                        await LoadSuppliersComboBoxAsync();
                        await LoadFacturesAsync();
                        ClearSupplierForm();

                        // Déclencher les événements pour le dashboard
                        EventBus.OnSupplierDeleted(this, supplierId, supplierName);
                        EventBus.OnDataChanged(this, "Fournisseur supprimé");

                        MessageBox.Show("Fournisseur supprimé avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la suppression du fournisseur", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearSupplier_Click(object sender, EventArgs e)
        {
            ClearSupplierForm();
        }

        // ==================== FACTURES EVENTS ====================
        private void DgvFactures_SelectionChanged()
        {
            if (dgvFactures.SelectedRows.Count > 0)
            {
                var displayItem = dgvFactures.SelectedRows[0].DataBoundItem;
                if (displayItem != null)
                {
                    dynamic item = displayItem;
                    _selectedFacture = item.FactureObj;
                    FillFactureForm(_selectedFacture);

                    // Sélectionner le fournisseur correspondant dans le ComboBox
                    if (_selectedFacture != null)
                    {
                        cmbSupplier.SelectedValue = _selectedFacture.SupplierId;
                        _selectedSupplier = _suppliers.FirstOrDefault(s => s.ID == _selectedFacture.SupplierId);
                    }
                }
            }
        }

        private void FillFactureForm(Facture facture)
        {
            if (facture == null) return;

            txtFactureNumber.Text = facture.Number;
            txtFactureAmount.Text = facture.Amount.ToString("N2");
            txtFactureAdvance.Text = facture.Advance.ToString("N2");
            dtpInvoiceDate.Value = facture.InvoiceDate;
            dtpDueDate.Value = facture.DueDate;
        }

        private void ClearFactureForm()
        {
            txtFactureNumber.Text = "";
            txtFactureAmount.Text = "";
            txtFactureAdvance.Text = "0";
            dtpInvoiceDate.Value = DateTime.Today;
            dtpDueDate.Value = DateTime.Today.AddDays(30);
            _selectedFacture = null;
            dgvFactures.ClearSelection();
        }

        private async void BtnAddFacture_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbSupplier.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez sélectionner un fournisseur", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtFactureNumber.Text))
                {
                    MessageBox.Show("Le numéro de facture est obligatoire", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtFactureAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Le montant doit être positif", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtFactureAdvance.Text, out decimal advance))
                    advance = 0;

                var selectedSupplier = cmbSupplier.SelectedItem as Supplier;
                var facture = new Facture
                {
                    Number = txtFactureNumber.Text.Trim(),
                    SupplierId = selectedSupplier.ID,
                    Amount = amount,
                    Advance = advance,
                    InvoiceDate = dtpInvoiceDate.Value,
                    DueDate = dtpDueDate.Value,
                    CreatedDate = DateTime.Now
                };

                if (_factureService.AddFacture(facture))
                {
                    await LoadFacturesAsync();
                    ClearFactureForm();

                    // Déclencher les événements pour le dashboard
                    EventBus.OnFactureAdded(this, facture.Id, facture.Number, facture.SupplierId, facture.Amount);
                    EventBus.OnDataChanged(this, "Nouvelle facture ajoutée");

                    MessageBox.Show("Facture ajoutée avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnUpdateFacture_Click(object sender, EventArgs e)
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

                if (string.IsNullOrWhiteSpace(txtFactureNumber.Text))
                {
                    MessageBox.Show("Le numéro de facture est obligatoire", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtFactureAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Le montant doit être positif", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtFactureAdvance.Text, out decimal advance))
                    advance = 0;

                var selectedSupplier = cmbSupplier.SelectedItem as Supplier;

                _selectedFacture.Number = txtFactureNumber.Text.Trim();
                _selectedFacture.SupplierId = selectedSupplier.ID;
                _selectedFacture.Amount = amount;
                _selectedFacture.Advance = advance;
                _selectedFacture.InvoiceDate = dtpInvoiceDate.Value;
                _selectedFacture.DueDate = dtpDueDate.Value;

                if (_factureService.UpdateFacture(_selectedFacture))
                {
                    await LoadFacturesAsync();
                    ClearFactureForm();

                    // Déclencher les événements pour le dashboard
                    EventBus.OnFactureUpdated(this, _selectedFacture.Id, _selectedFacture.Number, _selectedFacture.SupplierId, _selectedFacture.Amount);
                    EventBus.OnDataChanged(this, "Facture modifiée");

                    MessageBox.Show("Facture modifiée avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDeleteFacture_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedFacture == null)
                {
                    MessageBox.Show("Veuillez sélectionner une facture", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer la facture {_selectedFacture.Number}?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int factureId = _selectedFacture.Id;

                    if (_factureService.DeleteFacture(factureId))
                    {
                        await LoadFacturesAsync();
                        ClearFactureForm();

                        // Déclencher les événements pour le dashboard
                        EventBus.OnFactureDeleted(this, factureId, _selectedFacture.Number, _selectedFacture.SupplierId, _selectedFacture.Amount);
                        EventBus.OnDataChanged(this, "Facture supprimée");

                        MessageBox.Show("Facture supprimée avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearFacture_Click(object sender, EventArgs e)
        {
            ClearFactureForm();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnsubscribeFromEvents();
            base.OnFormClosing(e);
        }
    }
}