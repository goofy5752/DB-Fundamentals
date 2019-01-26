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
	GROUP BY DepositGroup

-- ZDR