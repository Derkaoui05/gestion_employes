namespace GestionEmployes.Forms
{
    partial class ReportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvReports = new System.Windows.Forms.DataGridView();
            //this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            //this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.btnCurrentWeek = new System.Windows.Forms.Button();
            //this.btnCustomPeriod = new System.Windows.Forms.Button();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblTotalEmployees = new System.Windows.Forms.Label();
            this.lblTotalAdvances = new System.Windows.Forms.Label();
            this.lblTotalPenalties = new System.Windows.Forms.Label();
            this.lblTotalNetSalary = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).BeginInit();
            this.SuspendLayout();
            
            // ReportForm - Modern styling
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1600, 900);
            this.BackColor = GestionEmployes.Utils.Theme.BackgroundColor;
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Name = "ReportForm";
            this.Text = "Rapports des Employés";

            // Header Panel
            var headerPanel = new System.Windows.Forms.Panel();
            headerPanel.Location = new System.Drawing.Point(20, 20);
            headerPanel.Size = new System.Drawing.Size(1540, 60);
            headerPanel.BackColor = GestionEmployes.Utils.Theme.CardBackground;
            var headerLabel = new System.Windows.Forms.Label();
            headerLabel.Text = "📊 Rapports et Statistiques";
            headerLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            headerLabel.ForeColor = GestionEmployes.Utils.Theme.PrimaryColor;
            headerLabel.AutoSize = true;
            headerLabel.Location = new System.Drawing.Point(20, 15);
            headerPanel.Controls.Add(headerLabel);
            this.Controls.Add(headerPanel);

            // Filter Card
            var filterCard = new System.Windows.Forms.Panel();
            filterCard.Location = new System.Drawing.Point(20, 100);
            filterCard.Size = new System.Drawing.Size(1540, 100);
            filterCard.BackColor = GestionEmployes.Utils.Theme.CardBackground;
            filterCard.Padding = new System.Windows.Forms.Padding(20);

            //// label1 - Date début
            //this.label1.Text = "Date début";
            //this.label1.Location = new System.Drawing.Point(20, 20);
            //this.label1.Size = new System.Drawing.Size(100, 20);
            //this.label1.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            //this.label1.ForeColor = GestionEmployes.Utils.Theme.TextColor;
            //this.label1.Name = "label1";
            //filterCard.Controls.Add(this.label1);

            //// dtpStartDate - FIXED HEIGHT TO 35
            //this.dtpStartDate.Location = new System.Drawing.Point(20, 45);
            //this.dtpStartDate.Size = new System.Drawing.Size(200, 35);
            //this.dtpStartDate.Font = new System.Drawing.Font("Segoe UI", 11F);
            //this.dtpStartDate.Name = "dtpStartDate";
            //filterCard.Controls.Add(this.dtpStartDate);

            //// label2 - Date fin
            //this.label2.Text = "Date fin";
            //this.label2.Location = new System.Drawing.Point(240, 20);
            //this.label2.Size = new System.Drawing.Size(100, 20);
            //this.label2.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            //this.label2.ForeColor = GestionEmployes.Utils.Theme.TextColor;
            //this.label2.Name = "label2";
            //filterCard.Controls.Add(this.label2);

            //// dtpEndDate - FIXED HEIGHT TO 35
            //this.dtpEndDate.Location = new System.Drawing.Point(240, 45);
            //this.dtpEndDate.Size = new System.Drawing.Size(200, 35);
            //this.dtpEndDate.Font = new System.Drawing.Font("Segoe UI", 11F);
            //this.dtpEndDate.Name = "dtpEndDate";
            //filterCard.Controls.Add(this.dtpEndDate);

            // Buttons
            this.btnCurrentWeek.Text = "📅 Semaine actuelle";
            this.btnCurrentWeek.Name = "btnCurrentWeek";
            this.btnCurrentWeek.Location = new System.Drawing.Point(460, 45);
            this.btnCurrentWeek.Size = new System.Drawing.Size(200, 35);
            filterCard.Controls.Add(this.btnCurrentWeek);

            //this.btnCustomPeriod.Text = "🔍 Période perso";
            //this.btnCustomPeriod.Name = "btnCustomPeriod";
            //this.btnCustomPeriod.Location = new System.Drawing.Point(680, 45);
            //this.btnCustomPeriod.Size = new System.Drawing.Size(180, 35);
            //filterCard.Controls.Add(this.btnCustomPeriod);

            this.btnExportExcel.Text = "📥 Exporter Excel";
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Location = new System.Drawing.Point(880, 45);
            this.btnExportExcel.Size = new System.Drawing.Size(180, 35);
            filterCard.Controls.Add(this.btnExportExcel);

            this.btnRefresh.Text = "🔄 Actualiser";
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Location = new System.Drawing.Point(1080, 45);
            this.btnRefresh.Size = new System.Drawing.Size(180, 35);
            filterCard.Controls.Add(this.btnRefresh);

            // Status label
            this.lblStatus.Location = new System.Drawing.Point(1280, 45);
            this.lblStatus.Size = new System.Drawing.Size(260, 35);
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblStatus.ForeColor = GestionEmployes.Utils.Theme.TextSecondary;
            this.lblStatus.Text = "Prêt";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblStatus.Name = "lblStatus";
            filterCard.Controls.Add(this.lblStatus);

            this.Controls.Add(filterCard);

            // Stats Cards - Modern Dashboard Style
            int statsY = 220;
            int cardWidth = 370;
            int cardHeight = 120;
            int cardSpacing = 20;

            // Card 1 - Total Employés
            var card1 = new System.Windows.Forms.Panel();
            card1.Location = new System.Drawing.Point(20, statsY);
            card1.Size = new System.Drawing.Size(cardWidth, cardHeight);
            card1.BackColor = GestionEmployes.Utils.Theme.CardBackground;
            
            var card1Icon = new System.Windows.Forms.Label();
            card1Icon.Text = "👥";
            card1Icon.Font = new System.Drawing.Font("Segoe UI", 25F);
            card1Icon.Location = new System.Drawing.Point(20, 35);
            card1Icon.AutoSize = true;
            card1.Controls.Add(card1Icon);

            this.lblTotalEmployees.Location = new System.Drawing.Point(123, 45);
            this.lblTotalEmployees.Size = new System.Drawing.Size(270, 50);
            this.lblTotalEmployees.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
            this.lblTotalEmployees.ForeColor = GestionEmployes.Utils.Theme.TextColor;
            this.lblTotalEmployees.Text = "Total employés: 0";
            this.lblTotalEmployees.Name = "lblTotalEmployees";
            this.lblTotalEmployees.AutoSize = false;
            this.lblTotalEmployees.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            card1.Controls.Add(this.lblTotalEmployees);
            this.Controls.Add(card1);

            // Card 2 - Total Avances
            var card2 = new System.Windows.Forms.Panel();
            card2.Location = new System.Drawing.Point(20 + cardWidth + cardSpacing, statsY);
            card2.Size = new System.Drawing.Size(cardWidth, cardHeight);
            card2.BackColor = GestionEmployes.Utils.Theme.CardBackground;
            
            var card2Icon = new System.Windows.Forms.Label();
            card2Icon.Text = "💰";
            card2Icon.Font = new System.Drawing.Font("Segoe UI", 25F);
            card2Icon.Location = new System.Drawing.Point(20, 35);
            card2Icon.AutoSize = true;
            card2.Controls.Add(card2Icon);

            this.lblTotalAdvances.Location = new System.Drawing.Point(123, 45);
            this.lblTotalAdvances.Size = new System.Drawing.Size(270, 50);
            this.lblTotalAdvances.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
            this.lblTotalAdvances.ForeColor = GestionEmployes.Utils.Theme.WarningColor;
            this.lblTotalAdvances.Text = "Total avances: 0.00 DH";
            this.lblTotalAdvances.Name = "lblTotalAdvances";
            this.lblTotalAdvances.AutoSize = false;
            this.lblTotalAdvances.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            card2.Controls.Add(this.lblTotalAdvances);
            this.Controls.Add(card2);

            // Card 3 - Total Pénalités
            var card3 = new System.Windows.Forms.Panel();
            card3.Location = new System.Drawing.Point(20 + (cardWidth + cardSpacing) * 2, statsY);
            card3.Size = new System.Drawing.Size(cardWidth, cardHeight);
            card3.BackColor = GestionEmployes.Utils.Theme.CardBackground;
            
            var card3Icon = new System.Windows.Forms.Label();
            card3Icon.Text = "⚠️";
            card3Icon.Font = new System.Drawing.Font("Segoe UI", 25F);
            card3Icon.Location = new System.Drawing.Point(20, 35);
            card3Icon.AutoSize = true;
            card3.Controls.Add(card3Icon);

            this.lblTotalPenalties.Location = new System.Drawing.Point(123, 45);
            this.lblTotalPenalties.Size = new System.Drawing.Size(270, 50);
            this.lblTotalPenalties.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
            this.lblTotalPenalties.ForeColor = GestionEmployes.Utils.Theme.DangerColor;
            this.lblTotalPenalties.Text = "Total pénalités: 0.00 DH";
            this.lblTotalPenalties.Name = "lblTotalPenalties";
            this.lblTotalPenalties.AutoSize = false;
            this.lblTotalPenalties.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            card3.Controls.Add(this.lblTotalPenalties);
            this.Controls.Add(card3);

            // Card 4 - Total Salaires Nets
            var card4 = new System.Windows.Forms.Panel();
            card4.Location = new System.Drawing.Point(20 + (cardWidth + cardSpacing) * 3, statsY);
            card4.Size = new System.Drawing.Size(cardWidth, cardHeight);
            card4.BackColor = GestionEmployes.Utils.Theme.CardBackground;
            
            var card4Icon = new System.Windows.Forms.Label();
            card4Icon.Text = "💵";
            card4Icon.Font = new System.Drawing.Font("Segoe UI", 25F);
            card4Icon.Location = new System.Drawing.Point(20, 35);
            card4Icon.AutoSize = true;
            card4.Controls.Add(card4Icon);

            this.lblTotalNetSalary.Location = new System.Drawing.Point(123, 45);
            this.lblTotalNetSalary.Size = new System.Drawing.Size(270, 50);
            this.lblTotalNetSalary.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
            this.lblTotalNetSalary.ForeColor = GestionEmployes.Utils.Theme.SuccessColor;
            this.lblTotalNetSalary.Text = "Salaires nets: 0.00 DH";
            this.lblTotalNetSalary.Name = "lblTotalNetSalary";
            this.lblTotalNetSalary.AutoSize = false;
            this.lblTotalNetSalary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            card4.Controls.Add(this.lblTotalNetSalary);
            this.Controls.Add(card4);

            // DataGridView Card
            var gridCard = new System.Windows.Forms.Panel();
            gridCard.Location = new System.Drawing.Point(20, statsY + cardHeight + 20);
            gridCard.Size = new System.Drawing.Size(1540, 500);
            gridCard.BackColor = GestionEmployes.Utils.Theme.CardBackground;
            gridCard.Padding = new System.Windows.Forms.Padding(15);

            // dgvReports
            this.dgvReports.Location = new System.Drawing.Point(15, 15);
            this.dgvReports.Size = new System.Drawing.Size(1510, 470);
            this.dgvReports.Name = "dgvReports";
            this.dgvReports.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            gridCard.Controls.Add(this.dgvReports);
            this.Controls.Add(gridCard);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvReports;
        //private System.Windows.Forms.DateTimePicker dtpStartDate;
        //private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Button btnCurrentWeek;
        //private System.Windows.Forms.Button btnCustomPeriod;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblTotalEmployees;
        private System.Windows.Forms.Label lblTotalAdvances;
        private System.Windows.Forms.Label lblTotalPenalties;
        private System.Windows.Forms.Label lblTotalNetSalary;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}