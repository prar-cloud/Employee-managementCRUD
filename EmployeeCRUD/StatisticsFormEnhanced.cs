using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class StatisticsFormEnhanced : Form
    {
        private LocalStorageRepository _repository;
        private Panel _panelStats = null!;

        public StatisticsFormEnhanced(LocalStorageRepository repository)
        {
            _repository = repository;
            InitializeComponents();
            LoadStatistics();
        }

        private void InitializeComponents()
        {
            Text = "Employee Statistics Dashboard";
            Size = new Size(700, 650);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(236, 240, 241);

            Label lblTitle = new Label
            {
                Text = "Statistics Dashboard",
                Location = new Point(20, 20),
                Size = new Size(650, 40),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                TextAlign = ContentAlignment.MiddleCenter
            };

            _panelStats = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(650, 480),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            Button btnClose = new Button
            {
                Text = "Close",
                Location = new Point(290, 575),
                Size = new Size(120, 45),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnClose.Click += (s, e) => Close();

            Controls.Add(lblTitle);
            Controls.Add(_panelStats);
            Controls.Add(btnClose);
        }

        private void LoadStatistics()
        {
            _panelStats.Controls.Clear();

            if (_repository.GetTotalCount() == 0)
            {
                Label lblNoData = new Label
                {
                    Text = "No employee data available",
                    Location = new Point(200, 200),
                    Size = new Size(250, 30),
                    Font = new Font("Segoe UI", 12, FontStyle.Italic),
                    ForeColor = Color.Gray
                };
                _panelStats.Controls.Add(lblNoData);
                return;
            }

            int yPosition = 20;

            // Section: Employee Stats
            AddSectionHeader("Employee Overview", yPosition);
            yPosition += 40;

            AddStatLabel("Total Employees:", _repository.GetTotalCount().ToString(),
                yPosition, Color.FromArgb(52, 152, 219));
            yPosition += 45;

            var oldestEmployee = _repository.GetOldestEmployee();
            string oldestText = oldestEmployee != null
                ? $"{oldestEmployee.Name} ({oldestEmployee.Age} years)"
                : "N/A";
            AddStatLabel("Oldest Employee:", oldestText,
                yPosition, Color.FromArgb(230, 126, 34));
            yPosition += 45;

            var youngestEmployee = _repository.GetYoungestEmployee();
            string youngestText = youngestEmployee != null
                ? $"{youngestEmployee.Name} ({youngestEmployee.Age} years)"
                : "N/A";
            AddStatLabel("Youngest Employee:", youngestText,
                yPosition, Color.FromArgb(26, 188, 156));
            yPosition += 60;

            // Section: Salary Stats
            AddSectionHeader("Salary Overview", yPosition);
            yPosition += 40;

            AddStatLabel("Average Salary:", $"${_repository.GetAverageSalary():N2}",
                yPosition, Color.FromArgb(46, 204, 113));
            yPosition += 45;

            AddStatLabel("Total Salary Expense:", $"${_repository.GetTotalSalary():N2}",
                yPosition, Color.FromArgb(155, 89, 182));
            yPosition += 45;

            var topPerformer = _repository.GetTopPerformer();
            string topPerformerText = topPerformer != null
                ? $"{topPerformer.Name} (${topPerformer.Salary:N2})"
                : "N/A";
            AddStatLabel("Highest Paid:", topPerformerText,
                yPosition, Color.FromArgb(241, 196, 15));
            yPosition += 60;

            // Section: Vacation Stats
            AddSectionHeader("Vacation Overview", yPosition);
            yPosition += 40;

            var employees = _repository.GetAllEmployees();
            int totalVacAvailable = employees.Sum(e => e.VacationDaysAvailable);
            int totalVacUsed = employees.Sum(e => e.VacationDaysUsed);
            int totalVacRemaining = totalVacAvailable - totalVacUsed;

            AddStatLabel("Total Vacation Days (Available):", totalVacAvailable.ToString(),
                yPosition, Color.FromArgb(41, 128, 185));
            yPosition += 45;

            AddStatLabel("Total Vacation Days (Used):", totalVacUsed.ToString(),
                yPosition, Color.FromArgb(192, 57, 43));
            yPosition += 45;

            AddStatLabel("Total Vacation Days (Remaining):", totalVacRemaining.ToString(),
                yPosition, Color.FromArgb(39, 174, 96));
            yPosition += 60;

            // Section: Pending Requests
            var allVacations = _repository.GetAllVacationRecords();
            int pendingCount = allVacations.Count(v => v.Status == "Pending");

            AddSectionHeader("Pending Items", yPosition);
            yPosition += 40;

            AddStatLabel("Pending Vacation Requests:", pendingCount.ToString(),
                yPosition, Color.FromArgb(231, 76, 60));
        }

        private void AddSectionHeader(string title, int yPosition)
        {
            Label lblSection = new Label
            {
                Text = title,
                Location = new Point(20, yPosition),
                Size = new Size(600, 30),
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(236, 240, 241),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            _panelStats.Controls.Add(lblSection);
        }

        private void AddStatLabel(string title, string value, int yPosition, Color accentColor)
        {
            Label lblTitle = new Label
            {
                Text = title,
                Location = new Point(40, yPosition),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            Label lblValue = new Label
            {
                Text = value,
                Location = new Point(350, yPosition),
                Size = new Size(270, 30),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = accentColor,
                TextAlign = ContentAlignment.TopRight
            };

            _panelStats.Controls.Add(lblTitle);
            _panelStats.Controls.Add(lblValue);
        }
    }
}
