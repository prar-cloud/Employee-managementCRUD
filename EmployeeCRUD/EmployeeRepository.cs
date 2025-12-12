using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCRUD
{
    public class EmployeeRepository
    {
        // In-memory storage - data will persist during application runtime
        private static List<Employee> _employees = new List<Employee>();

        public void AddEmployee(Employee emp)
        {
            _employees.Add(emp);
            Console.WriteLine("✓ Employee added successfully");
        }

        public void ViewAllEmployees()
        {
            if (_employees.Count == 0)
            {
                Console.WriteLine("\nNo employees found.");
                return;
            }

            Console.WriteLine("\n==== Employee List ====");
            Console.WriteLine("------------------------");
            foreach (var emp in _employees)
            {
                Console.WriteLine($"Roll: {emp.RollNumber,-6} | Name: {emp.Name,-20} | Age: {emp.Age,-3} | Salary: ${emp.Salary:N2}");
            }
            Console.WriteLine("------------------------");
        }

        public void DeleteEmployee(int rollNumber)
        {
            var employee = _employees.FirstOrDefault(e => e.RollNumber == rollNumber);
            if (employee != null)
            {
                _employees.Remove(employee);
                Console.WriteLine("✓ Employee Deleted.");
            }
            else
            {
                Console.WriteLine("✗ No Employee Found.");
            }
        }

        public void UpdateEmployee(Employee emp)
        {
            var existingEmployee = _employees.FirstOrDefault(e => e.RollNumber == emp.RollNumber);
            if (existingEmployee != null)
            {
                existingEmployee.Name = emp.Name;
                existingEmployee.Age = emp.Age;
                existingEmployee.Salary = emp.Salary;
                Console.WriteLine("✓ Employee updated.");
            }
            else
            {
                Console.WriteLine("✗ No employee found to update.");
            }
        }

        public void Stats()
        {
            if (_employees.Count == 0)
            {
                Console.WriteLine("\nNo employees in the database.");
                return;
            }

            int total = _employees.Count;
            double avg = (double)_employees.Average(e => e.Salary);
            var topPerformer = _employees.OrderByDescending(e => e.Salary).FirstOrDefault();

            Console.WriteLine("\n==== Employee Statistics ====");
            Console.WriteLine($"Total Employees: {total}");
            Console.WriteLine($"Average Salary: ${avg:N2}");
            Console.WriteLine($"Top Performer: {topPerformer?.Name ?? "N/A"} (Salary: ${topPerformer?.Salary:N2 ?? 0})");
            Console.WriteLine("=============================");
        }

        public bool EmployeeExists(int rollNumber)
        {
            return _employees.Any(e => e.RollNumber == rollNumber);
        }

        public List<Employee> GetAllEmployees()
        {
            return _employees.ToList();
        }

        public int GetTotalCount()
        {
            return _employees.Count;
        }

        public double GetAverageSalary()
        {
            return _employees.Count > 0 ? (double)_employees.Average(e => e.Salary) : 0;
        }

        public Employee? GetTopPerformer()
        {
            return _employees.OrderByDescending(e => e.Salary).FirstOrDefault();
        }

        public double GetTotalSalary()
        {
            return (double)_employees.Sum(e => e.Salary);
        }

        public Employee? GetOldestEmployee()
        {
            return _employees.OrderByDescending(e => e.Age).FirstOrDefault();
        }

        public Employee? GetYoungestEmployee()
        {
            return _employees.OrderBy(e => e.Age).FirstOrDefault();
        }
    }
}
