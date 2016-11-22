using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using UniWatch.DataAccess;
using UniWatch.Models;
using UniWatch.ViewModels.Lecture;

namespace UniWatch.Controllers
{
    public class LectureController : Controller
    {
        #region Fields and Properties

        private readonly IDataAccess _manager;

        #endregion

        #region Constructors

        public LectureController() : this(new AppDbContext())
        {
        }

        public LectureController(AppDbContext context)
        {
            _manager = new DataAccess.DataAccess(context);
        }

        #endregion

        #region Index

        /// <summary>
        /// Display all the recorded lectures for the class
        /// </summary>
        /// <param name="classId">The class ID</param>
        /// <returns>The index view</returns>
        [HttpGet]
        public ActionResult Index(int classId)
        {
            var report = GetReportViewModel(classId);

            // If the class does not exist,
            // then display an error
            if(report == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No class with id {classId}");
            }

            var updates = new List<UpdateLectureItem>
            {
                new UpdateLectureItem { StudentId = 1, LectureId = 1, Present = true }
            };
            _manager.LectureManager.Update(1, updates);

            return View(report);
        }

        #endregion

        #region Override

        /// <summary>
        /// Display the student attendance for a specific lecture
        /// </summary>
        /// <param name="lectureId">The lecture ID</param>
        /// <returns>The override view</returns>
        [HttpGet]
        public ActionResult Override(int lectureId)
        {
            var lvm = GetLectureViewModel(lectureId);

            // If the lecture does not exist,
            // then display an error
            if (lvm == null)
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
        public ActionResult Override(LectureViewModel lvm)
        {
            var lecture = _manager.LectureManager.Get(lvm.LectureId);

            if (lecture == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No lecture with id {lvm.LectureId}");
            }

            foreach (var attendance in lecture.Attendance)
            {
                bool? present = lvm.LectureItems
                    .FirstOrDefault(item => attendance.Student.Id == item.StudentId)?.Present ?? null;

                if (present.HasValue)
                {
                    attendance.Present = present.Value;
                }
            }

            // Needed to do something with the lecture class to get the details properly
            ViewBag.ClassName = lecture.Class.Name;


            _manager.LectureManager.Update(lecture);

            return RedirectToAction("Override", "Lecture", new { lectureId = lecture.Id });
        }

        #endregion

        #region Create

        [HttpGet]
        public ActionResult Create(int classId)
        {
            var @class = _manager.ClassManager.GetById(classId);

            if(@class == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No class with id {classId}");
            }

            var lecture = new Lecture() { Class = @class };

            return View(lecture);
        }

        [HttpPost]
        public ActionResult Create(Lecture lecture)
        {
            if(lecture.Class == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "No class id");
            }

            if(Request.Files.Count == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "No files selected");
            }

            var fileStreams = new List<Stream>(Request.Files.Count);

            foreach(HttpPostedFileBase file in Request.Files)
            {
                if(file.ContentLength == 0 /*&& file.ContentLength >= MAX_SIZE*/)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Selected file {file.FileName} is out of bounds");
                }

                fileStreams.Add(file.InputStream);
            }

            _manager.LectureManager.RecordLecture(lecture.Class.Id, fileStreams);
            // TODO: while the system is creating the lecture and taking attendance, display a load screen

            return RedirectToAction("Index", "Lecture", new { classId = lecture.Class.Id });
        }

        #endregion

        #region Delete

        [HttpGet]
        public ActionResult Delete(int lectureId)
        {
            var lvm = GetLectureViewModel(lectureId);

            if (lvm == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No lecture with id {lectureId}");
            }

            return View(lvm);
        }

        [HttpPost]
        public ActionResult Delete(LectureViewModel lvm)
        {
            if (lvm == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No lecture given");
            }

            var lecture = _manager.LectureManager.Delete(lvm.LectureId);

            return RedirectToAction("Index", new { classId = lecture.Class.Id });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get a report for the class ID
        /// </summary>
        /// <param name="classId">The ID of the class</param>
        /// <returns>All the lectures, students, and attendance for a class</returns>
        private ReportViewModel GetReportViewModel(int classId)
        {
            ReportViewModel report = null;
            var @class = _manager.ClassManager.GetById(classId);

            // If a class was found,
            // then add the other required information
            if(@class != null)
            {
                report = new ReportViewModel()
                {
                    ClassId = @class.Id,
                    ClassName = @class.Name,
                    Lectures = new List<Lecture>(@class.Lectures),
                    Statuses = new List<AttendanceStatus>(@class.Enrollment.Count)
                };

                foreach (var lecture in @class.Lectures)
                {
                    foreach (var attendance in lecture.Attendance)
                    {
                        if (report.Statuses.All(s => s.Student.Id != attendance.Student.Id))
                        {
                            report.Statuses.Add(new AttendanceStatus
                            {
                                Student = attendance.Student,
                                Attendance = new Dictionary<int, bool>(@class.Lectures.Count)
                            });
                        }

                        var status = report.Statuses.FirstOrDefault(s => s.Student.Id == attendance.Student.Id);
                        if (status != null)
                        {
                            status.Attendance[lecture.Id] = attendance.Present;
                        }
                    }
                }
            }

            return report;
        }

        /// <summary>
        /// Get the lecture view model for a lecture
        /// </summary>
        /// <param name="lectureId">The id for the lecture</param>
        /// <returns>The lecture view model (or null if no lecture was found)</returns>
        private LectureViewModel GetLectureViewModel(int lectureId)
        {
            LectureViewModel lvm = null;
            var lecture = _manager.LectureManager.Get(lectureId);

            if(lecture != null)
            {
                lvm = new LectureViewModel()
                {
                    ClassId = lecture.Class.Id,
                    LectureId = lecture.Id,
                    LectureDate = lecture.RecordDate,
                    LectureItems = new List<UpdateLectureItem>(lecture.Attendance.Count)
                };

                foreach (var a in lecture.Attendance)
                {
                    lvm.LectureItems.Add(
                        new UpdateLectureItem
                        {
                            StudentId = a.Student.Id,
                            StudentName = $"{a.Student.FirstName} {a.Student.LastName}",
                            Present = a.Present
                        }
                    );
                }
            }

            return lvm;
        }

        #endregion
    }
}