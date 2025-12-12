using System;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class EmployeeForm : Form
    {
        private EmployeeRepository _repository;
        private Employee? _existingEmployee;
        private bool _isEditMode;

        private Label _lblTitle;
        private Label _lblRollNumber;
        private Label _lblName;
        private Label _lblAge;
        private Label _lblSalary;

        private TextBox _txtRollNumber;
        private TextBox _txtName;
        private NumericUpDown _numAge;
        private NumericUpDown _numSalary;

        private Button _btnSave;
        private Button _btnCancel;

        public EmployeeForm(EmployeeRepository repository, Employee? existingEmployee = null)
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
            Text = _isEditMode ? "Edit Employee" : "Add Employee";
            Size = new Size(450, 400);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Title
            _lblTitle = new Label
            {
                Text = _isEditMode ? "Edit Employee Information" : "Add New Employee",
                Location = new Point(20, 20),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            // Roll Number
            _lblRollNumber = new Label
            {
                Text = "Roll Number:",
                Location = new Point(30, 70),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _txtRollNumber = new TextBox
            {
                Location = new Point(160, 70),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 10),
                Enabled = !_isEditMode // Disable in edit mode
            };

            // Name
            _lblName = new Label
            {
                Text = "Name:",
                Location = new Point(30, 115),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _txtName = new TextBox
            {
                Location = new Point(160, 115),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 10)
            };

            // Age
            _lblAge = new Label
            {
                Text = "Age:",
                Location = new Point(30, 160),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _numAge = new NumericUpDown
            {
                Location = new Point(160, 160),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 10),
                Minimum = 18,
                Maximum = 100,
                Value = 18
            };

            // Salary
            _lblSalary = new Label
            {
                Text = "Salary:",
                Location = new Point(30, 205),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _numSalary = new NumericUpDown
            {
                Location = new Point(160, 205),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 10),
                Minimum = 0,
                Maximum = 10000000,
                DecimalPlaces = 2,
                Increment = 1000,
                Value = 0
            };

            // Save Button
            _btnSave = new Button
            {
                Text = _isEditMode ? "Update" : "Save",
                Location = new Point(160, 280),
                Size = new Size(110, 40),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _btnSave.Click += BtnSave_Click;

            // Cancel Button
            _btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(290, 280),
                Size = new Size(110, 40),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            // Add controls to form
            Controls.Add(_lblTitle);
            Controls.Add(_lblRollNumber);
            Controls.Add(_txtRollNumber);
            Controls.Add(_lblName);
            Controls.Add(_txtName);
            Controls.Add(_lblAge);
            Controls.Add(_numAge);
            Controls.Add(_lblSalary);
            Controls.Add(_numSalary);
            Controls.Add(_btnSave);
            Controls.Add(_btnCancel);
        }

        private void LoadEmployeeData()
        {
            if (_existingEmployee != null)
            {
                _txtRollNumber.Text = _existingEmployee.RollNumber.ToString();
                _txtName.Text = _existingEmployee.Name;
                _numAge.Value = _existingEmployee.Age;
                _numSalary.Value = (decimal)_existingEmployee.Salary;
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                // Get values
                if (!int.TryParse(_txtRollNumber.Text, out int rollNumber))
                {
                    MessageBox.Show("Please enter a valid roll number.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _txtRollNumber.Focus();
                    return;
                }

                string name = _txtName.Text.Trim();
                int age = (int)_numAge.Value;
                decimal salary = _numSalary.Value;

                // Create employee object
                var employee = new Employee
                {
                    RollNumber = rollNumber,
                    Name = name,
                    Age = age,
                    Salary = salary
                };

                // Validate
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
