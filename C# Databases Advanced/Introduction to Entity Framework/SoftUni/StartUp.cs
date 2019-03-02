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
                var employeeInfo = RemoveTown(context);
                Console.WriteLine(employeeInfo);
            }
        }

        // 3.	Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var employee in context.Employees.OrderBy(x => x.EmployeeId))
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        // 4.	Employees with Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var sb = new StringBuilder();

            foreach (var employee in context.Employees.Where(x => x.Salary > 50000).OrderBy(x => x.FirstName))
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        // 5.	Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var sb = new StringBuilder();

            foreach (var employee in context.Employees.Where(x => x.Department.Name == "Research and Development").OrderBy(x => x.Salary))
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from Research and Development - ${employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        // 6.	Adding a New Address and Updating Employee
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

        // 7.	Employees and Projects
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

        // 8.	Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var addressInfo = context.Addresses
                .Where(x => x.Employees.Any())
                .Select(x => new
                {
                    x.AddressText,
                    TownName = x.Town.Name,
                    EmployeeCount = x.Employees.Count
                }
                ).ToList()
                .OrderByDescending(x => x.EmployeeCount)
                .ThenBy(x => x.TownName)
                .ThenBy(x => x.AddressText)
                .Take(10)
                .ToList();

            foreach (var address in addressInfo)
            {
                sb.AppendLine($"{address.AddressText}, {address.TownName} - {address.EmployeeCount} employees");
            }

            return sb.ToString().TrimEnd();
        }

        // 9.	Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var neededEmployee = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    Projects = x.EmployeesProjects
                    .Select(y => new
                    {
                        ProjectName = y.Project.Name
                    })
                    .OrderBy(p => p.ProjectName)
                    .ToList()
                })
                .ToList();

            foreach (var employee in neededEmployee)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
                foreach (var projects in employee.Projects)
                {
                    sb.AppendLine($"{projects.ProjectName}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        // 10.	Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var contextInfo = context.Departments
                .Where(x => x.Employees.Count > 5)
                .Select(d => new
                {
                    d.Name,
                    ManagerFullName = d.Manager.FirstName + " " + d.Manager.LastName,
                    Employee = d.Employees.Select(e => new
                    {
                        FullName = e.FirstName + " " + e.LastName,
                        e.JobTitle
                    })
                    .OrderBy(x => x.JobTitle)
                    .ToList()
                })
                .OrderBy(x => x.Employee.Count)
                .ToList();

            foreach (var info in contextInfo)
            {
                sb.AppendLine($"{info.Name} – {info.ManagerFullName}");
                foreach (var employee in info.Employee)
                {
                    sb.AppendLine($"{employee.FullName} - {employee.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        // 11.	Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            string dateFormat = "M/d/yyyy h:mm:ss tt";

            var sb = new StringBuilder();

            var latestProjects = context.Projects
                .OrderBy(x => x.Name)
                .ThenByDescending(x => x.StartDate)
                .Select(x => new
                {
                    x.Name,
                    x.Description,
                    StartDate = x.StartDate.ToString(dateFormat, CultureInfo.InvariantCulture)
                })
                .Take(10)
                .ToList();

            foreach (var p in latestProjects)
            {
                sb.AppendLine(p.Name);
                sb.AppendLine(p.Description);
                sb.AppendLine(p.StartDate);
            }

            return sb.ToString().TrimEnd();
        }

        // 12.	Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employeesToIncrease = context.Employees
                .Where(x => x.Department.Name == "Engineering" ||
                            x.Department.Name == "Marketing" ||
                            x.Department.Name == "Tool Design" ||
                            x.Department.Name == "Information Services")
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();

            foreach (var item in employeesToIncrease)
            {
                item.Salary *= 1.12m;
            }

            foreach (var e in employeesToIncrease)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        // 13.	Find Employees by First Name Starting With "Sa"
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employees = context.Employees
                .Where(x => x.FirstName.StartsWith("Sa"))
                .Select(e => new
                {
                    FullName = e.FirstName + " " + e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(x => x.FullName)
                .ToList();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FullName} - {e.JobTitle} - (${e.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        // 14.	Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var project = context.Projects.FirstOrDefault(x => x.ProjectId == 2);

            var employeeProjects = context.EmployeesProjects
                .Where(x => x.ProjectId == 2)
                .ToList();

            foreach (var proj in employeeProjects)
            {
                context.EmployeesProjects.Remove(proj);
            }

            context.Projects.Remove(project);

            context.SaveChanges();

            foreach (var em in context.Projects)
            {
                sb.AppendLine($"{em.Name}");
            }

            return sb.ToString().TrimEnd();
        }

        // 15.	Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            StringBuilder builder = new StringBuilder();

            var empMod = context.Employees
                .Where(x => x.Address.Town.Name == "Seattle")
                .ToList();

            var addresses = context.Addresses
                .Where(x => x.Town.Name == "Seattle")
                .ToList();

            var empAddresses = context.Employees
                .Select(x => x.Address)
                .ToList();

            empMod.ForEach(x => x.AddressId = null);


            builder.AppendLine($"{addresses.Count} addresses in Seattle were deleted");

            addresses.ForEach(x => x.TownId = null);

            var town = context.Towns.FirstOrDefault(x => x.Name == "Seattle");
            context.Towns.Remove(town);

            context.SaveChanges();

            return builder.ToString().TrimEnd();
        }
    }
}