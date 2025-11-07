using GestionEmployes.Models;
using GestionEmployes.Services;
using GestionEmployes.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestionEmployes.Forms
{
    public partial class UnifiedEmployeeForm : Form
    {
        private readonly IEmployeService _employeService;
        private readonly IAvanceService _avanceService;
        private readonly IAbsenceService _absenceService;

        private List<Employe> _employes;
        private List<Avance> _avances;
        private List<Absence> _absences;
        private Employe _selectedEmploye;
        private dynamic _selectedOperation; // Pour stocker l'opération sélectionnée (Avance ou Absence)

        // Employee Section Controls
        private DataGridView dgvEmployees;
        private TextBox txtCin;
        private TextBox txtNom;
        private TextBox txtPrenom;
        private TextBox txtUtilisateur;
        private TextBox txtSalaire;
        private Button btnAddEmployee;
        private Button btnUpdateEmployee;
        private Button btnDeleteEmployee;
        private Button btnClearEmployee;
        private Label lblEmployeeCount;

        // Operation Section Controls
        private DataGridView dgvOperations;
        private ComboBox cmbEmployeSelect;
        private ComboBox cmbOperationType;
        private TextBox txtAmount;
        private DateTimePicker dtpDate;
        private Label lblAmountLabel;
        private Button btnAddOperation;
        private Button btnUpdateOperation;
        private Button btnDeleteOperation;
        private Button btnClearOperation;
        private Label lblOperationCount;

        // Split Container
        private SplitContainer splitContainer;

        // Nouvelles couleurs personnalisées
        private Color PrimaryColor = Color.FromArgb(41, 128, 185);    // Bleu principal
        private Color SecondaryColor = Color.FromArgb(52, 152, 219);  // Bleu secondaire
        private Color SuccessColor = Color.FromArgb(39, 174, 96);     // Vert succès
        private Color DangerColor = Color.FromArgb(231, 76, 60);      // Rouge danger
        private Color WarningColor = Color.FromArgb(230, 126, 34);    // Orange avertissement
        private Color InfoColor = Color.FromArgb(52, 152, 219);       // Bleu info
        private Color CardBackground = Color.FromArgb(248, 249, 250); // Fond carte
        private Color TextColor = Color.FromArgb(33, 37, 41);         // Texte principal
        private Color HeaderColor = Color.FromArgb(52, 73, 94);       // Couleur en-têtes (comme SupplierDetailsForm)
        private Color FilterPanelColor = Color.FromArgb(236, 240, 241); // Fond panneaux formulaire

        public UnifiedEmployeeForm(IEmployeService employeService, IAvanceService avanceService, IAbsenceService absenceService)
        {
            _employeService = employeService;
            _avanceService = avanceService;
            _absenceService = absenceService;

            this.Text = "Gestion Intégrée - Employés, Avances et Absences";
            this.Size = new Size(1700, 1000);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 245, 249);
            this.Padding = new Padding(10);
            this.AutoScroll = false;

            CreateSplitContainer();
            this.Load += UnifiedEmployeeForm_Load;
        }

        private void CreateSplitContainer()
        {
            // Create main split container
            splitContainer = new SplitContainer
            {
                Location = new Point(10, 10),
                Size = new Size(1650, 900),
                Orientation = Orientation.Vertical,
                SplitterDistance = 800, // Left panel width
                SplitterWidth = 8,
                BackColor = Color.FromArgb(240, 245, 249),
                BorderStyle = BorderStyle.None
            };

            // Left Panel - Employee Management
            var leftPanel = CreateCardPanel(0, 0, splitContainer.Panel1.Width, splitContainer.Panel1.Height, "Gestion des Employés");
            CreateEmployeeSection(leftPanel);
            splitContainer.Panel1.Controls.Add(leftPanel);

            // Right Panel - Operations Management
            var rightPanel = CreateCardPanel(0, 0, splitContainer.Panel2.Width, splitContainer.Panel2.Height, "Gestion des Avances et Absences");
            CreateOperationSection(rightPanel);
            splitContainer.Panel2.Controls.Add(rightPanel);

            this.Controls.Add(splitContainer);
        }

        // ======================== EMPLOYEE SECTION ========================
        private void CreateEmployeeSection(Panel card)
        {
            // DataGridView for Employees
            dgvEmployees = new DataGridView
            {
                Location = new Point(20, 90),
                Size = new Size(card.Width - 40, 280),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                Font = new Font("Segoe UI", 10F),
                RowHeadersVisible = false
            };

            // Style amélioré pour le DataGridView
            dgvEmployees.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            dgvEmployees.ColumnHeadersHeight = 45;
            dgvEmployees.EnableHeadersVisualStyles = false;

            dgvEmployees.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                ForeColor = TextColor,
                SelectionBackColor = Color.FromArgb(220, 237, 255),
                SelectionForeColor = TextColor
            };

            dgvEmployees.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 250, 252)
            };

            // Configure columns
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cin",
                HeaderText = "CIN",
                DataPropertyName = "Cin",
                Width = 120
            });

            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nom",
                HeaderText = "NOM",
                DataPropertyName = "Nom",
                Width = 150
            });

            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Prenom",
                HeaderText = "PRENOM",
                DataPropertyName = "Prenom",
                Width = 150
            });

            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Utilisateur",
                HeaderText = "UTILISATEUR",
                DataPropertyName = "Utilisateur",
                Width = 150
            });

            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Salaire",
                HeaderText = "SALAIRE",
                DataPropertyName = "Salaire",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvEmployees.SelectionChanged += (s, e) => DgvEmployees_SelectionChanged();
            card.Controls.Add(dgvEmployees);

            // Input fields - Row 1 (uniform spacing below grid: 50px)
            int inputY = 420;
            // Background panel for the employee form area
            var employeeFormBg = new Panel
            {
                Location = new Point(15, inputY),
                Size = new Size(card.Width - 40, 420),
                BackColor = FilterPanelColor,
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            card.Controls.Add(employeeFormBg);
            int innerY = 25; // extra top padding to avoid clipping
            int col1X = 30;  // extra left padding to avoid clipping first characters
            int empFieldWidth = (employeeFormBg.Width - 110) / 2; // leave a 50px gap + extra padding
            int col2X = col1X + empFieldWidth + 50;

            // CORRECTION: Labels avec fond blanc pour meilleure lisibilité
            var lblCin = CreateLabel("CIN *", col1X, innerY, empFieldWidth);
            txtCin = CreateTextBox(col1X, innerY + 25, empFieldWidth, 40);
            employeeFormBg.Controls.Add(lblCin);
            employeeFormBg.Controls.Add(txtCin);

            var lblNom = CreateLabel("Nom *", col2X, innerY, empFieldWidth);
            txtNom = CreateTextBox(col2X, innerY + 25, empFieldWidth, 40);
            employeeFormBg.Controls.Add(lblNom);
            employeeFormBg.Controls.Add(txtNom);

            // Input fields - Row 2
            innerY += 80;

            var lblPrenom = CreateLabel("Prénom *", col1X, innerY, empFieldWidth);
            txtPrenom = CreateTextBox(col1X, innerY + 25, empFieldWidth, 40);
            employeeFormBg.Controls.Add(lblPrenom);
            employeeFormBg.Controls.Add(txtPrenom);

            var lblUtilisateur = CreateLabel("Utilisateur *", col2X, innerY, empFieldWidth);
            txtUtilisateur = CreateTextBox(col2X, innerY + 25, empFieldWidth, 40);
            employeeFormBg.Controls.Add(lblUtilisateur);
            employeeFormBg.Controls.Add(txtUtilisateur);

            // Input fields - Row 3
            innerY += 80;

            var lblSalaire = CreateLabel("Salaire (DH)", col1X, innerY, empFieldWidth);
            txtSalaire = CreateTextBox(col1X, innerY + 25, empFieldWidth, 40);
            employeeFormBg.Controls.Add(lblSalaire);
            employeeFormBg.Controls.Add(txtSalaire);

            // Buttons - 2 per row
            innerY += 100;
            int empButtonWidth = 180;
            int empButtonHeight = 50;
            int empColSpacing = 200; // x offset between columns

            btnAddEmployee = CreateButton("Ajouter", col1X, innerY, SuccessColor);
            btnAddEmployee.Size = new Size(empButtonWidth, empButtonHeight);

            btnUpdateEmployee = CreateButton("Modifier", col1X + empColSpacing, innerY, InfoColor);
            btnUpdateEmployee.Size = new Size(empButtonWidth, empButtonHeight);

            btnDeleteEmployee = CreateButton("Supprimer", col1X, innerY + 60, DangerColor);
            btnDeleteEmployee.Size = new Size(empButtonWidth, empButtonHeight);

            btnClearEmployee = CreateButton("Effacer", col1X + empColSpacing, innerY + 60, WarningColor);
            btnClearEmployee.Size = new Size(empButtonWidth, empButtonHeight);

            btnAddEmployee.Click += BtnAddEmployee_Click;
            btnUpdateEmployee.Click += BtnUpdateEmployee_Click;
            btnDeleteEmployee.Click += BtnDeleteEmployee_Click;
            btnClearEmployee.Click += BtnClearEmployee_Click;

            employeeFormBg.Controls.Add(btnAddEmployee);
            employeeFormBg.Controls.Add(btnUpdateEmployee);
            employeeFormBg.Controls.Add(btnDeleteEmployee);
            employeeFormBg.Controls.Add(btnClearEmployee);

            // Employee count label
            lblEmployeeCount = new Label
            {
                Location = new Point(20, employeeFormBg.Bottom + 12),
                Size = new Size(card.Width - 40, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Text = "Employés: 0",
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblEmployeeCount);
        }

        // ======================== OPERATION SECTION ========================
        private void CreateOperationSection(Panel card)
        {
            // Place DataGridView on top, form below (switched)
            // DataGridView for Operations on top
            int gridTopY = 90;
            // Top section - Form fields start below grid, with extra spacing
            int inputY = 420;
            int col1X = 20, col2X = 20;

            // Background panel for the operations form area
            var operationFormBg = new Panel
            {
                Location = new Point(15, inputY),
                Size = new Size(card.Width - 40, 420),
                BackColor = FilterPanelColor,
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            card.Controls.Add(operationFormBg);

            int opInnerY = 20;

            // CORRECTION: Label avec largeur spécifique
            var lblEmployeSelect = CreateLabel("Sélectionner Employé *", col1X, opInnerY, card.Width - 40);
            cmbEmployeSelect = new ComboBox
            {
                Location = new Point(col1X, opInnerY + 25),
                Size = new Size(card.Width - 480, 40),
                DisplayMember = "ToString",
                ValueMember = "Cin",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                FormattingEnabled = true,
                BackColor = Color.White,
                ForeColor = TextColor
            };
            cmbEmployeSelect.SelectedIndexChanged += (s, e) =>
            {
                UpdateOperationGrid();
                if (_selectedOperation == null) // Ne réinitialiser que si on n'est pas en mode modification
                {
                    RefreshOperationForm();
                }
            };
            operationFormBg.Controls.Add(lblEmployeSelect);
            operationFormBg.Controls.Add(cmbEmployeSelect);

            // Operation type and amount row
            opInnerY += 90;
            int fieldWidth = (operationFormBg.Width - 80) / 2; // leave a 40px gap between columns

            // CORRECTION: Labels avec largeur spécifique
            var lblOperationType = CreateLabel("Type d'Opération *", col1X, opInnerY, fieldWidth);
            cmbOperationType = new ComboBox
            {
                Location = new Point(col1X, opInnerY + 25),
                Size = new Size(fieldWidth, 40),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                FormattingEnabled = true,
                BackColor = Color.White,
                ForeColor = TextColor
            };
            cmbOperationType.Items.AddRange(new[] { "Avance", "Absence" });
            cmbOperationType.SelectedIndex = 0;
            cmbOperationType.SelectedIndexChanged += CmbOperationType_SelectedIndexChanged;
            operationFormBg.Controls.Add(lblOperationType);
            operationFormBg.Controls.Add(cmbOperationType);

            // Amount/Penalty field
            lblAmountLabel = CreateLabel("Montant (DH) *", col1X + fieldWidth + 40, opInnerY, fieldWidth);
            txtAmount = CreateTextBox(col1X + fieldWidth + 40, opInnerY + 25, fieldWidth, 40);
            operationFormBg.Controls.Add(lblAmountLabel);
            operationFormBg.Controls.Add(txtAmount);

            // Date picker
            opInnerY += 90;
            // CORRECTION: Label avec largeur spécifique
            var lblDate = CreateLabel("Date *", col1X, opInnerY, card.Width - 40);
            dtpDate = new DateTimePicker
            {
                Location = new Point(col1X, opInnerY + 25),
                Size = new Size(card.Width - 480, 40),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                ForeColor = TextColor
            };
            operationFormBg.Controls.Add(lblDate);
            operationFormBg.Controls.Add(dtpDate);

            // DataGridView for Operations
            dgvOperations = new DataGridView
            {
                Location = new Point(20, gridTopY),
                Size = new Size(card.Width - 40, 280),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                Font = new Font("Segoe UI", 10F),
                RowHeadersVisible = false
            };

            // Style amélioré pour le DataGridView des opérations
            dgvOperations.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            dgvOperations.ColumnHeadersHeight = 45;
            dgvOperations.EnableHeadersVisualStyles = false;

            dgvOperations.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                ForeColor = TextColor,
                SelectionBackColor = Color.FromArgb(220, 237, 255),
                SelectionForeColor = TextColor
            };

            dgvOperations.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 250, 252)
            };

            // Configure operation grid columns
            dgvOperations.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Type",
                HeaderText = "TYPE",
                DataPropertyName = "Type",
                Width = 150
            });

            dgvOperations.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Employe",
                HeaderText = "EMPLOYE",
                DataPropertyName = "EmployeeName",
                Width = 180
            });

            dgvOperations.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "MONTANT",
                DataPropertyName = "Amount",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvOperations.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Date",
                HeaderText = "DATE",
                DataPropertyName = "Date",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy",
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvOperations.SelectionChanged += (s, e) => DgvOperations_SelectionChanged();
            card.Controls.Add(dgvOperations);

            // Operation Buttons (2 per row for consistency)
            int buttonWidth = 180;
            int buttonHeight = 50;
            int opBtnY = opInnerY + 90;
            int opBtnColSpacing = 200;
            btnAddOperation = CreateButton("Ajouter", col2X, opBtnY, SuccessColor);
            btnUpdateOperation = CreateButton("Modifier", col2X + opBtnColSpacing, opBtnY, InfoColor);
            btnDeleteOperation = CreateButton("Supprimer", col2X, opBtnY + 60, DangerColor);
            btnClearOperation = CreateButton("Effacer", col2X + opBtnColSpacing, opBtnY + 60, WarningColor);

            // Adjust button sizes
            btnAddOperation.Size = new Size(buttonWidth, buttonHeight);
            btnUpdateOperation.Size = new Size(buttonWidth, buttonHeight);
            btnDeleteOperation.Size = new Size(buttonWidth, buttonHeight);
            btnClearOperation.Size = new Size(buttonWidth, buttonHeight);

            btnAddOperation.Click += BtnAddOperation_Click;
            btnUpdateOperation.Click += BtnUpdateOperation_Click;
            btnDeleteOperation.Click += BtnDeleteOperation_Click;
            btnClearOperation.Click += BtnClearOperation_Click;

            operationFormBg.Controls.Add(btnAddOperation);
            operationFormBg.Controls.Add(btnUpdateOperation);
            operationFormBg.Controls.Add(btnDeleteOperation);
            operationFormBg.Controls.Add(btnClearOperation);

            // Operation count label
            lblOperationCount = new Label
            {
                Location = new Point(20, operationFormBg.Bottom + 10),
                Size = new Size(card.Width - 40, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Text = "Opérations: 0",
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblOperationCount);
        }

        // ======================== HELPER METHODS ========================
        private Panel CreateCardPanel(int x, int y, int width, int height, string title)
        {
            var card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.White,
                Padding = new Padding(20),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Header panel styled like SupplierDetailsForm
            var headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(width - 40, 70),
                BackColor = HeaderColor,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            var titleLabel = new Label
            {
                Text = title.ToUpperInvariant(),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };
            headerPanel.Controls.Add(titleLabel);
            card.Controls.Add(headerPanel);

            return card;
        }

        // CORRECTION: Nouvelle méthode CreateLabel avec largeur paramétrable
        private Label CreateLabel(string text, int x, int y, int width)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = HeaderColor,
                AutoSize = false,
                Padding = new Padding(6, 0, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                //BackColor = Color.White, // CORRECTION: Fond blanc pour meilleure lisibilité
                UseCompatibleTextRendering = true
            };
        }

        // CORRECTION: Ancienne méthode conservée pour compatibilité
        private Label CreateLabel(string text, int x, int y)
        {
            return CreateLabel(text, x, y, 400); // Valeur par défaut
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
                Size = new Size(155, 50),
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

        // ======================== LOAD DATA ========================
        private async void UnifiedEmployeeForm_Load(object sender, EventArgs e)
        {
            try
            {
                await LoadAllData();
                UpdateLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur au chargement: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadAllData()
        {
            _employes = await _employeService.GetAllEmployesAsync();
            _avances = await _avanceService.GetAllAvancesAsync();
            _absences = await _absenceService.GetAllAbsencesAsync();

            dgvEmployees.DataSource = new List<Employe>(_employes);
            cmbEmployeSelect.DataSource = new List<Employe>(_employes);

            UpdateOperationGrid();
        }

        // ======================== EMPLOYEE SECTION - EVENTS ========================
        private void DgvEmployees_SelectionChanged()
        {
            if (dgvEmployees.SelectedRows.Count > 0)
            {
                _selectedEmploye = dgvEmployees.SelectedRows[0].DataBoundItem as Employe;
                FillEmployeeForm(_selectedEmploye);
            }
        }

        private void FillEmployeeForm(Employe employe)
        {
            if (employe == null) return;

            txtCin.Text = employe.Cin;
            txtNom.Text = employe.Nom;
            txtPrenom.Text = employe.Prenom;
            txtUtilisateur.Text = employe.Utilisateur;
            txtSalaire.Text = employe.Salaire?.ToString("N2") ?? "";
            txtCin.Enabled = false;
        }

        private void ClearEmployeeForm()
        {
            txtCin.Text = "";
            txtNom.Text = "";
            txtPrenom.Text = "";
            txtUtilisateur.Text = "";
            txtSalaire.Text = "";
            txtCin.Enabled = true;
            _selectedEmploye = null;
            dgvEmployees.ClearSelection();
        }

        private async void BtnAddEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCin.Text) || string.IsNullOrWhiteSpace(txtNom.Text) ||
                    string.IsNullOrWhiteSpace(txtPrenom.Text) || string.IsNullOrWhiteSpace(txtUtilisateur.Text))
                {
                    MessageBox.Show("CIN, Nom, Prénom et Utilisateur sont obligatoires", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal? salaire = null;
                if (!string.IsNullOrWhiteSpace(txtSalaire.Text))
                {
                    if (!decimal.TryParse(txtSalaire.Text, out decimal sal))
                    {
                        MessageBox.Show("Salaire doit être un nombre valide", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    salaire = sal;
                }

                var employe = new Employe(txtCin.Text, txtNom.Text, txtPrenom.Text, txtUtilisateur.Text, salaire);
                await _employeService.CreateEmployeAsync(employe);

                // Déclencher les événements pour le dashboard
                EventBus.OnEmployeAdded(this, employe.Cin, employe.Nom, employe.Prenom);
                EventBus.OnDataChanged(this, "Nouvel employé ajouté");

                await LoadAllData();
                ClearEmployeeForm();
                UpdateLabels();
                MessageBox.Show("Employé ajouté avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnUpdateEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedEmploye == null)
                {
                    MessageBox.Show("Veuillez sélectionner un employé", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtPrenom.Text))
                {
                    MessageBox.Show("Nom et Prénom sont obligatoires", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal? salaire = null;
                if (!string.IsNullOrWhiteSpace(txtSalaire.Text))
                {
                    if (!decimal.TryParse(txtSalaire.Text, out decimal sal))
                    {
                        MessageBox.Show("Salaire doit être un nombre valide", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    salaire = sal;
                }

                var employe = new Employe
                {
                    Cin = _selectedEmploye.Cin,
                    Nom = txtNom.Text,
                    Prenom = txtPrenom.Text,
                    Utilisateur = txtUtilisateur.Text,
                    Salaire = salaire
                };

                await _employeService.UpdateEmployeAsync(employe);

                // Déclencher les événements pour le dashboard
                EventBus.OnEmployeUpdated(this, employe.Cin, employe.Nom, employe.Prenom);
                EventBus.OnDataChanged(this, "Employé modifié");

                await LoadAllData();
                ClearEmployeeForm();
                UpdateLabels();
                MessageBox.Show("Employé modifié avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDeleteEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedEmploye == null)
                {
                    MessageBox.Show("Veuillez sélectionner un employé", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Vérifier s'il y a des avances ou absences liées à cet employé
                bool hasAvances = _avances?.Any(a => a.EmployeCin == _selectedEmploye.Cin) ?? false;
                bool hasAbsences = _absences?.Any(a => a.EmployeCin == _selectedEmploye.Cin) ?? false;

                if (hasAvances || hasAbsences)
                {
                    MessageBox.Show($"Impossible de supprimer {_selectedEmploye.Nom} {_selectedEmploye.Prenom} car il a des avances ou absences associées. Supprimez d'abord les opérations liées.",
                                  "Suppression impossible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer {_selectedEmploye.Nom} {_selectedEmploye.Prenom}?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    await _employeService.DeleteEmployeAsync(_selectedEmploye.Cin);

                    // Déclencher les événements pour le dashboard
                    EventBus.OnEmployeDeleted(this, _selectedEmploye.Cin, _selectedEmploye.Nom, _selectedEmploye.Prenom);
                    EventBus.OnDataChanged(this, "Employé supprimé");

                    await LoadAllData();
                    ClearEmployeeForm();
                    UpdateLabels();
                    MessageBox.Show("Employé supprimé avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearEmployee_Click(object sender, EventArgs e)
        {
            ClearEmployeeForm();
        }

        // ======================== OPERATION SECTION - EVENTS ========================
        private void CmbOperationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string operationType = cmbOperationType.SelectedItem?.ToString() ?? "Avance";
            lblAmountLabel.Text = operationType == "Avance" ? "Montant (DH) *" : "Pénalité (DH) *";
        }

        private void RefreshOperationForm()
        {
            txtAmount.Text = "";
            dtpDate.Value = DateTime.Today;
            cmbOperationType.Enabled = true;
            _selectedOperation = null;
        }

        private void UpdateOperationGrid()
        {
            var employe = cmbEmployeSelect.SelectedItem as Employe;
            if (employe == null)
            {
                dgvOperations.DataSource = null;
                return;
            }

            var operations = new List<dynamic>();

            // Ajouter les avances
            foreach (var avance in _avances.Where(a => a.EmployeCin == employe.Cin))
            {
                var emp = _employes.FirstOrDefault(e => e.Cin == avance.EmployeCin);
                operations.Add(new
                {
                    Type = "Avance",
                    EmployeeName = emp != null ? emp.Nom + " " + emp.Prenom : "",
                    Amount = avance.Montant,
                    Date = avance.DateAvance,
                    Id = avance.Id,
                    OperationType = "Avance",
                    EmployeCin = avance.EmployeCin
                });
            }

            // Ajouter les absences
            foreach (var absence in _absences.Where(a => a.EmployeCin == employe.Cin))
            {
                var emp = _employes.FirstOrDefault(e => e.Cin == absence.EmployeCin);
                operations.Add(new
                {
                    Type = "Absence",
                    EmployeeName = emp != null ? emp.Nom + " " + emp.Prenom : "",
                    Amount = absence.Penalite,
                    Date = absence.DateAbsence,
                    Id = absence.Id,
                    OperationType = "Absence",
                    EmployeCin = absence.EmployeCin
                });
            }

            dgvOperations.DataSource = operations.OrderByDescending(o => o.Date).ToList();
        }

        private void ClearOperationForm()
        {
            txtAmount.Text = "";
            dtpDate.Value = DateTime.Today;
            cmbOperationType.SelectedIndex = 0;
            cmbOperationType.Enabled = true; // Réactiver le changement de type
            _selectedOperation = null;
            dgvOperations.ClearSelection();
        }

        private void DgvOperations_SelectionChanged()
        {
            if (dgvOperations.SelectedRows.Count > 0)
            {
                _selectedOperation = dgvOperations.SelectedRows[0].DataBoundItem;
                FillOperationForm(_selectedOperation);
            }
        }

        private void FillOperationForm(dynamic operation)
        {
            if (operation == null) return;

            string operationType = operation.GetType().GetProperty("OperationType").GetValue(operation).ToString();
            decimal amount = (decimal)operation.GetType().GetProperty("Amount").GetValue(operation);
            DateTime date = (DateTime)operation.GetType().GetProperty("Date").GetValue(operation);
            string employeCin = operation.GetType().GetProperty("EmployeCin")?.GetValue(operation)?.ToString();

            // Sélectionner l'employé correspondant si disponible
            if (!string.IsNullOrEmpty(employeCin))
            {
                var employe = _employes.FirstOrDefault(e => e.Cin == employeCin);
                if (employe != null)
                {
                    cmbEmployeSelect.SelectedItem = employe;
                }
            }

            cmbOperationType.SelectedItem = operationType;
            cmbOperationType.Enabled = false; // Désactiver le changement de type lors de la modification
            txtAmount.Text = amount.ToString("N2");
            dtpDate.Value = date;
        }

        private async void BtnAddOperation_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedOperation != null)
                {
                    MessageBox.Show("Veuillez d'abord effacer le formulaire ou utiliser le bouton Modifier", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var employe = cmbEmployeSelect.SelectedItem as Employe;
                if (employe == null)
                {
                    MessageBox.Show("Veuillez sélectionner un employé", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Montant/Pénalité doit être un nombre positif", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string operationType = cmbOperationType.SelectedItem?.ToString() ?? "Avance";

                if (operationType == "Avance")
                {
                    var avance = new Avance(amount, dtpDate.Value.Date, employe);
                    await _avanceService.CreateAvanceAsync(avance);

                    // Déclencher les événements pour le dashboard
                    EventBus.OnAvanceAdded(this, avance.Id, employe.Cin, avance.Montant);
                    EventBus.OnDataChanged(this, "Nouvelle avance ajoutée");
                }
                else
                {
                    var absence = new Absence(amount, dtpDate.Value.Date, employe);
                    await _absenceService.CreateAbsenceAsync(absence);

                    // Déclencher les événements pour le dashboard
                    EventBus.OnAbsenceAdded(this, absence.Id, employe.Cin, absence.Penalite);
                    EventBus.OnDataChanged(this, "Nouvelle absence ajoutée");
                }

                await LoadAllData();
                UpdateOperationGrid();
                ClearOperationForm();
                UpdateLabels();
                MessageBox.Show("Opération ajoutée avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnUpdateOperation_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedOperation == null)
                {
                    MessageBox.Show("Veuillez sélectionner une opération à modifier", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var employe = cmbEmployeSelect.SelectedItem as Employe;
                if (employe == null)
                {
                    MessageBox.Show("Veuillez sélectionner un employé", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Montant/Pénalité doit être un nombre positif", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string operationType = _selectedOperation.GetType().GetProperty("OperationType").GetValue(_selectedOperation).ToString();
                long id = (long)_selectedOperation.GetType().GetProperty("Id").GetValue(_selectedOperation);

                if (operationType == "Avance")
                {
                    var avance = await _avanceService.GetAvanceByIdAsync(id);
                    if (avance == null)
                    {
                        MessageBox.Show("Avance introuvable", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    avance.Montant = amount;
                    avance.DateAvance = dtpDate.Value.Date;
                    avance.EmployeCin = employe.Cin;

                    await _avanceService.UpdateAvanceAsync(avance);

                    // Déclencher les événements pour le dashboard
                    EventBus.OnAvanceUpdated(this, avance.Id, employe.Cin, avance.Montant);
                    EventBus.OnDataChanged(this, "Avance modifiée");
                }
                else
                {
                    var absence = await _absenceService.GetAbsenceByIdAsync(id);
                    if (absence == null)
                    {
                        MessageBox.Show("Absence introuvable", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    absence.Penalite = amount;
                    absence.DateAbsence = dtpDate.Value.Date;
                    absence.EmployeCin = employe.Cin;

                    await _absenceService.UpdateAbsenceAsync(absence);

                    // Déclencher les événements pour le dashboard
                    EventBus.OnAbsenceUpdated(this, absence.Id, employe.Cin, absence.Penalite);
                    EventBus.OnDataChanged(this, "Absence modifiée");
                }

                await LoadAllData();
                UpdateOperationGrid();
                ClearOperationForm();
                UpdateLabels();
                MessageBox.Show("Opération modifiée avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDeleteOperation_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvOperations.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner une opération", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer cette opération?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var selectedRow = dgvOperations.SelectedRows[0];
                    var dataItem = selectedRow.DataBoundItem;
                    var operationType = dataItem.GetType().GetProperty("OperationType").GetValue(dataItem).ToString();
                    var id = (long)dataItem.GetType().GetProperty("Id").GetValue(dataItem);
                    var amount = (decimal)dataItem.GetType().GetProperty("Amount").GetValue(dataItem);
                    var employeName = dataItem.GetType().GetProperty("EmployeeName").GetValue(dataItem).ToString();

                    // Trouver l'employé correspondant
                    var employe = _employes.FirstOrDefault(em => $"{em.Nom} {em.Prenom}" == employeName);

                    if (operationType == "Avance")
                    {
                        await _avanceService.DeleteAvanceAsync(id);

                        // Déclencher les événements pour le dashboard
                        if (employe != null)
                        {
                            EventBus.OnAvanceDeleted(this, id, employe.Cin, amount);
                            EventBus.OnDataChanged(this, "Avance supprimée");
                        }
                    }
                    else
                    {
                        await _absenceService.DeleteAbsenceAsync(id);

                        // Déclencher les événements pour le dashboard
                        if (employe != null)
                        {
                            EventBus.OnAbsenceDeleted(this, id, employe.Cin, amount);
                            EventBus.OnDataChanged(this, "Absence supprimée");
                        }
                    }

                    await LoadAllData();
                    UpdateOperationGrid();
                    UpdateLabels();
                    MessageBox.Show("Opération supprimée avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearOperation_Click(object sender, EventArgs e)
        {
            ClearOperationForm();
        }

        // ======================== UPDATE LABELS ========================
        private void UpdateLabels()
        {
            lblEmployeeCount.Text = $"Employés: {_employes?.Count ?? 0}";
            lblOperationCount.Text = $"Opérations: {(_avances?.Count ?? 0) + (_absences?.Count ?? 0)}";
        }
    }
}