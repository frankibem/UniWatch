using System.Collections.Generic;
using UniWatch.Models;

namespace UniWatch.ViewModels.Lecture
{
    public class ReportViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public List<Models.Lecture> Lectures { get; set; }
        public List<AttendanceStatus> Statuses { get; set; }

        public ReportViewModel()
        {
            Lectures = new List<Models.Lecture>();
            Statuses = new List<AttendanceStatus>();
        }
    }

    public class AttendanceStatus
    {
        public Student Student { get; set; }
        public Dictionary<int, bool> Attendance { get; set; }

        public AttendanceStatus()
        {
            Attendance = new Dictionary<int, bool>();
        }
    }
}