using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class ModernPayrollForm : Form
    {
        // Modern color palette
        private readonly Color PrimaryDark = Color.FromArgb(37, 42, 52);
        private readonly Color CardBackground = Color.FromArgb(255, 255, 255);
        private readonly Color PrimaryAccent = Color.FromArgb(99, 102, 241);
        private readonly Color SuccessColor = Color.FromArgb(16, 185, 129);
        private readonly Color TextPrimary = Color.FromArgb(31, 41, 55);
        private readonly Color TextSecondary = Color.FromArgb(107, 114, 128);
        private readonly Color BackgroundGray = Color.FromArgb(243, 244, 246);
        private readonly Color BorderColor = Color.FromArgb(229, 231, 235);

        private LocalStorageRepository _repository = null!;
        private DataGridView _payrollGrid = null!;
        private ComboBox _employeeFilterCombo = null!;
        private Label _totalPayrollLabel = null!;

        public ModernPayrollForm(LocalStorageRepository repository)
        {
            _repository = repository;
            InitializeComponents();
            LoadPayrollData();
        }

        private void InitializeComponents()
        {
            // Form settings
            this.Text = "Payroll Management";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BackgroundGray;
            this.MinimumSize = new Size(900, 600);

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
                Text = "💰 Payroll Management",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(30, 30)
            };

            var subtitleLabel = new Label
            {
                Text = "Track and manage employee compensation",
                Font = new Font("Segoe UI", 12),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(30, 75)
            };

            // Filter panel
            var filterPanel = CreateFilterPanel();
            filterPanel.Location = new Point(30, 120);

            // Stats cards
            _totalPayrollLabel = new Label
            {
                Text = "Total Payroll: $0.00",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = SuccessColor,
                AutoSize = true,
                Location = new Point(30, 180),
                BackColor = Color.Transparent
            };

            // DataGrid card
            var gridCard = CreateDataGridCard();
            gridCard.Location = new Point(30, 220);
            gridCard.Size = new Size(1020, 380);

            // Close button
            var closeButton = new Button
            {
                Text = "✕ Close",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = TextSecondary,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 45),
                Location = new Point(930, 615),
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
                headerLabel, subtitleLabel, filterPanel, _totalPayrollLabel, gridCard, closeButton
            });
            this.Controls.Add(mainPanel);
        }

        private Panel CreateFilterPanel()
        {
            var panel = new Panel
            {
                Size = new Size(1020, 50),
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

            _payrollGrid = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(980, 340),
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
            _payrollGrid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryAccent;
            _payrollGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _payrollGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            _payrollGrid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10);
            _payrollGrid.ColumnHeadersHeight = 45;

            _payrollGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 231, 255);
            _payrollGrid.DefaultCellStyle.SelectionForeColor = TextPrimary;
            _payrollGrid.DefaultCellStyle.BackColor = CardBackground;
            _payrollGrid.DefaultCellStyle.ForeColor = TextPrimary;
            _payrollGrid.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
            _payrollGrid.RowTemplate.Height = 50;

            _payrollGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);

            _payrollGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            _payrollGrid.GridColor = BorderColor;

            card.Controls.Add(_payrollGrid);
            return card;
        }

        private void LoadPayrollData(int? employeeId = null)
        {
            try
            {
                List<Employee> employees = _repository.GetAllEmployees();

                if (employees == null || employees.Count == 0)
                {
                    _payrollGrid.DataSource = null;
                    _totalPayrollLabel.Text = "Total Payroll: $0.00";
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

                // Create payroll display data
                var payrollData = filteredEmployees.Select(emp => new
                {
                    ID = emp.RollNumber,
                    Name = emp.Name,
                    Department = emp.Department ?? "N/A",
                    Position = emp.Position ?? "N/A",
                    Salary = emp.Salary,
                    HireDate = emp.HireDate?.ToString("MMM dd, yyyy") ?? "N/A",
                    Status = emp.IsActive ? "Active" : "Inactive"
                }).ToList();

                _payrollGrid.DataSource = payrollData;

                // Configure columns
                if (_payrollGrid.Columns.Count > 0)
                {
                    _payrollGrid.Columns["ID"].Width = 70;
                    _payrollGrid.Columns["Name"].Width = 200;
                    _payrollGrid.Columns["Salary"].DefaultCellStyle.Format = "C2";
                    _payrollGrid.Columns["Salary"].DefaultCellStyle.ForeColor = SuccessColor;
                    _payrollGrid.Columns["Salary"].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }

                // Calculate total
                double totalPayroll = filteredEmployees
                    .Where(e => e.IsActive)
                    .Sum(e => (double)e.Salary);
                _totalPayrollLabel.Text = $"Total Payroll: ${totalPayroll:N2}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading payroll data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EmployeeFilter_Changed(object? sender, EventArgs e)
        {
            if (_employeeFilterCombo.SelectedIndex == 0)
            {
                LoadPayrollData();
            }
            else
            {
                var selectedText = _employeeFilterCombo.SelectedItem?.ToString() ?? "";
                if (selectedText.Contains(" - "))
                {
                    var idPart = selectedText.Split(" - ")[0];
                    if (int.TryParse(idPart, out int empId))
                    {
                        LoadPayrollData(empId);
                    }
                }
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
