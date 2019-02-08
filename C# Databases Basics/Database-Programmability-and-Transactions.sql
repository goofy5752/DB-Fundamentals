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
--CREATE PROC ufn_GetSalaryLevel @salary DECIMAL(18,4)
--AS
--SELECT @salary AS 'Salary',
--	CASE
--	WHEN @salary < 30000 THEN 'Low'
--	WHEN @salary BETWEEN 30000 AND 50000 THEN 'Average'
--	WHEN @salary > 50000 THEN 'High'
--	END AS 'Salary Level'
--
--EXEC ufn_GetSalaryLevel 43000