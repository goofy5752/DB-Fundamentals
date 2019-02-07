SELECT d.[Name], e.EmployeeID, FirstName + ' ' + LastName AS [Full Name], MAX(Salary) AS Salary
	FROM Employees AS e
	JOIN Departments AS d ON d.DepartmentID = e.DepartmentID 
	GROUP BY d.[Name], e.EmployeeID, FirstName + ' ' + LastName
	ORDER BY d.[Name], MAX(Salary) DESC, [Full Name]