using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class MainForm : Form
    {
        private EmployeeRepository _repository;
        private DataGridView _employeeGrid;
        private Button _btnAdd;
        private Button _btnEdit;
        private Button _btnDelete;
        private Button _btnRefresh;
        private Button _btnStats;

        public MainForm()
        {
            _repository = new EmployeeRepository();
            InitializeComponents();
            LoadEmployees();
        }

        private void InitializeComponents()
        {
            // Form settings
            Text = "Employee Management System";
            Size = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;

            // DataGridView
            _employeeGrid = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(840, 450),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _employeeGrid.DoubleClick += (s, e) => EditEmployee();

            // Buttons
            int buttonY = 490;
            int buttonWidth = 120;
            int buttonHeight = 40;
            int spacing = 140;

            _btnAdd = new Button
            {
                Text = "Add Employee",
                Location = new Point(20, buttonY),
                Size = new Size(buttonWidth, buttonHeight),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _btnAdd.Click += (s, e) => AddEmployee();

            _btnEdit = new Button
            {
                Text = "Edit Employee",
                Location = new Point(20 + spacing, buttonY),
                Size = new Size(buttonWidth, buttonHeight),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _btnEdit.Click += (s, e) => EditEmployee();

            _btnDelete = new Button
            {
                Text = "Delete Employee",
                Location = new Point(20 + spacing * 2, buttonY),
                Size = new Size(buttonWidth, buttonHeight),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _btnDelete.Click += (s, e) => DeleteEmployee();

            _btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(20 + spacing * 3, buttonY),
                Size = new Size(buttonWidth, buttonHeight),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _btnRefresh.Click += (s, e) => LoadEmployees();

            _btnStats = new Button
            {
                Text = "Statistics",
                Location = new Point(20 + spacing * 4, buttonY),
                Size = new Size(buttonWidth, buttonHeight),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _btnStats.Click += (s, e) => ShowStatistics();

            // Add controls to form
            Controls.Add(_employeeGrid);
            Controls.Add(_btnAdd);
            Controls.Add(_btnEdit);
            Controls.Add(_btnDelete);
            Controls.Add(_btnRefresh);
            Controls.Add(_btnStats);
        }

        private void LoadEmployees()
        {
            var employees = _repository.GetAllEmployees();
            _employeeGrid.DataSource = null;
            _employeeGrid.DataSource = employees.ToList();

            // Customize column headers
            if (_employeeGrid.Columns.Count > 0)
            {
                _employeeGrid.Columns["RollNumber"].HeaderText = "Roll Number";
                _employeeGrid.Columns["Name"].HeaderText = "Name";
                _employeeGrid.Columns["Age"].HeaderText = "Age";
                _employeeGrid.Columns["Salary"].HeaderText = "Salary";
                _employeeGrid.Columns["Salary"].DefaultCellStyle.Format = "C2";
            }
        }

        private void AddEmployee()
        {
            var addForm = new EmployeeForm(_repository);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadEmployees();
            }
        }

        private void EditEmployee()
        {
            if (_employeeGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an employee to edit.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedEmployee = (Employee)_employeeGrid.SelectedRows[0].DataBoundItem;
            var editForm = new EmployeeForm(_repository, selectedEmployee);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadEmployees();
            }
        }

        private void DeleteEmployee()
        {
            if (_employeeGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an employee to delete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedEmployee = (Employee)_employeeGrid.SelectedRows[0].DataBoundItem;
            var result = MessageBox.Show(
                $"Are you sure you want to delete employee '{selectedEmployee.Name}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _repository.DeleteEmployee(selectedEmployee.RollNumber);
                    MessageBox.Show("Employee deleted successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadEmployees();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting employee: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowStatistics()
        {
            var statsForm = new StatisticsForm(_repository);
            statsForm.ShowDialog();
        }
    }
}
