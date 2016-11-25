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
            // Initialize the DataAccess object. DataAccess allows you to access the class manager and student manager 
            // without the need to directly access the database.
            _dataAccess = new DataAccess.DataAccess();
        } 

        // GET: Student
        //The Index function takes a classId as a parameter
        [HttpGet]
        public ActionResult Index(int classId)
        {
            //The classI
            var students = _dataAccess.ClassManager.GetEnrolledStudents(classId);
            return View(students);
        }

        // GET: Unenroll
        [HttpGet]
        public ActionResult UnEnroll(int classId, int studentId)
        {
            var @class = _dataAccess.ClassManager.GetById(classId);
            var student = _dataAccess.UserManager.GetStudentById(studentId);

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

        [HttpGet]
        public ActionResult Enroll(int classId, String studentId)
        {
            var @class = _dataAccess.ClassManager.GetById(classId);
            var EnrollViewModel = new EnrollViewModel()
            {
                //Get Class
            };

            var result = _dataAccess.UserManager.SearchStudent(studentId);
            var enrolled = _dataAccess.ClassManager.EnrollStudent(classId, studentId);
            var eSet = new HashSet<Student>(enrolled);
            foreach (Student student in result)
            {
                EnrollViewModel.StudentsFound.Add(new StudentFound())
                {
                    Student = student
                    Enrolled = eSetContains(student);
                }
            }

            //var student = _dataAccess.UserManager.GetStudentById(studentId);
            //var EnrollStudent = _classManager.EnrollStudent(classId, studentId);
            return View(EnrollViewModel());
        }

        // GET: Get
        [HttpGet]
        public ActionResult Get(int studentId)
        {
            var student = _dataAccess.UserManager.GetStudentById(studentId);
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