namespace GestionEmployes.Forms
{
    partial class EmployeForm
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

        #endregion
        //private void InitializeComponent1()
        //{
        //    this.dgvEmployes = new System.Windows.Forms.DataGridView();
        //    this.txtCin = new System.Windows.Forms.TextBox();
        //    this.txtNom = new System.Windows.Forms.TextBox();
        //    this.txtPrenom = new System.Windows.Forms.TextBox();
        //    this.txtUtilisateur = new System.Windows.Forms.TextBox();
        //    this.txtMotDePasse = new System.Windows.Forms.TextBox();
        //    this.txtSalaire = new System.Windows.Forms.TextBox();
        //    this.btnAdd = new System.Windows.Forms.Button();
        //    this.btnUpdate = new System.Windows.Forms.Button();
        //    this.btnDelete = new System.Windows.Forms.Button();
        //    this.btnClear = new System.Windows.Forms.Button();
        //    this.btnRefresh = new System.Windows.Forms.Button();

        //    // Labels
        //    this.lblCin = new System.Windows.Forms.Label();
        //    this.lblNom = new System.Windows.Forms.Label();
        //    this.lblPrenom = new System.Windows.Forms.Label();
        //    this.lblUtilisateur = new System.Windows.Forms.Label();
        //    this.lblMotDePasse = new System.Windows.Forms.Label();
        //    this.lblSalaire = new System.Windows.Forms.Label();

        //    ((System.ComponentModel.ISupportInitialize)(this.dgvEmployes)).BeginInit();
        //    this.SuspendLayout();

        //    // dgvEmployes
        //    this.dgvEmployes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        //    this.dgvEmployes.Location = new System.Drawing.Point(12, 12);
        //    this.dgvEmployes.Name = "dgvEmployes";
        //    this.dgvEmployes.Size = new System.Drawing.Size(776, 250);
        //    this.dgvEmployes.TabIndex = 0;

        //    // lblCin
        //    this.lblCin.AutoSize = true;
        //    this.lblCin.Location = new System.Drawing.Point(12, 280);
        //    this.lblCin.Name = "lblCin";
        //    this.lblCin.Size = new System.Drawing.Size(35, 13);
        //    this.lblCin.TabIndex = 1;
        //    this.lblCin.Text = "CIN:";

        //    // txtCin
        //    this.txtCin.Location = new System.Drawing.Point(100, 280);
        //    this.txtCin.Name = "txtCin";
        //    this.txtCin.Size = new System.Drawing.Size(150, 20);
        //    this.txtCin.TabIndex = 2;

        //    // lblNom
        //    this.lblNom.AutoSize = true;
        //    this.lblNom.Location = new System.Drawing.Point(12, 310);
        //    this.lblNom.Name = "lblNom";
        //    this.lblNom.Size = new System.Drawing.Size(38, 13);
        //    this.lblNom.TabIndex = 3;
        //    this.lblNom.Text = "Nom:";

        //    // txtNom
        //    this.txtNom.Location = new System.Drawing.Point(100, 310);
        //    this.txtNom.Name = "txtNom";
        //    this.txtNom.Size = new System.Drawing.Size(150, 20);
        //    this.txtNom.TabIndex = 4;

        //    // lblPrenom
        //    this.lblPrenom.AutoSize = true;
        //    this.lblPrenom.Location = new System.Drawing.Point(12, 340);
        //    this.lblPrenom.Name = "lblPrenom";
        //    this.lblPrenom.Size = new System.Drawing.Size(52, 13);
        //    this.lblPrenom.TabIndex = 5;
        //    this.lblPrenom.Text = "Prénom:";

        //    // txtPrenom
        //    this.txtPrenom.Location = new System.Drawing.Point(100, 340);
        //    this.txtPrenom.Name = "txtPrenom";
        //    this.txtPrenom.Size = new System.Drawing.Size(150, 20);
        //    this.txtPrenom.TabIndex = 6;

        //    // lblUtilisateur
        //    this.lblUtilisateur.AutoSize = true;
        //    this.lblUtilisateur.Location = new System.Drawing.Point(300, 280);
        //    this.lblUtilisateur.Name = "lblUtilisateur";
        //    this.lblUtilisateur.Size = new System.Drawing.Size(60, 13);
        //    this.lblUtilisateur.TabIndex = 7;
        //    this.lblUtilisateur.Text = "Utilisateur:";

        //    // txtUtilisateur
        //    this.txtUtilisateur.Location = new System.Drawing.Point(380, 280);
        //    this.txtUtilisateur.Name = "txtUtilisateur";
        //    this.txtUtilisateur.Size = new System.Drawing.Size(150, 20);
        //    this.txtUtilisateur.TabIndex = 8;

        //    // lblMotDePasse
        //    this.lblMotDePasse.AutoSize = true;
        //    this.lblMotDePasse.Location = new System.Drawing.Point(300, 310);
        //    this.lblMotDePasse.Name = "lblMotDePasse";
        //    this.lblMotDePasse.Size = new System.Drawing.Size(80, 13);
        //    this.lblMotDePasse.TabIndex = 9;
        //    this.lblMotDePasse.Text = "Mot de passe:";

        //    // txtMotDePasse
        //    this.txtMotDePasse.Location = new System.Drawing.Point(380, 310);
        //    this.txtMotDePasse.Name = "txtMotDePasse";
        //    this.txtMotDePasse.Size = new System.Drawing.Size(150, 20);
        //    this.txtMotDePasse.TabIndex = 10;
        //    this.txtMotDePasse.UseSystemPasswordChar = true;

