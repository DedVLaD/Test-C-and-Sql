using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    public class Program
    {

        private static decimal averageScore;
        private static int theNumberStudentsTeacher;

        public static void Main()
        {
            var lessonMap = new Dictionary<string, LessonType>
            {
                { "Математика", LessonType.Mathematics },
                { "Физика", LessonType.Physics }
            };

            var teachers = ReadPeopleFromFile<Teacher>("Учителя.txt", lessonMap); 
            var students = ReadPeopleFromFile<Student>("Ученики.txt", lessonMap); 
            var exams = ReadExamsFromFile("Экзамены.txt", lessonMap, teachers, students);

            PrintTeachers(teachers);
            PrintStudents(students);
            PrintExams(exams);
            SearchTeachersParameters(exams, teachers);
            PrintAverageScoreAndTheNumberStudents(averageScore, theNumberStudentsTeacher);

            Console.ReadKey();

            //1. Исправить ошибки если есть
            //2. Найти учителя у которого в классе меньше всего учеников
            //3. Найти средний бал экзамена по Физике за 2023г.
            //4. Получить количество учеников которые по экзамену Математики получили больше 90 баллов, где учитель Alex
            //5. Найти учителя который второй по количеству учеников
        }

        
        private static void SearchTeachersParameters(List<Exams> exams, List<Teacher> teachers)
        {
            var studentCountsByTeacher = exams
                .GroupBy(e => e.TeacherId)
                .Select(group => new { TeacherId = group.Key, StudentCount = group.Select(e => e.StudentId).Distinct().Count() })
                .OrderBy(result => result.StudentCount)
                .ToList();

            if (studentCountsByTeacher.Count > 1)
            {
                var teacherWithFewestStudents = teachers.FirstOrDefault(t => t.ID == studentCountsByTeacher.First().TeacherId);
                var teacherWithSecondFewestStudents = teachers.FirstOrDefault(t => t.ID == studentCountsByTeacher.Skip(1).First().TeacherId);

                if (teacherWithFewestStudents != null && teacherWithSecondFewestStudents != null)
                {
                    // 2. Найти учителя у которого в классе меньше всего учеников
                    Console.WriteLine($"Учитель с наименьшим количеством учеников: {teacherWithFewestStudents.Name} {teacherWithFewestStudents.LastName}, Количество учеников: {studentCountsByTeacher.First().StudentCount}");
                    // 5. Найти учителя который второй по количеству учеников
                    Console.WriteLine($"Учитель, занимающий второе место по количеству учеников: {teacherWithSecondFewestStudents.Name} {teacherWithSecondFewestStudents.LastName}, Количество учеников: {studentCountsByTeacher.Skip(1).First().StudentCount}");
                }
                else
                {
                    Console.WriteLine("Один из учителей не найден.");
                }
            }
            else if (studentCountsByTeacher.Count == 1)
            {
                var teacherWithFewestStudents = teachers.FirstOrDefault(t => t.ID == studentCountsByTeacher.First().TeacherId);
                if (teacherWithFewestStudents != null)
                {
                    Console.WriteLine($"Учитель с наименьшим количеством учеников: {teacherWithFewestStudents.Name} {teacherWithFewestStudents.LastName}, Количество учеников: {studentCountsByTeacher.First().StudentCount}");
                }
                else
                {
                    Console.WriteLine("Учитель с наименьшим количеством учеников не найден.");
                }
            }
            else
            {
                Console.WriteLine("Невозможно определить учителя с наименьшим количеством учеников.");
            }
            Console.WriteLine();
        }

        // Чтение данных из файлов
        private static List<T> ReadPeopleFromFile<T>(string filePath, Dictionary<string, LessonType> lessonMap) where T : Person, new()
        {
            var people = new List<T>();
            int idCounter = 1;
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string headerLine = reader.ReadLine();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (typeof(T) == typeof(Teacher) && parts.Length == 4)
                        {
                            LessonType lessonType;
                            if (!lessonMap.TryGetValue(parts[3], out lessonType))
                            {
                                Console.WriteLine($"Неизвестный урок: {parts[3]}");
                                continue;
                            }
                            people.Add(new Teacher
                            {
                                ID = idCounter++,
                                Name = parts[0],
                                LastName = parts[1],
                                Age = int.Parse(parts[2]),
                                Lesson = lessonType
                            } as T);
                        }
                        else if (typeof(T) == typeof(Student) && parts.Length == 3)
                        {
                            people.Add(new Student
                            {
                                ID = idCounter++,
                                Name = parts[0],
                                LastName = parts[1],
                                Age = int.Parse(parts[2])
                            } as T);
                        }
                        else
                        {
                            Console.WriteLine($"Неверный формат строки: {line}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла {filePath}: {ex.Message}");
            }

            return people;
        }

        
        private static List<Exams> ReadExamsFromFile(string filePath, Dictionary<string, LessonType> lessonMap, List<Teacher> teachers, List<Student> students)
        {
            var exams = new List<Exams>();
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string headerLine = reader.ReadLine();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 5)
                        {
                            LessonType lessonType;
                            if (!lessonMap.TryGetValue(parts[2], out lessonType))
                            {
                                Console.WriteLine($"Неизвестный урок: {parts[2]}");
                                continue;
                            }

                            Exams exam = new Exams
                            {
                                StudentId = long.Parse(parts[0]),
                                TeacherId = long.Parse(parts[1]),
                                Lesson = lessonType,
                                Score = decimal.Parse(parts[3]),
                                ExamDate = DateTime.Parse(parts[4]),
                                Student = students.FirstOrDefault(s => s.ID == long.Parse(parts[0])),
                                Teacher = teachers.FirstOrDefault(t => t.ID == long.Parse(parts[1]))
                            };
                            exams.Add(exam);
                        }
                        else
                        {
                            Console.WriteLine($"Неверный формат строки: {line}");
                        }
                    }
                }
                // 3. Найти средний балл экзамена по Физике за 2023г.
                averageScore = exams.Any() ? exams.Average(e => e.Score) : 0;

                // 4. Получить количество учеников, получивших более 90 баллов по экзамену Математики с учителем Alex
                theNumberStudentsTeacher = exams.Count(e => e.Teacher.Name == "Alex" && e.Lesson == LessonType.Mathematics && e.Score > 90);
                

                return exams;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла {filePath}: {ex.Message}");
            }
            
            return exams;
        }

        
        // Вывод данных
        private static void PrintAverageScoreAndTheNumberStudents(decimal averageScore, int theNumberStudentsTeacher)
        {
            Console.WriteLine($"Средний балл экзамена по Физике за 2023 год: {averageScore.ToString("0.00")}");

            Console.WriteLine($"Количество учеников, которые по экзамену по математике получили больше 90 баллов, где учитель Alex: {theNumberStudentsTeacher}");
        }

        private static void PrintTeachers(List<Teacher> teachers)
        {
            Console.WriteLine("Список учителей:");
            Console.WriteLine("ID\tИмя\tФамилия\tВозраст\tПредмет");
            foreach (var teacher in teachers)
            {
                string lessonName = Enum.GetName(typeof(LessonType), teacher.Lesson);
                Console.WriteLine($"{teacher.ID}\t{teacher.Name}\t{teacher.LastName}\t{teacher.Age}\t{lessonName}");
            }
            Console.WriteLine();
        }

        private static void PrintStudents(List<Student> students)
        {
            Console.WriteLine("Список учеников:");
            Console.WriteLine("ID\tИмя\tФамилия\tВозраст");
            foreach (var student in students)
            {
                Console.WriteLine($"{student.ID}\t{student.Name}\t{student.LastName}\t{student.Age}");
            }
            Console.WriteLine();
        }

        private static void PrintExams(List<Exams> exams)
        {
            Console.WriteLine("Список экзаменов:");
            Console.WriteLine("ID ученика\tID учителя\t\tПредмет\t\t\tОценка\t\tДата экзамена");
            foreach (var exam in exams)
            {
                string lessonName = Enum.GetName(typeof(LessonType), exam.Lesson);
                Console.WriteLine($"{exam.StudentId}\t\t{exam.TeacherId}\t\t\t{lessonName}\t\t{exam.Score}\t\t{exam.ExamDate:yyyy-MM-dd}");
            }
            Console.WriteLine();
        } 
    }
}
