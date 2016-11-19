using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniWatch.DataAccess;
using UniWatch.ViewModels;

namespace UniWatch.Controllers
{
    public class StudentController : Controller
    {
        private IDataAccess _dataAccess;
        
        public StudentController()
        {
            _dataAccess = new DataAccess.DataAccess();
        } 

        // GET: Student
        public ActionResult Index(int classId)
        {
            var students = _dataAccess.ClassManager.GetEnrolledStudents(classId);
            return View(students);
        }

        // GET: Unenroll
        [HttpGet]
        public ActionResult UnEnroll(int classId, int studentId)
        {
            var @class = _dataAccess.ClassManager.GetById(classId);
            var student = _dataAccess.StudentManager.GetById(studentId);

            return View(new UnEnrollViewModel() { Class = @class, Student = student });
        }

        // GET: Enroll
        [HttpGet]
        public ActionResult Enroll(int classId)
        {
            var @class = _dataAccess.ClassManager.GetById(classId);
            //var EnrollStudent = _classManager.EnrollStudent(classId, studentId);
            return View(new EnrollViewModel() { Class = @class });
        }

        // GET: Get
        [HttpGet]
        public ActionResult Get(int studentId)
        {
            var student = _dataAccess.StudentManager.GetById(studentId);
            return RedirectToAction("Index", new { studentId = studentId });
        }

        // POST: Unenroll
        [HttpPost, ActionName("UnEnroll")]
        [ValidateAntiForgeryToken]
        public ActionResult UnEnrollCofirmed(int classId, int studentId)
        {
            _dataAccess.ClassManager.UnEnrollStudent(classId, studentId);
            return RedirectToAction("Index", new { classId = classId });
        }

        //POST: Enroll
        [HttpPost, ActionName("Enroll")]
        [ValidateAntiForgeryToken]
        public ActionResult EnrollConfirmed(int classId, int studentId)
        {
            _dataAccess.ClassManager.EnrollStudent(classId, studentId);
            return RedirectToAction("Index", new { classId = classId });
        }
    }
} 