--1.  Сколько учеников у каждого учителя. Сортировать по количеству учеников 
--от меньшего
SELECT 
	t.ID,
    t.name, 
    COUNT(s.ID) AS NumberOfStudents
FROM 
    Teachers AS t
LEFT JOIN 
    Students AS s ON t.ID = s.TeacherID
GROUP BY 
    t.ID, t.name
ORDER BY 
    COUNT(s.ID);


--2.  Найти ученика, у которого максимальный бал по Математике с 01.01.2021 
--по 01.01.2022, не брать учителей, у которых возраст старше 40.
SELECT TOP 1
	s.ID, 
    s.Name, 
    AVG(CASE WHEN e.LessonID = '2' THEN e.Score ELSE 0 END) AS MaxAvgScore
FROM 
    Students AS s
LEFT JOIN 
    Exams AS e ON s.ID = e.StudentID
LEFT JOIN 
    Teachers AS t ON s.TeacherID = t.ID
LEFT JOIN
	Lessons AS l ON l.ID = e.LessonID
WHERE 
    e.Date BETWEEN '01.01.2021' AND '01.01.2022'
	AND e.LessonID = '2'
    AND t.Age <= 40 
GROUP BY 
    s.ID, s.Name
ORDER BY 
    MaxAvgScore DESC 


--3. Найти ученика, который третий по баллам по Математике с 01.01.2021 по 01.01.2022.
SELECT 
    s.ID, 
    s.name, 
    AVG(CASE WHEN e.LessonID = '2' THEN e.Score ELSE 0 END) AS ThreeAvgScore
FROM 
    Students AS s
LEFT JOIN 
    Exams AS e ON s.ID = e.StudentID
LEFT JOIN 
    Teachers AS t ON s.TeacherID = t.ID
WHERE 
    e.date BETWEEN '2021-01-01' AND '2022-01-01'
    AND e.LessonID = '2'
GROUP BY 
    s.ID, s.name
ORDER BY 
    AVG(CASE WHEN e.LessonID = '2' THEN e.Score ELSE 0 END) DESC
OFFSET 2 ROWS FETCH FIRST 1 ROW ONLY;
