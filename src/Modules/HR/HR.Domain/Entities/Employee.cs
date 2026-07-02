using HR.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Domain.Entities
{
    public class Employee
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string? Phone { get; private set; }
        public Department Department { get; private set; }
        public EmploymentType EmploymentType { get; private set; }
        public string Position { get; private set; } = string.Empty;
        public decimal Salary { get; private set; }
        public DateTime HireDate { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Employee() { }

        public static Employee Create(
            string firstName, string lastName, string email,
            Department department, EmploymentType employmentType,
            string position, decimal salary, DateTime hireDate,
            string? phone = null)
        {
            return new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Email = email.ToLowerInvariant(),
                Phone = phone,
                Department = department,
                EmploymentType = employmentType,
                Position = position,
                Salary = salary,
                HireDate = hireDate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(string firstName, string lastName, string position, decimal salary, string? phone)
        {
            FirstName = firstName;
            LastName = lastName;
            Position = position;
            Salary = salary;
            Phone = phone;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
