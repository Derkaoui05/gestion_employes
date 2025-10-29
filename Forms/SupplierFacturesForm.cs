using GestionEmployes.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GestionEmployes.Forms
{
    public partial class SupplierFacturesForm : Form
    {
        private Supplier _supplier;
        private List<Facture> _factures;

        public SupplierFacturesForm(Supplier supplier, List<Facture> factures)
        {
            _supplier = supplier;
            _factures = factures;
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = $"Factures - {_supplier.Name}";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(240, 245, 249);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            CreateControls();
        }

        private void CreateControls()
        {
            // En-tête
            var headerPanel = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(860, 80),
                BackColor = Color.FromArgb(41, 128, 185),
                Padding = new Padding(15)
            };

            var nameLabel = new Label
            {
                Text = _supplier.Name,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true
            };

            var phoneLabel = new Label
            {
                Text = $"Tél: {_supplier.Phone}",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.White,
                Location = new Point(0, 35),
                AutoSize = true
            };

            headerPanel.Controls.Add(nameLabel);
            headerPanel.Controls.Add(phoneLabel);

            // Cartes de résumé
            var summaryPanel = new Panel
            {
                Location = new Point(10, 100),
                Size = new Size(860, 80),
                BackColor = Color.Transparent
            };

            var facturesSupplier = _factures.Where(f => f.SupplierId == _supplier.ID).ToList();
            decimal totalMontant = facturesSupplier.Sum(f => f.Amount);
            decimal totalAvance = facturesSupplier.Sum(f => f.Advance);
            decimal totalRestant = totalMontant - totalAvance;

            var cardTotal = CreateSummaryCard("Montant Total", totalMontant.ToString("N2") + " DH",
                                            Color.FromArgb(41, 128, 185), 0, 0, 280, 70);
            var cardAvance = CreateSummaryCard("Total Avances", totalAvance.ToString("N2") + " DH",
                                             Color.FromArgb(39, 174, 96), 285, 0, 280, 70);
            var cardRestant = CreateSummaryCard("Reste à Payer", totalRestant.ToString("N2") + " DH",
                                              Color.FromArgb(231, 76, 60), 570, 0, 280, 70);

            summaryPanel.Controls.Add(cardTotal);
            summaryPanel.Controls.Add(cardAvance);
            summaryPanel.Controls.Add(cardRestant);

            // DataGridView des factures
            var dgvFactures = new DataGridView
            {
                Location = new Point(10, 200),
                Size = new Size(860, 350),
                AutoGenerateColumns = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false
            };

            dgvFactures.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Number",
                HeaderText = "N° FACTURE",
                DataPropertyName = "Number",
                Width = 120
            });

            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Date",
                HeaderText = "DATE FACTURE",
                DataPropertyName = "InvoiceDate",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });

            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Echeance",
                HeaderText = "ÉCHÉANCE",
                DataPropertyName = "DueDate",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });

            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Montant",
                HeaderText = "MONTANT",
                DataPropertyName = "Amount",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Avance",
                HeaderText = "AVANCE",
                DataPropertyName = "Advance",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Reste",
                HeaderText = "RESTE",
                DataPropertyName = "Reste",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvFactures.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Statut",
                HeaderText = "STATUT",
                DataPropertyName = "Status",
                Width = 120
            });

            // Charger les données
            var facturesData = facturesSupplier.Select(f => new
            {
                f.Number,
                f.InvoiceDate,
                f.DueDate,
                Amount = f.Amount,
                Advance = f.Advance,
                Reste = f.Amount - f.Advance,
                Status = (f.Amount - f.Advance) <= 0 ? "Payée" : "En cours"
            }).OrderByDescending(f => f.InvoiceDate).ToList();

            dgvFactures.DataSource = facturesData;

            this.Controls.Add(headerPanel);
            this.Controls.Add(summaryPanel);
            this.Controls.Add(dgvFactures);
        }

        private Panel CreateSummaryCard(string title, string value, Color color, int x, int y, int width, int height)
        {
            var card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(10, 10),
                AutoSize = true
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(10, 30),
                AutoSize = true
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);

            return card;
        }
    }
}