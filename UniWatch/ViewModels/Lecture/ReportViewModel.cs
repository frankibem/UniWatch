using System.Collections.Generic;
using System.Linq;
using UniWatch.DataAccess;
using UniWatch.Models;
using WebGrease.Css.Extensions;

namespace UniWatch.ViewModels.Lecture
{
    public class ReportViewModel
    {
        public IEnumerable<Student> Students { get; set; }
        public Dictionary<int, ICollection<StudentAttendance>> Attendance { get; set; }
        public IEnumerable<Models.Lecture> Lectures { get; set; }
        public Class Class { get; set; }
    }
}