using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class PayrollForm : Form
    {
        private LocalStorageRepository _repository;
        private DataGridView _payrollGrid = null!;
        private Button _btnAdd = null!;
        private Button _btnClose = null!;
        private ComboBox _cmbFilterEmployee = null!;

        public PayrollForm(LocalStorageRepository repository)
        {
            _repository = repository;
            InitializeComponents();
            // LoadPayrollRecords() is already called by SelectedIndexChanged event when combo box is initialized
        }

        private void InitializeComponents()
        {
            Text = "Payroll Management";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(236, 240, 241);

            Label lblTitle = new Label
            {
                Text = "Payroll Records",
                Location = new Point(20, 20),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            Label lblFilter = new Label
            {
                Text = "Filter by Employee:",
                Location = new Point(20, 65),
                Size = new Size(130, 25),
                Font = new Font("Segoe UI", 10)
            };

            _cmbFilterEmployee = new ComboBox
            {
                Location = new Point(155, 65),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            _payrollGrid = new DataGridView
            {
                Location = new Point(20, 105),
                Size = new Size(950, 390),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };

            _btnAdd = new Button
            {
                Text = "Add Payroll Record",
                Location = new Point(20, 510),
                Size = new Size(160, 45),
                BackColor = Color.FromArgb(22, 160, 133),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _btnAdd.Click += (s, e) => AddPayrollRecord();

            _btnClose = new Button
            {
                Text = "Close",
                Location = new Point(820, 510),
                Size = new Size(150, 45),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _btnClose.Click += (s, e) => Close();

            Controls.Add(lblTitle);
            Controls.Add(lblFilter);
            Controls.Add(_cmbFilterEmployee);
            Controls.Add(_payrollGrid);
            Controls.Add(_btnAdd);
            Controls.Add(_btnClose);

            // Populate combo box and attach event handler AFTER all controls are added
            var employees = _repository.GetAllEmployees();
            _cmbFilterEmployee.Items.Add(new { RollNumber = 0, Display = "-- All Employees --" });
            foreach (var emp in employees)
            {
                _cmbFilterEmployee.Items.Add(new { RollNumber = emp.RollNumber, Display = $"{emp.RollNumber} - {emp.Name}" });
            }
            _cmbFilterEmployee.DisplayMember = "Display";
            _cmbFilterEmployee.ValueMember = "RollNumber";

            // Attach event handler before setting selected index
            _cmbFilterEmployee.SelectedIndexChanged += (s, e) => LoadPayrollRecords();
            _cmbFilterEmployee.SelectedIndex = 0;
        }

        private void LoadPayrollRecords()
        {
            try
            {
                if (_cmbFilterEmployee.SelectedItem == null)
                {
                    return;
                }

                var selectedItem = (dynamic)_cmbFilterEmployee.SelectedItem;
                if (selectedItem == null)
                {
                    return;
                }

                int rollNumber = selectedItem.RollNumber;

                var records = rollNumber == 0
                    ? _repository.GetAllPayrollRecords()
                    : _repository.GetPayrollRecordsByEmployee(rollNumber);

                _payrollGrid.DataSource = null;

                if (records == null || records.Count == 0)
                {
                    // Show empty grid with a message
                    _payrollGrid.DataSource = new List<PayrollRecord>();
                    return;
                }

                _payrollGrid.DataSource = records.ToList();
                _payrollGrid.Refresh();

                if (_payrollGrid.Columns.Count > 0 && _payrollGrid.ColumnCount > 0)
                {
                    try
                    {
                        if (_payrollGrid.Columns.Contains("Id") && _payrollGrid.Columns["Id"] != null)
                            _payrollGrid.Columns["Id"]!.Visible = false;

                        if (_payrollGrid.Columns.Contains("PayrollID") && _payrollGrid.Columns["PayrollID"] != null)
                        {
                            _payrollGrid.Columns["PayrollID"]!.HeaderText = "ID";
                            _payrollGrid.Columns["PayrollID"]!.Width = 50;
                        }

                        if (_payrollGrid.Columns.Contains("RollNumber") && _payrollGrid.Columns["RollNumber"] != null)
                        {
                            _payrollGrid.Columns["RollNumber"]!.HeaderText = "Emp ID";
                            _payrollGrid.Columns["RollNumber"]!.Width = 60;
                        }

                        if (_payrollGrid.Columns.Contains("EmployeeName") && _payrollGrid.Columns["EmployeeName"] != null)
                            _payrollGrid.Columns["EmployeeName"]!.HeaderText = "Employee";

                        if (_payrollGrid.Columns.Contains("BaseSalary") && _payrollGrid.Columns["BaseSalary"] != null)
                            _payrollGrid.Columns["BaseSalary"]!.DefaultCellStyle.Format = "C2";

                        if (_payrollGrid.Columns.Contains("Bonus") && _payrollGrid.Columns["Bonus"] != null)
                            _payrollGrid.Columns["Bonus"]!.DefaultCellStyle.Format = "C2";

                        if (_payrollGrid.Columns.Contains("Deductions") && _payrollGrid.Columns["Deductions"] != null)
                            _payrollGrid.Columns["Deductions"]!.DefaultCellStyle.Format = "C2";

                        if (_payrollGrid.Columns.Contains("NetPay") && _payrollGrid.Columns["NetPay"] != null)
                            _payrollGrid.Columns["NetPay"]!.DefaultCellStyle.Format = "C2";

                        if (_payrollGrid.Columns.Contains("PayPeriodStart") && _payrollGrid.Columns["PayPeriodStart"] != null)
                            _payrollGrid.Columns["PayPeriodStart"]!.DefaultCellStyle.Format = "MM/dd/yyyy";

                        if (_payrollGrid.Columns.Contains("PayPeriodEnd") && _payrollGrid.Columns["PayPeriodEnd"] != null)
                            _payrollGrid.Columns["PayPeriodEnd"]!.DefaultCellStyle.Format = "MM/dd/yyyy";

                        if (_payrollGrid.Columns.Contains("PaymentDate") && _payrollGrid.Columns["PaymentDate"] != null)
                            _payrollGrid.Columns["PaymentDate"]!.DefaultCellStyle.Format = "MM/dd/yyyy";

                        if (_payrollGrid.Columns.Contains("CreatedDate") && _payrollGrid.Columns["CreatedDate"] != null)
                            _payrollGrid.Columns["CreatedDate"]!.Visible = false;
                    }
                    catch
                    {
                        // Ignore column configuration errors
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading payroll records: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddPayrollRecord()
        {
            var form = new AddPayrollRecordForm(_repository);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadPayrollRecords();
            }
        }
    }
}
