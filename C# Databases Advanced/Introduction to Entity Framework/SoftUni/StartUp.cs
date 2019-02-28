using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var context = new SoftUniContext())
            {
                var employeeInfo = GetEmployeesInPeriod(context);
                Console.WriteLine(employeeInfo);
            }
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var employee in context.Employees.OrderBy(x => x.EmployeeId))
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var sb = new StringBuilder();

            foreach (var employee in context.Employees.Where(x => x.Salary > 50000).OrderBy(x => x.FirstName))
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var sb = new StringBuilder();

            foreach (var employee in context.Employees.Where(x => x.Department.Name == "Research and Development").OrderBy(x => x.Salary))
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from Research and Development - ${employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var address = new Address();
            address.AddressText = "Vitoshka 15";
            address.TownId = 4;

            var employeeToAdd = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");

            employeeToAdd.Address = address;

            context.SaveChanges();

            var sb = new StringBuilder();

            var employees = context.Employees
                .OrderByDescending(a => a.AddressId)
                .Select(x => x.Address.AddressText)
                .Take(10)
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine(employee);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder builder = new StringBuilder();

            string dateFormat = @"M/d/yyyy h:mm:ss tt";

            var employees = context.Employees
                .Where(x => x.EmployeesProjects
                    .Any(y => y.Project.StartDate.Year >= 2001 && y.Project.StartDate.Year <= 2003))
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    Projects = x.EmployeesProjects
                        .Select(y => new
                        {
                            ProjectName = y.Project.Name,
                            StartDate = y.Project.StartDate,
                            EndDate = y.Project.EndDate
                        })

                })
                .Take(10)
                .ToList();

            foreach (var emp in employees)
            {
                builder.AppendLine(
                    $"{emp.FirstName} {emp.LastName} - Manager: {emp.ManagerFirstName} {emp.ManagerLastName}");

                foreach (var pr in emp.Projects)
                {

                    string endDate = pr.EndDate == null
                        ? "not finished"
                        : ((DateTime)pr.EndDate).ToString(dateFormat, CultureInfo.InvariantCulture);

                    string startDate = pr.StartDate.ToString(dateFormat, CultureInfo.InvariantCulture);

                    builder.AppendLine(
                        $"--{pr.ProjectName} - {startDate} - {endDate}");
                }
            }

            return builder.ToString();
        }
    }
}