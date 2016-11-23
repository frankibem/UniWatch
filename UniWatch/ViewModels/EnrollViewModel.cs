using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniWatch.Models;

namespace UniWatch.ViewModels
{
    public class EnrollViewModel
    {
 
        public Class Class { get; set; }
        public List<StudentFound> StudentsFound { get; set; }

        //Default Constructor for EnrollViewModel
        public EnrollViewModel()
        {
            StudentsFound = new List<StudentFound>();
        }
    }

    public class StudentFound
    {
        public Student Student;
        public bool Enrolled;
    }
}
