using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class SettingsForm : Form
    {
        private readonly Color CardBackground = Color.FromArgb(255, 255, 255);
        private readonly Color PrimaryAccent = Color.FromArgb(99, 102, 241);
        private readonly Color SuccessColor = Color.FromArgb(16, 185, 129);
        private readonly Color WarningColor = Color.FromArgb(245, 158, 11);
        private readonly Color TextPrimary = Color.FromArgb(31, 41, 55);
        private readonly Color TextSecondary = Color.FromArgb(107, 114, 128);
        private readonly Color BackgroundGray = Color.FromArgb(243, 244, 246);

        private LocalStorageRepository _repository;
        private Label _lblDataLocation = null!;
        private Label _lblEmployeeCount = null!;
        private Label _lblPayrollCount = null!;
        private Label _lblVacationCount = null!;

        public SettingsForm(LocalStorageRepository repository)
        {
            _repository = repository;
            InitializeComponents();
            LoadSettings();
        }

        private void InitializeComponents()
        {
            // Form settings
            Text = "Settings";
            Size = new Size(700, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = BackgroundGray;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BackgroundGray,
                Padding = new Padding(20),
                AutoScroll = true
            };

            // Header
            var titleLabel = new Label
            {
                Text = "⚙️ Settings",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            // Data Storage Section
            var storageCard = CreateCard("💾 Data Storage", 70);

            var lblStorageTitle = new Label
            {
                Text = "Data Location:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(20, 50),
                Size = new Size(640, 25)
            };

            _lblDataLocation = new Label
            {
                Font = new Font("Segoe UI", 9),
                ForeColor = TextSecondary,
                Location = new Point(20, 75),
                Size = new Size(640, 50),
                AutoSize = false
            };

            var btnOpenFolder = CreateButton("📂 Open Data Folder", new Point(20, 130), new Size(180, 40), SuccessColor);
            btnOpenFolder.Click += OpenDataFolder_Click;

            var btnBackup = CreateButton("💾 Backup Data", new Point(210, 130), new Size(150, 40), PrimaryAccent);
            btnBackup.Click += BackupData_Click;

            storageCard.Controls.AddRange(new Control[] { lblStorageTitle, _lblDataLocation, btnOpenFolder, btnBackup });

            // Statistics Section
            var statsCard = CreateCard("📊 Data Statistics", 300);

            var lblStatsTitle = new Label
            {
                Text = "Current Data:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(20, 50),
                Size = new Size(640, 25)
            };

            _lblEmployeeCount = new Label
            {
                Text = "Employees: 0",
                Font = new Font("Segoe UI", 10),
                ForeColor = TextSecondary,
                Location = new Point(20, 85),
                Size = new Size(300, 25)
            };

            _lblPayrollCount = new Label
            {
                Text = "Payroll Records: 0",
                Font = new Font("Segoe UI", 10),
                ForeColor = TextSecondary,
                Location = new Point(20, 110),
                Size = new Size(300, 25)
            };

            _lblVacationCount = new Label
            {
                Text = "Vacation Records: 0",
                Font = new Font("Segoe UI", 10),
                ForeColor = TextSecondary,
                Location = new Point(20, 135),
                Size = new Size(300, 25)
            };

            var btnRefresh = CreateButton("🔄 Refresh", new Point(20, 170), new Size(120, 35), PrimaryAccent);
            btnRefresh.Click += (s, e) => LoadSettings();

            statsCard.Controls.AddRange(new Control[] { lblStatsTitle, _lblEmployeeCount, _lblPayrollCount, _lblVacationCount, btnRefresh });

            // About Section
            var aboutCard = CreateCard("ℹ️ About", 460);

            var lblAbout = new Label
            {
                Text = "Employee Management System\n" +
                       "Version 1.0\n\n" +
                       "A modern employee management application with:\n" +
                       "• Employee CRUD operations\n" +
                       "• Payroll management\n" +
                       "• Vacation tracking\n" +
                       "• Statistics dashboard\n\n" +
                       "All data is stored locally as JSON files.",
                Font = new Font("Segoe UI", 9),
                ForeColor = TextSecondary,
                Location = new Point(20, 50),
                Size = new Size(640, 170),
                AutoSize = false
            };

            aboutCard.Controls.Add(lblAbout);

            // Close Button
            var btnClose = CreateButton("Close", new Point(560, 520), new Size(120, 40), Color.FromArgb(107, 114, 128));
            btnClose.Click += (s, e) => Close();

            mainPanel.Controls.AddRange(new Control[] { titleLabel, storageCard, statsCard, aboutCard, btnClose });
            Controls.Add(mainPanel);
        }

        private Panel CreateCard(string title, int yPos)
        {
            var card = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(640, 220),
                BackColor = CardBackground
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(card.ClientRectangle, 12))
                {
                    card.Region = new Region(path);
                    using (var brush = new SolidBrush(CardBackground))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(20, 15),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            card.Controls.Add(titleLabel);
            return card;
        }

        private Button CreateButton(string text, Point location, Size size, Color bgColor)
        {
            var button = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = bgColor,
                FlatStyle = FlatStyle.Flat,
                Location = location,
                Size = size,
                Cursor = Cursors.Hand,
                TabStop = false
            };
            button.FlatAppearance.BorderSize = 0;

            // Hover effect
            var originalColor = bgColor;
            button.MouseEnter += (s, e) =>
            {
                button.BackColor = Color.FromArgb(
                    Math.Max(0, bgColor.R - 20),
                    Math.Max(0, bgColor.G - 20),
                    Math.Max(0, bgColor.B - 20)
                );
            };
            button.MouseLeave += (s, e) => button.BackColor = originalColor;

            // Rounded corners
            button.Paint += (s, e) =>
            {
                var btn = (Button)s;
                using (var path = GetRoundedRectPath(btn.ClientRectangle, 8))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    btn.Region = new Region(path);
                }
            };

            return button;
        }

        private void LoadSettings()
        {
            try
            {
                string dataDir = _repository.GetDataDirectory();
                _lblDataLocation.Text = dataDir;

                // Get file sizes
                string employeeFile = Path.Combine(dataDir, "employees.json");
                string payrollFile = Path.Combine(dataDir, "payroll.json");
                string vacationFile = Path.Combine(dataDir, "vacations.json");

                long totalSize = 0;
                if (File.Exists(employeeFile)) totalSize += new FileInfo(employeeFile).Length;
                if (File.Exists(payrollFile)) totalSize += new FileInfo(payrollFile).Length;
                if (File.Exists(vacationFile)) totalSize += new FileInfo(vacationFile).Length;

                // Get counts
                var employees = _repository.GetAllEmployees();
                var payrollRecords = _repository.GetAllPayrollRecords();
                var vacationRecords = _repository.GetAllVacationRecords();

                _lblEmployeeCount.Text = $"👥 Employees: {employees.Count} records";
                _lblPayrollCount.Text = $"💰 Payroll Records: {payrollRecords.Count} records";
                _lblVacationCount.Text = $"🏖️ Vacation Records: {vacationRecords.Count} records";

                // Show storage info
                _lblDataLocation.Text = $"{dataDir}\n\nTotal Storage: {FormatBytes(totalSize)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private void OpenDataFolder_Click(object? sender, EventArgs e)
        {
            try
            {
                string dataDir = _repository.GetDataDirectory();
                System.Diagnostics.Process.Start("explorer.exe", dataDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening folder: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BackupData_Click(object? sender, EventArgs e)
        {
            try
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    dialog.Description = "Select backup location";
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string dataDir = _repository.GetDataDirectory();
                        string backupDir = Path.Combine(dialog.SelectedPath, $"EmployeeCRUD_Backup_{DateTime.Now:yyyyMMdd_HHmmss}");
                        Directory.CreateDirectory(backupDir);

                        // Copy all JSON files
                        foreach (var file in Directory.GetFiles(dataDir, "*.json"))
                        {
                            string fileName = Path.GetFileName(file);
                            string destFile = Path.Combine(backupDir, fileName);
                            File.Copy(file, destFile, true);
                        }

                        MessageBox.Show($"Backup created successfully!\n\n{backupDir}", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating backup: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            float r = radius;
            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
