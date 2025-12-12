using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCRUD
{
    public class EmployeeService
    {
        private EmployeeRepository _employeeRepository = new EmployeeRepository();

        public void AddEmployee()
        {
            try
            {
                Console.Write("Enter Employee Name: ");
                string name = Console.ReadLine();

                Console.Write("Enter Age: ");
                int.TryParse(Console.ReadLine(), out int age);

                Console.Write("Enter Roll Number: ");
                int.TryParse(Console.ReadLine(), out int roll);

                Console.Write("Enter Salary: ");
                double.TryParse(Console.ReadLine(), out double salary);

                var emp = new Employee { Name = name, Age = age, RollNumber = roll, Salary = (decimal)salary };
                var validator = new EmployeeValidator();
                var res = validator.Validate(emp);

                if (!res.IsValid)
                {
                    foreach (var error in res.Errors)
                    {
                        Console.WriteLine($"{error.ErrorMessage}");
                    }
                    return;
                }
                if (_employeeRepository.EmployeeExists(roll))
                {
                    Console.WriteLine("Roll Number Already Exists");
                    return;
                }
                _employeeRepository.AddEmployee(emp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in adding employee");
                Console.WriteLine($"Error : {ex.Message}");
            }
        }
        public void UpdateEmployee()
        {
            try
            {
                Console.Write("Enter Roll Number of employee to update: ");
                if (!int.TryParse(Console.ReadLine(), out int roll) || roll <= 0)
                {
                    Console.WriteLine(" Invalid roll number.");
                    return;
                }

                if (!_employeeRepository.EmployeeExists(roll))
                {
                    Console.WriteLine("⚠️ Employee not found.");
                    return;
                }

                Console.Write("Enter New Name: ");
                string name = Console.ReadLine();

                Console.Write("Enter New Age: ");
                int.TryParse(Console.ReadLine(), out int age);

                Console.Write("Enter New Salary: ");
                double.TryParse(Console.ReadLine(), out double salary);

                var updatedEmp = new Employee { RollNumber = roll, Name = name, Age = age, Salary = (decimal)salary};
                var validator = new EmployeeValidator();
                var results = validator.Validate(updatedEmp);

                if (!results.IsValid)
                {
                    foreach (var error in results.Errors)
                    {
                        Console.WriteLine($" {error.ErrorMessage}");
                    }
                    return;
                }

                _employeeRepository.UpdateEmployee(updatedEmp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Something went wrong while updating.");
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        public void DeleteEmployee()
        {
            try
            {
                Console.Write("Enter the roll number of Employee to delete: ");
                if (!int.TryParse(Console.ReadLine(), out int roll) || roll <= 0)
                {
                    Console.WriteLine("Invalid roll number");
                    return;
                }
                if (!_employeeRepository.EmployeeExists(roll))
                {
                    Console.WriteLine("Employee Not found with the given roll number");
                    return;
                }
                _employeeRepository.DeleteEmployee(roll);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in deleting employee");
                Console.WriteLine($"Error : {ex.Message}");
            }
        }
        public void ViewAllEmployees()
        {
            _employeeRepository.ViewAllEmployees();
        }

        public void ShowStatistics()
        {
            _employeeRepository.Stats();
        }
    }
}
