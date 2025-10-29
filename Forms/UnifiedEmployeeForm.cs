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
                Location = new Point(20, 50),
                Size = new Size(card.Width - 40, 280),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                Font = new Font("Segoe UI", 10F)
            };

            // Style amélioré pour le DataGridView
            dgvEmployees.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            dgvEmployees.ColumnHeadersHeight = 45;

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

            // Input fields - Row 1
            int inputY = 350;
            int col1X = 20, col2X = (card.Width - 40) / 2 + 10;

            var lblCin = CreateLabel("CIN *", col1X, inputY);
            txtCin = CreateTextBox(col1X, inputY + 25, (card.Width - 60) / 2, 40);
            card.Controls.Add(lblCin);
            card.Controls.Add(txtCin);

            var lblNom = CreateLabel("Nom *", col2X, inputY);
            txtNom = CreateTextBox(col2X, inputY + 25, (card.Width - 60) / 2, 40);
            card.Controls.Add(lblNom);
            card.Controls.Add(txtNom);

            // Input fields - Row 2
            inputY += 80;

            var lblPrenom = CreateLabel("Prénom *", col1X, inputY);
            txtPrenom = CreateTextBox(col1X, inputY + 25, (card.Width - 60) / 2, 40);
            card.Controls.Add(lblPrenom);
            card.Controls.Add(txtPrenom);

            var lblUtilisateur = CreateLabel("Utilisateur *", col2X, inputY);
            txtUtilisateur = CreateTextBox(col2X, inputY + 25, (card.Width - 60) / 2, 40);
            card.Controls.Add(lblUtilisateur);
            card.Controls.Add(txtUtilisateur);

            // Input fields - Row 3
            inputY += 80;

            var lblSalaire = CreateLabel("Salaire (DH)", col1X, inputY);
            txtSalaire = CreateTextBox(col1X, inputY + 25, (card.Width - 60) / 2, 40);
            card.Controls.Add(lblSalaire);
            card.Controls.Add(txtSalaire);

            // Buttons - Row 4
            inputY += 100;
            int buttonWidth = (card.Width - 100) / 4;
            btnAddEmployee = CreateButton("Ajouter", col1X, inputY, SuccessColor);
            btnUpdateEmployee = CreateButton("Modifier", col1X + buttonWidth + 10, inputY, InfoColor);
            btnDeleteEmployee = CreateButton("Supprimer", col1X + (buttonWidth + 10) * 2, inputY, DangerColor);
            btnClearEmployee = CreateButton("Effacer", col1X + (buttonWidth + 10) * 3, inputY, WarningColor);

            // Adjust button sizes
            btnAddEmployee.Size = new Size(buttonWidth, 50);
            btnUpdateEmployee.Size = new Size(buttonWidth, 50);
            btnDeleteEmployee.Size = new Size(buttonWidth, 50);
            btnClearEmployee.Size = new Size(buttonWidth, 50);

            btnAddEmployee.Click += BtnAddEmployee_Click;
            btnUpdateEmployee.Click += BtnUpdateEmployee_Click;
            btnDeleteEmployee.Click += BtnDeleteEmployee_Click;
            btnClearEmployee.Click += BtnClearEmployee_Click;

            card.Controls.Add(btnAddEmployee);
            card.Controls.Add(btnUpdateEmployee);
            card.Controls.Add(btnDeleteEmployee);
            card.Controls.Add(btnClearEmployee);

            // Employee count label
            lblEmployeeCount = new Label
            {
                Location = new Point(col1X, inputY + 65),
                Size = new Size(card.Width - 40, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Text = "Employés: 0",
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(lblEmployeeCount);

            int shiftAmount = 30;
            lblNom.Left += shiftAmount;
            txtNom.Left += shiftAmount;
            lblUtilisateur.Left += shiftAmount;
            txtUtilisateur.Left += shiftAmount;
        }

        // ======================== OPERATION SECTION ========================
        private void CreateOperationSection(Panel card)
        {
            // Top section - Selection ComboBoxes
            int inputY = 50;
            int col1X = 20, col2X = 20;

            var lblEmployeSelect = CreateLabel("Sélectionner Employé *", col1X, inputY);
            cmbEmployeSelect = new ComboBox
            {
                Location = new Point(col1X, inputY + 25),
                Size = new Size(card.Width - 40, 40),
                DisplayMember = "ToString",
                ValueMember = "Cin",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                FormattingEnabled = true,
                BackColor = Color.White,
                ForeColor = TextColor
            };
            cmbEmployeSelect.SelectedIndexChanged += (s, e) => { UpdateOperationGrid(); RefreshOperationForm(); };
            card.Controls.Add(lblEmployeSelect);
            card.Controls.Add(cmbEmployeSelect);

            // Operation type and amount row
            inputY += 90;
            int fieldWidth = (card.Width - 60) / 2;

            var lblOperationType = CreateLabel("Type d'Opération *", col1X, inputY);
            cmbOperationType = new ComboBox
            {
                Location = new Point(col1X, inputY + 25),
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
            card.Controls.Add(lblOperationType);
            card.Controls.Add(cmbOperationType);

            // Amount/Penalty field
            lblAmountLabel = CreateLabel("Montant (DH) *", col1X + fieldWidth + 20, inputY);
            txtAmount = CreateTextBox(col1X + fieldWidth + 20, inputY + 25, fieldWidth, 40);
            card.Controls.Add(lblAmountLabel);
            card.Controls.Add(txtAmount);

            // Date picker
            inputY += 90;
            var lblDate = CreateLabel("Date *", col1X, inputY);
            dtpDate = new DateTimePicker
            {
                Location = new Point(col1X, inputY + 25),
                Size = new Size(card.Width - 40, 40),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                ForeColor = TextColor
            };
            card.Controls.Add(lblDate);
            card.Controls.Add(dtpDate);

            // DataGridView for Operations
            inputY += 90;
            dgvOperations = new DataGridView
            {
                Location = new Point(20, inputY),
                Size = new Size(card.Width - 40, 280),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false,
                Font = new Font("Segoe UI", 10F)
            };

            // Style amélioré pour le DataGridView des opérations
            dgvOperations.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            dgvOperations.ColumnHeadersHeight = 45;

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

            card.Controls.Add(dgvOperations);

            // Operation Buttons
            inputY += 300;
            int buttonWidth = (card.Width - 80) / 3;
            btnAddOperation = CreateButton("Ajouter", col2X, inputY, SuccessColor);
            btnDeleteOperation = CreateButton("Supprimer", col2X + buttonWidth + 10, inputY, DangerColor);
            btnClearOperation = CreateButton("Effacer", col2X + (buttonWidth + 10) * 2, inputY, WarningColor);

            // Adjust button sizes
            btnAddOperation.Size = new Size(buttonWidth, 50);
            btnDeleteOperation.Size = new Size(buttonWidth, 50);
            btnClearOperation.Size = new Size(buttonWidth, 50);

            btnAddOperation.Click += BtnAddOperation_Click;
            btnDeleteOperation.Click += BtnDeleteOperation_Click;
            btnClearOperation.Click += BtnClearOperation_Click;

            card.Controls.Add(btnAddOperation);
            card.Controls.Add(btnDeleteOperation);
            card.Controls.Add(btnClearOperation);

            // Operation count label
            lblOperationCount = new Label
            {
                Location = new Point(col2X, inputY + 65),
                Size = new Size(card.Width - 40, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Text = "Opérations: 0",
                TextAlign = ContentAlignment.MiddleCenter
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
                BackColor = CardBackground,
                Padding = new Padding(15),
                BorderStyle = BorderStyle.FixedSingle
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = PrimaryColor,
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
                operations.Add(new
                {
                    Type = "Avance",
                    EmployeeName = employe.Nom + " " + employe.Prenom,
                    Amount = avance.Montant,
                    Date = avance.DateAvance,
                    Id = avance.Id,
                    OperationType = "Avance"
                });
            }

            // Ajouter les absences
            foreach (var absence in _absences.Where(a => a.EmployeCin == employe.Cin))
            {
                operations.Add(new
                {
                    Type = "Absence",
                    EmployeeName = employe.Nom + " " + employe.Prenom,
                    Amount = absence.Penalite,
                    Date = absence.DateAbsence,
                    Id = absence.Id,
                    OperationType = "Absence"
                });
            }

            dgvOperations.DataSource = operations.OrderByDescending(o => o.Date).ToList();
        }

        private void ClearOperationForm()
        {
            txtAmount.Text = "";
            dtpDate.Value = DateTime.Today;
            cmbOperationType.SelectedIndex = 0;
        }

        private async void BtnAddOperation_Click(object sender, EventArgs e)
        {
            try
            {
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