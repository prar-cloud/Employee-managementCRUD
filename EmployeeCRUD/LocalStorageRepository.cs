using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace EmployeeCRUD
{
    public class LocalStorageRepository : IEmployeeRepository
    {
        private readonly string _dataDirectory;
        private readonly string _employeesFile;
        private readonly string _payrollFile;
        private readonly string _vacationFile;

        private List<Employee> _employees;
        private List<PayrollRecord> _payrollRecords;
        private List<VacationRecord> _vacationRecords;

        public LocalStorageRepository()
        {
            // Store data in application directory
            _dataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EmployeeCRUD"
            );

            _employeesFile = Path.Combine(_dataDirectory, "employees.json");
            _payrollFile = Path.Combine(_dataDirectory, "payroll.json");
            _vacationFile = Path.Combine(_dataDirectory, "vacations.json");

            // Create directory if it doesn't exist
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }

            // Load existing data or create new lists
            _employees = LoadFromFile<List<Employee>>(_employeesFile) ?? new List<Employee>();
            _payrollRecords = LoadFromFile<List<PayrollRecord>>(_payrollFile) ?? new List<PayrollRecord>();
            _vacationRecords = LoadFromFile<List<VacationRecord>>(_vacationFile) ?? new List<VacationRecord>();
        }

        #region File Operations

        private T? LoadFromFile<T>(string filePath) where T : class
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonSerializer.Deserialize<T>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from {filePath}: {ex.Message}");
            }
            return null;
        }

        private void SaveToFile<T>(string filePath, T data)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(data, options);
                File.WriteAllText(filePath, json);

                // Log the save operation
                string fileName = Path.GetFileName(filePath);
                string entityType = fileName.Replace(".json", "");
                DataQueryLogger.Log("SAVE", entityType.ToUpper(), $"Data written to {fileName}", filePath, true);
            }
            catch (Exception ex)
            {
                DataQueryLogger.Log("SAVE_ERROR", "FILE", ex.Message, filePath, false);
                throw new Exception($"Error saving to {filePath}: {ex.Message}", ex);
            }
        }

        #endregion

        #region Employee Operations

        public List<Employee> GetAllEmployees()
        {
            return _employees.Where(e => e.IsActive).ToList();
        }

        public Employee? GetEmployeeByRollNumber(int rollNumber)
        {
            return _employees.FirstOrDefault(e => e.RollNumber == rollNumber && e.IsActive);
        }

        public List<Employee> SearchEmployees(string searchTerm)
        {
            return _employees.Where(e =>
                e.IsActive &&
                (e.RollNumber.ToString().Contains(searchTerm) ||
                 e.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        public void AddEmployee(Employee employee)
        {
            employee.CreatedDate = DateTime.Now;
            employee.LastModifiedDate = DateTime.Now;
            _employees.Add(employee);
            DataQueryLogger.Log("INSERT", "EMPLOYEE", $"Added employee: {employee.Name} (ID: {employee.RollNumber})", _employeesFile, true);
            SaveToFile(_employeesFile, _employees);
        }

        public void UpdateEmployee(Employee employee)
        {
            var existing = _employees.FirstOrDefault(e => e.RollNumber == employee.RollNumber);
            if (existing != null)
            {
                existing.Name = employee.Name;
                existing.Age = employee.Age;
                existing.Department = employee.Department;
                existing.Position = employee.Position;
                existing.HireDate = employee.HireDate;
                existing.Email = employee.Email;
                existing.Phone = employee.Phone;
                existing.Salary = employee.Salary;
                existing.VacationDaysAvailable = employee.VacationDaysAvailable;
                existing.VacationDaysUsed = employee.VacationDaysUsed;
                existing.LastModifiedDate = DateTime.Now;

                DataQueryLogger.Log("UPDATE", "EMPLOYEE", $"Updated employee: {employee.Name} (ID: {employee.RollNumber})", _employeesFile, true);
                SaveToFile(_employeesFile, _employees);
            }
        }

        public void DeleteEmployee(int rollNumber)
        {
            var employee = _employees.FirstOrDefault(e => e.RollNumber == rollNumber);
            if (employee != null)
            {
                employee.IsActive = false;
                DataQueryLogger.Log("DELETE", "EMPLOYEE", $"Deleted employee: {employee.Name} (ID: {rollNumber})", _employeesFile, true);
                SaveToFile(_employeesFile, _employees);
            }
        }

        public bool EmployeeExists(int rollNumber)
        {
            return _employees.Any(e => e.RollNumber == rollNumber && e.IsActive);
        }

        public int GetTotalCount()
        {
            return _employees.Count(e => e.IsActive);
        }

        public decimal GetAverageSalary()
        {
            var activeEmployees = _employees.Where(e => e.IsActive).ToList();
            return activeEmployees.Count > 0 ? activeEmployees.Average(e => e.Salary) : 0;
        }

        public Employee? GetTopPerformer()
        {
            return _employees.Where(e => e.IsActive).OrderByDescending(e => e.Salary).FirstOrDefault();
        }

        public decimal GetTotalSalary()
        {
            return _employees.Where(e => e.IsActive).Sum(e => e.Salary);
        }

        public Employee? GetOldestEmployee()
        {
            return _employees.Where(e => e.IsActive).OrderByDescending(e => e.Age).FirstOrDefault();
        }

        public Employee? GetYoungestEmployee()
        {
            return _employees.Where(e => e.IsActive).OrderBy(e => e.Age).FirstOrDefault();
        }

        #endregion

        #region Payroll Operations

        public List<PayrollRecord> GetAllPayrollRecords()
        {
            if (_payrollRecords == null)
            {
                _payrollRecords = new List<PayrollRecord>();
            }

            var records = _payrollRecords.ToList();
            foreach (var record in records)
            {
                if (record != null)
                {
                    var emp = GetEmployeeByRollNumber(record.RollNumber);
                    if (emp != null)
                        record.EmployeeName = emp.Name;
                }
            }
            return records.OrderByDescending(p => p.PayPeriodStart).ToList();
        }

        public List<PayrollRecord> GetPayrollRecordsByEmployee(int rollNumber)
        {
            if (_payrollRecords == null)
            {
                _payrollRecords = new List<PayrollRecord>();
            }

            var records = _payrollRecords.Where(p => p != null && p.RollNumber == rollNumber).ToList();
            var emp = GetEmployeeByRollNumber(rollNumber);
            foreach (var record in records)
            {
                if (record != null)
                {
                    record.EmployeeName = emp?.Name;
                }
            }
            return records.OrderByDescending(p => p.PayPeriodStart).ToList();
        }

        public void AddPayrollRecord(PayrollRecord record)
        {
            if (_payrollRecords == null)
            {
                _payrollRecords = new List<PayrollRecord>();
            }

            // Get next ID
            record.PayrollID = _payrollRecords.Count > 0 ? _payrollRecords.Max(p => p.PayrollID) + 1 : 1;
            record.CreatedDate = DateTime.Now;

            var employee = GetEmployeeByRollNumber(record.RollNumber);
            if (employee != null)
            {
                record.EmployeeName = employee.Name;
            }

            _payrollRecords.Add(record);
            SaveToFile(_payrollFile, _payrollRecords);
        }

        #endregion

        #region Vacation Operations

        public List<VacationRecord> GetAllVacationRecords()
        {
            if (_vacationRecords == null)
            {
                _vacationRecords = new List<VacationRecord>();
            }

            var records = _vacationRecords.ToList();
            foreach (var record in records)
            {
                if (record != null)
                {
                    var emp = GetEmployeeByRollNumber(record.RollNumber);
                    if (emp != null)
                        record.EmployeeName = emp.Name;
                }
            }
            return records.OrderByDescending(v => v.StartDate).ToList();
        }

        public List<VacationRecord> GetVacationRecordsByEmployee(int rollNumber)
        {
            if (_vacationRecords == null)
            {
                _vacationRecords = new List<VacationRecord>();
            }

            var records = _vacationRecords.Where(v => v != null && v.RollNumber == rollNumber).ToList();
            var emp = GetEmployeeByRollNumber(rollNumber);
            foreach (var record in records)
            {
                if (record != null)
                {
                    record.EmployeeName = emp?.Name;
                }
            }
            return records.OrderByDescending(v => v.StartDate).ToList();
        }

        public void AddVacationRecord(VacationRecord record)
        {
            if (_vacationRecords == null)
            {
                _vacationRecords = new List<VacationRecord>();
            }

            // Get next ID
            record.VacationID = _vacationRecords.Count > 0 ? _vacationRecords.Max(v => v.VacationID) + 1 : 1;
            record.RequestDate = DateTime.Now;

            var employee = GetEmployeeByRollNumber(record.RollNumber);
            if (employee != null)
            {
                record.EmployeeName = employee.Name;
                if (record.Status == "Approved")
                {
                    employee.VacationDaysUsed += record.DaysCount;
                    UpdateEmployee(employee);
                }
            }

            _vacationRecords.Add(record);
            SaveToFile(_vacationFile, _vacationRecords);
        }

        public void UpdateVacationRecord(VacationRecord record)
        {
            if (_vacationRecords == null)
            {
                _vacationRecords = new List<VacationRecord>();
                return;
            }

            var existing = _vacationRecords.FirstOrDefault(v => v != null && v.VacationID == record.VacationID);
            if (existing != null)
            {
                var oldStatus = existing.Status;
                var oldDaysCount = existing.DaysCount;

                existing.VacationType = record.VacationType;
                existing.StartDate = record.StartDate;
                existing.EndDate = record.EndDate;
                existing.DaysCount = record.DaysCount;
                existing.Status = record.Status;
                existing.ApprovedBy = record.ApprovedBy;
                existing.ApprovalDate = record.ApprovalDate;
                existing.Reason = record.Reason;
                existing.Notes = record.Notes;

                // Update employee vacation days
                var employee = GetEmployeeByRollNumber(record.RollNumber);
                if (employee != null)
                {
                    if (oldStatus != "Approved" && record.Status == "Approved")
                    {
                        employee.VacationDaysUsed += record.DaysCount;
                        UpdateEmployee(employee);
                    }
                    else if (oldStatus == "Approved" && record.Status != "Approved")
                    {
                        employee.VacationDaysUsed -= oldDaysCount;
                        UpdateEmployee(employee);
                    }
                }

                SaveToFile(_vacationFile, _vacationRecords);
            }
        }

        public void AddVacationDays(int rollNumber, int days)
        {
            var employee = GetEmployeeByRollNumber(rollNumber);
            if (employee != null)
            {
                employee.VacationDaysAvailable += days;
                UpdateEmployee(employee);
            }
        }

        public void AddVacationDaysToAll(int days)
        {
            var activeEmployees = _employees.Where(e => e.IsActive).ToList();
            foreach (var employee in activeEmployees)
            {
                employee.VacationDaysAvailable += days;
                employee.LastModifiedDate = DateTime.Now;
            }
            SaveToFile(_employeesFile, _employees);
        }

        #endregion

        #region Utility

        public string GetDataDirectory()
        {
            return _dataDirectory;
        }

        public void ClearAllData()
        {
            _employees.Clear();
            _payrollRecords.Clear();
            _vacationRecords.Clear();

            SaveToFile(_employeesFile, _employees);
            SaveToFile(_payrollFile, _payrollRecords);
            SaveToFile(_vacationFile, _vacationRecords);
        }

        #endregion
    }
}
