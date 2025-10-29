using GestionEmployes.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GestionEmployes.Forms
{
    public partial class EmployeTransactionsForm : Form
    {
        private Employe _employe;
        private List<Avance> _avances;
        private List<Absence> _absences;

        public EmployeTransactionsForm(Employe employe, List<Avance> avances, List<Absence> absences)
        {
            _employe = employe;
            _avances = avances;
            _absences = absences;
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = $"Transactions - {_employe.Nom} {_employe.Prenom}";
            this.Size = new Size(800, 600);
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
                Size = new Size(760, 80),
                BackColor = Color.FromArgb(41, 128, 185),
                Padding = new Padding(15)
            };

            var nameLabel = new Label
            {
                Text = $"{_employe.Nom} {_employe.Prenom}",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true
            };

            var cinLabel = new Label
            {
                Text = $"CIN: {_employe.Cin}",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.White,
                Location = new Point(0, 35),
                AutoSize = true
            };

            headerPanel.Controls.Add(nameLabel);
            headerPanel.Controls.Add(cinLabel);

            // Cartes de résumé
            var summaryPanel = new Panel
            {
                Location = new Point(10, 100),
                Size = new Size(760, 80),
                BackColor = Color.Transparent
            };

            decimal totalAvances = _avances.Where(a => a.EmployeCin == _employe.Cin).Sum(a => a.Montant);
            decimal totalAbsences = _absences.Where(a => a.EmployeCin == _employe.Cin).Sum(a => a.Penalite);
            decimal salaireNet = (_employe.Salaire ?? 0) - totalAvances - totalAbsences;

            var cardAvances = CreateSummaryCard("Total Avances", totalAvances.ToString("N2") + " DH",
                                              Color.FromArgb(231, 76, 60), 0, 0, 250, 70);
            var cardAbsences = CreateSummaryCard("Total Pénalités", totalAbsences.ToString("N2") + " DH",
                                               Color.FromArgb(230, 126, 34), 255, 0, 250, 70);
            var cardNet = CreateSummaryCard("Salaire Net", salaireNet.ToString("N2") + " DH",
                                          Color.FromArgb(39, 174, 96), 510, 0, 250, 70);

            summaryPanel.Controls.Add(cardAvances);
            summaryPanel.Controls.Add(cardAbsences);
            summaryPanel.Controls.Add(cardNet);

            // DataGridView des transactions
            var dgvTransactions = new DataGridView
            {
                Location = new Point(10, 200),
                Size = new Size(760, 350),
                AutoGenerateColumns = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AllowUserToAddRows = false
            };

            dgvTransactions.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Type",
                HeaderText = "TYPE",
                DataPropertyName = "Type",
                Width = 100
            });

            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Date",
                HeaderText = "DATE",
                DataPropertyName = "Date",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });

            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Montant",
                HeaderText = "MONTANT",
                DataPropertyName = "Montant",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "DESCRIPTION",
                DataPropertyName = "Description",
                Width = 350
            });

            // Charger les données
            var transactions = new List<dynamic>();

            // Avances
            foreach (var avance in _avances.Where(a => a.EmployeCin == _employe.Cin).OrderByDescending(a => a.DateAvance))
            {
                transactions.Add(new
                {
                    Type = "AVANCE",
                    Date = avance.DateAvance,
                    Montant = avance.Montant,
                    Description = "Avance sur salaire"
                });
            }

            // Absences
            foreach (var absence in _absences.Where(a => a.EmployeCin == _employe.Cin).OrderByDescending(a => a.DateAbsence))
            {
                transactions.Add(new
                {
                    Type = "ABSENCE",
                    Date = absence.DateAbsence,
                    Montant = absence.Penalite,
                    Description = "Pénalité d'absence"
                });
            }

            dgvTransactions.DataSource = transactions;

            this.Controls.Add(headerPanel);
            this.Controls.Add(summaryPanel);
            this.Controls.Add(dgvTransactions);
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