namespace MiniORM.App
{
    using System.Linq;
    using Data;
    using Data.Entities;

    public class StartUp
    {
        public static void Main()
        {
            var connectionString = @"Server=DESKTOP-58UASJ8\SQLEXPRESS;Database=MiniORM;Integrated Security=True";

            SoftUniDbContext db = new SoftUniDbContext(connectionString);

            db.Employees.Add(new Employee
                (
                "Gosho",
                "Inserted",
                db.Departments.First().Id,
                true
                ));

            Employee employee = db.Employees.Last();
            employee.FirstName = "Modified";

            db.SaveChanges();
        }
    }
}