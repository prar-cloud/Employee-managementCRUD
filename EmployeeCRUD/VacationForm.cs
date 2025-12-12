using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class VacationForm : Form
    {
        private LocalStorageRepository _repository;
        private DataGridView _vacationGrid = null!;
        private Button _btnAdd = null!;
        private Button _btnApprove = null!;
        private Button _btnReject = null!;
        private Button _btnClose = null!;
        private ComboBox _cmbFilterEmployee = null!;

        public VacationForm(LocalStorageRepository repository)
        {
            _repository = repository;
            InitializeComponents();
            // LoadVacationRecords() is called by SelectedIndexChanged event
        }

        private void InitializeComponents()
        {
            Text = "Vacation Management";
            Size = new Size(1100, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(236, 240, 241);

            Label lblTitle = new Label
            {
                Text = "Vacation Records",
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

            _vacationGrid = new DataGridView
            {
                Location = new Point(20, 105),
                Size = new Size(1050, 390),
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
                Text = "Add Vacation",
                Location = new Point(20, 510),
                Size = new Size(140, 45),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _btnAdd.Click += (s, e) => AddVacationRequest();

            _btnApprove = new Button
            {
                Text = "Approve",
                Location = new Point(170, 510),
                Size = new Size(140, 45),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _btnApprove.Click += (s, e) => ApproveVacation();

            _btnReject = new Button
            {
                Text = "Reject",
                Location = new Point(320, 510),
                Size = new Size(140, 45),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _btnReject.Click += (s, e) => RejectVacation();

            _btnClose = new Button
            {
                Text = "Close",
                Location = new Point(920, 510),
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
            Controls.Add(_vacationGrid);
            Controls.Add(_btnAdd);
            Controls.Add(_btnApprove);
            Controls.Add(_btnReject);
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
            _cmbFilterEmployee.SelectedIndexChanged += (s, e) => LoadVacationRecords();
            _cmbFilterEmployee.SelectedIndex = 0;
        }

        private void LoadVacationRecords()
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
                    ? _repository.GetAllVacationRecords()
                    : _repository.GetVacationRecordsByEmployee(rollNumber);

                _vacationGrid.DataSource = null;

                if (records == null || records.Count == 0)
                {
                    // Show empty grid
                    _vacationGrid.DataSource = new System.Collections.Generic.List<VacationRecord>();
                    return;
                }

                _vacationGrid.DataSource = records.ToList();
                _vacationGrid.Refresh();

                if (_vacationGrid.Columns.Count > 0 && _vacationGrid.ColumnCount > 0)
                {
                    try
                    {
                        if (_vacationGrid.Columns.Contains("Id") && _vacationGrid.Columns["Id"] != null)
                            _vacationGrid.Columns["Id"]!.Visible = false;

                        if (_vacationGrid.Columns.Contains("VacationID") && _vacationGrid.Columns["VacationID"] != null)
                        {
                            _vacationGrid.Columns["VacationID"]!.HeaderText = "ID";
                            _vacationGrid.Columns["VacationID"]!.Width = 50;
                        }

                        if (_vacationGrid.Columns.Contains("RollNumber") && _vacationGrid.Columns["RollNumber"] != null)
                        {
                            _vacationGrid.Columns["RollNumber"]!.HeaderText = "Emp ID";
                            _vacationGrid.Columns["RollNumber"]!.Width = 60;
                        }

                        if (_vacationGrid.Columns.Contains("EmployeeName") && _vacationGrid.Columns["EmployeeName"] != null)
                            _vacationGrid.Columns["EmployeeName"]!.HeaderText = "Employee";

                        if (_vacationGrid.Columns.Contains("VacationType") && _vacationGrid.Columns["VacationType"] != null)
                            _vacationGrid.Columns["VacationType"]!.HeaderText = "Type";

                        if (_vacationGrid.Columns.Contains("StartDate") && _vacationGrid.Columns["StartDate"] != null)
                            _vacationGrid.Columns["StartDate"]!.DefaultCellStyle.Format = "MM/dd/yyyy";

                        if (_vacationGrid.Columns.Contains("EndDate") && _vacationGrid.Columns["EndDate"] != null)
                            _vacationGrid.Columns["EndDate"]!.DefaultCellStyle.Format = "MM/dd/yyyy";

                        if (_vacationGrid.Columns.Contains("DaysCount") && _vacationGrid.Columns["DaysCount"] != null)
                        {
                            _vacationGrid.Columns["DaysCount"]!.HeaderText = "Days";
                            _vacationGrid.Columns["DaysCount"]!.Width = 50;
                        }

                        if (_vacationGrid.Columns.Contains("Status") && _vacationGrid.Columns["Status"] != null)
                            _vacationGrid.Columns["Status"]!.Width = 90;

                        if (_vacationGrid.Columns.Contains("RequestDate") && _vacationGrid.Columns["RequestDate"] != null)
                            _vacationGrid.Columns["RequestDate"]!.DefaultCellStyle.Format = "MM/dd/yyyy";

                        if (_vacationGrid.Columns.Contains("ApprovalDate") && _vacationGrid.Columns["ApprovalDate"] != null)
                            _vacationGrid.Columns["ApprovalDate"]!.DefaultCellStyle.Format = "MM/dd/yyyy";

                        if (_vacationGrid.Columns.Contains("ApprovedBy") && _vacationGrid.Columns["ApprovedBy"] != null)
                            _vacationGrid.Columns["ApprovedBy"]!.Width = 100;
                    }
                    catch
                    {
                        // Ignore column configuration errors
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vacation records: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddVacationRequest()
        {
            var form = new AddVacationRequestForm(_repository);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadVacationRecords();
            }
        }

        private void ApproveVacation()
        {
            if (_vacationGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a vacation request to approve.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var record = (VacationRecord)_vacationGrid.SelectedRows[0].DataBoundItem;
            if (record.Status == "Approved")
            {
                MessageBox.Show("This vacation request is already approved.", "Already Approved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            record.Status = "Approved";
            record.ApprovedBy = "Manager"; // TODO: Get from logged-in user
            record.ApprovalDate = DateTime.Now;

            try
            {
                _repository.UpdateVacationRecord(record);
                MessageBox.Show("Vacation request approved successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadVacationRecords();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error approving vacation: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RejectVacation()
        {
            if (_vacationGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a vacation request to reject.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var record = (VacationRecord)_vacationGrid.SelectedRows[0].DataBoundItem;
            if (record.Status == "Rejected")
            {
                MessageBox.Show("This vacation request is already rejected.", "Already Rejected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            record.Status = "Rejected";
            record.ApprovedBy = "Manager"; // TODO: Get from logged-in user
            record.ApprovalDate = DateTime.Now;

            try
            {
                _repository.UpdateVacationRecord(record);
                MessageBox.Show("Vacation request rejected.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadVacationRecords();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error rejecting vacation: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
