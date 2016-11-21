using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            if (report == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No class with id {classId}");
            }

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
            if (lecture.Lecture == null)
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
            var lecture = _manager.LectureManager.Get(model.Lecture.Id);

            if (lecture == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No lecture with id {model.Lecture.Id}");
            }

            foreach (var s in model.Students)
            {
                lecture.Attendance.First(a => a.Student.Id == s.Id).Present =
                    model.Lecture.Attendance.First(a => a.Student.Id == s.Id).Present;
            }

            _manager.LectureManager.Update(lecture);

            return RedirectToAction("Override", new { lectureId = lecture.Id });
        }

        #endregion

        #region Create

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Lecture lecture)
        {
            return View();
        }

        #endregion

        #region Delete

        [HttpGet]
        public ActionResult Delete(int lectureId)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(Lecture lecture)
        {
            return View();
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
            if (@class != null)
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

            if (lecture != null)
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