using System;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class StatisticsForm : Form
    {
        private EmployeeRepository _repository;

        private Label _lblTitle;
        private Panel _panelStats;
        private Button _btnClose;

        public StatisticsForm(EmployeeRepository repository)
        {
            _repository = repository;
            InitializeComponents();
            LoadStatistics();
        }

        private void InitializeComponents()
        {
            // Form settings
            Text = "Employee Statistics";
            Size = new Size(600, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(236, 240, 241);

            // Title
            _lblTitle = new Label
            {
                Text = "Employee Statistics Dashboard",
                Location = new Point(20, 20),
                Size = new Size(550, 40),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Statistics Panel
            _panelStats = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(550, 330),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Close Button
            _btnClose = new Button
            {
                Text = "Close",
                Location = new Point(240, 420),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _btnClose.Click += (s, e) => Close();

            // Add controls to form
            Controls.Add(_lblTitle);
            Controls.Add(_panelStats);
            Controls.Add(_btnClose);
        }

        private void LoadStatistics()
        {
            _panelStats.Controls.Clear();

            if (_repository.GetTotalCount() == 0)
            {
                Label lblNoData = new Label
                {
                    Text = "No employee data available",
                    Location = new Point(150, 150),
                    Size = new Size(250, 30),
                    Font = new Font("Segoe UI", 12, FontStyle.Italic),
                    ForeColor = Color.Gray
                };
                _panelStats.Controls.Add(lblNoData);
                return;
            }

            int yPosition = 20;
            int labelHeight = 35;

            // Total Employees
            AddStatLabel("Total Employees:", _repository.GetTotalCount().ToString(),
                yPosition, Color.FromArgb(52, 152, 219));
            yPosition += labelHeight + 15;

            // Average Salary
            AddStatLabel("Average Salary:", $"${_repository.GetAverageSalary():N2}",
                yPosition, Color.FromArgb(46, 204, 113));
            yPosition += labelHeight + 15;

            // Total Salary
            AddStatLabel("Total Salary Expense:", $"${_repository.GetTotalSalary():N2}",
                yPosition, Color.FromArgb(155, 89, 182));
            yPosition += labelHeight + 15;

            // Top Performer
            var topPerformer = _repository.GetTopPerformer();
            string topPerformerText = topPerformer != null
                ? $"{topPerformer.Name} (${topPerformer.Salary:N2})"
                : "N/A";
            AddStatLabel("Highest Paid Employee:", topPerformerText,
                yPosition, Color.FromArgb(241, 196, 15));
            yPosition += labelHeight + 15;

            // Oldest Employee
            var oldestEmployee = _repository.GetOldestEmployee();
            string oldestText = oldestEmployee != null
                ? $"{oldestEmployee.Name} ({oldestEmployee.Age} years)"
                : "N/A";
            AddStatLabel("Oldest Employee:", oldestText,
                yPosition, Color.FromArgb(230, 126, 34));
            yPosition += labelHeight + 15;

            // Youngest Employee
            var youngestEmployee = _repository.GetYoungestEmployee();
            string youngestText = youngestEmployee != null
                ? $"{youngestEmployee.Name} ({youngestEmployee.Age} years)"
                : "N/A";
            AddStatLabel("Youngest Employee:", youngestText,
                yPosition, Color.FromArgb(26, 188, 156));
        }

        private void AddStatLabel(string title, string value, int yPosition, Color accentColor)
        {
            // Title Label
            Label lblTitle = new Label
            {
                Text = title,
                Location = new Point(30, yPosition),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            // Value Label
            Label lblValue = new Label
            {
                Text = value,
                Location = new Point(290, yPosition),
                Size = new Size(240, 30),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = accentColor,
                TextAlign = ContentAlignment.TopRight
            };

            _panelStats.Controls.Add(lblTitle);
            _panelStats.Controls.Add(lblValue);
        }
    }
}
