using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniWatch.DataAccess;
using UniWatch.Models;

namespace UniWatch.ViewModels.Lecture
{
    public class LectureViewModel
    {
        public Models.Lecture Lecture { get; set; }
        public List<Student> Students { get; set; }

        public LectureViewModel(int lectureId, IDataAccess manager)
        {
            Lecture = manager.LectureManager.Get(lectureId);
            Students = new List<Student>(Lecture.Attendance.Count);

            if (Lecture != null)
            {
                foreach (var attendance in Lecture.Attendance)
                {
                    Students.Add(attendance.Student);
                }
            }
        }

        public LectureViewModel(int lectureId) : this(lectureId, new DataAccess.DataAccess())
        {
            
        }

        public LectureViewModel()
        {
            
        }
    }
}