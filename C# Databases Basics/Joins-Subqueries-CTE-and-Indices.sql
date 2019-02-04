--                 Exercises: Joins, Subqueries, CTE and Indices

-- Problem 1. Employee Address
Select TOP(5) e.EmployeeID, e.JobTitle, a.AddressID, a.AddressText
	FROM Employees AS e
	RIGHT JOIN Addresses AS a ON e.AddressID = a.AddressID
	ORDER BY a.AddressID

-- Problem 2. Addresses with Towns
SELECT TOP(50) e.FirstName, e.LastName, t.[Name] AS Town, a.AddressText
	FROM Employees AS e
	JOIN Addresses AS a ON a.AddressID = e.AddressID
	JOIN Towns AS t ON t.TownID = a.TownID
	ORDER BY e.FirstName, e.LastName

-- Problem 3. Sales Employee
SELECT EmployeeID, FirstName, LastName, d.[Name]
	FROM Employees AS e
	JOIN Departments AS d ON d.DepartmentID = e.DepartmentID
	WHERE d.Name = 'Sales'
	ORDER BY EmployeeID

-- Problem 4. Employee Departments
SELECT TOP(5) e.EmployeeID, e.FirstName, e.Salary, d.[Name]
	FROM Employees AS e
	JOIN Departments AS d ON d.DepartmentID = e.DepartmentID
	WHERE Salary > 15000
	ORDER BY e.DepartmentID

-- Problem 5. Employees Without Project
SELECT TOP(3) e.EmployeeID, e.FirstName
	FROM Employees AS e
	LEFT JOIN EmployeesProjects AS ep ON ep.EmployeeID = e.EmployeeID
	WHERE ep.ProjectID IS NULL
	ORDER BY e.EmployeeID