        //    // lblSalaire
        //    this.lblSalaire.AutoSize = true;
        //    this.lblSalaire.Location = new System.Drawing.Point(300, 340);
        //    this.lblSalaire.Name = "lblSalaire";
        //    this.lblSalaire.Size = new System.Drawing.Size(48, 13);
        //    this.lblSalaire.TabIndex = 11;
        //    this.lblSalaire.Text = "Salaire:";

        //    // txtSalaire
        //    this.txtSalaire.Location = new System.Drawing.Point(380, 340);
        //    this.txtSalaire.Name = "txtSalaire";
        //    this.txtSalaire.Size = new System.Drawing.Size(150, 20);
        //    this.txtSalaire.TabIndex = 12;

        //    // btnAdd
        //    this.btnAdd.Location = new System.Drawing.Point(12, 380);
        //    this.btnAdd.Name = "btnAdd";
        //    this.btnAdd.Size = new System.Drawing.Size(75, 23);
        //    this.btnAdd.TabIndex = 13;
        //    this.btnAdd.Text = "Ajouter";
        //    this.btnAdd.UseVisualStyleBackColor = true;

        //    // btnUpdate
        //    this.btnUpdate.Location = new System.Drawing.Point(93, 380);
        //    this.btnUpdate.Name = "btnUpdate";
        //    this.btnUpdate.Size = new System.Drawing.Size(75, 23);
        //    this.btnUpdate.TabIndex = 14;
        //    this.btnUpdate.Text = "Modifier";
        //    this.btnUpdate.UseVisualStyleBackColor = true;

        //    // btnDelete
        //    this.btnDelete.Location = new System.Drawing.Point(174, 380);
        //    this.btnDelete.Name = "btnDelete";
        //    this.btnDelete.Size = new System.Drawing.Size(75, 23);
        //    this.btnDelete.TabIndex = 15;
        //    this.btnDelete.Text = "Supprimer";
        //    this.btnDelete.UseVisualStyleBackColor = true;

        //    // btnClear
        //    this.btnClear.Location = new System.Drawing.Point(255, 380);
        //    this.btnClear.Name = "btnClear";
        //    this.btnClear.Size = new System.Drawing.Size(75, 23);
        //    this.btnClear.TabIndex = 16;
        //    this.btnClear.Text = "Nettoyer";
        //    this.btnClear.UseVisualStyleBackColor = true;

        //    // btnRefresh
        //    this.btnRefresh.Location = new System.Drawing.Point(336, 380);
        //    this.btnRefresh.Name = "btnRefresh";
        //    this.btnRefresh.Size = new System.Drawing.Size(75, 23);
        //    this.btnRefresh.TabIndex = 17;
        //    this.btnRefresh.Text = "Actualiser";
        //    this.btnRefresh.UseVisualStyleBackColor = true;

        //    // EmployeForm
        //    this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        //    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        //    this.ClientSize = new System.Drawing.Size(800, 420);
        //    this.Controls.Add(this.btnRefresh);
        //    this.Controls.Add(this.btnClear);
        //    this.Controls.Add(this.btnDelete);
        //    this.Controls.Add(this.btnUpdate);
        //    this.Controls.Add(this.btnAdd);
        //    this.Controls.Add(this.txtSalaire);
        //    this.Controls.Add(this.lblSalaire);
        //    this.Controls.Add(this.txtMotDePasse);
        //    this.Controls.Add(this.lblMotDePasse);
        //    this.Controls.Add(this.txtUtilisateur);
        //    this.Controls.Add(this.lblUtilisateur);
        //    this.Controls.Add(this.txtPrenom);
        //    this.Controls.Add(this.lblPrenom);
        //    this.Controls.Add(this.txtNom);
        //    this.Controls.Add(this.lblNom);
        //    this.Controls.Add(this.txtCin);
        //    this.Controls.Add(this.lblCin);
        //    this.Controls.Add(this.dgvEmployes);
        //    this.Name = "EmployeForm";
        //    this.Text = "Gestion des Employés";
        //    ((System.ComponentModel.ISupportInitialize)(this.dgvEmployes)).EndInit();
        //    this.ResumeLayout(false);
        //    this.PerformLayout();
        //}

        //private System.Windows.Forms.DataGridView dgvEmployes;
        //private System.Windows.Forms.TextBox txtCin;
        //private System.Windows.Forms.TextBox txtNom;
        //private System.Windows.Forms.TextBox txtPrenom;
        //private System.Windows.Forms.TextBox txtUtilisateur;
        //private System.Windows.Forms.TextBox txtMotDePasse;
        //private System.Windows.Forms.TextBox txtSalaire;
        //private System.Windows.Forms.Button btnAdd;
        //private System.Windows.Forms.Button btnUpdate;
        //private System.Windows.Forms.Button btnDelete;
        //private System.Windows.Forms.Button btnClear;
        //private System.Windows.Forms.Button btnRefresh;
        //private System.Windows.Forms.Label lblCin;
        //private System.Windows.Forms.Label lblNom;
        //private System.Windows.Forms.Label lblPrenom;
        //private System.Windows.Forms.Label lblUtilisateur;
        //private System.Windows.Forms.Label lblMotDePasse;
        //private System.Windows.Forms.Label lblSalaire;
    }
}