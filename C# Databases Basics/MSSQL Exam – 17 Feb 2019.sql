--         Database Basics MSSQL Exam – 17 Feb 2019

--       School

--	 Section 1. DDL (30 pts)

-- 1. Database Design
CREATE TABLE Students
(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(30) NOT NULL,
	MiddleName NVARCHAR(25),
	LastName NVARCHAR(30) NOT NULL,
	Age INT,
	[Address] NVARCHAR(50),
	Phone NCHAR(10)
)

CREATE TABLE Subjects
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(20) NOT NULL,
	Lessons INT CHECK(Lessons > 0) NOT NULL
)

CREATE TABLE StudentsSubjects
(
	Id INT PRIMARY KEY IDENTITY,
	StudentId INT FOREIGN KEY REFERENCES Students(Id) NOT NULL,
	SubjectId INT FOREIGN KEY REFERENCES Subjects(Id) NOT NULL,
	Grade DECIMAL(15,2) CHECK (Grade BETWEEN 2 AND 6) NOT NULL
)

CREATE TABLE Exams
(
	Id INT PRIMARY KEY IDENTITY,
	[Date] DATETIME,
	SubjectId INT FOREIGN KEY REFERENCES Subjects(Id) NOT NULL
)

CREATE TABLE StudentsExams
(
	StudentId INT FOREIGN KEY REFERENCES Students(Id) NOT NULL,
	ExamId INT FOREIGN KEY REFERENCES Exams(Id) NOT NULL,
	Grade DECIMAL(15, 2) CHECK (Grade BETWEEN 2 AND 6) NOT NULL

	PRIMARY KEY (StudentId, ExamId)
)

CREATE TABLE Teachers
(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(20) NOT NULL,
	LastName NVARCHAR(20) NOT NULL,
	Address NVARCHAR(20) NOT NULL,
	Phone NCHAR(10),
	SubjectId INT FOREIGN KEY REFERENCES Subjects(Id) NOT NULL
)

CREATE TABLE StudentsTeachers
(
	StudentId INT FOREIGN KEY REFERENCES Students(Id) NOT NULL,
	TeacherId INT FOREIGN KEY REFERENCES Teachers(Id) NOT NULL

	PRIMARY KEY (StudentId, TeacherId)
)

ALTER TABLE Students
ADD CONSTRAINT CHK_StudentAge CHECK (Age BETWEEN 5 AND 100)

ALTER TABLE Students
ADD CONSTRAINT CHK_StudentAgePositive CHECK (Age > 0)

-- Section 2. DML (10 pts)

-- 2. Insert
INSERT INTO Teachers (FirstName, LastName, Address, Phone, SubjectId) VALUES 
('Ruthanne',	'Bamb',	'84948 Mesta Junction',	'3105500146',	6),
('Gerrard',	'Lowin',	'370 Talisman Plaza',	'3324874824',	2),
('Merrile',	'Lambdin',	'81 Dahle Plaza',	'4373065154',	5),
('Bert',	'Ivie',	'2 Gateway Circle',	'4409584510',	4)

INSERT INTO Subjects (Name, Lessons) VALUES
('Geometry',	12),
('Health',	10),
('Drama',	7),
('Sports',	9)

-- 3. Update
UPDATE StudentsSubjects
SET Grade = 6.00 
WHERE SubjectId IN (1, 2) AND Grade >= 5.50

-- 4. Delete
DELETE FROM StudentsTeachers
WHERE TeacherId IN ( 
SELECT Id FROM Teachers
WHERE Phone LIKE '%72%' AND Phone IS NOT NULL);

DELETE FROM Teachers
WHERE Phone LIKE '%72%'

-- Section 3. Querying (40 pts)

-- 5. Teen Students
SELECT FirstName, LastName, Age
	FROM Students
	WHERE Age >= 12
	ORDER BY FirstName, LastName

