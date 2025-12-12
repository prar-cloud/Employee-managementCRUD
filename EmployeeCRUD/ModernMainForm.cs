using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class ModernMainForm : Form
    {
        private LocalStorageRepository _repository = null!;

        // Professional Color Palette - Luxury Theme
        private readonly Color PrimaryDark = Color.FromArgb(30, 35, 45);        // Darker Navy
        private readonly Color PrimaryAccent = Color.FromArgb(88, 101, 242);    // Vibrant Indigo
        private readonly Color SecondaryAccent = Color.FromArgb(139, 92, 246);  // Rich Purple
        private readonly Color SuccessColor = Color.FromArgb(59, 130, 246);     // Professional Blue (replaced green)
        private readonly Color WarningColor = Color.FromArgb(251, 146, 60);     // Vibrant Orange
        private readonly Color DangerColor = Color.FromArgb(239, 68, 68);       // Bright Red
        private readonly Color InfoColor = Color.FromArgb(59, 130, 246);        // Sky Blue
        private readonly Color GoldAccent = Color.FromArgb(234, 179, 8);        // Luxury Gold
        private readonly Color CardBackground = Color.FromArgb(255, 255, 255);  // Pure White
        private readonly Color BackgroundGray = Color.FromArgb(250, 251, 252);  // Softer Gray
        private readonly Color TextPrimary = Color.FromArgb(15, 23, 42);        // Slate 900
        private readonly Color TextSecondary = Color.FromArgb(100, 116, 139);   // Slate 500
        private readonly Color BorderLight = Color.FromArgb(226, 232, 240);     // Light Border

        // Controls
        private Panel _topBar = null!;
        private Panel _sideBar = null!;
        private Panel _mainContent = null!;
        private DataGridView _employeeGrid = null!;
        private TextBox _txtSearch = null!;
        private Label _lblTitle = null!;
        private Label _lblConnectionStatus = null!;

        public ModernMainForm()
        {
            try
            {
                _repository = new LocalStorageRepository();
                InitializeModernUI();
                LoadEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing application: {ex.Message}", "Initialization Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _repository = new LocalStorageRepository();
                InitializeModernUI();
            }
        }

        private void InitializeModernUI()
        {
            // Form Settings
            Text = "Employee Management System";
            Size = new Size(1400, 850);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = BackgroundGray;
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            FormBorderStyle = FormBorderStyle.None;

            // Create Top Bar
            CreateTopBar();

            // Create Side Bar
            CreateSideBar();

            // Create Main Content Area
            CreateMainContent();

            // Create Employee Grid
            CreateModernGrid();

            // Create Action Cards
            CreateActionCards();
        }

        private void CreateTopBar()
        {
            _topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = CardBackground
            };

            // Add subtle shadow effect
            _topBar.Paint += (s, e) =>
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 0, _topBar.Height - 2, _topBar.Width, 2);
                }
            };

            // App Title with Icon
            var titlePanel = new Panel
            {
                Location = new Point(30, 20),
                Size = new Size(400, 40),
                BackColor = Color.Transparent
            };

            // Icon background circle
            var iconCircle = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(40, 40),
                BackColor = PrimaryAccent
            };
            iconCircle.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new LinearGradientBrush(
                    iconCircle.ClientRectangle,
                    PrimaryAccent,
                    SecondaryAccent,
                    LinearGradientMode.ForwardDiagonal))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, 39, 39);
                }

                // Draw icon (person symbol)
                using (var pen = new Pen(Color.White, 2))
                {
                    e.Graphics.DrawEllipse(pen, 14, 8, 12, 12);
                    e.Graphics.DrawArc(pen, 10, 20, 20, 15, 0, 180);
                }
            };
            titlePanel.Controls.Add(iconCircle);

            _lblTitle = new Label
            {
                Text = "Employee Management",
                Location = new Point(50, 0),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = TextPrimary,
                BackColor = Color.Transparent
            };
            titlePanel.Controls.Add(_lblTitle);

            var lblSubtitle = new Label
            {
                Text = "Manage your workforce efficiently",
                Location = new Point(50, 25),
                Size = new Size(300, 15),
                Font = new Font("Segoe UI", 9F),
                ForeColor = TextSecondary,
                BackColor = Color.Transparent
            };
            titlePanel.Controls.Add(lblSubtitle);

            _topBar.Controls.Add(titlePanel);

            // Connection Status Badge
            _lblConnectionStatus = new Label
            {
                Location = new Point(1150, 25),
                Size = new Size(220, 30),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                ForeColor = Color.White
            };
            _lblConnectionStatus.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(InfoColor))
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, _lblConnectionStatus.Width - 1, _lblConnectionStatus.Height - 1), 15))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };
            UpdateConnectionStatus();
            _topBar.Controls.Add(_lblConnectionStatus);

            Controls.Add(_topBar);
        }

        private void CreateSideBar()
        {
            _sideBar = new Panel
            {
                Location = new Point(0, 80),
                Size = new Size(280, 770),
                BackColor = CardBackground
            };

            _sideBar.Paint += (s, e) =>
            {
                // Gold vertical accent line on the left edge
                using (var goldBrush = new LinearGradientBrush(
                    new Rectangle(0, 0, 4, _sideBar.Height),
                    Color.FromArgb(234, 179, 8),   // Gold
                    Color.FromArgb(202, 138, 4),   // Darker Gold
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(goldBrush, 0, 0, 4, _sideBar.Height);
                }

                // Subtle shadow on the right edge
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, _sideBar.Width - 2, 0, 2, _sideBar.Height);
                }
            };

            int yPos = 30;

            // Create modern menu buttons
            CreateModernMenuButton("🏠 Dashboard", yPos, true, (s, e) => LoadEmployees());
            yPos += 60;

            CreateModernMenuButton("➕ Add Employee", yPos, false, (s, e) => AddEmployee());
            yPos += 60;

            CreateModernMenuButton("💼 Payroll", yPos, false, (s, e) => ShowPayroll());
            yPos += 60;

            CreateModernMenuButton("🏖️ Vacations", yPos, false, (s, e) => ShowVacation());
            yPos += 60;

            CreateModernMenuButton("📊 Statistics", yPos, false, (s, e) => ShowStatistics());
            yPos += 60;

            CreateModernMenuButton("📝 Query Log", yPos, false, (s, e) => ShowQueryLog());
            yPos += 60;

            CreateModernMenuButton("⚙️ Settings", yPos, false, (s, e) => ShowSettings());

            Controls.Add(_sideBar);
        }

        private void CreateModernMenuButton(string text, int yPos, bool isActive, EventHandler onClick)
        {
            var btn = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(240, 50),
                BackColor = isActive ? Color.FromArgb(240, 242, 255) : Color.Transparent,
                Cursor = Cursors.Hand
            };

            btn.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                if (isActive)
                {
                    using (var brush = new SolidBrush(Color.FromArgb(240, 242, 255)))
                    using (var path = GetRoundedRectPath(new Rectangle(0, 0, btn.Width - 1, btn.Height - 1), 12))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

                    // Active indicator
                    using (var indicatorBrush = new LinearGradientBrush(
                        new Rectangle(0, 0, 4, btn.Height),
                        PrimaryAccent, SecondaryAccent, LinearGradientMode.Vertical))
                    {
                        e.Graphics.FillRectangle(indicatorBrush, 0, 10, 4, 30);
                    }
                }
            };

            var label = new Label
            {
                Text = text,
                Location = new Point(15, 0),
                Size = new Size(220, 50),
                Font = new Font("Segoe UI", 11F, isActive ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isActive ? PrimaryAccent : TextSecondary,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            label.Click += onClick;
            btn.Click += onClick;

            btn.MouseEnter += (s, e) =>
            {
                if (!isActive) btn.BackColor = Color.FromArgb(248, 250, 252);
            };
            btn.MouseLeave += (s, e) =>
            {
                if (!isActive) btn.BackColor = Color.Transparent;
            };

            btn.Controls.Add(label);
            _sideBar.Controls.Add(btn);
        }

        private void CreateMainContent()
        {
            _mainContent = new Panel
            {
                Location = new Point(280, 80),
                Size = new Size(1120, 770),
                BackColor = BackgroundGray,
                AutoScroll = true
            };

            Controls.Add(_mainContent);
        }

        private void CreateModernGrid()
        {
            // Search Card
            var searchCard = new Panel
            {
                Location = new Point(30, 20),
                Size = new Size(1060, 80),
                BackColor = CardBackground
            };
            AddCardShadow(searchCard);

            var lblSearch = new Label
            {
                Text = "🔍 Search Employees",
                Location = new Point(20, 15),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = TextPrimary
            };
            searchCard.Controls.Add(lblSearch);

            _txtSearch = new TextBox
            {
                Location = new Point(5, 5),
                Size = new Size(344, 25),
                Font = new Font("Segoe UI", 11F),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = TextPrimary
            };
            _txtSearch.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    SearchEmployees();
                    e.Handled = true;
                }
            };
            _txtSearch.TextChanged += (s, e) =>
            {
                // Real-time search as user types
                if (string.IsNullOrWhiteSpace(_txtSearch.Text))
                {
                    LoadEmployees();
                }
            };

            var txtPanel = new Panel
            {
                Location = new Point(18, 43),
                Size = new Size(354, 35),
                BackColor = Color.White
            };
            txtPanel.Paint += (s, e) =>
            {
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, txtPanel.Width - 1, txtPanel.Height - 1), 8))
                using (var pen = new Pen(Color.FromArgb(203, 213, 225), 2))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawPath(pen, path);
                }
            };
            txtPanel.Controls.Add(_txtSearch);
            searchCard.Controls.Add(txtPanel);

            var btnSearch = CreateModernButton("Search", 390, 43, 100, 29, InfoColor);
            btnSearch.Click += (s, e) => SearchEmployees();
            searchCard.Controls.Add(btnSearch);

            var btnClear = CreateModernButton("Clear", 505, 43, 100, 29, TextSecondary);
            btnClear.Click += (s, e) =>
            {
                _txtSearch.Clear();
                LoadEmployees();
            };
            searchCard.Controls.Add(btnClear);

            _mainContent.Controls.Add(searchCard);

            // Grid Card
            var gridCard = new Panel
            {
                Location = new Point(30, 120),
                Size = new Size(1060, 500),
                BackColor = CardBackground
            };
            AddCardShadow(gridCard);

            _employeeGrid = new DataGridView
            {
                Location = new Point(15, 15),
                Size = new Size(1030, 470),
                BackgroundColor = CardBackground,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(226, 232, 240),
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowTemplate = { Height = 45 },
                Font = new Font("Segoe UI", 10F)
            };

            // Modern grid styling
            _employeeGrid.ColumnHeadersDefaultCellStyle.BackColor = BackgroundGray;
            _employeeGrid.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
            _employeeGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _employeeGrid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10);
            _employeeGrid.ColumnHeadersHeight = 50;

            _employeeGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 242, 255);
            _employeeGrid.DefaultCellStyle.SelectionForeColor = TextPrimary;
            _employeeGrid.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);

            _employeeGrid.AlternatingRowsDefaultCellStyle.BackColor = BackgroundGray;

            _employeeGrid.DoubleClick += (s, e) => EditEmployee();

            gridCard.Controls.Add(_employeeGrid);
            _mainContent.Controls.Add(gridCard);
        }

        private void CreateActionCards()
        {
            int xPos = 30;
            int yPos = 640;

            CreateActionCard("✏️ Edit", "Modify employee details", xPos, yPos, InfoColor, (s, e) => EditEmployee());
            xPos += 220;

            CreateActionCard("🗑️ Delete", "Remove employee", xPos, yPos, DangerColor, (s, e) => DeleteEmployee());
            xPos += 220;

            CreateActionCard("🔄 Refresh", "Reload data", xPos, yPos, SuccessColor, (s, e) => LoadEmployees());
            xPos += 220;

            CreateActionCard("📈 Add Days", "Add vacation days", xPos, yPos, WarningColor, (s, e) => AddVacationDays());
            xPos += 220;

            CreateActionCard("🎁 Add to All", "Add days to all", xPos, yPos, Color.FromArgb(139, 92, 246), (s, e) => AddVacationDaysToAll());
        }

        private void CreateActionCard(string title, string description, int x, int y, Color color, EventHandler onClick)
        {
            var card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(200, 90),
                BackColor = CardBackground,
                Cursor = Cursors.Hand
            };
            AddCardShadow(card);

            card.MouseEnter += (s, e) => card.BackColor = BackgroundGray;
            card.MouseLeave += (s, e) => card.BackColor = CardBackground;
            card.Click += onClick;

            var lblTitle = new Label
            {
                Text = title,
                Location = new Point(15, 20),
                Size = new Size(170, 25),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = color,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            lblTitle.Click += onClick;
            card.Controls.Add(lblTitle);

            var lblDesc = new Label
            {
                Text = description,
                Location = new Point(15, 45),
                Size = new Size(170, 20),
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = TextSecondary,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            lblDesc.Click += onClick;
            card.Controls.Add(lblDesc);

            _mainContent.Controls.Add(card);
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
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;

            btn.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // More curved/rounded corners (increased from 6 to 14)
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, btn.Width - 1, btn.Height - 1), 14))
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

            return btn;
        }

        #region Helper Methods

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

                // Draw shadow
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

        private void UpdateConnectionStatus()
        {
            _lblConnectionStatus.Text = "💾 JSON Storage";
            _lblConnectionStatus.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(SuccessColor))
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, _lblConnectionStatus.Width - 1, _lblConnectionStatus.Height - 1), 15))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };
        }

        #endregion

        #region Data Operations

        private void LoadEmployees()
        {
            try
            {
                if (_repository == null || _employeeGrid == null)
                    return;

                var employees = _repository.GetAllEmployees();
                _employeeGrid.DataSource = null;
                _employeeGrid.DataSource = employees?.ToList();
                ConfigureGridColumns();
            }
            catch (Exception ex)
            {
                ShowModernMessageBox("Error", $"Error loading employees: {ex.Message}", DangerColor);
            }
        }

        private void SearchEmployees()
        {
            try
            {
                string searchTerm = _txtSearch.Text.Trim();
                if (string.IsNullOrEmpty(searchTerm))
                {
                    LoadEmployees();
                    return;
                }

                var employees = _repository.SearchEmployees(searchTerm);
                _employeeGrid.DataSource = null;
                _employeeGrid.DataSource = employees?.ToList();
                ConfigureGridColumns();
            }
            catch (Exception ex)
            {
                ShowModernMessageBox("Error", $"Error searching: {ex.Message}", DangerColor);
            }
        }

        private void ConfigureGridColumns()
        {
            try
            {
                if (_employeeGrid?.Columns == null || _employeeGrid.Columns.Count == 0)
                    return;

                if (_employeeGrid.Columns.Contains("Id"))
                    _employeeGrid.Columns["Id"]!.Visible = false;
                if (_employeeGrid.Columns.Contains("IsActive"))
                    _employeeGrid.Columns["IsActive"]!.Visible = false;
                if (_employeeGrid.Columns.Contains("CreatedDate"))
                    _employeeGrid.Columns["CreatedDate"]!.Visible = false;
                if (_employeeGrid.Columns.Contains("LastModifiedDate"))
                    _employeeGrid.Columns["LastModifiedDate"]!.Visible = false;

                if (_employeeGrid.Columns.Contains("RollNumber"))
                {
                    _employeeGrid.Columns["RollNumber"]!.HeaderText = "ID";
                    _employeeGrid.Columns["RollNumber"]!.Width = 80;
                }
                if (_employeeGrid.Columns.Contains("Salary"))
                    _employeeGrid.Columns["Salary"]!.DefaultCellStyle.Format = "C2";
                if (_employeeGrid.Columns.Contains("HireDate"))
                    _employeeGrid.Columns["HireDate"]!.DefaultCellStyle.Format = "MM/dd/yyyy";
            }
            catch { }
        }

        private void AddEmployee()
        {
            var form = new EmployeeFormEnhanced(_repository);
            if (form.ShowDialog() == DialogResult.OK)
                LoadEmployees();
        }

        private void EditEmployee()
        {
            if (_employeeGrid.SelectedRows.Count == 0)
            {
                ShowModernMessageBox("Notice", "Please select an employee to edit", WarningColor);
                return;
            }

            var employee = (Employee)_employeeGrid.SelectedRows[0].DataBoundItem;
            var form = new EmployeeFormEnhanced(_repository, employee);
            if (form.ShowDialog() == DialogResult.OK)
                LoadEmployees();
        }

        private void DeleteEmployee()
        {
            if (_employeeGrid.SelectedRows.Count == 0)
            {
                ShowModernMessageBox("Notice", "Please select an employee to delete", WarningColor);
                return;
            }

            var employee = (Employee)_employeeGrid.SelectedRows[0].DataBoundItem;
            var result = MessageBox.Show(
                $"Delete employee '{employee.Name}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _repository.DeleteEmployee(employee.RollNumber);
                    ShowModernMessageBox("Success", "Employee deleted successfully", SuccessColor);
                    LoadEmployees();
                }
                catch (Exception ex)
                {
                    ShowModernMessageBox("Error", $"Error: {ex.Message}", DangerColor);
                }
            }
        }

        private void ShowPayroll()
        {
            var form = new PayrollForm(_repository);
            form.ShowDialog();
        }

        private void ShowVacation()
        {
            var form = new VacationForm(_repository);
            form.ShowDialog();
        }

        private void ShowStatistics()
        {
            var form = new StatisticsFormEnhanced(_repository);
            form.ShowDialog();
        }

        private void AddVacationDays()
        {
            var form = new AddVacationDaysForm(_repository);
            if (form.ShowDialog() == DialogResult.OK)
                LoadEmployees();
        }

        private void AddVacationDaysToAll()
        {
            var form = new AddVacationDaysToAllForm(_repository);
            if (form.ShowDialog() == DialogResult.OK)
                LoadEmployees();
        }

        private void ShowQueryLog()
        {
            var form = new QueryLogViewerForm();
            form.ShowDialog();
        }

        private void ShowSettings()
        {
            var form = new SettingsForm(_repository);
            form.ShowDialog();
        }

        private void ShowModernMessageBox(string title, string message, Color color)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}
