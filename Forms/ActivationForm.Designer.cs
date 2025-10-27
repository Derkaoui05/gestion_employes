using System.Drawing;
using System.Windows.Forms;

namespace GestionEmployes.Forms
{
    partial class ActivationForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Label lblMachineIdLabel;
        private Label lblMachineId;
        private Label lblActivationLabel;
        private TextBox txtActivationKey;
        private Button btnActivate;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblMachineIdLabel = new System.Windows.Forms.Label();
            this.lblMachineId = new System.Windows.Forms.Label();
            this.lblActivationLabel = new System.Windows.Forms.Label();
            this.txtActivationKey = new System.Windows.Forms.TextBox();
            this.btnActivate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblTitle.Location = new System.Drawing.Point(25, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(231, 38);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Activation Requise";
            // 
            // lblMachineIdLabel
            // 
            this.lblMachineIdLabel.AutoSize = true;
            this.lblMachineIdLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMachineIdLabel.Location = new System.Drawing.Point(25, 80);
            this.lblMachineIdLabel.Name = "lblMachineIdLabel";
            this.lblMachineIdLabel.Size = new System.Drawing.Size(202, 25);
            this.lblMachineIdLabel.TabIndex = 1;
            this.lblMachineIdLabel.Text = "Votre Machine ID (copié auto) :";
            // 
            // lblMachineId
            // 
            this.lblMachineId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.lblMachineId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMachineId.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMachineId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblMachineId.Location = new System.Drawing.Point(25, 110);
            this.lblMachineId.Name = "lblMachineId";
            this.lblMachineId.Padding = new System.Windows.Forms.Padding(8, 5, 8, 5);
            this.lblMachineId.Size = new System.Drawing.Size(400, 35);
            this.lblMachineId.TabIndex = 2;
            this.lblMachineId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblActivationLabel
            // 
            this.lblActivationLabel.AutoSize = true;
            this.lblActivationLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActivationLabel.Location = new System.Drawing.Point(25, 170);
            this.lblActivationLabel.Name = "lblActivationLabel";
            this.lblActivationLabel.Size = new System.Drawing.Size(133, 25);
            this.lblActivationLabel.TabIndex = 3;
            this.lblActivationLabel.Text = "Clé d'activation :";
            // 
            // txtActivationKey
            // 
            this.txtActivationKey.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtActivationKey.Location = new System.Drawing.Point(25, 200);
            this.txtActivationKey.Name = "txtActivationKey";
            this.txtActivationKey.Size = new System.Drawing.Size(400, 34);
            this.txtActivationKey.TabIndex = 0;
            this.txtActivationKey.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnActivate
            // 
            this.btnActivate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnActivate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActivate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnActivate.ForeColor = System.Drawing.Color.White;
            this.btnActivate.Location = new System.Drawing.Point(235, 260);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(90, 35);
            this.btnActivate.TabIndex = 1;
            this.btnActivate.Text = "Activer";
            this.btnActivate.UseVisualStyleBackColor = false;
            this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(335, 260);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 35);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Annuler";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ActivationForm
            // 
            this.AcceptButton = this.btnActivate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(450, 320);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnActivate);
            this.Controls.Add(this.txtActivationKey);
            this.Controls.Add(this.lblActivationLabel);
            this.Controls.Add(this.lblMachineId);
            this.Controls.Add(this.lblMachineIdLabel);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ActivationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Activation - GestionEmployes";
            this.Load += new System.EventHandler(this.ActivationForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}