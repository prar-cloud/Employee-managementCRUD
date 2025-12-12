using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class AddVacationRequestForm : Form
    {
        private LocalStorageRepository _repository;

        private ComboBox _cmbEmployee = null!;
        private ComboBox _cmbVacationType = null!;
        private DateTimePicker _dtpStartDate = null!;
        private DateTimePicker _dtpEndDate = null!;
        private Label _lblDaysCount = null!;
        private Label _lblAvailableDays = null!;
        private TextBox _txtReason = null!;
        private TextBox _txtNotes = null!;
        private Button _btnSubmit = null!;
        private Button _btnCancel = null!;

        public AddVacationRequestForm(LocalStorageRepository repository)
        {
            _repository = repository;
            InitializeComponents();
            LoadEmployees();
        }

        private void InitializeComponents()
        {
            // Form settings
            Text = "Add Vacation Request";
            Size = new Size(600, 650);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(236, 240, 241);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            Label lblTitle = new Label
            {
                Text = "Add Vacation Request",
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

            // Available Days Label
            _lblAvailableDays = new Label
            {
                Text = "Available: 0 days",
                Location = new Point(150, 100),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(127, 140, 141)
            };

            // Vacation Type
            Label lblVacationType = new Label
            {
                Text = "Vacation Type:",
                Location = new Point(20, 130),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _cmbVacationType = new ComboBox
            {
                Location = new Point(150, 130),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            _cmbVacationType.Items.AddRange(new string[] { "Annual", "Sick", "Personal", "Maternity/Paternity", "Unpaid", "Other" });
            _cmbVacationType.SelectedIndex = 0;

            // Start Date
            Label lblStartDate = new Label
            {
                Text = "Start Date:",
                Location = new Point(20, 170),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _dtpStartDate = new DateTimePicker
            {
                Location = new Point(150, 170),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10),
                Value = DateTime.Today
            };
            _dtpStartDate.ValueChanged += CalculateDays;

            // End Date
            Label lblEndDate = new Label
            {
                Text = "End Date:",
                Location = new Point(20, 210),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _dtpEndDate = new DateTimePicker
            {
                Location = new Point(150, 210),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10),
                Value = DateTime.Today.AddDays(1)
            };
            _dtpEndDate.ValueChanged += CalculateDays;

            // Days Count
            Label lblDaysLabel = new Label
            {
                Text = "Days Count:",
                Location = new Point(20, 250),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            _lblDaysCount = new Label
            {
                Text = "1 day",
                Location = new Point(150, 250),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 126, 34)
            };

            // Reason
            Label lblReason = new Label
            {
                Text = "Reason:",
                Location = new Point(20, 295),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _txtReason = new TextBox
            {
                Location = new Point(150, 295),
                Size = new Size(410, 80),
                Font = new Font("Segoe UI", 10),
                Multiline = true
            };

            // Notes
            Label lblNotes = new Label
            {
                Text = "Notes:",
                Location = new Point(20, 390),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10)
            };

            _txtNotes = new TextBox
            {
                Location = new Point(150, 390),
                Size = new Size(410, 80),
                Font = new Font("Segoe UI", 10),
                Multiline = true
            };

            // Buttons
            _btnSubmit = new Button
            {
                Text = "Submit Request",
                Location = new Point(150, 500),
                Size = new Size(160, 45),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnSubmit.Click += Submit_Click;

            _btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(330, 500),
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
            Controls.Add(_lblAvailableDays);
            Controls.Add(lblVacationType);
            Controls.Add(_cmbVacationType);
            Controls.Add(lblStartDate);
            Controls.Add(_dtpStartDate);
            Controls.Add(lblEndDate);
            Controls.Add(_dtpEndDate);
            Controls.Add(lblDaysLabel);
            Controls.Add(_lblDaysCount);
            Controls.Add(lblReason);
            Controls.Add(_txtReason);
            Controls.Add(lblNotes);
            Controls.Add(_txtNotes);
            Controls.Add(_btnSubmit);
            Controls.Add(_btnCancel);

            // Initial calculation
            CalculateDays(null, EventArgs.Empty);
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

                int remaining = emp.VacationDaysAvailable - emp.VacationDaysUsed;
                _lblAvailableDays.Text = $"Available: {remaining} days (Total: {emp.VacationDaysAvailable}, Used: {emp.VacationDaysUsed})";

                if (remaining < 5)
                {
                    _lblAvailableDays.ForeColor = Color.FromArgb(231, 76, 60);
                }
                else if (remaining < 10)
                {
                    _lblAvailableDays.ForeColor = Color.FromArgb(230, 126, 34);
                }
                else
                {
                    _lblAvailableDays.ForeColor = Color.FromArgb(46, 204, 113);
                }
            }
        }

        private void CalculateDays(object? sender, EventArgs e)
        {
            TimeSpan duration = _dtpEndDate.Value.Date - _dtpStartDate.Value.Date;
            int days = (int)duration.TotalDays + 1; // Include both start and end dates

            if (days < 0)
            {
                days = 0;
            }

            _lblDaysCount.Text = days == 1 ? "1 day" : $"{days} days";

            // Check if employee has enough vacation days
            if (_cmbEmployee.SelectedItem != null)
            {
                dynamic selectedItem = _cmbEmployee.SelectedItem;
                Employee emp = selectedItem.Employee;
                int remaining = emp.VacationDaysAvailable - emp.VacationDaysUsed;

                if (days > remaining)
                {
                    _lblDaysCount.ForeColor = Color.FromArgb(231, 76, 60);
                    _lblDaysCount.Text += " (Exceeds available days!)";
                }
                else
                {
                    _lblDaysCount.ForeColor = Color.FromArgb(230, 126, 34);
                }
            }
        }

        private void Submit_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_cmbEmployee.SelectedItem == null)
                {
                    MessageBox.Show("Please select an employee.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_dtpEndDate.Value < _dtpStartDate.Value)
                {
                    MessageBox.Show("End date must be after start date.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                dynamic selectedItem = _cmbEmployee.SelectedItem;
                Employee emp = selectedItem.Employee;

                TimeSpan duration = _dtpEndDate.Value.Date - _dtpStartDate.Value.Date;
                int daysCount = (int)duration.TotalDays + 1;

                int remaining = emp.VacationDaysAvailable - emp.VacationDaysUsed;
                if (daysCount > remaining)
                {
                    var result = MessageBox.Show(
                        $"This request is for {daysCount} days, but the employee only has {remaining} days available.\n\nDo you want to submit anyway?",
                        "Insufficient Vacation Days",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                        return;
                }

                var vacationRecord = new VacationRecord
                {
                    RollNumber = emp.RollNumber,
                    VacationType = _cmbVacationType.SelectedItem?.ToString() ?? "Annual",
                    StartDate = _dtpStartDate.Value.Date,
                    EndDate = _dtpEndDate.Value.Date,
                    DaysCount = daysCount,
                    Status = "Pending",
                    RequestDate = DateTime.Now,
                    Reason = _txtReason.Text.Trim(),
                    Notes = _txtNotes.Text.Trim()
                };

                _repository.AddVacationRecord(vacationRecord);

                MessageBox.Show("Vacation request submitted successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error submitting vacation request: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
