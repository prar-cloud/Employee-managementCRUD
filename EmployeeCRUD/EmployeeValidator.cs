using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace EmployeeCRUD
{
    public class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            RuleFor(e => e.Name).NotEmpty().WithMessage("Name is Required")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 Characters");

            RuleFor(e => e.Age).GreaterThan(0).WithMessage("Invalid Age")
                .LessThanOrEqualTo(120).WithMessage("Age must be realistic");

            RuleFor(e => e.RollNumber).GreaterThan(0).WithMessage("Roll Number must be positive");

            RuleFor(e => e.Salary).GreaterThanOrEqualTo(0).WithMessage("Salary cannot be negative");
        }
    }
}
