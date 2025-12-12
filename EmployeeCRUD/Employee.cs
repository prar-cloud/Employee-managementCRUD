using System;
using System.Text.Json.Serialization;

namespace EmployeeCRUD
{
    public class Employee
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("rollNumber")]
        public int RollNumber { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("age")]
        public int Age { get; set; }

        [JsonPropertyName("department")]
        public string? Department { get; set; }

        [JsonPropertyName("position")]
        public string? Position { get; set; }

        [JsonPropertyName("hireDate")]
        public DateTime? HireDate { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("salary")]
        public decimal Salary { get; set; }

        [JsonPropertyName("vacationDaysAvailable")]
        public int VacationDaysAvailable { get; set; } = 8;

        [JsonPropertyName("vacationDaysUsed")]
        public int VacationDaysUsed { get; set; } = 0;

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = true;

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [JsonPropertyName("lastModifiedDate")]
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;
    }

    public class PayrollRecord
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("payrollID")]
        public int PayrollID { get; set; }

        [JsonPropertyName("rollNumber")]
        public int RollNumber { get; set; }

        [JsonPropertyName("payPeriodStart")]
        public DateTime PayPeriodStart { get; set; }

        [JsonPropertyName("payPeriodEnd")]
        public DateTime PayPeriodEnd { get; set; }

        [JsonPropertyName("baseSalary")]
        public decimal BaseSalary { get; set; }

        [JsonPropertyName("bonus")]
        public decimal Bonus { get; set; } = 0;

        [JsonPropertyName("deductions")]
        public decimal Deductions { get; set; } = 0;

        [JsonPropertyName("netPay")]
        public decimal NetPay { get; set; }

        [JsonPropertyName("paymentDate")]
        public DateTime? PaymentDate { get; set; }

        [JsonPropertyName("paymentStatus")]
        public string PaymentStatus { get; set; } = "Pending";

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // For display purposes
        [JsonIgnore]
        public string? EmployeeName { get; set; }
    }

    public class VacationRecord
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("vacationID")]
        public int VacationID { get; set; }

        [JsonPropertyName("rollNumber")]
        public int RollNumber { get; set; }

        [JsonPropertyName("vacationType")]
        public string VacationType { get; set; } = "Annual";

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("daysCount")]
        public int DaysCount { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Pending";

        [JsonPropertyName("requestDate")]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [JsonPropertyName("approvedBy")]
        public string? ApprovedBy { get; set; }

        [JsonPropertyName("approvalDate")]
        public DateTime? ApprovalDate { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        // For display purposes
        [JsonIgnore]
        public string? EmployeeName { get; set; }
    }
}
