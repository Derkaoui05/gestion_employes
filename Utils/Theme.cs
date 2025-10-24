using System;
using System.Drawing;
using System.Windows.Forms;

namespace GestionEmployes.Utils
{
    public static class Theme
    {
        // 🎨 Couleurs principales - Modern Color Palette
        public static Color PrimaryColor = Color.FromArgb(99, 102, 241);      // Indigo
        public static Color PrimaryDark = Color.FromArgb(79, 70, 229);        // Darker Indigo
        public static Color SecondaryColor = Color.FromArgb(139, 92, 246);    // Purple
        public static Color AccentColor = Color.FromArgb(236, 72, 153);       // Pink
        public static Color SuccessColor = Color.FromArgb(34, 197, 94);       // Green
        public static Color WarningColor = Color.FromArgb(251, 146, 60);      // Orange
        public static Color DangerColor = Color.FromArgb(239, 68, 68);        // Red
        public static Color InfoColor = Color.FromArgb(59, 130, 246);         // Blue
        
        public static Color BackgroundColor = Color.FromArgb(248, 250, 252);  // Light Gray
        public static Color CardBackground = Color.White;
        public static Color TextColor = Color.FromArgb(30, 41, 59);           // Slate
        public static Color TextSecondary = Color.FromArgb(100, 116, 139);    // Gray
        public static Color BorderColor = Color.FromArgb(226, 232, 240);      // Light Border
        public static Color ShadowColor = Color.FromArgb(30, 0, 0, 0);        // Subtle Shadow

        // Compatibilité avec les appels existants Theme.Apply(this)
        public static void Apply(Form form)
        {
            if (form == null) return;
            ApplyToControls(form);
        }

        // 🧱 Appliquer le thème à tout le formulaire
        public static void ApplyToControls(Form form)
        {
            form.BackColor = BackgroundColor;
            form.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            form.ForeColor = TextColor;

            foreach (Control control in form.Controls)
            {
                ApplyTheme(control);
            }
        }

