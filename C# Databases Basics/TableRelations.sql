--               Exercises: Table Relations

-- Problem 1. One-To-One Relationship
CREATE DATABASE TableRelations

USE TableRelations

CREATE TABLE Persons
(
	PersonID INT PRIMARY KEY,
	FirstName VARCHAR(20) NOT NULL,
	Salary DECIMAL(15, 2),
	PassportID INT NOT NULL
)

CREATE TABLE Passports
(
	PassportID INT PRIMARY KEY,
	PassportNumber CHAR(8)
)

ALTER TABLE Persons
ADD CONSTRAINT FK_Persons_Passports 
FOREIGN KEY (PassportID) 
REFERENCES Passports(PassportID)

ALTER TABLE Persons
ADD UNIQUE (PassportID)

--ALTER TABLE Passports
--ADD UNIQUE (PassportNumber)

INSERT INTO Passports (PassportID, PassportNumber)
VALUES
	(101, 'N34FG21B'),
	(102, 'K65LO4R7'),
	(103, 'ZE657QP2')


INSERT INTO Persons (PersonID, FirstName, Salary, PassportID) 
VALUES
	(1, 'Roberto',  43300.00, 102),
	(2,	'Tom',	56100.00, 103),
	(3,	'Yana',	60200.00, 101)

-- Problem 2. One-To-Many Relationship
CREATE TABLE Manufacturers
(
	ManufacturerID INT PRIMARY KEY IDENTITY,
	[Name] VARCHAR(15) NOT NULL,
	EstablishedOn DATE NOT NULL
)

CREATE TABLE Models
(
	ModelID INT PRIMARY KEY IDENTITY(101, 1),
	[Name] VARCHAR(15) NOT NULL,
	ManufacturerID INT FOREIGN KEY REFERENCES Manufacturers(ManufacturerID)
)

INSERT INTO Manufacturers ([Name], EstablishedOn)
VALUES
	('BMW', '07/03/1916'),
	('Tesla', '01/01/2003'),
	('Lada', '01/05/1966')

INSERT INTO Models ([Name], ManufacturerID)
VALUES
	('X1',	1),
	('i6',	1),
	('Model S',	2),
	('Model X',	2),
	('Model 3',	2),
	('Nova',	3)