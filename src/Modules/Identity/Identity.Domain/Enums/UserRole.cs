namespace Identity.Domain.Enums
{

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
