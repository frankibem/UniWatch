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
    [Authorize(Roles = "Teacher")]
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
        [OverrideAuthorization]
        [Authorize(Roles = "Teacher, Student")]
        public ActionResult Index(int classId)
        {
            // TODO: modify view to show appropriate information for either user type
            var user = _manager.UserManager.GetUser();
            if(user is Teacher)
            {
                Teacher teacher = user as Teacher;
                var report = GetTeacherReport(classId);
                if(report == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                return View(report);
            }
            else
            {
                Student student = user as Student;
                return View(new TeacherReportViewModel());
            }
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            _manager.LectureManager.Update(lecture.Id, lvm.LectureItems);
            return RedirectToAction("Override", "Lecture", new { lectureId = lecture.Id });
        }

        /// <summary>
        /// Display the
        /// </summary>
        /// <param name="classId"></param>
        [HttpGet]
        public ActionResult Create(int classId)
        {
            var @class = _manager.ClassManager.GetById(classId);

            if(@class == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(@class);
        }

        /// <summary>
        /// Create a new lecture and upload the lecture attendance images
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <param name="files">The images to upload</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int classId, IEnumerable<HttpPostedFileBase> files)
        {
            var @class = _manager.ClassManager.GetById(classId);
            if(@class == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if(!files.Any() || (files.Count() == 0 && files.ElementAt(0) == null))
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

        /// <summary>
        /// Diplay some of the lecture information so the user
        /// knows which lecture they are about to delete
        /// </summary>
        /// <param name="lectureId">The id of the lecture to potentially delete</param>
        [HttpGet]
        public ActionResult Delete(int lectureId)
        {
            var lecture = _manager.LectureManager.Get(lectureId);

            if(lecture == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(lecture);
        }

        /// <summary>
        /// Confirm the deletion of the lecture
        /// </summary>
        /// <param name="lectureId">The id of the lecture to delete</param>
        /// <param name="classId">The id of the class the deleted lecture was associated with</param>
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(int lectureId, int classId)
        {
            var lecture = _manager.LectureManager.Delete(lectureId);

            if(lecture == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
                    Lectures = new List<Lecture>(@class.Lectures.OrderByDescending(lecture => lecture.RecordDate)),
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

                // Order statuses by first name
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