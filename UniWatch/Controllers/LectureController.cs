﻿using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IDataAccess _manager;

        public LectureController() : this(new AppDbContext())
        {
        }

        public LectureController(AppDbContext context)
        {
            _manager = new DataAccess.DataAccess(context);
        }

        /// <summary>
        /// Display all the recorded lectures for the class
        /// </summary>
        /// <returns>The index view</returns>
        //[Authorize]
        public ActionResult Index(int classId)
        {
            // Get all the lectures for the class
            var lectures = _manager.LectureManager.GetTeacherReport(classId);

            // If there are no lectures for the class or the class does not exist,
            // then display an error
            if (!lectures?.Any() ?? false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No class with id {classId}");
            }

            // Put the class name and students into the ViewBag
            // TODO: create a ViewModel for this later
            ViewBag.Title = _manager.ClassManager.GetById(classId).Name;
            ViewBag.Students = _manager.ClassManager.GetEnrolledStudents(classId).ToList();

            return View(lectures);
        }

        /// <summary>
        /// Display the blank form to create a lecture
        /// </summary>
        /// <returns>The create view</returns>
        //[Authorize]
        public ActionResult Create(int classId)
        {
            // TODO: Create a lecture for the class given, upload pictures
            var lecture = new Lecture()
            {
                Class = _manager.ClassManager.GetById(classId),
                RecordDate = DateTime.Today
            };

            return View(lecture);
        }

        //[Authorize]
        [HttpPost]
        public ActionResult Create(Lecture lecture)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0 /*&& file.ContentLength < MAX LENGTH*/)
                {
                    //var fileName = Path.GetFileName(file.FileName);
                    //var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    //file.SaveAs(path);

                    // TODO: Save images in blob, create images, create lecture in db

                    return RedirectToAction("Index", "Lecture", new { lecture.Class.Id });
                }
            }

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
            var lecture = _manager.LectureManager.Get(lectureId);

            if (lecture == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No lecture with id {lectureId}");
            }

            ViewBag.LectureId = lectureId;
            ViewBag.LectureDate = lecture.RecordDate.ToShortDateString();
            ViewBag.Students = _manager.ClassManager.GetEnrolledStudents(lecture.Class.Id).ToList();

            return View(lecture.Attendance);
        }

        //[Authorize]
        [HttpPost]
        public ActionResult Override(int lectureId, int studentId)
        {
            return View();
        }
    }
}