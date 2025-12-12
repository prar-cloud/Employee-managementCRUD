using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeCRUD
{
    public class ModernStatisticsForm : Form
    {
        // Modern color palette
        private readonly Color PrimaryDark = Color.FromArgb(37, 42, 52);
        private readonly Color CardBackground = Color.FromArgb(255, 255, 255);
        private readonly Color PrimaryAccent = Color.FromArgb(99, 102, 241);    // Indigo
        private readonly Color SecondaryAccent = Color.FromArgb(139, 92, 246);  // Purple
        private readonly Color SuccessColor = Color.FromArgb(16, 185, 129);     // Emerald
        private readonly Color WarningColor = Color.FromArgb(245, 158, 11);     // Amber
        private readonly Color DangerColor = Color.FromArgb(239, 68, 68);       // Red
        private readonly Color InfoColor = Color.FromArgb(59, 130, 246);        // Blue
        private readonly Color TextPrimary = Color.FromArgb(31, 41, 55);
        private readonly Color TextSecondary = Color.FromArgb(107, 114, 128);
        private readonly Color BackgroundGray = Color.FromArgb(243, 244, 246);

        private LocalStorageRepository _repository = null!;

        public ModernStatisticsForm(LocalStorageRepository repository)
        {
            _repository = repository;
            InitializeComponents();
            LoadStatistics();
        }

        private void InitializeComponents()
        {
            // Form settings
            this.Text = "Employee Statistics Dashboard";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BackgroundGray;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(1000, 700);

            // Main container with padding
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BackgroundGray,
                Padding = new Padding(30),
                AutoScroll = true
            };

            // Header
            var headerPanel = CreateHeader();
            headerPanel.Location = new Point(30, 30);
            headerPanel.Width = 1120;

            // Stats cards container
            var statsContainer = new Panel
            {
                Location = new Point(30, 120),
                Size = new Size(1120, 600),
                BackColor = Color.Transparent,
                AutoScroll = true
            };

            // Create stat cards
            CreateStatCards(statsContainer);

            mainPanel.Controls.AddRange(new Control[] { headerPanel, statsContainer });
            this.Controls.Add(mainPanel);
        }

        private Panel CreateHeader()
        {
            var panel = new Panel
            {
                Height = 80,
                BackColor = Color.Transparent
            };

            var titleLabel = new Label
            {
                Text = "📊 Statistics Dashboard",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var subtitleLabel = new Label
            {
                Text = "Overview of employee metrics and insights",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(0, 45)
            };

            var closeButton = new Button
            {
                Text = "✕ Close",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = TextSecondary,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 45),
                Location = new Point(1000, 10),
                Cursor = Cursors.Hand
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();

            // Rounded corners for button
            closeButton.Paint += (s, e) =>
            {
                var btn = (Button)s;
                using (var path = GetRoundedRectPath(btn.ClientRectangle, 8))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    btn.Region = new Region(path);
                }
            };

            panel.Controls.AddRange(new Control[] { titleLabel, subtitleLabel, closeButton });
            return panel;
        }

        private void CreateStatCards(Panel container)
        {
            List<Employee> employees = _repository.GetAllEmployees();

            if (employees == null || employees.Count == 0)
            {
                var noDataLabel = new Label
                {
                    Text = "No employee data available",
                    Font = new Font("Segoe UI", 14),
                    ForeColor = TextSecondary,
                    AutoSize = true,
                    Location = new Point(450, 200)
                };
                container.Controls.Add(noDataLabel);
                return;
            }

            int totalEmployees = employees.Count;
            double avgSalary = employees.Average(e => (double)e.Salary);
            double totalSalary = employees.Sum(e => (double)e.Salary);
            var topPerformer = employees.OrderByDescending(e => e.Salary).FirstOrDefault();
            var oldestEmployee = employees.OrderByDescending(e => e.Age).FirstOrDefault();
            var youngestEmployee = employees.OrderBy(e => e.Age).FirstOrDefault();
            double avgAge = employees.Average(e => e.Age);

            // Row 1 - Main metrics (4 cards)
            int yPos = 0;
            int cardWidth = 265;
            int cardHeight = 150;
            int spacing = 20;

            CreateStatCard(container, new Point(0, yPos), new Size(cardWidth, cardHeight),
                "👥", "Total Employees", totalEmployees.ToString(), PrimaryAccent);

            CreateStatCard(container, new Point(cardWidth + spacing, yPos), new Size(cardWidth, cardHeight),
                "💰", "Average Salary", $"${avgSalary:N2}", SuccessColor);

            CreateStatCard(container, new Point((cardWidth + spacing) * 2, yPos), new Size(cardWidth, cardHeight),
                "💵", "Total Payroll", $"${totalSalary:N2}", InfoColor);

            CreateStatCard(container, new Point((cardWidth + spacing) * 3, yPos), new Size(cardWidth, cardHeight),
                "📈", "Average Age", $"{avgAge:F1} years", WarningColor);

            // Row 2 - Employee highlights (3 larger cards)
            yPos += cardHeight + spacing;
            cardWidth = 360;
            cardHeight = 180;

            if (topPerformer != null)
            {
                CreateEmployeeCard(container, new Point(0, yPos), new Size(cardWidth, cardHeight),
                    "⭐", "Top Performer", topPerformer.Name,
                    $"Salary: ${topPerformer.Salary:N2}\n{topPerformer.Position ?? "N/A"}",
                    SecondaryAccent);
            }

            if (oldestEmployee != null)
            {
                CreateEmployeeCard(container, new Point(cardWidth + spacing, yPos), new Size(cardWidth, cardHeight),
                    "🎂", "Most Experienced", oldestEmployee.Name,
                    $"Age: {oldestEmployee.Age} years\n{oldestEmployee.Department ?? "N/A"}",
                    InfoColor);
            }

            if (youngestEmployee != null)
            {
                CreateEmployeeCard(container, new Point((cardWidth + spacing) * 2, yPos), new Size(cardWidth, cardHeight),
                    "🌟", "Youngest Employee", youngestEmployee.Name,
                    $"Age: {youngestEmployee.Age} years\n{youngestEmployee.Position ?? "N/A"}",
                    SuccessColor);
            }

            // Row 3 - Department breakdown
            yPos += cardHeight + spacing;
            var departmentStats = employees
                .Where(e => !string.IsNullOrEmpty(e.Department))
                .GroupBy(e => e.Department)
                .Select(g => new { Department = g.Key, Count = g.Count(), AvgSalary = g.Average(e => (double)e.Salary) })
                .OrderByDescending(x => x.Count)
                .ToList();

            if (departmentStats.Any())
            {
                CreateDepartmentBreakdownCard(container, new Point(0, yPos), new Size(1120, 200), departmentStats);
            }
        }

        private void CreateStatCard(Panel container, Point location, Size size, string icon, string label, string value, Color accentColor)
        {
            var card = new Panel
            {
                Location = location,
                Size = size,
                BackColor = CardBackground,
                Cursor = Cursors.Hand
            };

            // Add shadow and rounded corners
            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(card.ClientRectangle, 15))
                {
                    card.Region = new Region(path);

                    // Shadow effect
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                    {
                        var shadowRect = new Rectangle(3, 3, card.Width, card.Height);
                        e.Graphics.FillPath(shadowBrush, path);
                    }

                    using (var brush = new SolidBrush(CardBackground))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
            };

            // Icon background circle
            var iconPanel = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(60, 60),
                BackColor = Color.FromArgb(30, accentColor)
            };
            iconPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.FromArgb(30, accentColor)))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, 60, 60);
                }
            };

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 24),
                ForeColor = accentColor,
                Location = new Point(10, 8),
                Size = new Size(40, 40),
                BackColor = Color.Transparent
            };
            iconPanel.Controls.Add(iconLabel);

            var labelText = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = TextSecondary,
                Location = new Point(20, 90),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var valueText = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(20, 110),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            card.Controls.AddRange(new Control[] { iconPanel, labelText, valueText });

            // Hover effect
            card.MouseEnter += (s, e) =>
            {
                card.BackColor = Color.FromArgb(249, 250, 251);
                card.Invalidate();
            };
            card.MouseLeave += (s, e) =>
            {
                card.BackColor = CardBackground;
                card.Invalidate();
            };

            container.Controls.Add(card);
        }

        private void CreateEmployeeCard(Panel container, Point location, Size size, string icon, string title, string name, string details, Color accentColor)
        {
            var card = new Panel
            {
                Location = location,
                Size = size,
                BackColor = CardBackground
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(card.ClientRectangle, 15))
                {
                    card.Region = new Region(path);
                    using (var brush = new SolidBrush(CardBackground))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
            };

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 32),
                ForeColor = accentColor,
                Location = new Point(20, 20),
                Size = new Size(50, 50),
                BackColor = Color.Transparent
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = TextSecondary,
                Location = new Point(20, 80),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var nameLabel = new Label
            {
                Text = name,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(20, 105),
                Size = new Size(320, 30),
                BackColor = Color.Transparent
            };

            var detailsLabel = new Label
            {
                Text = details,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = TextSecondary,
                Location = new Point(20, 140),
                Size = new Size(320, 35),
                BackColor = Color.Transparent
            };

            card.Controls.AddRange(new Control[] { iconLabel, titleLabel, nameLabel, detailsLabel });
            container.Controls.Add(card);
        }

        private void CreateDepartmentBreakdownCard(Panel container, Point location, Size size, dynamic departmentStats)
        {
            var card = new Panel
            {
                Location = location,
                Size = size,
                BackColor = CardBackground,
                AutoScroll = true
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(card.ClientRectangle, 15))
                {
                    card.Region = new Region(path);
                    using (var brush = new SolidBrush(CardBackground))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
            };

            var titleLabel = new Label
            {
                Text = "🏢 Department Breakdown",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(20, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            card.Controls.Add(titleLabel);

            int xPos = 20;
            int yPos = 60;
            int index = 0;
            Color[] colors = { PrimaryAccent, SuccessColor, WarningColor, InfoColor, SecondaryAccent };

            foreach (var dept in departmentStats)
            {
                var deptCard = new Panel
                {
                    Location = new Point(xPos, yPos),
                    Size = new Size(210, 110),
                    BackColor = Color.FromArgb(10, colors[index % colors.Length])
                };

                deptCard.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var path = GetRoundedRectPath(deptCard.ClientRectangle, 10))
                    {
                        deptCard.Region = new Region(path);
                    }
                };

                var deptName = new Label
                {
                    Text = dept.Department,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = TextPrimary,
                    Location = new Point(15, 15),
                    Size = new Size(180, 25),
                    BackColor = Color.Transparent
                };

                var countLabel = new Label
                {
                    Text = $"{dept.Count} employees",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = TextSecondary,
                    Location = new Point(15, 45),
                    AutoSize = true,
                    BackColor = Color.Transparent
                };

                var salaryLabel = new Label
                {
                    Text = $"Avg: ${dept.AvgSalary:N0}",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = colors[index % colors.Length],
                    Location = new Point(15, 70),
                    AutoSize = true,
                    BackColor = Color.Transparent
                };

                deptCard.Controls.AddRange(new Control[] { deptName, countLabel, salaryLabel });
                card.Controls.Add(deptCard);

                xPos += 230;
                if (xPos > 900)
                {
                    xPos = 20;
                    yPos += 120;
                }
                index++;
            }

            container.Controls.Add(card);
        }

        private void LoadStatistics()
        {
            // Statistics are loaded when cards are created
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            float r = radius;
            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
