using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class AddVacationDaysToAllForm : Form
    {
        private LocalStorageRepository _repository;

        // Professional Color Palette - Luxury Theme
        private readonly Color PrimaryAccent = Color.FromArgb(88, 101, 242);
        private readonly Color SuccessColor = Color.FromArgb(59, 130, 246);     // Professional Blue (replaced green)
        private readonly Color DangerColor = Color.FromArgb(239, 68, 68);
        private readonly Color GoldAccent = Color.FromArgb(234, 179, 8);        // Luxury Gold
        private readonly Color CardBackground = Color.FromArgb(255, 255, 255);
        private readonly Color BackgroundGray = Color.FromArgb(250, 251, 252);
        private readonly Color TextPrimary = Color.FromArgb(15, 23, 42);
        private readonly Color TextSecondary = Color.FromArgb(100, 116, 139);

        // Controls
        private NumericUpDown _numDays = null!;
        private Label _lblEmployeeCount = null!;
        private Button _btnAdd = null!;
        private Button _btnCancel = null!;

        public AddVacationDaysToAllForm(LocalStorageRepository repository)
        {
            _repository = repository;
            InitializeComponents();
            LoadEmployeeCount();
        }

        private void InitializeComponents()
        {
            // Form settings
            Text = "Add Vacation Days to All Employees";
            Size = new Size(500, 350);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = BackgroundGray;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 10F);

            // Main card panel
            var mainCard = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(440, 270),
                BackColor = CardBackground
            };
            AddCardShadow(mainCard);

            // Title
            var lblTitle = new Label
            {
                Text = "🎁 Add Vacation Days to All",
                Location = new Point(20, 20),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = PrimaryAccent
            };
            mainCard.Controls.Add(lblTitle);

            // Description
            var lblDescription = new Label
            {
                Text = "Add vacation days to all active employees at once",
                Location = new Point(20, 55),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 10F),
                ForeColor = TextSecondary
            };
            mainCard.Controls.Add(lblDescription);

            // Employee count label
            _lblEmployeeCount = new Label
            {
                Text = "Active Employees: Loading...",
                Location = new Point(20, 90),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = TextPrimary
            };
            mainCard.Controls.Add(_lblEmployeeCount);

            // Days to add label
            var lblDays = new Label
            {
                Text = "Vacation Days to Add:",
                Location = new Point(20, 130),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = TextPrimary
            };
            mainCard.Controls.Add(lblDays);

            // Days numeric input
            _numDays = new NumericUpDown
            {
                Location = new Point(230, 128),
                Size = new Size(180, 30),
                Font = new Font("Segoe UI", 12F),
                Minimum = 1,
                Maximum = 365,
                Value = 8,
                TextAlign = HorizontalAlignment.Center
            };
            mainCard.Controls.Add(_numDays);

            // Info box
            var infoPanel = new Panel
            {
                Location = new Point(20, 175),
                Size = new Size(400, 40),
                BackColor = Color.FromArgb(240, 249, 255)
            };
            infoPanel.Paint += (s, e) =>
            {
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, infoPanel.Width - 1, infoPanel.Height - 1), 8))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var brush = new SolidBrush(Color.FromArgb(240, 249, 255)))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
            };

            var lblInfo = new Label
            {
                Text = "ℹ️  This will add days to ALL active employees",
                Location = new Point(10, 10),
                Size = new Size(380, 20),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(3, 105, 161),
                BackColor = Color.Transparent
            };
            infoPanel.Controls.Add(lblInfo);
            mainCard.Controls.Add(infoPanel);

            // Buttons
            _btnAdd = CreateModernButton("✓ Add to All", 230, 225, 90, 35, SuccessColor);
            _btnAdd.Click += BtnAdd_Click;
            mainCard.Controls.Add(_btnAdd);

            _btnCancel = CreateModernButton("Cancel", 330, 225, 90, 35, DangerColor);
            _btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            mainCard.Controls.Add(_btnCancel);

            Controls.Add(mainCard);
        }

        private void LoadEmployeeCount()
        {
            try
            {
                var employees = _repository.GetAllEmployees();
                _lblEmployeeCount.Text = $"👥 Active Employees: {employees.Count}";
            }
            catch (Exception ex)
            {
                _lblEmployeeCount.Text = $"Error: {ex.Message}";
                _lblEmployeeCount.ForeColor = DangerColor;
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            try
            {
                int days = (int)_numDays.Value;
                var employees = _repository.GetAllEmployees();

                if (employees.Count == 0)
                {
                    MessageBox.Show("No active employees found.", "No Employees",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var result = MessageBox.Show(
                    $"Add {days} vacation day{(days == 1 ? "" : "s")} to {employees.Count} employee{(employees.Count == 1 ? "" : "s")}?\n\n" +
                    $"This action will update all active employee records.",
                    "Confirm Addition",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _repository.AddVacationDaysToAll(days);

                    MessageBox.Show(
                        $"Successfully added {days} vacation day{(days == 1 ? "" : "s")} to {employees.Count} employee{(employees.Count == 1 ? "" : "s")}!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding vacation days: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Button CreateModernButton(string text, int x, int y, int width, int height, Color color)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;

            btn.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                // More curved/rounded corners (increased from 8 to 16)
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, btn.Width - 1, btn.Height - 1), 16))
                using (var brush = new SolidBrush(btn.BackColor))
                {
                    e.Graphics.FillPath(brush, path);
                }

                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                e.Graphics.DrawString(btn.Text, btn.Font, Brushes.White, btn.ClientRectangle, sf);
            };

            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = ControlPaint.Light(color, 0.1f);
            };
            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = color;
            };

            return btn;
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            float diameter = radius * 2.0F;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void AddCardShadow(Panel panel)
        {
            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (var shadowPath = GetRoundedRectPath(
                    new Rectangle(0, 0, panel.Width, panel.Height), 12))
                {
                    using (var brush = new SolidBrush(panel.BackColor))
                    {
                        e.Graphics.FillPath(brush, shadowPath);
                    }
                }
            };
        }
    }
}
