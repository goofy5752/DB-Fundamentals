--             Section I. Functions and Procedures

--             Part 1. Queries for SoftUni Database

-- Problem 1. Employees with Salary Above 35000
CREATE PROC usp_GetEmployeesSalaryAbove35000
AS
SELECT FirstName, LastName
	FROM Employees
	WHERE Salary > 35000

-- Problem 2. Employees with Salary Above Number
CREATE PROC usp_GetEmployeesSalaryAboveNumber @inputNumber DECIMAL(18,4)
AS
SELECT FirstName, LastName
	FROM Employees
	WHERE Salary >= @inputNumber

-- Problem 3. Town Names Starting With
CREATE PROC usp_GetTownsStartingWith @input VARCHAR(10)
AS
SELECT [Name]
	FROM Towns
	WHERE [Name] LIKE @input + '%'

-- Problem 4. Employees from Town
CREATE PROC usp_GetEmployeesFromTown @townName VARCHAR(MAX)
AS
SELECT e.FirstName, e.LastName
	FROM Employees AS e
	JOIN Addresses AS a ON a.AddressID = e.AddressID
	JOIN Towns AS t ON t.TownID = a.TownID
	WHERE t.[Name] = @townName

-- Problem 5. Salary Level Function
CREATE FUNCTION ufn_GetSalaryLevel (@salary DECIMAL(18, 4))
RETURNS CHAR(10)
BEGIN
    DECLARE @salaryLevel CHAR(10)
	IF (@salary < 30000)
BEGIN
	SET @salaryLevel = 'Low'
END
	ELSE IF(@salary BETWEEN 30000 AND 50000)
BEGIN
	SET @salaryLevel = 'Average'
END
	ELSE
BEGIN
	Set @salaryLevel = 'High'
END

RETURN @salaryLevel
END

-- Problem 6. Employees by Salary Level
CREATE PROC usp_EmployeesBySalaryLevel @salaryLevel VARCHAR(10)
AS
SELECT FirstName, LastName
	FROM Employees
	WHERE dbo.ufn_GetSalaryLevel(Salary) = @salaryLevel

-- Problem 7. Define Function
CREATE FUNCTION ufn_IsWordComprised(@setOfLetters VARCHAR(15), @word VARCHAR(15))
RETURNS BIT
BEGIN
	DECLARE @count INT = 1

WHILE (@count <= LEN(@word))
BEGIN
	DECLARE @currentLetter CHAR(1) = SUBSTRING(@word, @count, 1)
	DECLARE @charIndex INT = CHARINDEX(@currentletter, @setOfLetters)

	IF(@charIndex = 0)
	BEGIN
		RETURN 0
	END

	SET @count += 1
END
RETURN 1
END

-- Problem 8. * Delete Employees and Departments
CREATE PROC usp_DeleteEmployeesFromDepartment(@departmentId INT)
AS
  ALTER TABLE Departments
    ALTER COLUMN ManagerID INT NULL

  DELETE FROM EmployeesProjects
  WHERE EmployeeID IN
        (
          SELECT EmployeeID
          FROM Employees
          WHERE DepartmentID = @departmentId
        )

  UPDATE Employees
  SET ManagerID = NULL
  WHERE ManagerID IN
        (
          SELECT EmployeeID
          FROM Employees
          WHERE DepartmentID = @departmentId
        )

  UPDATE Departments
  SET ManagerID = NULL
  WHERE ManagerID IN
        (
          SELECT EmployeeID
          FROM Employees
          WHERE DepartmentID = @departmentId
        )

  DELETE FROM Employees
  WHERE EmployeeID IN
        (
          SELECT EmployeeID
          FROM Employees
          WHERE DepartmentID = @departmentId
        )

  DELETE FROM Departments
  WHERE DepartmentID = @departmentId

  SELECT COUNT(*) AS [Employees Count]
  FROM Employees AS E
    JOIN Departments AS D
      ON D.DepartmentID = E.DepartmentID
  WHERE E.DepartmentID = @departmentId

--				Part 2. Queries for Bank Database

-- Problem 9. Find Full Name
CREATE PROCEDURE usp_GetHoldersFullName
AS
SELECT FirstName + ' ' + LastName AS [Full Name]
	FROM AccountHolders

-- Problem 10. People with Balance Higher Than
CREATE PROCEDURE usp_GetHoldersWithBalanceHigherThan (@inputNumber DECIMAL(18, 4))
AS
SELECT k.FirstName, k.LastName
	FROM (
SELECT ah.Id, FirstName, LastName
	FROM AccountHolders AS ah
	JOIN Accounts AS a ON a.AccountHolderId = ah.Id
	GROUP BY ah.Id, FirstName, LastName 
	HAVING SUM(a.Balance) > @inputNumber) AS k
	ORDER BY k.FirstName, k.LastName

-- Problem 11. Future Value Function
CREATE FUNCTION ufn_CalculateFutureValue (@sum DECIMAL (18,4), @yearlyInterestRate FLOAT, @numberOfYears INT)
RETURNS DECIMAL(18, 4)
BEGIN
	DECLARE @totalSum DECIMAL(18, 4) = @sum * POWER((1 + @yearlyInterestRate), @numberOfYears)

	RETURN @totalSum
END

-- Problem 12. Calculating Interest
CREATE PROCEDURE usp_CalculateFutureValueForAccount (@accountId INT, @interestRate FLOAT)
AS
SELECT a.Id, ah.FirstName, ah.LastName, a.Balance, dbo.ufn_CalculateFutureValue(a.Balance, @interestRate, 5) AS [Balance In 5 years]
	FROM AccountHolders AS ah
	JOIN Accounts AS a ON a.AccountHolderId = ah.Id
	WHERE a.Id = @accountId

--                 Part 3. Queries for Diablo Database

-- Problem 13. *Scalar Function: Cash in User Games Odd Rows
CREATE FUNCTION ufn_CashInUsersGames (@gameName VARCHAR(MAX))
RETURNS TABLE
AS
RETURN
(
SELECT SUM(k.Cash) AS TotalCash
	FROM (
	SELECT Cash, ROW_NUMBER() OVER(ORDER BY Cash DESC) AS [Row]
		FROM Games AS g
		JOIN UsersGames AS ug ON ug.GameId = g.Id
		WHERE g.Name = @gameName ) AS k
		WHERE k.Row % 2 = 1 
)

--                 Section II. Triggers and Transactions

--                  Part 1. Queries for Bank Database

-- Problem 14. Create Table Logs