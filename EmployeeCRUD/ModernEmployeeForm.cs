using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class ModernEmployeeForm : Form
    {
        // Modern color palette
        private readonly Color PrimaryDark = Color.FromArgb(37, 42, 52);
        private readonly Color CardBackground = Color.FromArgb(255, 255, 255);
        private readonly Color PrimaryAccent = Color.FromArgb(99, 102, 241);    // Indigo
        private readonly Color SecondaryAccent = Color.FromArgb(139, 92, 246);  // Purple
        private readonly Color SuccessColor = Color.FromArgb(16, 185, 129);
        private readonly Color TextPrimary = Color.FromArgb(31, 41, 55);
        private readonly Color TextSecondary = Color.FromArgb(107, 114, 128);
        private readonly Color BorderColor = Color.FromArgb(229, 231, 235);
        private readonly Color InputBackground = Color.FromArgb(249, 250, 251);

        private LocalStorageRepository _repository = null!;
        private Employee? _existingEmployee;

        // Form controls
        private TextBox _rollNumberTextBox = null!;
        private TextBox _nameTextBox = null!;
        private NumericUpDown _ageNumeric = null!;
        private NumericUpDown _salaryNumeric = null!;
        private TextBox _departmentTextBox = null!;
        private TextBox _positionTextBox = null!;
        private TextBox _emailTextBox = null!;
        private TextBox _phoneTextBox = null!;
        private DateTimePicker _hireDatePicker = null!;
        private Button _saveButton = null!;
        private Button _cancelButton = null!;

        public ModernEmployeeForm(LocalStorageRepository repository, Employee? employee = null)
        {
            _repository = repository;
            _existingEmployee = employee;
            InitializeComponents();

            if (_existingEmployee != null)
            {
                LoadEmployeeData();
            }
        }

        private void InitializeComponents()
        {
            // Form settings
            this.Text = _existingEmployee == null ? "Add New Employee" : "Edit Employee";
            this.Size = new Size(800, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(243, 244, 246);
            this.Padding = new Padding(20);

            // Main container panel
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CardBackground,
                Padding = new Padding(40)
            };
            mainPanel.Paint += (s, e) => DrawRoundedPanel(e.Graphics, mainPanel, 20);

            // Header
            var headerLabel = new Label
            {
                Text = _existingEmployee == null ? "✨ Add New Employee" : "✏️ Edit Employee",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(40, 30)
            };

            var subtitleLabel = new Label
            {
                Text = _existingEmployee == null ? "Fill in the employee details below" : "Update employee information",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(40, 70)
            };

            // Create form fields in a flowing layout
            var formPanel = new Panel
            {
                Location = new Point(40, 110),
                Size = new Size(700, 470),
                AutoScroll = true,
                BackColor = CardBackground
            };

            int yPos = 0;
            int fieldSpacing = 85;

            // Roll Number
            CreateFormField(formPanel, "Employee ID", ref _rollNumberTextBox, yPos,
                _existingEmployee != null);
            if (_existingEmployee != null)
            {
                _rollNumberTextBox.Text = _existingEmployee.RollNumber.ToString();
            }
            yPos += fieldSpacing;

            // Name
            CreateFormField(formPanel, "Full Name", ref _nameTextBox, yPos);
            _nameTextBox.Text = _existingEmployee?.Name ?? "";
            yPos += fieldSpacing;

            // Two-column layout for Age and Salary
            CreateTwoColumnNumericFields(formPanel, yPos);
            yPos += fieldSpacing;

            // Department
            CreateFormField(formPanel, "Department", ref _departmentTextBox, yPos);
            _departmentTextBox.Text = _existingEmployee?.Department ?? "";
            yPos += fieldSpacing;

            // Position
            CreateFormField(formPanel, "Position", ref _positionTextBox, yPos);
            _positionTextBox.Text = _existingEmployee?.Position ?? "";
            yPos += fieldSpacing;

            // Email
            CreateFormField(formPanel, "Email Address", ref _emailTextBox, yPos);
            _emailTextBox.Text = _existingEmployee?.Email ?? "";
            yPos += fieldSpacing;

            // Phone
            CreateFormField(formPanel, "Phone Number", ref _phoneTextBox, yPos);
            _phoneTextBox.Text = _existingEmployee?.Phone ?? "";
            yPos += fieldSpacing;

            // Hire Date
            CreateDateField(formPanel, yPos);

            // Buttons
            var buttonPanel = new Panel
            {
                Location = new Point(40, 590),
                Size = new Size(700, 60),
                BackColor = CardBackground
            };

            _saveButton = CreateModernButton("Save Employee", SuccessColor, new Point(0, 0));
            _saveButton.Size = new Size(200, 50);
            _saveButton.Click += SaveButton_Click;

            _cancelButton = CreateModernButton("Cancel", Color.FromArgb(107, 114, 128), new Point(220, 0));
            _cancelButton.Size = new Size(150, 50);
            _cancelButton.Click += (s, e) => this.Close();

            buttonPanel.Controls.AddRange(new Control[] { _saveButton, _cancelButton });

            mainPanel.Controls.AddRange(new Control[] { headerLabel, subtitleLabel, formPanel, buttonPanel });
            this.Controls.Add(mainPanel);

            // Add close button
            var closeButton = new Button
            {
                Text = "×",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = TextSecondary,
                Size = new Size(40, 40),
                Location = new Point(this.Width - 60, 20),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = CardBackground
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();
            closeButton.MouseEnter += (s, e) => closeButton.ForeColor = Color.FromArgb(220, 38, 38);
            closeButton.MouseLeave += (s, e) => closeButton.ForeColor = TextSecondary;
            mainPanel.Controls.Add(closeButton);
        }

        private void CreateFormField(Panel parent, string labelText, ref TextBox textBox, int yPos, bool readOnly = false)
        {
            var label = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(0, yPos),
                AutoSize = true
            };

            textBox = new TextBox
            {
                Location = new Point(0, yPos + 25),
                Size = new Size(680, 40),
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.None,
                BackColor = InputBackground,
                ForeColor = TextPrimary,
                ReadOnly = readOnly
            };

            // Add padding to textbox
            textBox.Padding = new Padding(15, 10, 15, 10);
            textBox.Height = 45;

            // Custom paint for rounded corners
            textBox.Paint += (s, e) =>
            {
                var tb = (TextBox)s;
                using (var path = GetRoundedRectPath(tb.ClientRectangle, 8))
                using (var pen = new Pen(BorderColor, 1))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawPath(pen, path);
                }
            };

            parent.Controls.AddRange(new Control[] { label, textBox });
        }

        private void CreateTwoColumnNumericFields(Panel parent, int yPos)
        {
            // Age field
            var ageLabel = new Label
            {
                Text = "Age",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(0, yPos),
                AutoSize = true
            };

            _ageNumeric = new NumericUpDown
            {
                Location = new Point(0, yPos + 25),
                Size = new Size(330, 45),
                Font = new Font("Segoe UI", 12),
                Minimum = 18,
                Maximum = 100,
                Value = _existingEmployee?.Age ?? 25,
                BorderStyle = BorderStyle.None,
                BackColor = InputBackground
            };

            // Salary field
            var salaryLabel = new Label
            {
                Text = "Salary",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(350, yPos),
                AutoSize = true
            };

            _salaryNumeric = new NumericUpDown
            {
                Location = new Point(350, yPos + 25),
                Size = new Size(330, 45),
                Font = new Font("Segoe UI", 12),
                Minimum = 0,
                Maximum = 10000000,
                Value = _existingEmployee?.Salary ?? 50000,
                DecimalPlaces = 2,
                ThousandsSeparator = true,
                BorderStyle = BorderStyle.None,
                BackColor = InputBackground
            };

            parent.Controls.AddRange(new Control[] { ageLabel, _ageNumeric, salaryLabel, _salaryNumeric });
        }

        private void CreateDateField(Panel parent, int yPos)
        {
            var label = new Label
            {
                Text = "Hire Date",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(0, yPos),
                AutoSize = true
            };

            _hireDatePicker = new DateTimePicker
            {
                Location = new Point(0, yPos + 25),
                Size = new Size(680, 45),
                Font = new Font("Segoe UI", 12),
                Format = DateTimePickerFormat.Long,
                Value = _existingEmployee?.HireDate ?? DateTime.Now
            };

            parent.Controls.AddRange(new Control[] { label, _hireDatePicker });
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

        private void LoadEmployeeData()
        {
            if (_existingEmployee == null) return;

            _rollNumberTextBox.Text = _existingEmployee.RollNumber.ToString();
            _nameTextBox.Text = _existingEmployee.Name;
            _ageNumeric.Value = _existingEmployee.Age;
            _salaryNumeric.Value = _existingEmployee.Salary;
            _departmentTextBox.Text = _existingEmployee.Department ?? "";
            _positionTextBox.Text = _existingEmployee.Position ?? "";
            _emailTextBox.Text = _existingEmployee.Email ?? "";
            _phoneTextBox.Text = _existingEmployee.Phone ?? "";
            if (_existingEmployee.HireDate.HasValue)
                _hireDatePicker.Value = _existingEmployee.HireDate.Value;
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
                {
                    ShowModernMessageBox("Please enter employee name", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                if (_existingEmployee == null)
                {
                    // Add new employee
                    if (string.IsNullOrWhiteSpace(_rollNumberTextBox.Text))
                    {
                        ShowModernMessageBox("Please enter employee ID", "Validation Error", MessageBoxIcon.Warning);
                        return;
                    }

                    int rollNumber = int.Parse(_rollNumberTextBox.Text);

                    if (_repository.EmployeeExists(rollNumber))
                    {
                        ShowModernMessageBox("Employee with this ID already exists", "Duplicate ID", MessageBoxIcon.Warning);
                        return;
                    }

                    var newEmployee = new Employee
                    {
                        RollNumber = rollNumber,
                        Name = _nameTextBox.Text.Trim(),
                        Age = (int)_ageNumeric.Value,
                        Salary = _salaryNumeric.Value,
                        Department = _departmentTextBox.Text.Trim(),
                        Position = _positionTextBox.Text.Trim(),
                        Email = _emailTextBox.Text.Trim(),
                        Phone = _phoneTextBox.Text.Trim(),
                        HireDate = _hireDatePicker.Value,
                        IsActive = true
                    };

                    _repository.AddEmployee(newEmployee);
                    ShowModernMessageBox("Employee added successfully!", "Success", MessageBoxIcon.Information);
                }
                else
                {
                    // Update existing employee
                    _existingEmployee.Name = _nameTextBox.Text.Trim();
                    _existingEmployee.Age = (int)_ageNumeric.Value;
                    _existingEmployee.Salary = _salaryNumeric.Value;
                    _existingEmployee.Department = _departmentTextBox.Text.Trim();
                    _existingEmployee.Position = _positionTextBox.Text.Trim();
                    _existingEmployee.Email = _emailTextBox.Text.Trim();
                    _existingEmployee.Phone = _phoneTextBox.Text.Trim();
                    _existingEmployee.HireDate = _hireDatePicker.Value;

                    _repository.UpdateEmployee(_existingEmployee);
                    ShowModernMessageBox("Employee updated successfully!", "Success", MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                ShowModernMessageBox($"Error saving employee: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ShowModernMessageBox(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void DrawRoundedPanel(Graphics graphics, Control control, int radius)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = GetRoundedRectPath(control.ClientRectangle, radius))
            {
                control.Region = new Region(path);
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Add shadow effect
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        }
    }
}
