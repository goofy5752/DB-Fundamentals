--          Database Basics (MSSQL) Demo Exam

--		    Colonial Journey

--          Section 1. DDL (30 pts)

-- 1. Database Design
CREATE TABLE Planets
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] VARCHAR(30) NOT NULL
)

CREATE TABLE Spaceports
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) NOT NULL,
	PlanetId INT FOREIGN KEY REFERENCES Planets(Id) NOT NULL
)

CREATE TABLE Spaceships
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) NOT NULL,
	Manufacturer VARCHAR(30) NOT NULL,
	LightSpeedRate INT DEFAULT 0
)

CREATE TABLE Colonists
(
	Id INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(20) NOT NULL,
	LastName VARCHAR(20) NOT NULL,
	Ucn VARCHAR(10) NOT NULL UNIQUE,
	BirthDate DATE NOT NULL
)

CREATE TABLE Journeys
(
	Id INT PRIMARY KEY IDENTITY,
	JourneyStart DATETIME NOT NULL,
	JourneyEnd DATETIME NOT NULL,
	Purpose VARCHAR(11),
	DestinationSpaceportId INT FOREIGN KEY REFERENCES Spaceports(Id) NOT NULL,
	SpaceshipId INT FOREIGN KEY REFERENCES Spaceships(Id) NOT NULL
)

CREATE TABLE TravelCards
(
	Id INT PRIMARY KEY IDENTITY,
	CardNumber CHAR(10) NOT NULL UNIQUE,
	JobDuringJourney VARCHAR(8),
	ColonistId INT FOREIGN KEY REFERENCES Colonists(Id) NOT NULL,
	JourneyId INT FOREIGN KEY REFERENCES Journeys(Id) NOT NULL
)

ALTER TABLE Journeys
ADD CONSTRAINT CHK_Journey CHECK (Purpose = 'Medical' AND Purpose = 'Technical' AND Purpose = 'Educational' AND Purpose = 'Military')

ALTER TABLE TravelCards
ADD CONSTRAINT CHK_TravelCard CHECK (JobDuringJourney = 'Engineer' AND JobDuringJourney = 'Pilot' AND JobDuringJourney = 'Trooper' AND JobDuringJourney = 'Cleaner' AND JobDuringJourney = 'Cook')

-- Section 2. DML (10 pts)

-- 2. Insert
INSERT INTO Planets VALUES
('Mars'),
('Earth'),
('Jupiter'),
('Saturn')

INSERT INTO Spaceships VALUES
('Golf',	'VW',	3),
('WakaWaka',	'Wakanda',	4),
('Falcon9',	'SpaceX',	1),
('Bed',	'Vidolov',	6)