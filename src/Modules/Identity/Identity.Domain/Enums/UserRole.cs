namespace Identity.Domain.Enums
{
    /// <summary>
    /// Kept "User" and "Manager" for backward compatibility with any
    /// already-registered accounts (role is stored as a string column, so
    /// existing rows still deserialize fine). Going forward, every new
    /// account should get one of the module-scoped roles below instead —
    /// each module has a "Manager" (full control within that module) and a
    /// narrower operational role that can't approve/delete/manage others.
    /// </summary>
    public enum UserRole
    {
        User = 0,
        Manager = 1,
        Admin = 2,

        // Human Resources
        HRManager = 3,
        HREmployee = 4,

        // Sales
        SalesManager = 5,
        SalesRep = 6,

        // Finance
        FinanceManager = 7,
        FinanceSpecialist = 8,

        // Inventory
        InventoryManager = 9,
        InventoryStaff = 10,
    }
}
