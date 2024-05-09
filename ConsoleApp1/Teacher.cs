using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp1.Program;

namespace ConsoleApp1
{
    public class Teacher : Person
    {
        public LessonType Lesson { get; set; }
    }
}
