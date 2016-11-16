using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniWatch.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult Index(int classId)
        {
            return View();
        }

        // GET: Unenroll
        public ActionResult Unenroll(int classId, int studentId)
        {
            return View();
        }

        // GET: Enroll
        public ActionResult Enroll(int classId)
        {
            return View();
        }

        // GET: Get
        public ActionResult Get(int studentId)
        {
            return View();
        }

        // POST: Unenroll
        public ActionResult Unenroll()
        {
            return View();
        }

        //POST: Enroll
        public ActionResult Enroll()
        {
            return View();
        }
    }
} 