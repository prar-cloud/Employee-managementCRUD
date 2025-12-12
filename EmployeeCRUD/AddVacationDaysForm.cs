using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class AddVacationDaysForm : Form
    {
        private LocalStorageRepository _repository;
        private ComboBox _cmbEmployee = null!;
        private Label _lblCurrentDays = null!;
        private NumericUpDown _numDaysToAdd = null!;
        private Button _btnSave = null!;
        private Button _btnCancel = null!;

        public AddVacationDaysForm(LocalStorageRepository repository)
        {
            _repository = repository;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Text = "Add Vacation Days";
            Size = new Size(500, 350);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(236, 240, 241);

            Label lblTitle = new Label
            {
                Text = "Add Vacation Days to Employee",
                Location = new Point(20, 20),
                Size = new Size(450, 35),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            Label lblEmployee = new Label
            {
                Text = "Select Employee:",
                Location = new Point(30, 75),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            _cmbEmployee = new ComboBox
            {
                Location = new Point(190, 75),
                Size = new Size(270, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            _cmbEmployee.SelectedIndexChanged += CmbEmployee_SelectedIndexChanged;

            // Load employees
            var employees = _repository.GetAllEmployees();
            foreach (var emp in employees)
            {
                _cmbEmployee.Items.Add(emp);
            }
            _cmbEmployee.DisplayMember = "Name";
            _cmbEmployee.ValueMember = "RollNumber";

            _lblCurrentDays = new Label
            {
                Text = "Current vacation days: -",
                Location = new Point(30, 120),
                Size = new Size(430, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(127, 140, 141)
            };

            Label lblDaysToAdd = new Label
            {
                Text = "Days to Add:",
                Location = new Point(30, 165),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            _numDaysToAdd = new NumericUpDown
            {
                Location = new Point(190, 165),
                Size = new Size(270, 25),
                Font = new Font("Segoe UI", 10),
                Minimum = 1,
                Maximum = 365,
                Value = 5
            };

            Label lblNote = new Label
            {
                Text = "Note: This will add vacation days to the employee's\navailable balance. Typically used for annual allocations.",
                Location = new Point(30, 210),
                Size = new Size(430, 40),
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(127, 140, 141)
            };

            _btnSave = new Button
            {
                Text = "Add Days",
                Location = new Point(210, 270),
                Size = new Size(120, 45),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _btnSave.Click += BtnSave_Click;

            _btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(340, 270),
                Size = new Size(120, 45),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _btnCancel.Click += (s, e) => Close();

            Controls.Add(lblTitle);
            Controls.Add(lblEmployee);
            Controls.Add(_cmbEmployee);
            Controls.Add(_lblCurrentDays);
            Controls.Add(lblDaysToAdd);
            Controls.Add(_numDaysToAdd);
            Controls.Add(lblNote);
            Controls.Add(_btnSave);
            Controls.Add(_btnCancel);
        }

        private void CmbEmployee_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_cmbEmployee.SelectedItem != null)
            {
                var employee = (Employee)_cmbEmployee.SelectedItem;
                _lblCurrentDays.Text = $"Current vacation days: {employee.VacationDaysAvailable} available, " +
                                      $"{employee.VacationDaysUsed} used";
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (_cmbEmployee.SelectedItem == null)
            {
                MessageBox.Show("Please select an employee.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var employee = (Employee)_cmbEmployee.SelectedItem;
            int daysToAdd = (int)_numDaysToAdd.Value;

            var result = MessageBox.Show(
                $"Are you sure you want to add {daysToAdd} vacation days to {employee.Name}?\n\n" +
                $"Current: {employee.VacationDaysAvailable} days\n" +
                $"New Total: {employee.VacationDaysAvailable + daysToAdd} days",
                "Confirm Addition",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _repository.AddVacationDays(employee.RollNumber, daysToAdd);
                    MessageBox.Show($"Successfully added {daysToAdd} vacation days to {employee.Name}!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding vacation days: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