-- 6. Cool Addresses
SELECT FirstName + ' ' + ISNULL(MiddleName, '') + ' ' + LastName, Address
	FROM Students
	WHERE [Address] LIKE '%road%'
	ORDER BY FirstName, LastName, Address

-- 7. 42 Phones
SELECT k.FirstName, k.Address, k.Phone
	FROM(
SELECT FirstName,ISNULL(MiddleName, 'k') AS MiddleName, Address , Phone
	FROM Students
	WHERE Phone LIKE '42%' AND MiddleName != 'k') AS k
	ORDER BY FirstName

-- 8. Students Teachers
SELECT s.FirstName, s.LastName, COUNT(st.TeacherId)
	FROM StudentsTeachers AS st
	JOIN Students AS s ON s.Id = st.StudentId
	GROUP BY s.FirstName, s.LastName

-- 9. Subjects with Students
SELECT t.FirstName + ' ' + t.LastName AS FullName, s.[Name] + '-' + CAST(s.Lessons AS varchar(12)) AS Subjects, COUNT(st.StudentId) AS Students
	FROM Teachers AS t
	JOIN StudentsTeachers AS st ON st.TeacherId = t.Id
	JOIN Subjects AS s ON s.Id = t.SubjectId
	GROUP BY t.FirstName, t.LastName, s.[Name], s.Lessons
	ORDER BY Students DESC, FullName, Subjects

-- 10. Students to Go
SELECT s.FirstName + ' ' + s.LastName AS [Full Name]
	FROM Students AS s
	LEFT JOIN StudentsExams AS se ON se.StudentId = s.Id
	WHERE se.StudentId IS NULL
	ORDER BY [Full Name]

-- 11. Busiest Teachers
SELECT TOP(10) t.FirstName, t.LastName, COUNT(st.StudentId) AS Students
	FROM Teachers AS t
	JOIN StudentsTeachers AS st ON st.TeacherId = t.Id
	GROUP BY t.FirstName, t.LastName
	ORDER BY Students DESC, t.FirstName

-- 12. Top Students
SELECT TOP(10) s.FirstName, s.LastName, FORMAT(AVG(se.Grade), 'N', 'en-us') AS Grade
	FROM Students AS s
	JOIN StudentsExams AS se ON se.StudentId = s.Id
	GROUP BY s.FirstName, s.LastName
	ORDER BY Grade DESC, FirstName, LastName

-- 13. Second Highest Grade
SELECT k.FirstName, k.LastName, k.Grade
	FROM(
SELECT FirstName, LastName, Grade, DENSE_RANK() OVER (PARTITION BY FirstName, LastName ORDER BY Grade DESC) AS GradeRank 
	FROM Students AS s
	JOIN StudentsSubjects AS sb ON sb.StudentId = s.Id) AS k
	WHERE GradeRank = 1
	GROUP BY k.FirstName, k.LastName, k.Grade
	ORDER BY k.Grade DESC

-- 14. Not So In The Studying
SELECT 
	CASE 
	WHEN s.MiddleName IS NULL THEN FirstName + ' ' + s.LastName
	ELSE FirstName + ' ' + s.MiddleName + ' ' + s.LastName
	END AS [Full Name]
	FROM Students AS s
	LEFT JOIN StudentsSubjects AS ss ON ss.StudentId = s.Id
	WHERE ss.StudentId IS NULL
	ORDER BY [Full Name]

-- 15. Top Student per Teacher
SELECT t.FirstName + ' ' + t.LastName AS [Teacher Full Name], 
	   sb.[Name] AS SubjectName, 
	   s.FirstName + ' ' + s.LastName AS [Student Full Name] ,
       FORMAT(AVG(ss.Grade), 'N', 'en-us') AS Grade, 
       DENSE_RANK() OVER (ORDER BY AVG(ss.Grade) DESC) AS RankGrade
	FROM Teachers AS T
	JOIN StudentsTeachers AS st ON st.TeacherId = t.Id
	JOIN Students AS s ON s.Id = st.StudentId
	JOIN StudentsSubjects AS ss ON ss.StudentId = s.Id
	JOIN Subjects AS sb ON sb.Id = ss.SubjectId
	GROUP BY t.FirstName, t.LastName, sb.[Name], s.FirstName, s.LastName

