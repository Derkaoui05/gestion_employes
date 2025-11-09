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
        private TextBox txtFactureAdvance; // LECTURE SEULE maintenant
        private DateTimePicker dtpInvoiceDate;
        private DateTimePicker dtpDueDate;
        private Button btnAddFacture;
        private Button btnViewPayments; // NOUVEAU : Remplace "Modifier"
        private Button btnDeleteFacture;
        private Button btnClearFacture;

        // Nouvelles couleurs personnalisées
        private Color PrimaryColor = Color.FromArgb(41, 128, 185);
        private Color SecondaryColor = Color.FromArgb(52, 152, 219);
        private Color SuccessColor = Color.FromArgb(39, 174, 96);
        private Color DangerColor = Color.FromArgb(231, 76, 60);
        private Color WarningColor = Color.FromArgb(230, 126, 34);
        private Color InfoColor = Color.FromArgb(52, 152, 219);
        private Color CardBackground = Color.FromArgb(248, 249, 250);
        private Color TextColor = Color.FromArgb(33, 37, 41);
        private Color HeaderColor = Color.FromArgb(52, 73, 94);
        private Color FilterPanelColor = Color.FromArgb(236, 240, 241);

        public SupplierDetailsForm()
        {
            _supplierService = new SupplierService();
            _factureService = new FactureService();

            this.Text = "Gestion Fournisseurs & Factures";
            this.Size = new Size(1800, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(240, 245, 249);
            this.Padding = new Padding(15);
            this.AutoScroll = false;

            CreateAllControls();
            this.Load += SupplierDetailsForm_Load;
            dgvFactures.DataBindingComplete += DgvFactures_DataBindingComplete;

            SubscribeToEvents();
        }

        private void DgvFactures_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvFactures.Rows)
            {
                if (row.Cells["Status"].Value != null)
                {
                    string status = row.Cells["Status"].Value.ToString();

                    if (status == "Non payée")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(231, 76, 60);
                    else if (status == "En cours")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(230, 126, 34);
                    else if (status == "Payée")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(39, 174, 96);

                    row.DefaultCellStyle.ForeColor = Color.White;
                }
            }
        }

        private void SubscribeToEvents()
        {
            EventBus.SupplierAdded += OnSupplierAdded;
            EventBus.SupplierUpdated += OnSupplierUpdated;
            EventBus.SupplierDeleted += OnSupplierDeleted;
            EventBus.FactureAdded += OnFactureAdded;
            EventBus.FactureUpdated += OnFactureUpdated;
            EventBus.FactureDeleted += OnFactureDeleted;
        }

        private void UnsubscribeFromEvents()
        {
            EventBus.SupplierAdded -= OnSupplierAdded;
            EventBus.SupplierUpdated -= OnSupplierUpdated;
            EventBus.SupplierDeleted -= OnSupplierDeleted;
            EventBus.FactureAdded -= OnFactureAdded;
            EventBus.FactureUpdated -= OnFactureUpdated;
            EventBus.FactureDeleted -= OnFactureDeleted;
        }

        private void OnSupplierAdded(object sender, SupplierEventArgs e)
        {
            LoadSuppliersAsync();
        }

        private void OnSupplierUpdated(object sender, SupplierEventArgs e)
        {
            LoadSuppliersAsync();
        }

        private void OnSupplierDeleted(object sender, SupplierEventArgs e)
        {
            LoadSuppliersAsync();
        }

        private void OnFactureAdded(object sender, FactureEventArgs e)
        {
            LoadFacturesAsync();
        }

        private void OnFactureUpdated(object sender, FactureEventArgs e)
        {
            LoadFacturesAsync();
        }

        private void OnFactureDeleted(object sender, FactureEventArgs e)
        {
            LoadFacturesAsync();
        }

        private void CreateAllControls()
        {
            // ==================== SPLIT CONTAINER - 50% / 50% ====================
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterWidth = 8,
                BackColor = Color.FromArgb(240, 245, 249),
                BorderStyle = BorderStyle.None,
                FixedPanel = FixedPanel.None
            };

            var splitContainerRef = splitContainer;

            this.Load += (s, e) =>
            {
                if (splitContainerRef.Width > 0)
                {
                    splitContainerRef.SplitterDistance = splitContainerRef.Width / 2;
                }
            };

            bool userMovedSplitter = false;
            splitContainerRef.SplitterMoved += (s, e) => { userMovedSplitter = true; };

            splitContainerRef.Resize += (s, e) =>
            {
                if (!userMovedSplitter && splitContainerRef.Width > 0)
                {
                    splitContainerRef.SplitterDistance = splitContainerRef.Width / 2;
                }
            };

            // ==================== LEFT PANEL: SUPPLIERS ====================
            var leftPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Header Suppliers
            var leftHeaderPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(leftPanel.Width + 40, 70),
                BackColor = HeaderColor,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            var leftTitle = new Label
            {
                Text = "GESTION DES FOURNISSEURS",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };
            leftHeaderPanel.Controls.Add(leftTitle);
            leftPanel.Controls.Add(leftHeaderPanel);

            // ==================== SUPPLIERS DATAGRIDVIEW ====================
            dgvSuppliers = new DataGridView
            {
                Location = new Point(20, 90),
                Size = new Size(leftPanel.Width - 60, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                RowHeadersVisible = false
            };

            dgvSuppliers.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            dgvSuppliers.ColumnHeadersHeight = 45;
            dgvSuppliers.EnableHeadersVisualStyles = false;

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

            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ID",
                HeaderText = "ID",
                DataPropertyName = "ID",
                Width = 80
            });
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "NOM DU FOURNISSEUR",
                DataPropertyName = "Name",
                Width = 300
            });
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Phone",
                HeaderText = "TÉLÉPHONE",
                DataPropertyName = "Phone",
                Width = 400
            });

            dgvSuppliers.SelectionChanged += (s, e) => DgvSuppliers_SelectionChanged();
            leftPanel.Controls.Add(dgvSuppliers);

            // ==================== SUPPLIERS INPUT SECTION ====================
            int inputY = 410;
            int col1X = 20;
            int controlWidth = 400;
            int controlHeight = 40;

            var formPanel = new Panel
            {
                Location = new Point(15, inputY - 10),
                Size = new Size(leftPanel.Width - 50, 400),
                BackColor = FilterPanelColor,
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var lblName = CreateLabel("NOM DU FOURNISSEUR *", col1X, 20);
            txtName = CreateTextBox(col1X, 50, controlWidth, controlHeight);
            formPanel.Controls.Add(lblName);
            formPanel.Controls.Add(txtName);

            var lblPhone = CreateLabel("TÉLÉPHONE *", col1X, 110);
            txtPhone = CreateTextBox(col1X, 140, controlWidth, controlHeight);
            formPanel.Controls.Add(lblPhone);
            formPanel.Controls.Add(txtPhone);

            // ==================== SUPPLIERS BUTTONS ====================
            int buttonY = 200;
            int buttonWidth = 180;
            int buttonHeight = 45;

            btnAddSupplier = CreateButton("AJOUTER", col1X, buttonY, buttonWidth, buttonHeight, SuccessColor);
            btnUpdateSupplier = CreateButton("MODIFIER", col1X + 200, buttonY, buttonWidth, buttonHeight, InfoColor);
            btnDeleteSupplier = CreateButton("SUPPRIMER", col1X, buttonY + 60, buttonWidth, buttonHeight, DangerColor);
            btnClearSupplier = CreateButton("EFFACER", col1X + 200, buttonY + 60, buttonWidth, buttonHeight, WarningColor);

            btnAddSupplier.Click += BtnAddSupplier_Click;
            btnUpdateSupplier.Click += BtnUpdateSupplier_Click;
            btnDeleteSupplier.Click += BtnDeleteSupplier_Click;
            btnClearSupplier.Click += BtnClearSupplier_Click;

            formPanel.Controls.Add(btnAddSupplier);
            formPanel.Controls.Add(btnUpdateSupplier);
            formPanel.Controls.Add(btnDeleteSupplier);
            formPanel.Controls.Add(btnClearSupplier);

            leftPanel.Controls.Add(formPanel);
            splitContainer.Panel1.Controls.Add(leftPanel);

            // ==================== RIGHT PANEL: FACTURES ====================
            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20),
                BorderStyle = BorderStyle.FixedSingle
            };

            var rightHeaderPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(rightPanel.Width + 40, 70),
                BackColor = HeaderColor,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            var rightTitle = new Label
            {
                Text = "GESTION DES FACTURES",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true,
            };
            rightHeaderPanel.Controls.Add(rightTitle);
            rightPanel.Controls.Add(rightHeaderPanel);

            // ==================== FACTURES DATAGRIDVIEW ====================
            dgvFactures = new DataGridView
            {
                Location = new Point(20, 90),
                Size = new Size(rightPanel.Width - 60, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                RowHeadersVisible = false
            };

            dgvFactures.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            dgvFactures.ColumnHeadersHeight = 45;
            dgvFactures.EnableHeadersVisualStyles = false;

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

            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ID",
                HeaderText = "ID",
                DataPropertyName = "Id",
                Width = 60
            });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Number",
                HeaderText = "N° FACTURE",
                DataPropertyName = "Number",
                Width = 100
            });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SupplierName",
                HeaderText = "FOURNISSEUR",
                DataPropertyName = "SupplierName",
                Width = 130
            });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "MONTANT",
                DataPropertyName = "Amount",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Advance",
                HeaderText = "AVANCE",
                DataPropertyName = "Advance",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Remaining",
                HeaderText = "RESTE",
                DataPropertyName = "Remaining",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "STATUT",
                DataPropertyName = "Status",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvFactures.SelectionChanged += (s, e) => DgvFactures_SelectionChanged();
            rightPanel.Controls.Add(dgvFactures);

            // ==================== FACTURES INPUT SECTION ====================
            inputY = 410;
            col1X = 20;
            int col2X = 450;
            controlWidth = 380;
            controlHeight = 40;

            var factureFormPanel = new Panel
            {
                Location = new Point(15, inputY - 10),
                Size = new Size(rightPanel.Width - 50, 410),
                BackColor = FilterPanelColor,
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Ligne 1: Fournisseur + N° Facture
            var lblSupplier = CreateLabel("FOURNISSEUR *", col1X, 20);
            cmbSupplier = new ComboBox
            {
                Location = new Point(col1X, 50),
                Width = controlWidth - 70,
                Height = controlHeight,
                Font = new Font("Segoe UI", 11F),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Name",
                ValueMember = "ID",
                BackColor = Color.White,
                ForeColor = TextColor
            };
            cmbSupplier.SelectedIndexChanged += (s, e) => CmbSupplier_SelectedIndexChanged();

            var lblNumber = CreateLabel("N° FACTURE *", col2X, 20);
            txtFactureNumber = CreateTextBox(col2X, 50, controlWidth - 70, controlHeight);

            factureFormPanel.Controls.Add(lblSupplier);
            factureFormPanel.Controls.Add(cmbSupplier);
            factureFormPanel.Controls.Add(lblNumber);
            factureFormPanel.Controls.Add(txtFactureNumber);

            // Ligne 2: Montant + Avance (LECTURE SEULE)
            var lblAmount = CreateLabel("MONTANT (DH) *", col1X, 110);
            txtFactureAmount = CreateTextBox(col1X, 140, controlWidth - 70, controlHeight);

            var lblAdvance = CreateLabel("AVANCE TOTALE (DH)", col2X, 110);
            txtFactureAdvance = CreateTextBox(col2X, 140, controlWidth - 70, controlHeight);
            txtFactureAdvance.ReadOnly = true; // LECTURE SEULE
            txtFactureAdvance.BackColor = Color.FromArgb(240, 240, 240);
            txtFactureAdvance.Text = "0.00";

            factureFormPanel.Controls.Add(lblAmount);
            factureFormPanel.Controls.Add(txtFactureAmount);
            factureFormPanel.Controls.Add(lblAdvance);
            factureFormPanel.Controls.Add(txtFactureAdvance);

            // Ligne 3: Date Facture + Échéance
            var lblInvoiceDate = CreateLabel("DATE FACTURE *", col1X, 200);
            dtpInvoiceDate = new DateTimePicker
            {
                Location = new Point(col1X, 230),
                Width = controlWidth - 70,
                Height = controlHeight,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.White,
                ForeColor = TextColor
            };

            var lblDueDate = CreateLabel("ÉCHÉANCE *", col2X, 200);
            dtpDueDate = new DateTimePicker
            {
                Location = new Point(col2X, 230),
                Width = controlWidth - 70,
                Height = controlHeight,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddDays(30),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.White,
                ForeColor = TextColor
            };

            factureFormPanel.Controls.Add(lblInvoiceDate);
            factureFormPanel.Controls.Add(dtpInvoiceDate);
            factureFormPanel.Controls.Add(lblDueDate);
            factureFormPanel.Controls.Add(dtpDueDate);

            // ==================== FACTURES BUTTONS (MODIFIÉS) ====================
            int factureButtonY = 290;
            int factureButtonWidth = 180;
            int factureButtonHeight = 45;

            btnAddFacture = CreateButton("AJOUTER", col1X, factureButtonY, factureButtonWidth, factureButtonHeight, SuccessColor);

            // REMPLACE "MODIFIER" par "VOIR PAIEMENTS"
            btnViewPayments = CreateButton("💳 PAIEMENTS", col1X + 200, factureButtonY, factureButtonWidth, factureButtonHeight, InfoColor);

            btnDeleteFacture = CreateButton("SUPPRIMER", col1X, factureButtonY + 60, factureButtonWidth, factureButtonHeight, DangerColor);
            btnClearFacture = CreateButton("EFFACER", col1X + 200, factureButtonY + 60, factureButtonWidth, factureButtonHeight, WarningColor);

            btnAddFacture.Click += BtnAddFacture_Click;
            btnViewPayments.Click += BtnViewPayments_Click; // NOUVEAU
            btnDeleteFacture.Click += BtnDeleteFacture_Click;
            btnClearFacture.Click += BtnClearFacture_Click;

            factureFormPanel.Controls.Add(btnAddFacture);
            factureFormPanel.Controls.Add(btnViewPayments);
            factureFormPanel.Controls.Add(btnDeleteFacture);
            factureFormPanel.Controls.Add(btnClearFacture);

            rightPanel.Controls.Add(factureFormPanel);
            splitContainer.Panel2.Controls.Add(rightPanel);

            this.Controls.Add(splitContainer);
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
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = HeaderColor,
                AutoSize = false
            };
        }

        private TextBox CreateTextBox(int x, int y, int width, int height)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                Font = new Font("Segoe UI", 11F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = TextColor
            };
        }

        private Button CreateButton(string text, int x, int y, int width, int height, Color backColor)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
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
                        Number = facture.Number ?? "",
                        SupplierName = facture.Supplier?.Name ?? "Inconnu",
                        Amount = facture.Amount,
                        Advance = facture.Advance,
                        Remaining = remaining,
                        Status = remaining == facture.Amount ? "Non payée" : (remaining > 0 ? "En cours" : "Payée"),
                        InvoiceDate = facture.InvoiceDate,
                        DueDate = facture.DueDate,
                        FactureObj = facture
                    });
                }

                dgvFactures.DataSource = displayList;
                _selectedFacture = null;
                ClearFactureForm();
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

                    if (_selectedSupplier != null)
                    {
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
            try
            {
                if (dgvFactures.SelectedRows.Count > 0 && dgvFactures.SelectedRows[0].DataBoundItem != null)
                {
                    var displayItem = dgvFactures.SelectedRows[0].DataBoundItem;

                    var factureObjProperty = displayItem.GetType().GetProperty("FactureObj");
                    if (factureObjProperty != null)
                    {
                        _selectedFacture = factureObjProperty.GetValue(displayItem) as Facture;

                        if (_selectedFacture != null)
                        {
                            FillFactureForm(_selectedFacture);

                            cmbSupplier.SelectedValue = _selectedFacture.SupplierId;
                            _selectedSupplier = _suppliers.FirstOrDefault(s => s.ID == _selectedFacture.SupplierId);
                        }
                    }
                }
                else
                {
                    _selectedFacture = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sélection: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _selectedFacture = null;
            }
        }

        private void FillFactureForm(Facture facture)
        {
            if (facture == null) return;

            txtFactureNumber.Text = facture.Number;
            txtFactureAmount.Text = facture.Amount.ToString("N2");
            txtFactureAdvance.Text = facture.Advance.ToString("N2"); // Affichage en lecture seule
            dtpInvoiceDate.Value = facture.InvoiceDate;
            dtpDueDate.Value = facture.DueDate;
        }

        private void ClearFactureForm()
        {
            txtFactureNumber.Text = "";
            txtFactureAmount.Text = "";
            txtFactureAdvance.Text = "0.00";
            dtpInvoiceDate.Value = DateTime.Today;
            dtpDueDate.Value = DateTime.Today.AddDays(30);

            _selectedFacture = null;

            if (dgvFactures != null)
            {
                dgvFactures.ClearSelection();
            }
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

                var selectedSupplier = cmbSupplier.SelectedItem as Supplier;
                var facture = new Facture
                {
                    Number = txtFactureNumber.Text.Trim(),
                    SupplierId = selectedSupplier.ID,
                    Amount = amount,
                    Advance = 0, // Toujours 0 au départ
                    InvoiceDate = dtpInvoiceDate.Value,
                    DueDate = dtpDueDate.Value,
                    CreatedDate = DateTime.Now
                };

                if (_factureService.AddFacture(facture))
                {
                    await LoadFacturesAsync();
                    ClearFactureForm();

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

        // ==================== NOUVEAU: BOUTON VOIR PAIEMENTS ====================
        private async void BtnViewPayments_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedFacture == null)
                {
                    MessageBox.Show("Veuillez sélectionner une facture", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Ouvrir le formulaire de gestion des paiements
                var paymentsForm = new PaymentsForm(_selectedFacture);
                paymentsForm.ShowDialog();

                // Après fermeture du formulaire de paiements, recharger les données
                await LoadFacturesAsync();
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
                    string factureNumber = _selectedFacture.Number ?? string.Empty;
                    int supplierId = _selectedFacture.SupplierId;
                    decimal factureAmount = _selectedFacture.Amount;

                    if (_factureService.DeleteFacture(factureId))
                    {
                        EventBus.OnFactureDeleted(this, factureId, factureNumber, supplierId, factureAmount);

                        await LoadFacturesAsync();
                        ClearFactureForm();

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