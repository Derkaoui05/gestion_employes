namespace GestionEmployes.Forms
{
    partial class AvanceForm
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
            this.dgvAvances = new System.Windows.Forms.DataGridView();
            this.cmbEmploye = new System.Windows.Forms.ComboBox();
            this.txtMontant = new System.Windows.Forms.TextBox();
            this.dtpDateAvance = new System.Windows.Forms.DateTimePicker();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblSelectedId = new System.Windows.Forms.Label();
            this.lblSelectedMontant = new System.Windows.Forms.Label();
            this.lblSelectedDate = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAvances)).BeginInit();
            this.SuspendLayout();
            
            // AvanceForm - Modern styling
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.BackColor = GestionEmployes.Utils.Theme.BackgroundColor;
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Name = "AvanceForm";
            this.Text = "Gestion des Avances";

            // Header Panel
            var headerPanel = new System.Windows.Forms.Panel();
            headerPanel.Location = new System.Drawing.Point(20, 20);
            headerPanel.Size = new System.Drawing.Size(1140, 60);
            headerPanel.BackColor = GestionEmployes.Utils.Theme.CardBackground;
            var headerLabel = new System.Windows.Forms.Label();
            headerLabel.Text = " Gestion des Avances";
            headerLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            headerLabel.ForeColor = GestionEmployes.Utils.Theme.PrimaryColor;
            headerLabel.AutoSize = true;
            headerLabel.Location = new System.Drawing.Point(20, 15);
            headerPanel.Controls.Add(headerLabel);
            this.Controls.Add(headerPanel);

            // DataGridView Card
            var gridCard = new System.Windows.Forms.Panel();
            gridCard.Location = new System.Drawing.Point(20, 100);
            gridCard.Size = new System.Drawing.Size(1140, 320);
            gridCard.BackColor = GestionEmployes.Utils.Theme.CardBackground;
            gridCard.Padding = new System.Windows.Forms.Padding(15);
            
            // dgvAvances
            this.dgvAvances.Location = new System.Drawing.Point(15, 15);
            this.dgvAvances.Size = new System.Drawing.Size(1110, 290);
            this.dgvAvances.Name = "dgvAvances";
            this.dgvAvances.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            gridCard.Controls.Add(this.dgvAvances);
            this.Controls.Add(gridCard);

            // Form Card
            var formCard = new System.Windows.Forms.Panel();
            formCard.Location = new System.Drawing.Point(20, 440);
            formCard.Size = new System.Drawing.Size(1140, 250);
            formCard.BackColor = GestionEmployes.Utils.Theme.CardBackground;
            formCard.Padding = new System.Windows.Forms.Padding(30, 25, 30, 25);

            var formTitle = new System.Windows.Forms.Label();
            formTitle.Text = "Informations de l'avance";
            formTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            formTitle.ForeColor = GestionEmployes.Utils.Theme.TextColor;
            formTitle.Location = new System.Drawing.Point(30, 25);
            formTitle.AutoSize = true;
            formCard.Controls.Add(formTitle);

            int col1X = 30;
            int col2X = 400;
            int col3X = 770;
            int startY = 70;

            // label1 - Employé
            this.label1.Text = "Employé";
            this.label1.Location = new System.Drawing.Point(col1X, startY);
            this.label1.Size = new System.Drawing.Size(300, 20);
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = GestionEmployes.Utils.Theme.TextColor;
            this.label1.Name = "label1";
            formCard.Controls.Add(this.label1);

            // cmbEmploye
            this.cmbEmploye.Location = new System.Drawing.Point(col1X, startY + 25);
            this.cmbEmploye.Size = new System.Drawing.Size(320, 35);
            this.cmbEmploye.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbEmploye.Name = "cmbEmploye";
            this.cmbEmploye.FormattingEnabled = true;
            formCard.Controls.Add(this.cmbEmploye);

            // label2 - Montant
            this.label2.Text = "Montant (DH)";
            this.label2.Location = new System.Drawing.Point(col2X, startY);
            this.label2.Size = new System.Drawing.Size(300, 20);
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = GestionEmployes.Utils.Theme.TextColor;
            this.label2.Name = "label2";
            formCard.Controls.Add(this.label2);

            // txtMontant
            this.txtMontant.Location = new System.Drawing.Point(col2X, startY + 25);
            this.txtMontant.Size = new System.Drawing.Size(320, 35);
            this.txtMontant.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtMontant.Name = "txtMontant";
            formCard.Controls.Add(this.txtMontant);

            // label3 - Date
            this.label3.Text = "Date d'avance";
            this.label3.Location = new System.Drawing.Point(col3X, startY);
            this.label3.Size = new System.Drawing.Size(300, 20);
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = GestionEmployes.Utils.Theme.TextColor;
            this.label3.Name = "label3";
            formCard.Controls.Add(this.label3);

            // dtpDateAvance
            this.dtpDateAvance.Location = new System.Drawing.Point(col3X, startY + 25);
            this.dtpDateAvance.Size = new System.Drawing.Size(320, 35);
            this.dtpDateAvance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpDateAvance.Name = "dtpDateAvance";
            formCard.Controls.Add(this.dtpDateAvance);

            // Buttons
            int btnY = startY + 80;
            this.btnAdd.Text = " Ajouter";
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Location = new System.Drawing.Point(col1X, btnY);
            this.btnAdd.Size = new System.Drawing.Size(140, 45);
            formCard.Controls.Add(this.btnAdd);

            this.btnUpdate.Text = " Modifier";
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Location = new System.Drawing.Point(col1X + 155, btnY);
            this.btnUpdate.Size = new System.Drawing.Size(140, 45);
            formCard.Controls.Add(this.btnUpdate);

            this.btnDelete.Text = " Supprimer";
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Location = new System.Drawing.Point(col1X + 310, btnY);
            this.btnDelete.Size = new System.Drawing.Size(140, 45);
            formCard.Controls.Add(this.btnDelete);

            this.btnClear.Text = " Effacer";
            this.btnClear.Name = "btnClear";
            this.btnClear.Location = new System.Drawing.Point(col1X + 465, btnY);
            this.btnClear.Size = new System.Drawing.Size(140, 45);
            formCard.Controls.Add(this.btnClear);

            this.btnRefresh.Text = " Actualiser";
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Location = new System.Drawing.Point(col1X + 620, btnY);
            this.btnRefresh.Size = new System.Drawing.Size(140, 45);
            formCard.Controls.Add(this.btnRefresh);

            // Total label
            this.lblTotal.Location = new System.Drawing.Point(col1X, btnY + 60);
            this.lblTotal.Size = new System.Drawing.Size(400, 25);
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = GestionEmployes.Utils.Theme.PrimaryColor;
            this.lblTotal.Text = "Total des avances: 0.00 DH";
            this.lblTotal.Name = "lblTotal";
            formCard.Controls.Add(this.lblTotal);

            // Hidden labels for selection tracking
            this.lblSelectedId.Visible = false;
            this.lblSelectedId.Name = "lblSelectedId";
            this.lblSelectedMontant.Visible = false;
            this.lblSelectedMontant.Name = "lblSelectedMontant";
            this.lblSelectedDate.Visible = false;
            this.lblSelectedDate.Name = "lblSelectedDate";
            formCard.Controls.Add(this.lblSelectedId);
            formCard.Controls.Add(this.lblSelectedMontant);
            formCard.Controls.Add(this.lblSelectedDate);

            this.Controls.Add(formCard);

            ((System.ComponentModel.ISupportInitialize)(this.dgvAvances)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAvances;
        private System.Windows.Forms.ComboBox cmbEmploye;
        private System.Windows.Forms.TextBox txtMontant;
        private System.Windows.Forms.DateTimePicker dtpDateAvance;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblSelectedId;
        private System.Windows.Forms.Label lblSelectedMontant;
        private System.Windows.Forms.Label lblSelectedDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}