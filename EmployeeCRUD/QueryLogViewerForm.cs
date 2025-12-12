using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class QueryLogViewerForm : Form
    {
        // Professional Color Palette - Luxury Dark theme
        private readonly Color PrimaryDark = Color.FromArgb(30, 30, 46);
        private readonly Color SecondaryDark = Color.FromArgb(45, 48, 71);
        private readonly Color AccentBlue = Color.FromArgb(137, 180, 250);
        private readonly Color AccentCyan = Color.FromArgb(137, 220, 235);       // Professional Cyan (replaced green)
        private readonly Color AccentRed = Color.FromArgb(243, 139, 168);
        private readonly Color AccentYellow = Color.FromArgb(249, 226, 175);
        private readonly Color GoldAccent = Color.FromArgb(234, 179, 8);        // Luxury Gold
        private readonly Color TextLight = Color.FromArgb(205, 214, 244);
        private readonly Color TextMuted = Color.FromArgb(147, 153, 178);
        private readonly Color BorderColor = Color.FromArgb(69, 71, 90);

        private DataGridView _logGrid = null!;
        private Button _btnRefresh = null!;
        private Button _btnClear = null!;
        private Button _btnClose = null!;
        private Label _lblTitle = null!;
        private Label _lblCount = null!;
        private Panel _headerPanel = null!;

        public QueryLogViewerForm()
        {
            InitializeComponents();
            LoadLogs();
        }

        private void InitializeComponents()
        {
            // Form settings
            Text = "Data Query Log Viewer";
            Size = new Size(1200, 700);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = PrimaryDark;
            Font = new Font("Consolas", 10F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Header Panel
            _headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1200, 80),
                BackColor = SecondaryDark
            };
            _headerPanel.Paint += (s, e) =>
            {
                // Gold accent line at the top
                using (var goldBrush = new LinearGradientBrush(
                    new Rectangle(0, 0, _headerPanel.Width, 4),
                    GoldAccent,
                    Color.FromArgb(202, 138, 4),  // Darker gold
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(goldBrush, 0, 0, _headerPanel.Width, 4);
                }

                // Border line at the bottom
                using (var pen = new Pen(BorderColor, 2))
                {
                    e.Graphics.DrawLine(pen, 0, _headerPanel.Height - 1, _headerPanel.Width, _headerPanel.Height - 1);
                }
            };

            // Title
            _lblTitle = new Label
            {
                Text = "📊 DATA QUERY LOG",
                Location = new Point(30, 20),
                Size = new Size(500, 35),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = AccentBlue,
                BackColor = Color.Transparent
            };
            _headerPanel.Controls.Add(_lblTitle);

            // Count label
            _lblCount = new Label
            {
                Text = "0 queries logged",
                Location = new Point(30, 55),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 10F),
                ForeColor = TextMuted,
                BackColor = Color.Transparent
            };
            _headerPanel.Controls.Add(_lblCount);

            Controls.Add(_headerPanel);

            // Main grid panel
            var gridPanel = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(1150, 490),
                BackColor = SecondaryDark
            };
            AddRoundedBorder(gridPanel);

            // Data Grid
            _logGrid = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(1130, 470),
                BackgroundColor = SecondaryDark,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = BorderColor,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowTemplate = { Height = 35 },
                Font = new Font("Consolas", 9F)
            };

            // Modern grid styling - Dark theme
            _logGrid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryDark;
            _logGrid.ColumnHeadersDefaultCellStyle.ForeColor = TextLight;
            _logGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _logGrid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10);
            _logGrid.ColumnHeadersHeight = 45;

            _logGrid.DefaultCellStyle.BackColor = SecondaryDark;
            _logGrid.DefaultCellStyle.ForeColor = TextLight;
            _logGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(69, 71, 90);
            _logGrid.DefaultCellStyle.SelectionForeColor = AccentBlue;
            _logGrid.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);

            _logGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(40, 42, 60);

            gridPanel.Controls.Add(_logGrid);
            Controls.Add(gridPanel);

            // Buttons
            int btnY = 610;
            _btnRefresh = CreateButton("🔄 Refresh", 750, btnY, AccentBlue);
            _btnRefresh.Click += (s, e) => LoadLogs();
            Controls.Add(_btnRefresh);

            _btnClear = CreateButton("🗑️ Clear Logs", 900, btnY, AccentRed);
            _btnClear.Click += BtnClear_Click;
            Controls.Add(_btnClear);

            _btnClose = CreateButton("✕ Close", 1050, btnY, TextMuted);
            _btnClose.Click += (s, e) => Close();
            Controls.Add(_btnClose);
        }

        private void LoadLogs()
        {
            try
            {
                var logs = DataQueryLogger.GetLogs();
                _logGrid.DataSource = null;
                _logGrid.DataSource = logs;

                ConfigureColumns();

                _lblCount.Text = $"{logs.Count} {(logs.Count == 1 ? "query" : "queries")} logged";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading logs: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureColumns()
        {
            if (_logGrid.Columns.Count == 0) return;

            if (_logGrid.Columns.Contains("Timestamp"))
            {
                _logGrid.Columns["Timestamp"]!.HeaderText = "Time";
                _logGrid.Columns["Timestamp"]!.Width = 180;
                _logGrid.Columns["Timestamp"]!.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            }

            if (_logGrid.Columns.Contains("Operation"))
            {
                _logGrid.Columns["Operation"]!.HeaderText = "Operation";
                _logGrid.Columns["Operation"]!.Width = 120;
                _logGrid.Columns["Operation"]!.DefaultCellStyle.Font = new Font("Consolas", 9F, FontStyle.Bold);
            }

            if (_logGrid.Columns.Contains("Entity"))
            {
                _logGrid.Columns["Entity"]!.HeaderText = "Entity";
                _logGrid.Columns["Entity"]!.Width = 120;
                _logGrid.Columns["Entity"]!.DefaultCellStyle.ForeColor = AccentYellow;
            }

            if (_logGrid.Columns.Contains("Details"))
            {
                _logGrid.Columns["Details"]!.HeaderText = "Details";
                _logGrid.Columns["Details"]!.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            if (_logGrid.Columns.Contains("FilePath"))
            {
                _logGrid.Columns["FilePath"]!.Visible = false;
            }

            if (_logGrid.Columns.Contains("Success"))
            {
                _logGrid.Columns["Success"]!.HeaderText = "Status";
                _logGrid.Columns["Success"]!.Width = 100;
            }

            // Custom painting for status column
            _logGrid.CellFormatting += LogGrid_CellFormatting;
        }

        private void LogGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (_logGrid.Columns[e.ColumnIndex].Name == "Success")
            {
                if (e.Value != null && e.Value is bool success)
                {
                    e.Value = success ? "✓ SUCCESS" : "✗ FAILED";
                    e.CellStyle.ForeColor = success ? AccentCyan : AccentRed;
                    e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    e.FormattingApplied = true;
                }
            }

            if (_logGrid.Columns[e.ColumnIndex].Name == "Operation")
            {
                if (e.Value != null)
                {
                    string operation = e.Value.ToString() ?? "";
                    switch (operation)
                    {
                        case "INSERT":
                            e.CellStyle.ForeColor = AccentCyan;
                            break;
                        case "UPDATE":
                            e.CellStyle.ForeColor = AccentBlue;
                            break;
                        case "DELETE":
                            e.CellStyle.ForeColor = AccentRed;
                            break;
                        case "SAVE":
                            e.CellStyle.ForeColor = AccentYellow;
                            break;
                    }
                }
            }
        }

        private void BtnClear_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to clear all query logs?",
                "Confirm Clear",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                DataQueryLogger.ClearLogs();
                LoadLogs();
            }
        }

        private Button CreateButton(string text, int x, int y, Color color)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(130, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = PrimaryDark,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;

            btn.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // More curved/rounded corners (increased from 8 to 18 for softer look)
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, btn.Width - 1, btn.Height - 1), 18))
                using (var brush = new SolidBrush(btn.BackColor))
                {
                    e.Graphics.FillPath(brush, path);
                }

                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                e.Graphics.DrawString(btn.Text, btn.Font, new SolidBrush(btn.ForeColor), btn.ClientRectangle, sf);
            };

            btn.MouseEnter += (s, e) => { btn.BackColor = ControlPaint.Light(color, 0.2f); };
            btn.MouseLeave += (s, e) => { btn.BackColor = color; };

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

        private void AddRoundedBorder(Panel panel)
        {
            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 12))
                using (var pen = new Pen(BorderColor, 2))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            };
        }
    }
}
