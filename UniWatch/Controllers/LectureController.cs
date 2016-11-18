using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UniWatch.DataAccess;
using UniWatch.Models;

namespace UniWatch.Controllers
{
    public class LectureController : Controller
    {
        private readonly DataAccess.DataAccess _manager = new DataAccess.DataAccess();

        /// <summary>
        /// Display all the recorded lectures for the class
        /// </summary>
        /// <returns>The index view</returns>
        //[Authorize]
        public ActionResult Index(int classId)
        {
            var lectures = _manager.LectureManager.GetTeacherReport(classId);

            if (lectures == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No class with id {classId}");
            }

            ViewBag.Title = _manager.ClassManager.GetById(classId).Name;
            ViewBag.Students = _manager.ClassManager.GetEnrolledStudents(classId).ToList();

            return View(lectures);
        }

        /// <summary>
        /// Display the blank form to create a lecture
        /// </summary>
        /// <returns>The create view</returns>
        //[Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //[Authorize]
        [HttpPost]
        public ActionResult Create(Lecture lecture)
        {
            return View();
        }

        //[Authorize]
        [HttpPost]
        public ActionResult Delete(int lectureId)
        {
            return View();
        }

        //[Authorize]
        public ActionResult Override(int lectureId)
        {
            return View();
        }

        //[Authorize]
        [HttpPost]
        public ActionResult Override(int lectureId, int studentId)
        {
            return View();
        }
    }
}