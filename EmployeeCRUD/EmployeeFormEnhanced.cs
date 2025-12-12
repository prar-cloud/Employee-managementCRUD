using System;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class EmployeeFormEnhanced : Form
    {
        private LocalStorageRepository _repository;
        private Employee? _existingEmployee;
        private bool _isEditMode;

        private TextBox _txtRollNumber = null!;
        private TextBox _txtName = null!;
        private NumericUpDown _numAge = null!;
        private TextBox _txtDepartment = null!;
        private TextBox _txtPosition = null!;
        private DateTimePicker _dtpHireDate = null!;
        private TextBox _txtEmail = null!;
        private TextBox _txtPhone = null!;
        private NumericUpDown _numSalary = null!;
        private Label _lblVacationInfo = null!;

        public EmployeeFormEnhanced(LocalStorageRepository repository, Employee? existingEmployee = null)
        {
            _repository = repository;
            _existingEmployee = existingEmployee;
            _isEditMode = existingEmployee != null;

            InitializeComponents();
            if (_isEditMode)
            {
                LoadEmployeeData();
            }
        }

        private void InitializeComponents()
        {
            // Form settings
            Text = _isEditMode ? "Edit Employee" : "Add New Employee";
            Size = new Size(550, 650);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(236, 240, 241);

            // Title
            Label lblTitle = new Label
            {
                Text = _isEditMode ? "Edit Employee Information" : "Add New Employee",
                Location = new Point(20, 20),
                Size = new Size(500, 35),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            int yPos = 70;
            int labelX = 30;
            int controlX = 200;
            int controlWidth = 310;
            int rowHeight = 45;

            // Roll Number
            CreateLabel("Roll Number (ID):", labelX, yPos);
            _txtRollNumber = CreateTextBox(controlX, yPos, controlWidth);
            _txtRollNumber.Enabled = !_isEditMode;
            yPos += rowHeight;

            // Name
            CreateLabel("Name:", labelX, yPos);
            _txtName = CreateTextBox(controlX, yPos, controlWidth);
            yPos += rowHeight;

            // Age
            CreateLabel("Age:", labelX, yPos);
            _numAge = new NumericUpDown
            {
                Location = new Point(controlX, yPos),
                Size = new Size(controlWidth, 25),
                Font = new Font("Segoe UI", 10),
                Minimum = 18,
                Maximum = 100,
                Value = 25
            };
            Controls.Add(_numAge);
            yPos += rowHeight;

            // Department
            CreateLabel("Department:", labelX, yPos);
            _txtDepartment = CreateTextBox(controlX, yPos, controlWidth);
            yPos += rowHeight;

            // Position
            CreateLabel("Position:", labelX, yPos);
            _txtPosition = CreateTextBox(controlX, yPos, controlWidth);
            yPos += rowHeight;

            // Hire Date
            CreateLabel("Hire Date:", labelX, yPos);
            _dtpHireDate = new DateTimePicker
            {
                Location = new Point(controlX, yPos),
                Size = new Size(controlWidth, 25),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short
            };
            Controls.Add(_dtpHireDate);
            yPos += rowHeight;

            // Email
            CreateLabel("Email:", labelX, yPos);
            _txtEmail = CreateTextBox(controlX, yPos, controlWidth);
            yPos += rowHeight;

            // Phone
            CreateLabel("Phone:", labelX, yPos);
            _txtPhone = CreateTextBox(controlX, yPos, controlWidth);
            yPos += rowHeight;

            // Salary
            CreateLabel("Salary:", labelX, yPos);
            _numSalary = new NumericUpDown
            {
                Location = new Point(controlX, yPos),
                Size = new Size(controlWidth, 25),
                Font = new Font("Segoe UI", 10),
                Minimum = 0,
                Maximum = 10000000,
                DecimalPlaces = 2,
                Increment = 1000,
                Value = 0,
                ThousandsSeparator = true
            };
            Controls.Add(_numSalary);
            yPos += rowHeight;

            // Vacation Days Info (Read-only in edit mode)
            if (_isEditMode)
            {
                _lblVacationInfo = new Label
                {
                    Location = new Point(30, yPos),
                    Size = new Size(480, 25),
                    Font = new Font("Segoe UI", 9, FontStyle.Italic),
                    ForeColor = Color.FromArgb(127, 140, 141)
                };
                Controls.Add(_lblVacationInfo);
                yPos += 35;
            }

            // Buttons
            Button btnSave = new Button
            {
                Text = _isEditMode ? "Update" : "Save",
                Location = new Point(260, yPos + 20),
                Size = new Size(120, 45),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnSave.Click += BtnSave_Click;

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(390, yPos + 20),
                Size = new Size(120, 45),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.Add(lblTitle);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
        }

        private void CreateLabel(string text, int x, int y)
        {
            Label label = new Label
            {
                Text = text,
                Location = new Point(x, y + 3),
                Size = new Size(160, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            Controls.Add(label);
        }

        private TextBox CreateTextBox(int x, int y, int width)
        {
            TextBox textBox = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 25),
                Font = new Font("Segoe UI", 10)
            };
            Controls.Add(textBox);
            return textBox;
        }

        private void LoadEmployeeData()
        {
            if (_existingEmployee != null)
            {
                _txtRollNumber.Text = _existingEmployee.RollNumber.ToString();
                _txtName.Text = _existingEmployee.Name;
                _numAge.Value = _existingEmployee.Age;
                _txtDepartment.Text = _existingEmployee.Department ?? "";
                _txtPosition.Text = _existingEmployee.Position ?? "";
                if (_existingEmployee.HireDate.HasValue)
                    _dtpHireDate.Value = _existingEmployee.HireDate.Value;
                _txtEmail.Text = _existingEmployee.Email ?? "";
                _txtPhone.Text = _existingEmployee.Phone ?? "";
                _numSalary.Value = _existingEmployee.Salary;

                if (_lblVacationInfo != null)
                {
                    _lblVacationInfo.Text = $"Vacation Days: {_existingEmployee.VacationDaysAvailable} available, " +
                                           $"{_existingEmployee.VacationDaysUsed} used " +
                                           $"(Use 'Add Vacation Days' button to modify)";
                }
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                // Validate Roll Number
                if (!int.TryParse(_txtRollNumber.Text, out int rollNumber) || rollNumber <= 0)
                {
                    MessageBox.Show("Please enter a valid roll number (positive integer).", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _txtRollNumber.Focus();
                    return;
                }

                // Validate Name
                string name = _txtName.Text.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Please enter employee name.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _txtName.Focus();
                    return;
                }

                // Create employee object
                var employee = new Employee
                {
                    RollNumber = rollNumber,
                    Name = name,
                    Age = (int)_numAge.Value,
                    Department = _txtDepartment.Text.Trim(),
                    Position = _txtPosition.Text.Trim(),
                    HireDate = _dtpHireDate.Value,
                    Email = _txtEmail.Text.Trim(),
                    Phone = _txtPhone.Text.Trim(),
                    Salary = _numSalary.Value
                };

                // Preserve existing vacation days in edit mode
                if (_isEditMode && _existingEmployee != null)
                {
                    employee.Id = _existingEmployee.Id;
                    employee.VacationDaysAvailable = _existingEmployee.VacationDaysAvailable;
                    employee.VacationDaysUsed = _existingEmployee.VacationDaysUsed;
                    employee.CreatedDate = _existingEmployee.CreatedDate;
                }

                // Validate using FluentValidation
                var validator = new EmployeeValidator();
                var validationResult = validator.Validate(employee);

                if (!validationResult.IsValid)
                {
                    string errors = string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage));
                    MessageBox.Show(errors, "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if roll number exists (only for add mode)
                if (!_isEditMode && _repository.EmployeeExists(rollNumber))
                {
                    MessageBox.Show("An employee with this roll number already exists.", "Duplicate Entry",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _txtRollNumber.Focus();
                    return;
                }

                // Save or update
                if (_isEditMode)
                {
                    _repository.UpdateEmployee(employee);
                    MessageBox.Show("Employee updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _repository.AddEmployee(employee);
                    MessageBox.Show("Employee added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
