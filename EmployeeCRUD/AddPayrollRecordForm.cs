using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class AddPayrollRecordForm : Form
    {
        private LocalStorageRepository _repository;

        private ComboBox _cmbEmployee = null!;
        private DateTimePicker _dtpPayPeriodStart = null!;
        private DateTimePicker _dtpPayPeriodEnd = null!;
        private NumericUpDown _numBaseSalary = null!;
        private NumericUpDown _numBonus = null!;
        private NumericUpDown _numDeductions = null!;
        private Label _lblNetPay = null!;
        private DateTimePicker _dtpPaymentDate = null!;
        private ComboBox _cmbPaymentStatus = null!;
        private TextBox _txtNotes = null!;
        private Button _btnSave = null!;
        private Button _btnCancel = null!;

        public AddPayrollRecordForm(LocalStorageRepository repository)
        {
            _repository = repository;
            InitializeComponents();
            LoadEmployees();
        }

        private void InitializeComponents()
        {
            // Form settings
            Text = "Add Payroll Record";
            Size = new Size(600, 650);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(236, 240, 241);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            Label lblTitle = new Label
            {
                Text = "Add Payroll Record",
                Location = new Point(20, 20),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            // Employee
            Label lblEmployee = new Label
            {
                Text = "Employee:",
                Location = new Point(20, 70),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _cmbEmployee = new ComboBox
            {
                Location = new Point(150, 70),
                Size = new Size(410, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            _cmbEmployee.SelectedIndexChanged += Employee_SelectedIndexChanged;

            // Pay Period Start
            Label lblPayPeriodStart = new Label
            {
                Text = "Pay Period Start:",
                Location = new Point(20, 110),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _dtpPayPeriodStart = new DateTimePicker
            {
                Location = new Point(150, 110),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10)
            };
            _dtpPayPeriodStart.ValueChanged += CalculateNetPay;

            // Pay Period End
            Label lblPayPeriodEnd = new Label
            {
                Text = "Pay Period End:",
                Location = new Point(20, 150),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _dtpPayPeriodEnd = new DateTimePicker
            {
                Location = new Point(150, 150),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10),
                Value = DateTime.Now.AddDays(14)
            };
            _dtpPayPeriodEnd.ValueChanged += CalculateNetPay;

            // Base Salary
            Label lblBaseSalary = new Label
            {
                Text = "Base Salary:",
                Location = new Point(20, 190),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _numBaseSalary = new NumericUpDown
            {
                Location = new Point(150, 190),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10),
                Minimum = 0,
                Maximum = 1000000,
                DecimalPlaces = 2,
                ThousandsSeparator = true,
                Value = 0
            };
            _numBaseSalary.ValueChanged += CalculateNetPay;

            // Bonus
            Label lblBonus = new Label
            {
                Text = "Bonus:",
                Location = new Point(20, 230),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _numBonus = new NumericUpDown
            {
                Location = new Point(150, 230),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10),
                Minimum = 0,
                Maximum = 1000000,
                DecimalPlaces = 2,
                ThousandsSeparator = true,
                Value = 0
            };
            _numBonus.ValueChanged += CalculateNetPay;

            // Deductions
            Label lblDeductions = new Label
            {
                Text = "Deductions:",
                Location = new Point(20, 270),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _numDeductions = new NumericUpDown
            {
                Location = new Point(150, 270),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10),
                Minimum = 0,
                Maximum = 1000000,
                DecimalPlaces = 2,
                ThousandsSeparator = true,
                Value = 0
            };
            _numDeductions.ValueChanged += CalculateNetPay;

            // Net Pay (calculated)
            Label lblNetPayLabel = new Label
            {
                Text = "Net Pay:",
                Location = new Point(20, 310),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            _lblNetPay = new Label
            {
                Text = "$0.00",
                Location = new Point(150, 310),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 160, 133)
            };

            // Payment Date
            Label lblPaymentDate = new Label
            {
                Text = "Payment Date:",
                Location = new Point(20, 355),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _dtpPaymentDate = new DateTimePicker
            {
                Location = new Point(150, 355),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10)
            };

            // Payment Status
            Label lblPaymentStatus = new Label
            {
                Text = "Payment Status:",
                Location = new Point(20, 395),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _cmbPaymentStatus = new ComboBox
            {
                Location = new Point(150, 395),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            _cmbPaymentStatus.Items.AddRange(new string[] { "Pending", "Processed", "Paid", "Cancelled" });
            _cmbPaymentStatus.SelectedIndex = 0;

            // Notes
            Label lblNotes = new Label
            {
                Text = "Notes:",
                Location = new Point(20, 435),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _txtNotes = new TextBox
            {
                Location = new Point(150, 435),
                Size = new Size(410, 80),
                Font = new Font("Segoe UI", 10),
                Multiline = true
            };

            // Buttons
            _btnSave = new Button
            {
                Text = "Save Record",
                Location = new Point(150, 540),
                Size = new Size(150, 45),
                BackColor = Color.FromArgb(22, 160, 133),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnSave.Click += Save_Click;

            _btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(320, 540),
                Size = new Size(120, 45),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            // Add all controls
            Controls.Add(lblTitle);
            Controls.Add(lblEmployee);
            Controls.Add(_cmbEmployee);
            Controls.Add(lblPayPeriodStart);
            Controls.Add(_dtpPayPeriodStart);
            Controls.Add(lblPayPeriodEnd);
            Controls.Add(_dtpPayPeriodEnd);
            Controls.Add(lblBaseSalary);
            Controls.Add(_numBaseSalary);
            Controls.Add(lblBonus);
            Controls.Add(_numBonus);
            Controls.Add(lblDeductions);
            Controls.Add(_numDeductions);
            Controls.Add(lblNetPayLabel);
            Controls.Add(_lblNetPay);
            Controls.Add(lblPaymentDate);
            Controls.Add(_dtpPaymentDate);
            Controls.Add(lblPaymentStatus);
            Controls.Add(_cmbPaymentStatus);
            Controls.Add(lblNotes);
            Controls.Add(_txtNotes);
            Controls.Add(_btnSave);
            Controls.Add(_btnCancel);
        }

        private void LoadEmployees()
        {
            var employees = _repository.GetAllEmployees();
            _cmbEmployee.Items.Clear();

            foreach (var emp in employees)
            {
                _cmbEmployee.Items.Add(new { Employee = emp, Display = $"{emp.RollNumber} - {emp.Name}" });
            }

            _cmbEmployee.DisplayMember = "Display";

            if (_cmbEmployee.Items.Count > 0)
                _cmbEmployee.SelectedIndex = 0;
        }

        private void Employee_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_cmbEmployee.SelectedItem != null)
            {
                dynamic selectedItem = _cmbEmployee.SelectedItem;
                Employee emp = selectedItem.Employee;

                // Auto-fill base salary from employee's salary
                _numBaseSalary.Value = emp.Salary;
                CalculateNetPay(null, EventArgs.Empty);
            }
        }

        private void CalculateNetPay(object? sender, EventArgs e)
        {
            decimal baseSalary = _numBaseSalary.Value;
            decimal bonus = _numBonus.Value;
            decimal deductions = _numDeductions.Value;

            decimal netPay = baseSalary + bonus - deductions;

            _lblNetPay.Text = netPay.ToString("C2");
        }

        private void Save_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_cmbEmployee.SelectedItem == null)
                {
                    MessageBox.Show("Please select an employee.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_dtpPayPeriodEnd.Value < _dtpPayPeriodStart.Value)
                {
                    MessageBox.Show("Pay period end date must be after start date.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                dynamic selectedItem = _cmbEmployee.SelectedItem;
                Employee emp = selectedItem.Employee;

                decimal baseSalary = _numBaseSalary.Value;
                decimal bonus = _numBonus.Value;
                decimal deductions = _numDeductions.Value;
                decimal netPay = baseSalary + bonus - deductions;

                var payrollRecord = new PayrollRecord
                {
                    RollNumber = emp.RollNumber,
                    PayPeriodStart = _dtpPayPeriodStart.Value,
                    PayPeriodEnd = _dtpPayPeriodEnd.Value,
                    BaseSalary = baseSalary,
                    Bonus = bonus,
                    Deductions = deductions,
                    NetPay = netPay,
                    PaymentDate = _dtpPaymentDate.Value,
                    PaymentStatus = _cmbPaymentStatus.SelectedItem?.ToString() ?? "Pending",
                    Notes = _txtNotes.Text.Trim()
                };

                _repository.AddPayrollRecord(payrollRecord);

                MessageBox.Show("Payroll record added successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving payroll record: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