-- 16. Average Grade per Subject
SELECT s.[Name], AVG(ss.Grade)
	FROM Subjects AS s
	JOIN StudentsSubjects AS ss ON ss.SubjectId = s.Id
	GROUP BY s.[Name], s.Id
	ORDER BY s.Id

-- 17. Exams Information
SELECT k.Quarter, k.SubjectName, SUM(k.StudentsCount) AS StudentsCount
	FROM (
SELECT 
    CASE 
	WHEN [Date] IS NULL THEN 'TBA'
	WHEN DATEPART(MM, [Date]) BETWEEN 1 AND 3 THEN 'Q1'
    WHEN DATEPART(MM, [Date]) BETWEEN 3 AND 6 THEN 'Q2'
    WHEN DATEPART(MM, [Date]) BETWEEN 6 AND 9 THEN 'Q3' 
    ELSE 'Q4'
	END AS [Quarter], s.[Name] AS SubjectName, COUNT(se.StudentId) AS StudentsCount
	FROM Exams AS e
	JOIN Subjects AS s ON s.Id = e.SubjectId
	JOIN StudentsExams AS se ON se.ExamId = e.Id
	WHERE se.Grade >= 4.00
	GROUP BY e.Date, s.[Name]) AS k
	GROUP BY Quarter, k.SubjectName
	ORDER BY [Quarter]

-- Section 4. Programmability (20 pts)

-- 18. Exam Grades
CREATE FUNCTION udf_ExamGradesToUpdate(@studentId INT, @grade DECIMAL(15,2))  
RETURNS TABLE  
AS  
RETURN   
(  
    SELECT P.ProductID, P.Name, SUM(SD.LineTotal) AS 'Total'  
    FROM Production.Product AS P   
    JOIN Sales.SalesOrderDetail AS SD ON SD.ProductID = P.ProductID  
    JOIN Sales.SalesOrderHeader AS SH ON SH.SalesOrderID = SD.SalesOrderID  
    JOIN Sales.Customer AS C ON SH.CustomerID = C.CustomerID  
    WHERE C.StoreID = @storeid  
    GROUP BY P.ProductID, P.Name  
)

-- 19. Exclude from school
CREATE FUNCTION udf_ExamGradesToUpdate(@studentId INT, @grade DECIMAL(15,2))
RETURNS VARCHAR(MAX)
AS
BEGIN
	DECLARE @student INT = ( SELECT Id FROM Students WHERE Id = @studentId )

	IF ( @student IS NULL )
	BEGIN
		RETURN 'The student with provided id does not exist in the school!'
	END

	IF ( @grade > 6.00 )
	BEGIN
		RETURN 'Grade cannot be above 6.00!'
	END

	DECLARE @grades VARCHAR(MAX) = 
			(SELECT CONCAT('You have to update ', 
			COUNT(se.Grade),' grades for the student ', 
			s.FirstName)		   
		FROM StudentsExams AS se
		JOIN Students AS s ON se.StudentId = s.Id
		WHERE StudentId = @studentId AND Grade BETWEEN @grade AND @grade+0.50
		GROUP BY se.StudentId,s.FirstName )
		
		RETURN @grades
END
	
-- 20. Deleted Student
CREATE TABLE ExcludedStudents(StudentId INT , StudentName NVARCHAR(50))
 
CREATE TRIGGER tr_deletedstudents_
  ON Students
  AFTER DELETE AS
  INSERT INTO ExcludedStudents(StudentId,StudentName)
      (SELECT Id,FirstName + ' ' + LastName FROM deleted)