using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UniWatch.DataAccess;
using UniWatch.Models;
using UniWatch.ViewModels;

namespace UniWatch.Controllers
{
    public class LectureController : Controller
    {
        private readonly IDataAccess _manager;

        public LectureController() : this(new AppDbContext())
        { }

        public LectureController(AppDbContext context)
        {
            _manager = new DataAccess.DataAccess(context);
        }

        /// <summary>
        /// Display all the recorded lectures for the class
        /// </summary>
        /// <param name="classId">The class ID</param>
        [HttpGet]
        public ActionResult Index(int classId)
        {
            var report = GetTeacherReport(classId);
            if(report == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No class with id {classId}");
            }

            return View(report);
        }

        /// <summary>
        /// Display the student attendance for a specific lecture
        /// </summary>
        /// <param name="lectureId">The lecture ID</param>
        /// <returns>The override view</returns>
        [HttpGet]
        public ActionResult Override(int lectureId)
        {
            var lvm = GetUpdateViewModel(lectureId);

            // If the lecture does not exist,
            // then display an error
            if(lvm == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No lecture with id {lectureId}");
            }

            return View(lvm);
        }

        /// <summary>
        /// Updates the student attendance for a lecture
        /// </summary>
        /// <param name="lvm">The updated lecture view model</param>
        /// <returns>The updated override view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Override(UpdateLectureViewModel lvm)
        {
            var lecture = _manager.LectureManager.Get(lvm.Lecture.Id);

            if(lecture == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No lecture with id {lvm.Lecture.Id}");
            }

            var updates = new List<UpdateLectureItem>(lecture.Attendance.Count);

            updates.AddRange(from attendance in lecture.Attendance
                             let present = lvm.LectureItems
                                .FirstOrDefault(item => attendance.Student.Id == item.StudentId)?.Present ?? null
                             where present.HasValue && attendance.Present != present.Value
                             select new UpdateLectureItem
                             {
                                 StudentId = attendance.Student.Id,
                                 LectureId = lecture.Id,
                                 Present = present.Value
                             }
            );

            _manager.LectureManager.Update(lecture.Id, updates);

            return RedirectToAction("Override", "Lecture", new { lectureId = lecture.Id });
        }

        [HttpGet]
        public ActionResult Create(int classId)
        {
            var @class = _manager.ClassManager.GetById(classId);

            if(@class == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No class with id {classId}");
            }

            return View(@class);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int classId, IEnumerable<HttpPostedFileBase> files)
        {
            var @class = _manager.ClassManager.GetById(classId);
            if(@class == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No class with id {classId}");
            }

            if(files.Count() == 0)
            {
                ViewBag.ErrorMessage = "No file selected";
                return View(@class);
            }

            var validImageTypes = new string[]
            {
                "image/gif",
                "image/jpeg",
                "image/pjpeg",
                "image/png",
                "image/bmp"
            };

            var images = new List<Stream>(files.Count());
            foreach(var file in files)
            {
                // TODO: Determine upload limit
                if(file.ContentLength <= 0 /*|| file.ContentLength >= MAX_SIZE*/)
                {
                    ViewBag.ErrorMessage = "File must not be empty and must not exceed MAX_SIZE";
                    return View(@class);
                }
                else if(!validImageTypes.Contains(file.ContentType))
                {
                    ViewBag.ErrorMessage = "File type must be either gif, jpeg or png";
                    return View(@class);
                }

                images.Add(file.InputStream);
            }

            // TODO: Uncomment when services are functional
            //_manager.LectureManager.RecordLecture(@class.Id, images);
            return RedirectToAction("Index", "Lecture", new { classId = @class.Id });
        }

        [HttpGet]
        public ActionResult Delete(int lectureId)
        {
            var lecture = _manager.LectureManager.Get(lectureId);

            if(lecture == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No lecture with id {lectureId}");
            }

            return View(lecture);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(int lectureId, int classId)
        {
            var lecture = _manager.LectureManager.Delete(lectureId);

            if(lecture == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No lecture given");
            }

            return RedirectToAction("Index", new { classId = classId });
        }

        /// <summary>
        /// Get a report for the given class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <returns>All the lectures, students, and attendance for a class</returns>
        private TeacherReportViewModel GetTeacherReport(int classId)
        {
            TeacherReportViewModel report = null;
            var @class = _manager.ClassManager.GetById(classId);

            if(@class != null)
            {
                report = new TeacherReportViewModel()
                {
                    ClassId = @class.Id,
                    ClassName = @class.Name,
                    Lectures = new List<Lecture>(@class.Lectures),
                    Statuses = new List<AttendanceStatus>(@class.Enrollment.Count)
                };

                // statusMap[studentId] -> AttendanceStatus
                var statusMap = new Dictionary<int, AttendanceStatus>(@class.Enrollment.Count);
                foreach(var enrollment in @class.Enrollment)
                {
                    // TODO: verify that Student object is included
                    var attStatus = new AttendanceStatus()
                    {
                        Student = enrollment.Student
                    };
                    report.Statuses.Add(attStatus);
                    statusMap.Add(enrollment.Student.Id, attStatus);
                }

                foreach(var lecture in @class.Lectures)
                {
                    foreach(var attendance in lecture.Attendance)
                    {
                        var attStatus = statusMap[attendance.Student.Id];
                        attStatus.Attendance.Add(lecture.Id, attendance.Present);
                    }
                }

                // Order by first name
                report.Statuses = report.Statuses.OrderBy(s => s.Student.FirstName).ToList();
            }

            return report;
        }

        /// <summary>
        /// Get the lecture view model for a lecture
        /// </summary>
        /// <param name="lectureId">The id for the lecture</param>
        /// <returns>The lecture view model (or null if no lecture was found)</returns>
        private UpdateLectureViewModel GetUpdateViewModel(int lectureId)
        {
            UpdateLectureViewModel lvm = null;
            var lecture = _manager.LectureManager.Get(lectureId);

            if(lecture != null)
            {
                lvm = new UpdateLectureViewModel()
                {
                    Class = lecture.Class,
                    Lecture = lecture,
                    LectureItems = new List<UpdateLectureItem>(lecture.Attendance.Count),
                };

                foreach(var a in lecture.Attendance)
                {
                    lvm.LectureItems.Add(
                        new UpdateLectureItem
                        {
                            StudentId = a.Student.Id,
                            StudentName = $"{a.Student.FirstName} {a.Student.LastName}",
                            LectureId = lecture.Id,
                            Present = a.Present
                        }
                    );
                }

                lvm.LectureItems = lvm.LectureItems.OrderBy(item => item.StudentName).ToList();
            }

            return lvm;
        }
    }
}