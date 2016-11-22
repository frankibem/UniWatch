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
            var lecture = GetLectureViewModel(lectureId);

            // If the lecture does not exist,
            // then display an error
            if(lecture.Lecture == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No lecture with id {lectureId}");
            }

            return View(lecture);
        }

        /// <summary>
        /// Updates the student attendance for a lecture
        /// </summary>
        /// <param name="model">The updated lecture view model</param>
        /// <returns>The updated override view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Override(LectureViewModel model)
        {
            //var lecture = _manager.LectureManager.Get(model.Lecture.Id);

            //if (lecture == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No lecture with id {model.Lecture.Id}");
            //}

            //foreach (var student in model.Students)
            //{
            //    lecture.Attendance.First(a => a.Student.Id == student.Id).Present =
            //        model.Lecture.Attendance.First(a => a.Student.Id == student.Id).Present;
            //}

            //_manager.LectureManager.Update(lecture);

            //return RedirectToAction("Override", new { lectureId = lecture.Id });

            // TODO: Delete this line
            return View("Index", 0);
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

            if(lvm.Lecture == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No lecture with id {lectureId}");
            }

            return View(lvm);
        }

        [HttpPost]
        public ActionResult Delete(LectureViewModel lvm)
        {
            if(lvm.Lecture == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No lecture given");
            }

            var lecture = _manager.LectureManager.Delete(lvm.Lecture.Id);

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

            // Get the class associated with the given class id
            var @class = _manager.ClassManager.GetById(classId);

            // If a class was found,
            // then add the other required information
            if(@class != null)
            {
                report = new ReportViewModel
                {
                    Students = _manager.ClassManager.GetEnrolledStudents(classId).ToList(),
                    Lectures = _manager.LectureManager.GetTeacherReport(classId)
                        .OrderByDescending(lecture => lecture.RecordDate).ToList(),
                    Attendance = new Dictionary<int, ICollection<StudentAttendance>>()
                };

                // Add each lecture attendance to the attendance dictionary
                report.Lectures.ForEach(lecture => report.Attendance[lecture.Id] = lecture.Attendance);
            }

            return report;
        }

        private LectureViewModel GetLectureViewModel(int lectureId)
        {
            LectureViewModel lvm = null;
            var lecture = _manager.LectureManager.Get(lectureId);

            if(lecture != null)
            {
                lvm = new LectureViewModel
                {
                    Lecture = lecture,
                    Students = new List<Student>(lecture.Attendance.Count)
                };

                lecture.Attendance.ForEach(a => lvm.Students.Add(a.Student));
            }
            return lvm;
        }

        #endregion
    }
}