        // 🎨 Create a modern card panel
        public static Panel CreateCard(int x, int y, int width, int height)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = CardBackground,
                Padding = new Padding(20)
            };
            return panel;
        }

        // 📝 Create a styled label
        public static Label CreateLabel(string text, int x, int y, bool isTitle = false)
        {
            var label = new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = isTitle ? TextColor : TextSecondary,
                Font = isTitle ? new Font("Segoe UI", 11F, FontStyle.Bold) : new Font("Segoe UI", 9.5F, FontStyle.Regular)
            };
            return label;
        }

        // 🔘 Create a modern styled button
        public static Button CreateButton(string text, int x, int y, string variant = "primary")
        {
            var button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(120, 40),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;

            switch (variant.ToLower())
            {
                case "success":
                    button.BackColor = SuccessColor;
                    button.ForeColor = Color.White;
                    break;
                case "warning":
                    button.BackColor = WarningColor;
                    button.ForeColor = Color.White;
                    break;
                case "danger":
                    button.BackColor = DangerColor;
                    button.ForeColor = Color.White;
                    break;
                case "info":
                    button.BackColor = InfoColor;
                    button.ForeColor = Color.White;
                    break;
                case "secondary":
                    button.BackColor = SecondaryColor;
                    button.ForeColor = Color.White;
                    break;
                default: // primary
                    button.BackColor = PrimaryColor;
                    button.ForeColor = Color.White;
                    break;
            }

            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(button.BackColor, 0.2f);
            return button;
        }

        // 📦 Create a styled text box
        public static TextBox CreateTextBox(int x, int y, int width = 250)
        {
            var textBox = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 32),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = TextColor,
                Font = new Font("Segoe UI", 10F)
            };
            return textBox;
        }

        // 🔧 Appliquer le thème à chaque contrôle
        private static void ApplyTheme(Control control)
        {
            // ✅ Utilisation des "is" plutôt que du pattern matching switch pour compatibilité
            if (control is Label)
            {
                control.ForeColor = TextColor;
            }
            else if (control is Button button)
            {
                ApplyButtonVariant(button);
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                button.Cursor = Cursors.Hand;
                button.Height = 40;
                button.Width = Math.Max(120, button.Width);
                button.Margin = new Padding(8, 5, 8, 5);
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.FlatStyle = FlatStyle.Flat;
                comboBox.BackColor = Color.White;
                comboBox.ForeColor = TextColor;
                comboBox.Font = new Font("Segoe UI", 10F);
            }
            else if (control is DateTimePicker dtp)
            {
                dtp.Font = new Font("Segoe UI", 10F);
            }
            else if (control is TextBox textBox)
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.BackColor = Color.White;
                textBox.ForeColor = TextColor;
                textBox.Font = new Font("Segoe UI", 10F);
            }
            else if (control is DataGridView grid)
            {
                StyleDataGrid(grid);
            }

            // 🌀 Pour les conteneurs : appliquer récursivement
            if (control is Panel || control is GroupBox || control is TabControl || control is TabPage)
            {
                foreach (Control inner in control.Controls)
                {
                    ApplyTheme(inner);
                }
            }
        }

        // 🟦 Style des boutons selon leur nom et Tag
        private static void ApplyButtonVariant(Button button)
        {
            string buttonName = button.Name?.ToLower() ?? "";
            string tag = button.Tag?.ToString()?.ToLower() ?? "";

            // Vérifier d'abord le nom du bouton
            if (buttonName.Contains("ajouter") || buttonName.Contains("add"))
            {
                button.BackColor = SuccessColor;
                button.ForeColor = Color.White;
            }
            else if (buttonName.Contains("modifier") || buttonName.Contains("update"))
            {
                button.BackColor = InfoColor;
                button.ForeColor = Color.White;
            }
            else if (buttonName.Contains("supprimer") || buttonName.Contains("delete"))
            {
                button.BackColor = DangerColor;
                button.ForeColor = Color.White;
            }
            else if (buttonName.Contains("vider") || buttonName.Contains("clear") || buttonName.Contains("effacer"))
            {
                button.BackColor = Color.FromArgb(148, 163, 184);
                button.ForeColor = Color.White;
            }
            else if (buttonName.Contains("actualiser") || buttonName.Contains("refresh"))
            {
                button.BackColor = SecondaryColor;
                button.ForeColor = Color.White;
            }
            else if (buttonName.Contains("export") || buttonName.Contains("exporter"))
            {
                button.BackColor = WarningColor;
                button.ForeColor = Color.White;
            }
            else if (buttonName.Contains("connexion") || buttonName.Contains("login"))
            {
                button.BackColor = PrimaryColor;
                button.ForeColor = Color.White;
            }
            else if (buttonName.Contains("annuler") || buttonName.Contains("cancel") || buttonName.Contains("quitter"))
            {
                button.BackColor = Color.FromArgb(100, 116, 139);
                button.ForeColor = Color.White;
            }
            // Sinon, utiliser le Tag si disponible
            else if (tag == "success")
            {
                button.BackColor = SuccessColor;
                button.ForeColor = Color.White;
            }
            else if (tag == "warning")
            {
                button.BackColor = WarningColor;
                button.ForeColor = Color.White;
            }
            else if (tag == "danger")
            {
                button.BackColor = DangerColor;
                button.ForeColor = Color.White;
            }
            else if (tag == "info")
            {
                button.BackColor = InfoColor;
                button.ForeColor = Color.White;
            }
            else if (tag == "primary")
            {
                button.BackColor = PrimaryColor;
                button.ForeColor = Color.White;
            }
            else
            {
                button.BackColor = PrimaryColor;
                button.ForeColor = Color.White;
            }

            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(button.BackColor, 0.2f);
        }

        // 📊 Style moderne DataGridView
        private static void StyleDataGrid(DataGridView grid)
        {
            grid.BorderStyle = BorderStyle.None;
            grid.BackgroundColor = CardBackground;
            grid.EnableHeadersVisualStyles = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;

            // Modern header style
            grid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 8, 10, 8);
            grid.ColumnHeadersHeight = 45;

            // Modern cell style
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = TextColor;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 231, 255); // Light indigo
            grid.DefaultCellStyle.SelectionForeColor = PrimaryDark;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            grid.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
            grid.GridColor = BorderColor;
            grid.RowTemplate.Height = 40;

            // Alternating row colors for better readability
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        }

        // 🎨 Style TabControl
        public static void StyleTabControl(TabControl tabControl)
        {
            tabControl.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            tabControl.ItemSize = new Size(120, 45);
            tabControl.Padding = new Point(20, 8);
        }
    }
}