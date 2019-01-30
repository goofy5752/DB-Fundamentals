--              Exercises: Data Aggregation

-- Problem 1. Records’ Count
SELECT COUNT(*)
	FROM WizzardDeposits

-- Problem 2. Longest Magic Wand
SELECT TOP(1) MagicWandSize AS 'LongestMagicWand'
	FROM WizzardDeposits
	ORDER BY LongestMagicWand DESC

-- Problem 3. Longest Magic Wand per Deposit Groups
SELECT DepositGroup, MAX(MagicWandSize) AS 'LongestMagicWand'
	FROM WizzardDeposits
	GROUP BY DepositGroup

-- Problem 4. * Smallest Deposit Group per Magic Wand Size
SELECT TOP(2) DepositGroup
	FROM WizzardDeposits
	GROUP BY DepositGroup
	ORDER BY AVG(MagicWandSize)

-- Problem 5. Deposits Sum
SELECT DepositGroup, SUM(DepositAmount) AS 'TotalSum'
	FROM WizzardDeposits
	GROUP BY DepositGroup

-- Problem 6. Deposits Sum for Ollivander Family
SELECT DepositGroup, SUM(DepositAmount) AS 'TotalSum'
	FROM WizzardDeposits
	WHERE MagicWandCreator = 'Ollivander family'
	GROUP BY DepositGroup

-- Problem 7. Deposits Filter
SELECT DepositGroup, SUM(DepositAmount) AS 'TotalSum'
    FROM WizzardDeposits
	WHERE MagicWandCreator = 'Ollivander family'
	GROUP BY DepositGroup
	HAVING SUM(DepositAmount) < 150000
	ORDER BY TotalSum DESC

-- Problem 8. Deposit Charge
SELECT DepositGroup, MagicWandCreator, MIN(DepositCharge) as 'MinDepositCharge'
	FROM WizzardDeposits
	GROUP BY DepositGroup, MagicWandCreator
	ORDER BY MagicWandCreator, DepositGroup

-- Problem 9. Age Groups
SELECT age_range, COUNT(*) FROM 
(
select 
  case
   when Age BETWEEN 0 AND 10 then '[0-10]'
   when Age BETWEEN 11 AND 20 then '[11-20]'
   when Age BETWEEN 21 AND 30 then '[21-30]'
   when Age BETWEEN 31 AND 40 then '[31-40]'
   when Age BETWEEN 41 AND 50 then '[41-50]'
   when Age BETWEEN 51 AND 60 then '[51-60]'
   ELSE '[61+]'
 END as age_range 
 from WizzardDeposits
) t
group by age_range

-- Problem 10. First Letter
SELECT LEFT(FirstName, 1) AS 'FirstLetter'
	FROM WizzardDeposits
	WHERE DepositGroup = 'Troll Chest'
	GROUP BY LEFT(FirstName, 1)

-- Problem 11. Average Interest
SELECT DepositGroup, IsDepositExpired,  AVG(DepositInterest) AS 'AverageInterest'
	FROM WizzardDeposits
	WHERE DepositStartDate > '01/01/1985'
	GROUP BY DepositGroup, IsDepositExpired
	ORDER BY DepositGroup DESC, IsDepositExpired

-- Problem 12.* Rich Wizard, Poor Wizard
SELECT SUM(k.Diff) AS SumDifference
	FROM (
SELECT wd.DepositAmount -
	(SELECT w.DepositAmount FROM WizzardDeposits AS w WHERE w.Id = wd.Id + 1) AS Diff
	FROM WizzardDeposits as wd
	) AS k

-- Problem 13. Departments Total Salaries
SELECT DepartmentID, SUM(Salary) AS TotalSum
	FROM Employees
	GROUP BY DepartmentID

-- Problem 14. Employees Minimum Salaries
SELECT DepartmentID, MIN(Salary) AS 'MinimumSalary'
	FROM Employees
	WHERE DepartmentID IN (2, 5, 7)
	AND HireDate > '01/01/2000'
	GROUP BY DepartmentID

-- Problem 15. Employees Average Salaries
SELECT * INTO NewEmployeeTable
	FROM Employees
	WHERE Salary > 30000

DELETE FROM NewEmployeeTable
	WHERE ManagerID = 42

UPDATE NewEmployeeTable
SET Salary += 5000
WHERE DepartmentID = 1

SELECT DepartmentID, AVG(Salary) AS 'AVERAGE SALARY'
	FROM NewEmployeeTable
	GROUP BY DepartmentID

-- Problem 16. Employees Maximum Salaries
SELECT DepartmentID, MAX(Salary) AS 'MaxSalary'
	FROM Employees
	GROUP BY DepartmentID 
	HAVING MAX(Salary) NOT BETWEEN 30000 AND 70000

-- Problem 17. Employees Count Salaries
SELECT COUNT(*)
	FROM Employees
	WHERE ManagerID IS NULL

-- Problem 18. *3rd Highest Salary
SELECT DISTINCT k.DepartmentID, k.Salary
	FROM(
	(SELECT DepartmentID, Salary,
	DENSE_RANK() OVER (PARTITION BY DepartmentId ORDER BY Salary DESC) AS RankSalary
	FROM Employees)) AS k
	WHERE RankSalary = 3

-- Problem 19. **Salary Challenge
SELECT TOP(10) FirstName, LastName, DepartmentID
	FROM Employees AS e
	WHERE Salary > 
	(SELECT AVG(Salary) FROM Employees AS em WHERE e.DepartmentID = em.DepartmentID)
	ORDER BY DepartmentID