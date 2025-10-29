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
    public partial class SupplierForm : Form
    {
        private SupplierService _supplierService;
        private List<Supplier> _suppliers;
        private Supplier _selectedSupplier;

        // Controls
        private DataGridView dgvSuppliers;
        private TextBox txtName;
        private TextBox txtPhone;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClear;
        private Label lblCount;

        public SupplierForm()
        {
            _supplierService = new SupplierService();

            this.Text = "Gestion des Fournisseurs";
            this.Size = new Size(1400, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Theme.BackgroundColor;
            this.Padding = new Padding(15);

            Theme.Apply(this);
            CreateAllControls();
            this.Load += SupplierForm_Load;
        }

        private void CreateAllControls()
        {
            Console.WriteLine("🔧 [SupplierForm] Création des contrôles...");

            // ==================== MAIN CARD ====================
            var mainCard = CreateCardPanel(15, 15, 1350, 700, "🏢 Gestion des Fournisseurs");

            // ==================== DATAGRIDVIEW ====================
            dgvSuppliers = new DataGridView
            {
                Location = new Point(20, 50),
                Size = new Size(1310, 220),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false
            };

            // Columns - Simplifié
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ID",
                HeaderText = "ID",
                DataPropertyName = "ID",
                Width = 60
            });

            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Nom",
                DataPropertyName = "Name",
                Width = 300
            });

            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Phone",
                HeaderText = "Téléphone",
                DataPropertyName = "Phone",
                Width = 150
            });


            dgvSuppliers.SelectionChanged += (s, e) =>
            {
                Console.WriteLine("📍 [SupplierForm] Sélection changée");
                DgvSuppliers_SelectionChanged();
            };

            mainCard.Controls.Add(dgvSuppliers);

            // ==================== INPUT SECTION ====================
            int inputY = 290;
            int col1X = 20, col2X = 700;

            // Name
            var lblName = CreateLabel("Nom du Fournisseur", col1X, inputY);
            txtName = CreateTextBox(col1X, inputY + 25, 650, 35);
            txtName.Name = "txtName";
            mainCard.Controls.Add(lblName);
            mainCard.Controls.Add(txtName);

            // Phone
            var lblPhone = CreateLabel("Téléphone", col2X, inputY);
            txtPhone = CreateTextBox(col2X, inputY + 25, 650, 35);
            txtPhone.Name = "txtPhone";
            mainCard.Controls.Add(lblPhone);
            mainCard.Controls.Add(txtPhone);

            inputY += 75;

            // IsActive Checkbox
            //chkIsActive = new CheckBox
            //{
            //    Location = new Point(col1X, inputY + 5),
            //    Size = new Size(650, 30),
            //    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            //    Text = "✓ Fournisseur Actif",
            //    Checked = true,
            //    ForeColor = Theme.TextColor,
            //    Name = "chkIsActive"
            //};
            //mainCard.Controls.Add(chkIsActive);

            inputY += 60;

            // ==================== BUTTONS ====================
            btnAdd = CreateButton("➕ Ajouter", col1X, inputY, Theme.SuccessColor);
            btnAdd.Name = "btnAdd";
            btnAdd.Click += (s, e) =>
            {
                Console.WriteLine("🔘 [SupplierForm] Bouton Ajouter cliqué");
                BtnAdd_Click(s, e);
            };

            btnUpdate = CreateButton("✏️ Modifier", col1X + 165, inputY, Theme.InfoColor);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Click += (s, e) =>
            {
                Console.WriteLine("🔘 [SupplierForm] Bouton Modifier cliqué");
                BtnUpdate_Click(s, e);
            };

            btnDelete = CreateButton("🗑️ Supprimer", col1X + 330, inputY, Theme.DangerColor);
            btnDelete.Name = "btnDelete";
            btnDelete.Click += (s, e) =>
            {
                Console.WriteLine("🔘 [SupplierForm] Bouton Supprimer cliqué");
                BtnDelete_Click(s, e);
            };

            btnClear = CreateButton("🧹 Effacer", col1X + 495, inputY, Theme.WarningColor);
            btnClear.Name = "btnClear";
            btnClear.Click += (s, e) =>
            {
                Console.WriteLine("🔘 [SupplierForm] Bouton Effacer cliqué");
                BtnClear_Click(s, e);
            };

            mainCard.Controls.Add(btnAdd);
            mainCard.Controls.Add(btnUpdate);
            mainCard.Controls.Add(btnDelete);
            mainCard.Controls.Add(btnClear);

            lblCount = new Label
            {
                Location = new Point(col1X + 660, inputY + 8),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Theme.PrimaryColor,
                Text = "Fournisseurs: 0",
                Name = "lblCount"
            };
            mainCard.Controls.Add(lblCount);

            this.Controls.Add(mainCard);
            Console.WriteLine("✅ [SupplierForm] Contrôles créés avec succès");
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
                Size = new Size(650, 20),
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

        private async void SupplierForm_Load(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("📂 [SupplierForm] Chargement du formulaire...");
                await LoadSuppliers();
                Console.WriteLine("✅ [SupplierForm] Formulaire chargé avec succès");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SupplierForm] Erreur de chargement: {ex.Message}");
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadSuppliers()
        {
            try
            {
                Console.WriteLine("🔄 [SupplierForm] Chargement des fournisseurs...");
                var suppliers = _supplierService.GetAllSuppliers();
                _suppliers = suppliers;

                dgvSuppliers.DataSource = new List<Supplier>(_suppliers);
                UpdateLabel();

                Console.WriteLine($"✅ [SupplierForm] {_suppliers.Count} fournisseurs chargés");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SupplierForm] Erreur LoadSuppliers: {ex.Message}");
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvSuppliers_SelectionChanged()
        {
            try
            {
                if (dgvSuppliers.SelectedRows.Count > 0)
                {
                    _selectedSupplier = dgvSuppliers.SelectedRows[0].DataBoundItem as Supplier;
                    Console.WriteLine($"📌 [SupplierForm] Fournisseur sélectionné: {_selectedSupplier?.Name}");
                    FillForm(_selectedSupplier);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SupplierForm] Erreur SelectionChanged: {ex.Message}");
            }
        }

        private void FillForm(Supplier supplier)
        {
            if (supplier == null) return;

            txtName.Text = supplier.Name ?? "";
            txtPhone.Text = supplier.Phone ?? "";
            //chkIsActive.Checked = supplier.IsActive;

            Console.WriteLine($"📝 [SupplierForm] Formulaire rempli: {supplier.Name}");
        }

        private void ClearForm()
        {
            Console.WriteLine("🧹 [SupplierForm] Effacement du formulaire");
            txtName.Text = "";
            txtPhone.Text = "";
            _selectedSupplier = null;
            dgvSuppliers.ClearSelection();
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("➕ [SupplierForm] Ajout d'un fournisseur...");

                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    Console.WriteLine("⚠️ [SupplierForm] Nom vide");
                    MessageBox.Show("Le nom du fournisseur est obligatoire", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var supplier = new Supplier
                {
                    Name = txtName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Contact = "",
                    Email = "",
                    Address = "",
                    CreatedDate = DateTime.Now
                };

                Console.WriteLine($"📌 Création supplier: {supplier.Name}");

                if (_supplierService.AddSupplier(supplier))
                {
                    await LoadSuppliers();
                    ClearForm();

                    // 🔴 ÉMETTRE L'ÉVÉNEMENT AVEC LES BONNES VARIABLES
                    EventBus.OnSupplierAdded(this, supplier.ID, supplier.Name);
                    Console.WriteLine("✅ Fournisseur ajouté au service + Événement émis");

                    MessageBox.Show("Fournisseur ajouté avec succès!", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Console.WriteLine("❌ Le service a retourné false");
                    MessageBox.Show("Erreur lors de l'ajout", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SupplierForm] Erreur BtnAdd: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("✏️ [SupplierForm] Modification d'un fournisseur...");

                if (_selectedSupplier == null)
                {
                    Console.WriteLine("⚠️ [SupplierForm] Aucun fournisseur sélectionné");
                    MessageBox.Show("Veuillez sélectionner un fournisseur", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    Console.WriteLine("⚠️ [SupplierForm] Nom vide");
                    MessageBox.Show("Le nom du fournisseur est obligatoire", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _selectedSupplier.Name = txtName.Text.Trim();
                _selectedSupplier.Phone = txtPhone.Text.Trim();
                //_selectedSupplier.IsActive = chkIsActive.Checked;

                Console.WriteLine($"📌 Mise à jour: {_selectedSupplier.Name}");

                if (_supplierService.UpdateSupplier(_selectedSupplier))
                {
                    await LoadSuppliers();
                    ClearForm();

                    // 🔴 ÉMETTRE L'ÉVÉNEMENT AVEC LES BONNES VARIABLES
                    EventBus.OnSupplierUpdated(this, _selectedSupplier.ID, _selectedSupplier.Name);
                    Console.WriteLine("✅ Fournisseur modifié au service + Événement émis");

                    MessageBox.Show("Fournisseur modifié avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Console.WriteLine("❌ Le service a retourné false");
                    MessageBox.Show("Erreur lors de la modification", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SupplierForm] Erreur BtnUpdate: {ex.Message}");
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("🗑️ [SupplierForm] Suppression d'un fournisseur...");

                if (_selectedSupplier == null)
                {
                    Console.WriteLine("⚠️ [SupplierForm] Aucun fournisseur sélectionné");
                    MessageBox.Show("Veuillez sélectionner un fournisseur", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer {_selectedSupplier.Name}?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Console.WriteLine($"📌 Suppression confirmée: {_selectedSupplier.Name}");

                    // 🔴 SAUVEGARDER LES VARIABLES AVANT LA SUPPRESSION
                    int supplierId = _selectedSupplier.ID;
                    string supplierName = _selectedSupplier.Name;

                    if (_supplierService.DeleteSupplier(supplierId))
                    {
                        await LoadSuppliers();
                        ClearForm();

                        // 🔴 ÉMETTRE L'ÉVÉNEMENT AVEC LES VARIABLES SAUVEGARDÉES
                        EventBus.OnSupplierDeleted(this, supplierId, supplierName);
                        Console.WriteLine("✅ Fournisseur supprimé au service + Événement émis");

                        MessageBox.Show("Fournisseur supprimé avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Console.WriteLine("❌ Le service a retourné false");
                        MessageBox.Show("Erreur lors de la suppression", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SupplierForm] Erreur BtnDelete: {ex.Message}");
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void UpdateLabel()
        {
            lblCount.Text = $"Fournisseurs: {_suppliers?.Count ?? 0}";
            Console.WriteLine($"📊 [SupplierForm] Total fournisseurs: {_suppliers?.Count ?? 0}");
        }
    }
}