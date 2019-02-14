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
ADD CONSTRAINT CHK_Journeys CHECK (Purpose IN('Medical', 'Technical' ,'Educational','Military'))

ALTER TABLE TravelCards
ADD CONSTRAINT CHK_TravelCards CHECK (JobDuringJourney IN('Engineer', 'Pilot', 'Trooper', 'Cleaner', 'Cook'))

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

-- 3. Update
UPDATE Spaceships
SET LightSpeedRate += 1
WHERE Id BETWEEN 8 AND 12

-- 4. Delete
DELETE FROM TravelCards
WHERE JourneyId IN (1, 2 ,3)

DELETE TOP(3) 
FROM Journeys

--  Section 3. Querying (40 pts)

-- 5. Select all travel cards
SELECT CardNumber, JobDuringJourney
	FROM TravelCards
	ORDER BY CardNumber

-- 6. Select all colonists
SELECT Id, FirstName + ' ' + LastName AS [Full Name], Ucn
	FROM Colonists
	ORDER BY FirstName, LastName, Id

-- 7. Select all military journeys
SELECT Id, FORMAT( JourneyStart, 'dd/MM/yyyy', 'en-US' ), FORMAT( JourneyEnd, 'dd/MM/yyyy', 'en-US' )
	FROM Journeys
	WHERE Purpose = 'Military'
	ORDER BY JourneyStart

-- 8. Select all pilots
SELECT c.Id, FirstName + ' ' + LastName AS [Full Name]
	FROM Colonists AS c
	JOIN TravelCards AS tc ON tc.ColonistId = c.Id
	WHERE tc.JobDuringJourney = 'Pilot'
	ORDER BY c.Id

-- 9. Count colonists
SELECT COUNT(*)
	FROM Colonists AS c
	JOIN TravelCards AS tc ON tc.ColonistId = c.Id
	JOIN Journeys AS j ON j.Id = tc.JourneyId
	WHERE j.Purpose = 'Technical'

-- 10. Select the fastest spaceship
SELECT TOP(1) ss.[Name], sp.[Name]
	FROM Spaceports as sp
	JOIN Journeys AS j ON j.DestinationSpaceportId = sp.Id
	JOIN Spaceships AS ss ON ss.Id = j.SpaceshipId
	ORDER BY ss.LightSpeedRate DESC