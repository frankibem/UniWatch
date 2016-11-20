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

        public ReportViewModel(int classId, IDataAccess manager)
        {
            // Get the class associated with the given class id
            Class = manager.ClassManager.GetById(classId);

            // If a class was found,
            // then add the other required information
            if (Class != null)
            {
                Students = manager.ClassManager.GetEnrolledStudents(classId).ToList();
                Lectures = manager.LectureManager.GetTeacherReport(classId)
                    .OrderByDescending(lecture => lecture.RecordDate).ToList();
                Attendance = new Dictionary<int, ICollection<StudentAttendance>>();

                // Add each lecture attendance to the attendance dictionary
                Lectures.ForEach(lecture => Attendance[lecture.Id] = lecture.Attendance);
            }
        }

        public ReportViewModel(int classId) : this(classId, new DataAccess.DataAccess())
        {
            
        }
    }
}