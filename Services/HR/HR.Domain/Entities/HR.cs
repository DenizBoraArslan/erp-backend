using Common.Models;

namespace HR.Domain.Entities;

public class Employee : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = string.Empty;
    public EmploymentType EmploymentType { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public decimal Salary { get; set; }
    public int DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? ManagerId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Attendance : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public AttendanceStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class Leave : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LeaveType Type { get; set; }
    public LeaveStatus Status { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public enum EmploymentType
{
    FullTime = 0,
    PartTime = 1,
    Contract = 2,
    Temporary = 3
}

public enum AttendanceStatus
{
    Present = 0,
    Absent = 1,
    Late = 2,
    LeaveDay = 3
}

public enum LeaveType
{
    Annual = 0,
    Sick = 1,
    Personal = 2,
    Maternity = 3
}

public enum LeaveStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Cancelled = 3
}
