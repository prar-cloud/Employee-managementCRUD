using System.Collections.Generic;

namespace EmployeeCRUD
{
    public interface IEmployeeRepository
    {
        // Employee operations
        List<Employee> GetAllEmployees();
        Employee? GetEmployeeByRollNumber(int rollNumber);
        List<Employee> SearchEmployees(string searchTerm);
        void AddEmployee(Employee employee);
        void UpdateEmployee(Employee employee);
        void DeleteEmployee(int rollNumber);
        bool EmployeeExists(int rollNumber);
        int GetTotalCount();
        decimal GetAverageSalary();
        Employee? GetTopPerformer();
        decimal GetTotalSalary();
        Employee? GetOldestEmployee();
        Employee? GetYoungestEmployee();

        // Payroll operations
        List<PayrollRecord> GetAllPayrollRecords();
        List<PayrollRecord> GetPayrollRecordsByEmployee(int rollNumber);
        void AddPayrollRecord(PayrollRecord record);

        // Vacation operations
        List<VacationRecord> GetAllVacationRecords();
        List<VacationRecord> GetVacationRecordsByEmployee(int rollNumber);
        void AddVacationRecord(VacationRecord record);
        void UpdateVacationRecord(VacationRecord record);
        void AddVacationDays(int rollNumber, int days);
        void AddVacationDaysToAll(int days);
    }
}
