using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp1.Program;

namespace ConsoleApp1
{
    public class Exams
    {
        public LessonType Lesson { get; set; }

        public long StudentId { get; set; }
        public long TeacherId { get; set; }

        public decimal Score { get; set; }
        public DateTime ExamDate { get; set; }

        public Student Student { get; set; }
        public Teacher Teacher { get; set; }
    }

    public enum LessonType
    {
        Mathematics = 1,
        Physics = 2
    }
}
