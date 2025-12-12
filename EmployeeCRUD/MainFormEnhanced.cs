using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class MainFormEnhanced : Form
    {
        private LocalStorageRepository _repository = null!;
        private DataGridView _employeeGrid = null!;
        private TextBox _txtSearch = null!;
        private Button _btnSearch = null!;
        private Button _btnClearSearch = null!;
        private Button _btnAdd = null!;
        private Button _btnEdit = null!;
        private Button _btnDelete = null!;
        private Button _btnRefresh = null!;
        private Button _btnStats = null!;
        private Button _btnPayroll = null!;
        private Button _btnVacation = null!;
        private Button _btnAddVacationDays = null!;
        private Label _lblConnectionStatus = null!;

        public MainFormEnhanced()
        {
            try
            {
                _repository = new LocalStorageRepository();
                InitializeComponents();
                UpdateConnectionStatus();
                LoadEmployees(); // Load after all components are initialized
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing application: {ex.Message}",
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _repository = new LocalStorageRepository();
                InitializeComponents();
                UpdateConnectionStatus();
                try
                {
                    LoadEmployees();
                }
                catch
                {
                    // Ignore errors loading empty employee list
                }
            }
        }

        private void InitializeComponents()
        {
            // Form settings
            Text = "Employee Management System - Enhanced";
            Size = new Size(1100, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(236, 240, 241);

            // Connection Status Label
            _lblConnectionStatus = new Label
            {
                Location = new Point(20, 10),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            // Search Box
            Label lblSearch = new Label
            {
                Text = "Search (ID/Name):",
                Location = new Point(20, 45),
                Size = new Size(130, 25),
                Font = new Font("Segoe UI", 10)
            };

            _txtSearch = new TextBox
            {
                Location = new Point(155, 45),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10)
            };
            _txtSearch.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    SearchEmployees();
                    e.Handled = true;
                }
            };

            _btnSearch = new Button
            {
                Text = "Search",
                Location = new Point(465, 43),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            _btnSearch.Click += (s, e) => SearchEmployees();

            _btnClearSearch = new Button
            {
                Text = "Clear",
                Location = new Point(575, 43),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            _btnClearSearch.Click += (s, e) =>
            {
                _txtSearch.Clear();
                LoadEmployees();
            };

            // DataGridView
            _employeeGrid = new DataGridView
            {
                Location = new Point(20, 85),
                Size = new Size(1050, 480),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            _employeeGrid.DoubleClick += (s, e) => EditEmployee();

            // Buttons Row 1
            int buttonY = 580;
            int buttonWidth = 120;
            int buttonHeight = 40;
            int spacing = 135;

            _btnAdd = CreateButton("Add Employee", 20, buttonY, buttonWidth, buttonHeight, Color.FromArgb(46, 204, 113));
            _btnAdd.Click += (s, e) => AddEmployee();

            _btnEdit = CreateButton("Edit Employee", 20 + spacing, buttonY, buttonWidth, buttonHeight, Color.FromArgb(52, 152, 219));
            _btnEdit.Click += (s, e) => EditEmployee();

            _btnDelete = CreateButton("Delete Employee", 20 + spacing * 2, buttonY, buttonWidth, buttonHeight, Color.FromArgb(231, 76, 60));
            _btnDelete.Click += (s, e) => DeleteEmployee();

            _btnRefresh = CreateButton("Refresh", 20 + spacing * 3, buttonY, buttonWidth, buttonHeight, Color.FromArgb(155, 89, 182));
            _btnRefresh.Click += (s, e) => LoadEmployees();

            _btnStats = CreateButton("Statistics", 20 + spacing * 4, buttonY, buttonWidth, buttonHeight, Color.FromArgb(241, 196, 15));
            _btnStats.Click += (s, e) => ShowStatistics();

            // Buttons Row 2
            buttonY = 630;
            _btnPayroll = CreateButton("Payroll", 20, buttonY, buttonWidth, buttonHeight, Color.FromArgb(22, 160, 133));
            _btnPayroll.Click += (s, e) => ShowPayroll();

            _btnVacation = CreateButton("Vacations", 20 + spacing, buttonY, buttonWidth, buttonHeight, Color.FromArgb(230, 126, 34));
            _btnVacation.Click += (s, e) => ShowVacation();

            _btnAddVacationDays = CreateButton("Add Vac. Days", 20 + spacing * 2, buttonY, buttonWidth, buttonHeight, Color.FromArgb(41, 128, 185));
            _btnAddVacationDays.Click += (s, e) => AddVacationDays();

            // Add controls to form
            Controls.Add(_lblConnectionStatus);
            Controls.Add(lblSearch);
            Controls.Add(_txtSearch);
            Controls.Add(_btnSearch);
            Controls.Add(_btnClearSearch);
            Controls.Add(_employeeGrid);
            Controls.Add(_btnAdd);
            Controls.Add(_btnEdit);
            Controls.Add(_btnDelete);
            Controls.Add(_btnRefresh);
            Controls.Add(_btnStats);
            Controls.Add(_btnPayroll);
            Controls.Add(_btnVacation);
            Controls.Add(_btnAddVacationDays);
        }

        private Button CreateButton(string text, int x, int y, int width, int height, Color bgColor)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = bgColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
        }

        private void UpdateConnectionStatus()
        {
            string dataPath = _repository.GetDataDirectory();
            _lblConnectionStatus.Text = $"💾 Data stored in: {dataPath}";
            _lblConnectionStatus.ForeColor = Color.FromArgb(52, 152, 219);
        }

        private void LoadEmployees()
        {
            try
            {
                if (_repository == null)
                {
                    MessageBox.Show("Repository not initialized", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_employeeGrid == null)
                {
                    MessageBox.Show("Employee grid not initialized", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var employees = _repository.GetAllEmployees();
                if (employees == null)
                {
                    employees = new List<Employee>();
                }

                _employeeGrid.DataSource = null;
                _employeeGrid.DataSource = employees.ToList();
                ConfigureGridColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}\n\nStack: {ex.StackTrace}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                _employeeGrid.DataSource = employees.ToList();
                ConfigureGridColumns();

                if (employees.Count == 0)
                {
                    MessageBox.Show("No employees found matching your search.", "Search Results",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching employees: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureGridColumns()
        {
            try
            {
                if (_employeeGrid == null || _employeeGrid.Columns == null || _employeeGrid.Columns.Count == 0)
                    return;

                // Hide technical columns
                if (_employeeGrid.Columns.Contains("Id"))
                    _employeeGrid.Columns["Id"]!.Visible = false;
                if (_employeeGrid.Columns.Contains("IsActive"))
                    _employeeGrid.Columns["IsActive"]!.Visible = false;
                if (_employeeGrid.Columns.Contains("CreatedDate"))
                    _employeeGrid.Columns["CreatedDate"]!.Visible = false;
                if (_employeeGrid.Columns.Contains("LastModifiedDate"))
                    _employeeGrid.Columns["LastModifiedDate"]!.Visible = false;

                // Configure visible columns with null checks
                if (_employeeGrid.Columns.Contains("RollNumber"))
                {
                    _employeeGrid.Columns["RollNumber"]!.HeaderText = "ID";
                    _employeeGrid.Columns["RollNumber"]!.Width = 60;
                }
                if (_employeeGrid.Columns.Contains("Name"))
                    _employeeGrid.Columns["Name"]!.HeaderText = "Name";
                if (_employeeGrid.Columns.Contains("Age"))
                {
                    _employeeGrid.Columns["Age"]!.HeaderText = "Age";
                    _employeeGrid.Columns["Age"]!.Width = 50;
                }
                if (_employeeGrid.Columns.Contains("Department"))
                    _employeeGrid.Columns["Department"]!.HeaderText = "Department";
                if (_employeeGrid.Columns.Contains("Position"))
                    _employeeGrid.Columns["Position"]!.HeaderText = "Position";
                if (_employeeGrid.Columns.Contains("Email"))
                    _employeeGrid.Columns["Email"]!.HeaderText = "Email";
                if (_employeeGrid.Columns.Contains("Phone"))
                    _employeeGrid.Columns["Phone"]!.HeaderText = "Phone";
                if (_employeeGrid.Columns.Contains("Salary"))
                {
                    _employeeGrid.Columns["Salary"]!.HeaderText = "Salary";
                    _employeeGrid.Columns["Salary"]!.DefaultCellStyle.Format = "C2";
                }
                if (_employeeGrid.Columns.Contains("VacationDaysAvailable"))
                {
                    _employeeGrid.Columns["VacationDaysAvailable"]!.HeaderText = "Vac. Available";
                    _employeeGrid.Columns["VacationDaysAvailable"]!.Width = 80;
                }
                if (_employeeGrid.Columns.Contains("VacationDaysUsed"))
                {
                    _employeeGrid.Columns["VacationDaysUsed"]!.HeaderText = "Vac. Used";
                    _employeeGrid.Columns["VacationDaysUsed"]!.Width = 80;
                }
                if (_employeeGrid.Columns.Contains("HireDate"))
                    _employeeGrid.Columns["HireDate"]!.DefaultCellStyle.Format = "MM/dd/yyyy";
            }
            catch (Exception ex)
            {
                // Silently fail if column configuration fails
                System.Diagnostics.Debug.WriteLine($"Error configuring columns: {ex.Message}");
            }
        }

        private void AddEmployee()
        {
            var addForm = new EmployeeFormEnhanced(_repository);
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
            var editForm = new EmployeeFormEnhanced(_repository, selectedEmployee);
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
            var statsForm = new StatisticsFormEnhanced(_repository);
            statsForm.ShowDialog();
        }

        private void ShowPayroll()
        {
            var payrollForm = new PayrollForm(_repository);
            payrollForm.ShowDialog();
        }

        private void ShowVacation()
        {
            var vacationForm = new VacationForm(_repository);
            vacationForm.ShowDialog();
        }

        private void AddVacationDays()
        {
            var addVacDaysForm = new AddVacationDaysForm(_repository);
            addVacDaysForm.ShowDialog();
            LoadEmployees(); // Refresh to show updated vacation days
        }
    }
}
