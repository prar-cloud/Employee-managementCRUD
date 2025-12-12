using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class ModernVacationForm : Form
    {
        // Modern color palette
        private readonly Color PrimaryDark = Color.FromArgb(37, 42, 52);
        private readonly Color CardBackground = Color.FromArgb(255, 255, 255);
        private readonly Color PrimaryAccent = Color.FromArgb(99, 102, 241);
        private readonly Color SuccessColor = Color.FromArgb(16, 185, 129);
        private readonly Color WarningColor = Color.FromArgb(245, 158, 11);
        private readonly Color DangerColor = Color.FromArgb(239, 68, 68);
        private readonly Color TextPrimary = Color.FromArgb(31, 41, 55);
        private readonly Color TextSecondary = Color.FromArgb(107, 114, 128);
        private readonly Color BackgroundGray = Color.FromArgb(243, 244, 246);
        private readonly Color BorderColor = Color.FromArgb(229, 231, 235);

        private LocalStorageRepository _repository = null!;
        private DataGridView _vacationGrid = null!;
        private ComboBox _employeeFilterCombo = null!;

        public ModernVacationForm(LocalStorageRepository repository)
        {
            _repository = repository;
            InitializeComponents();
            LoadVacationData();
        }

        private void InitializeComponents()
        {
            // Form settings
            this.Text = "Vacation Management";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BackgroundGray;
            this.MinimumSize = new Size(1000, 650);

            // Main panel
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BackgroundGray,
                Padding = new Padding(30)
            };

            // Header section
            var headerLabel = new Label
            {
                Text = "🏖️ Vacation Management",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(30, 30)
            };

            var subtitleLabel = new Label
            {
                Text = "Track employee vacation days and time off",
                Font = new Font("Segoe UI", 12),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(30, 75)
            };

            // Action buttons panel
            var actionPanel = CreateActionPanel();
            actionPanel.Location = new Point(30, 120);

            // Filter panel
            var filterPanel = CreateFilterPanel();
            filterPanel.Location = new Point(30, 190);

            // DataGrid card
            var gridCard = CreateDataGridCard();
            gridCard.Location = new Point(30, 260);
            gridCard.Size = new Size(1120, 400);

            // Close button
            var closeButton = new Button
            {
                Text = "✕ Close",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = TextSecondary,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 45),
                Location = new Point(1030, 670),
                Cursor = Cursors.Hand
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();
            closeButton.Paint += (s, e) =>
            {
                var btn = (Button)s;
                using (var path = GetRoundedRectPath(btn.ClientRectangle, 8))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    btn.Region = new Region(path);
                }
            };

            mainPanel.Controls.AddRange(new Control[] {
                headerLabel, subtitleLabel, actionPanel, filterPanel, gridCard, closeButton
            });
            this.Controls.Add(mainPanel);
        }

        private Panel CreateActionPanel()
        {
            var panel = new Panel
            {
                Size = new Size(1120, 60),
                BackColor = Color.Transparent
            };

            var addVacationBtn = CreateModernButton("+ Add Vacation Days", SuccessColor, new Point(0, 0));
            addVacationBtn.Size = new Size(200, 50);
            addVacationBtn.Click += AddVacationDays_Click;

            var refreshBtn = CreateModernButton("🔄 Refresh", PrimaryAccent, new Point(220, 0));
            refreshBtn.Size = new Size(150, 50);
            refreshBtn.Click += (s, e) => LoadVacationData();

            panel.Controls.AddRange(new Control[] { addVacationBtn, refreshBtn });
            return panel;
        }

        private Panel CreateFilterPanel()
        {
            var panel = new Panel
            {
                Size = new Size(1120, 50),
                BackColor = Color.Transparent
            };

            var filterLabel = new Label
            {
                Text = "Filter by Employee:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(0, 13)
            };

            _employeeFilterCombo = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(170, 10),
                Size = new Size(300, 35),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            _employeeFilterCombo.SelectedIndexChanged += EmployeeFilter_Changed;

            panel.Controls.AddRange(new Control[] { filterLabel, _employeeFilterCombo });
            return panel;
        }

        private Panel CreateDataGridCard()
        {
            var card = new Panel
            {
                BackColor = CardBackground
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(card.ClientRectangle, 15))
                {
                    card.Region = new Region(path);
                    using (var brush = new SolidBrush(CardBackground))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
            };

            _vacationGrid = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(1080, 360),
                BackgroundColor = CardBackground,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                Font = new Font("Segoe UI", 10)
            };

            // Modern grid styling
            _vacationGrid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryAccent;
            _vacationGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _vacationGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            _vacationGrid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10);
            _vacationGrid.ColumnHeadersHeight = 45;

            _vacationGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 231, 255);
            _vacationGrid.DefaultCellStyle.SelectionForeColor = TextPrimary;
            _vacationGrid.DefaultCellStyle.BackColor = CardBackground;
            _vacationGrid.DefaultCellStyle.ForeColor = TextPrimary;
            _vacationGrid.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
            _vacationGrid.RowTemplate.Height = 50;

            _vacationGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);

            _vacationGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            _vacationGrid.GridColor = BorderColor;

            card.Controls.Add(_vacationGrid);
            return card;
        }

        private void LoadVacationData(int? employeeId = null)
        {
            try
            {
                List<Employee> employees = _repository.GetAllEmployees();

                if (employees == null || employees.Count == 0)
                {
                    _vacationGrid.DataSource = null;
                    return;
                }

                // Populate filter dropdown
                if (_employeeFilterCombo.Items.Count == 0)
                {
                    _employeeFilterCombo.Items.Add("All Employees");
                    foreach (var emp in employees)
                    {
                        _employeeFilterCombo.Items.Add($"{emp.RollNumber} - {emp.Name}");
                    }
                    _employeeFilterCombo.SelectedIndex = 0;
                }

                // Filter data if needed
                var filteredEmployees = employeeId.HasValue
                    ? employees.Where(e => e.RollNumber == employeeId.Value).ToList()
                    : employees;

                // Create vacation display data
                var vacationData = filteredEmployees.Select(emp => new
                {
                    ID = emp.RollNumber,
                    Name = emp.Name,
                    Department = emp.Department ?? "N/A",
                    Position = emp.Position ?? "N/A",
                    Available = emp.VacationDaysAvailable,
                    Used = emp.VacationDaysUsed,
                    Remaining = emp.VacationDaysAvailable - emp.VacationDaysUsed,
                    Status = GetVacationStatus(emp.VacationDaysAvailable - emp.VacationDaysUsed)
                }).ToList();

                _vacationGrid.DataSource = vacationData;

                // Configure columns
                if (_vacationGrid.Columns.Count > 0)
                {
                    _vacationGrid.Columns["ID"].Width = 70;
                    _vacationGrid.Columns["Name"].Width = 180;
                    _vacationGrid.Columns["Department"].Width = 150;
                    _vacationGrid.Columns["Position"].Width = 150;
                    _vacationGrid.Columns["Available"].HeaderText = "Total Available";
                    _vacationGrid.Columns["Available"].Width = 120;
                    _vacationGrid.Columns["Used"].HeaderText = "Days Used";
                    _vacationGrid.Columns["Used"].Width = 100;
                    _vacationGrid.Columns["Remaining"].HeaderText = "Days Remaining";
                    _vacationGrid.Columns["Remaining"].Width = 130;
                    _vacationGrid.Columns["Remaining"].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

                    // Color code remaining days
                    _vacationGrid.CellFormatting += (s, e) =>
                    {
                        if (_vacationGrid.Columns[e.ColumnIndex].Name == "Remaining" && e.Value != null)
                        {
                            int remaining = (int)e.Value;
                            if (remaining > 10)
                                e.CellStyle.ForeColor = SuccessColor;
                            else if (remaining > 5)
                                e.CellStyle.ForeColor = WarningColor;
                            else
                                e.CellStyle.ForeColor = DangerColor;
                        }
                        else if (_vacationGrid.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
                        {
                            string status = e.Value.ToString() ?? "";
                            if (status == "Good")
                                e.CellStyle.ForeColor = SuccessColor;
                            else if (status == "Low")
                                e.CellStyle.ForeColor = WarningColor;
                            else if (status == "Critical")
                                e.CellStyle.ForeColor = DangerColor;
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vacation data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetVacationStatus(int remainingDays)
        {
            if (remainingDays > 10) return "Good";
            if (remainingDays > 5) return "Low";
            return "Critical";
        }

        private void AddVacationDays_Click(object? sender, EventArgs e)
        {
            var addVacationForm = new ModernAddVacationDaysForm(_repository);
            if (addVacationForm.ShowDialog() == DialogResult.OK)
            {
                LoadVacationData();
            }
        }

        private void EmployeeFilter_Changed(object? sender, EventArgs e)
        {
            if (_employeeFilterCombo.SelectedIndex == 0)
            {
                LoadVacationData();
            }
            else
            {
                var selectedText = _employeeFilterCombo.SelectedItem?.ToString() ?? "";
                if (selectedText.Contains(" - "))
                {
                    var idPart = selectedText.Split(" - ")[0];
                    if (int.TryParse(idPart, out int empId))
                    {
                        LoadVacationData(empId);
                    }
                }
            }
        }

        private Button CreateModernButton(string text, Color bgColor, Point location)
        {
            var button = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = bgColor,
                FlatStyle = FlatStyle.Flat,
                Location = location,
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

    // Modern Add Vacation Days Form
    public class ModernAddVacationDaysForm : Form
    {
        private readonly Color CardBackground = Color.FromArgb(255, 255, 255);
        private readonly Color PrimaryAccent = Color.FromArgb(99, 102, 241);
        private readonly Color SuccessColor = Color.FromArgb(16, 185, 129);
        private readonly Color TextPrimary = Color.FromArgb(31, 41, 55);
        private readonly Color TextSecondary = Color.FromArgb(107, 114, 128);
        private readonly Color InputBackground = Color.FromArgb(249, 250, 251);
        private readonly Color BorderColor = Color.FromArgb(229, 231, 235);

        private LocalStorageRepository _repository = null!;
        private ComboBox _employeeCombo = null!;
        private NumericUpDown _daysNumeric = null!;

        public ModernAddVacationDaysForm(LocalStorageRepository repository)
        {
            _repository = repository;
            InitializeComponents();
            LoadEmployees();
        }

        private void InitializeComponents()
        {
            this.Text = "Add Vacation Days";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(243, 244, 246);
            this.Padding = new Padding(15);

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CardBackground,
                Padding = new Padding(30)
            };
            mainPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(mainPanel.ClientRectangle, 15))
                {
                    mainPanel.Region = new Region(path);
                }
            };

            var titleLabel = new Label
            {
                Text = "🏖️ Add Vacation Days",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(30, 30)
            };

            var employeeLabel = new Label
            {
                Text = "Select Employee:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(30, 100)
            };

            _employeeCombo = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(30, 125),
                Size = new Size(410, 35),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var daysLabel = new Label
            {
                Text = "Number of Days:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(30, 180)
            };

            _daysNumeric = new NumericUpDown
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(30, 205),
                Size = new Size(200, 35),
                Minimum = 1,
                Maximum = 365,
                Value = 10
            };

            var addButton = new Button
            {
                Text = "Add Days",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = SuccessColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 45),
                Location = new Point(30, 260),
                Cursor = Cursors.Hand
            };
            addButton.FlatAppearance.BorderSize = 0;
            addButton.Click += AddButton_Click;
            addButton.Paint += (s, e) =>
            {
                var btn = (Button)s;
                using (var path = GetRoundedRectPath(btn.ClientRectangle, 8))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    btn.Region = new Region(path);
                }
            };

            var cancelButton = new Button
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = TextSecondary,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 45),
                Location = new Point(200, 260),
                Cursor = Cursors.Hand
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (s, e) => this.Close();
            cancelButton.Paint += (s, e) =>
            {
                var btn = (Button)s;
                using (var path = GetRoundedRectPath(btn.ClientRectangle, 8))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    btn.Region = new Region(path);
                }
            };

            mainPanel.Controls.AddRange(new Control[] {
                titleLabel, employeeLabel, _employeeCombo, daysLabel, _daysNumeric, addButton, cancelButton
            });
            this.Controls.Add(mainPanel);
        }

        private void LoadEmployees()
        {
            List<Employee> employees = _repository.GetAllEmployees();
            foreach (var emp in employees)
            {
                _employeeCombo.Items.Add($"{emp.RollNumber} - {emp.Name}");
            }
            if (_employeeCombo.Items.Count > 0)
                _employeeCombo.SelectedIndex = 0;
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_employeeCombo.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select an employee", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedText = _employeeCombo.SelectedItem?.ToString() ?? "";
                var idPart = selectedText.Split(" - ")[0];
                int empId = int.Parse(idPart);

                List<Employee> employees = _repository.GetAllEmployees();
                var employee = employees.FirstOrDefault(e => e.RollNumber == empId);

                if (employee != null)
                {
                    employee.VacationDaysAvailable += (int)_daysNumeric.Value;
                    _repository.UpdateEmployee(employee);

                    MessageBox.Show($"Added {_daysNumeric.Value} vacation days to {employee.Name}",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
