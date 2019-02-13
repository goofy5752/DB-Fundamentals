--           Database Basics MS SQL Exam – 24 Jun 2018

--						Supermarket 

--                 Section 1. DDL (30 pts)

-- 1. Database Design
CREATE TABLE Categories
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(30) NOT NULL
)

CREATE TABLE Items
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(30) NOT NULL,
	Price DECIMAL(15, 2) NOT NULL,
	CategoryId INT FOREIGN KEY REFERENCES Categories(Id) NOT NULL
)

CREATE TABLE Employees
(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	Phone CHAR(12) NOT NULL,
	Salary DECIMAL(15, 2) NOT NULL
)

CREATE TABLE Orders
(
	Id INT PRIMARY KEY IDENTITY,
	[DateTime] DATETIME NOT NULL,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id) NOT NULL
)

CREATE TABLE OrderItems
(
	OrderId INT FOREIGN KEY REFERENCES Orders(Id) NOT NULL,
	ItemId INT FOREIGN KEY REFERENCES Items(Id) NOT NULL,
	Quantity INT CHECK(Quantity > 0) NOT NULL

	PRIMARY KEY (OrderId, ItemId)
)

CREATE TABLE Shifts
(
	Id INT IDENTITY NOT NULL,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id) NOT NULL,
	CheckIn DATETIME NOT NULL,
	CheckOut DATETIME NOT NULL

	PRIMARY KEY (Id, EmployeeId)
)

ALTER TABLE Shifts
ADD CONSTRAINT CHK_Person CHECK (CheckIn < CheckOut)

--                Section 2. DML (10 pts)

--  2. Insert
INSERT INTO Employees (FirstName, LastName, Phone, Salary) VALUES
('Stoyan',	'Petrov',	'888-785-8573',	500.25),
('Stamat',	'Nikolov',	'789-613-1122',	999995.25),
('Evgeni',	'Petkov',	'645-369-9517',	1234.51),
('Krasimir',	'Vidolov',	'321-471-9982',	50.25)

INSERT INTO Items ([Name], Price, CategoryId) VALUES
('Tesla battery', 154.25, 8),
('Chess', 30.25, 8),
('Juice', 5.32, 1),
('Glasses', 10,	8),
('Bottle of water',	1, 1)

-- 3. Update
UPDATE Items
SET Price *= 1.27
WHERE CategoryId IN (1, 2, 3)

-- 4. Delete
DELETE FROM OrderItems
WHERE OrderId = 48

--                Section 3. Querying (40 pts)

-- 5. Richest People
SELECT Id, FirstName
	FROM Employees
	WHERE Salary > 6500
	ORDER BY FirstName, Id

-- 6. Cool Phone Numbers
SELECT FirstName + ' ' + LastName AS [Full Name],
	 Phone AS [Phone Number]
	FROM Employees
	WHERE Phone LIKE '3%'
	ORDER BY FirstName, Phone

-- 7. Employee Statistics
SELECT e.FirstName, e.LastName, COUNT(*) AS [Count]
	FROM Employees AS e
	JOIN Orders AS o ON o.EmployeeId = e.Id
	GROUP BY e.FirstName, e.LastName
	ORDER BY [Count] DESC, FirstName

-- 8. Hard Workers Club
SELECT 
		FirstName, 
		LastName, 
		AVG(DATEDIFF(HOUR, s.CheckIn, s.CheckOut)) AS [Work hours]
	FROM Employees AS e
	JOIN Shifts AS s ON s.EmployeeId = e.Id
	GROUP BY e.Id, FirstName, LastName
	HAVING AVG(DATEDIFF(HOUR, s.CheckIn, s.CheckOut)) > 7
	ORDER BY [Work hours] DESC, e.Id

-- 9. The Most Expensive Order
SELECT TOP(1) oi.OrderId, SUM(i.Price * oi.Quantity) AS 'TotalPrice'
	FROM OrderItems AS oi
	JOIN Items AS i ON i.Id = oi.ItemId
	GROUP BY oi.OrderId
	ORDER BY TotalPrice DESC

-- 10. Rich Item, Poor Item
SELECT TOP(10) oi.OrderId, MAX(i.Price) AS 'ExpensivePrice', MIN(i.Price) AS 'CheapPrice'
	FROM Items AS i
	JOIN OrderItems AS oi ON oi.ItemId = i.Id
	GROUP BY oi.OrderId 
	ORDER BY ExpensivePrice DESC, oi.OrderId

-- 11. Cashiers
SELECT e.Id, e.FirstName, e.LastName
	FROM Employees AS e
	JOIN Orders AS o ON o.EmployeeId = e.Id
	GROUP BY e.Id, e.FirstName, e.LastName
	ORDER BY e.Id

-- 12. Lazy Employees
SELECT DISTINCT e.Id, e.FirstName + ' ' + e.LastName AS [Full Name]
	FROM Employees AS e
	JOIN Shifts AS s ON s.EmployeeId = e.Id
	WHERE DATEDIFF(HOUR, s.CheckIn, s.CheckOut) < 4
	ORDER BY e.Id

-- 13. Sellers
SELECT TOP(10) e.FirstName + ' ' + e.LastName AS [Full Name],
	   SUM(oi.Quantity * i.Price) AS [Total Price],
	   SUM(oi.Quantity) AS [Items]
	FROM Employees AS e
	JOIN Orders AS o ON o.EmployeeId = e.Id
	JOIN OrderItems AS oi ON oi.OrderId = o.Id
	JOIN Items AS i ON i.Id = oi.ItemId
	WHERE o.[DateTime] < '2018-06-15'
	GROUP BY e.FirstName, e.LastName
	ORDER BY [Total Price] DESC, Items DESC

-- 14. Tough days