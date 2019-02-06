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

-- Problem 6. Employees Hired After
SELECT e.FirstName, e.LastName, e.HireDate, d.[Name]
	FROM Employees AS e
	JOIN Departments AS d ON d.DepartmentID = e.DepartmentID
	WHERE d.[Name] = 'Sales' OR d.[Name] = 'Finance'
	ORDER BY e.HireDate

-- Problem 7. Employees with Project
SELECT TOP(5) e.EmployeeID, e.FirstName, p.[Name]
	FROM Employees AS e
	JOIN EmployeesProjects AS ep ON ep.EmployeeID = e.EmployeeID
	JOIN Projects AS p ON p.ProjectID = ep.ProjectID
	WHERE p.EndDate IS NULL
	ORDER BY e.EmployeeID

-- Problem 8. Employee 24
SELECT e.EmployeeID, e.FirstName,
	CASE
    WHEN p.StartDate > '01-01-2005' THEN NULL
	WHEN p.StartDate < '01-01-2005' THEN p.[Name]
	END AS ProjectName
	FROM Employees AS e
	JOIN EmployeesProjects AS ep ON ep.EmployeeID = e.EmployeeID
	JOIN Projects AS p ON p.ProjectID = ep.ProjectID
	WHERE e.EmployeeID = 24

-- Problem 9. Employee Manager
SELECT emp.EmployeeID, emp.FirstName, mng.EmployeeID, mng.FirstName
	FROM Employees AS emp
	JOIN Employees AS mng ON mng.EmployeeID = emp.ManagerID
	WHERE emp.ManagerID IN (3, 7)
	ORDER BY emp.EmployeeID

-- Problem 10. Employee Summary
SELECT TOP(50) emp.EmployeeID, emp.FirstName + ' ' + emp.LastName, mng.FirstName + ' ' +         mng.LastName, d.[Name]
	FROM Employees AS emp
	JOIN Employees AS mng ON mng.EmployeeID = emp.ManagerID
	JOIN Departments AS d ON d.DepartmentID = emp.DepartmentID
	ORDER BY emp.EmployeeID

-- Problem 11. Min Average Salary
SELECT TOP(1) (SELECT AVG(Salary)) AS AVERAGESALARY FROM Employees
GROUP BY DepartmentID
ORDER BY (AVERAGESALARY)

-- Problem 12. Highest Peaks in Bulgaria
SELECT c.CountryCode, m.MountainRange, p.PeakName, p.Elevation
	FROM Countries AS c
	JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode
	JOIN Mountains AS m ON m.Id = mc.MountainId
	JOIN Peaks AS p ON p.MountainId = m.Id
	WHERE p.Elevation > 2835 AND c.CountryCode = 'BG'
	ORDER BY p.Elevation DESC

-- Problem 13. Count Mountain Ranges
SELECT c.CountryCode, COUNT(m.MountainRange) AS MountainRanges
	FROM Countries AS c
	JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode
	JOIN Mountains AS m ON m.Id = mc.MountainId
	WHERE c.CountryCode IN ('BG', 'RU', 'US')
	GROUP BY c.CountryCode

-- Problem 14. Countries with Rivers
SELECT TOP(5) c.CountryName, r.RiverName
	FROM Countries AS c
	--JOIN Continents AS co ON co.ContinentCode = c.ContinentCode
	LEFT JOIN CountriesRivers AS cr ON cr.CountryCode = c.CountryCode
	LEFT JOIN Rivers AS r ON r.Id = cr.RiverId
	WHERE c.ContinentCode = 'AF'
	ORDER BY c.CountryName

-- Problem 15. *Continents and Currencies
SELECT MostUsedCurrency.ContinentCode, MostUsedCurrency.CurrencyCode, MostUsedCurrency.CurrencyUsage
	FROM
	(SELECT c.ContinentCode, c.CurrencyCode , COUNT(c.CurrencyCode) AS CurrencyUsage,
		DENSE_RANK() OVER (PARTITION BY c.ContinentCode ORDER BY COUNT(c.CurrencyCode) DESC) AS CurrencyRank
		FROM Countries AS c
		GROUP BY c.ContinentCode, c.CurrencyCode
		HAVING COUNT(c.CurrencyCode) > 1) AS MostUsedCurrency
	WHERE MostUsedCurrency.CurrencyRank = 1
	ORDER BY MostUsedCurrency.ContinentCode

-- Problem 16. Countries without any Mountains
SELECT 
    ((SELECT 
            COUNT(CountryCode)
        FROM
            Countries) - (SELECT DISTINCT
            COUNT(DISTINCT CountryCode)
        FROM
            MountainsCountries)) AS CountryCode;

-- Problem 17. Highest Peak and Longest River by Country
SELECT TOP(5) c.CountryName,
	MAX(p.Elevation) AS HighestPeakElevation,
	MAX(r.[Length]) AS LongestRiverLenght
	FROM Countries AS c
	JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode
	JOIN Mountains AS m ON m.Id = mc.MountainId
	JOIN Peaks AS p ON p.MountainId = m.Id
	JOIN CountriesRivers AS cr ON cr.CountryCode = c.CountryCode
	JOIN Rivers AS r ON r.Id = cr.RiverId
	GROUP BY c.CountryName
	ORDER BY HighestPeakElevation DESC, LongestRiverLenght DESC, c.CountryName

-- Problem 18. *Highest Peak Name and Elevation by Country
SELECT TOP(5) c.CountryName AS Country,
	   ISNULL(p.PeakName, '(no highest peak)'),
	   ISNULL(MAX(p.Elevation), 0),
	   ISNULL(m.MountainRange, '(no mountain)')
	FROM Countries AS c
	LEFT JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode
	LEFT JOIN Mountains AS m ON m.Id = mc.MountainId
	LEFT JOIN Peaks AS p ON p.MountainId = m.Id
	GROUP BY c.CountryName, p.PeakName, m.MountainRange
	ORDER BY c.CountryName, p.PeakName
