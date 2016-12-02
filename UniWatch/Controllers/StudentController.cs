using System;
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
    public class StudentController : Controller
    {
        private IDataAccess _dataAccess;

        public StudentController() : this(new DataAccess.DataAccess())
        { }

        public StudentController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        // GET: /Student/Index?classId=1
        /// <summary>
        /// Display a list of students in a class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        [HttpGet]
        public ActionResult Index(int classId)
        {
            var @class = _dataAccess.ClassManager.GetById(classId);
            if(@class == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var students = _dataAccess.ClassManager.GetEnrolledStudents(classId);
            return View(new ViewModels.EnrollIndexViewModel
            {
                Class = @class,
                EnrolledStudents = students
            });
        }

        /// <summary>
        /// Returns the view to enroll a student in a class. 
        /// </summary>
        /// <param name="classId">The id of the class</param>
        [HttpGet]
        public ActionResult Enroll(int classId)
        {
            var @class = _dataAccess.ClassManager.GetById(classId);
            if(@class == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EnrollStudentViewModel viewModel = new EnrollStudentViewModel
            {
                Class = @class,
                StudentsFound = null
            };

            return View(viewModel);
        }

        /// <summary>
        /// Returns the view to enroll a student in a class. Searches for students
        /// that match the given id
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <param name="searchTerm">The student search parameter</param>
        [HttpPost]
        public ActionResult Search(int classId, string searchTerm)
        {
            var @class = _dataAccess.ClassManager.GetById(classId);
            if(@class == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var found = _dataAccess.UserManager.SearchStudent(searchTerm);
            var enrolled = _dataAccess.ClassManager.GetEnrolledStudents(classId);

            // Set of student ids (for enrolled students)
            HashSet<int> enrolledSet = new HashSet<int>();
            foreach(var student in enrolled)
                enrolledSet.Add(student.Id);

            EnrollStudentViewModel viewModel = new EnrollStudentViewModel
            {
                Class = @class
            };

            foreach(Student student in found)
            {
                viewModel.StudentsFound.Add(new StudentFound
                {
                    Student = student,
                    Enrolled = enrolledSet.Contains(student.Id)
                });
            }

            return View("Enroll", viewModel);
        }

        // POST: /Student/Enroll?classId=1&studentId=1
        /// <summary>
        /// Enrolls a student into a class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <param name="studentId">The id of the student to enroll</param>
        [HttpPost]
        [ActionName("Enroll")]
        [ValidateAntiForgeryToken]
        public ActionResult EnrollConfirmed(int classId, int studentId)
        {
            var @class = _dataAccess.ClassManager.GetById(classId);
            var student = _dataAccess.UserManager.GetStudentById(studentId);
            if(@class == null || student == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                _dataAccess.ClassManager.EnrollStudent(classId, studentId);
            }
            catch(InvalidOperationException)
            {
                // Student is already enrolled in that class.
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }

            return RedirectToAction("Index", new { classId = classId });
        }

        // GET: /Student/Unenroll?classId=1&studentId=1
        /// <summary>
        /// Returns the view to unenroll a student from a class 
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <param name="studentId">The id of the student</param>
        [HttpGet]
        public ActionResult UnEnroll(int classId, int studentId)
        {
            var @class = _dataAccess.ClassManager.GetById(classId);
            var student = _dataAccess.UserManager.GetStudentById(studentId);

            if(@class == null || student == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(new UnEnrollViewModel() { Class = @class, Student = student });
        }

        // POST: Unenroll
        /// <summary>
        /// Unenrolls a student from a class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <param name="studentId">The id of the student to enroll</param>
        [HttpPost]
        [ActionName("UnEnroll")]
        [ValidateAntiForgeryToken]
        public ActionResult UnEnrollCofirmed(int classId, int studentId)
        {
            var @class = _dataAccess.ClassManager.GetById(classId);
            var student = _dataAccess.UserManager.GetStudentById(studentId);
            if(@class == null || student == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                _dataAccess.ClassManager.UnEnrollStudent(classId, studentId);
            }
            catch(InvalidOperationException)
            {
                // Student is not enrolled in that class.
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return RedirectToAction("Index", new { classId = classId });
        }

        /// <summary>
        /// Returns the view to search for a student (for uploading profile images)
        /// </summary>
        /// <param name="searchTerm">The parameter used to search for a student</param>
        [HttpGet]
        [ActionName("Profile")]
        public ActionResult StudentProfile(string searchTerm)
        {
            var students = _dataAccess.UserManager.SearchStudent(searchTerm);

            if(Request.IsAjaxRequest())
            {
                return PartialView("_ProfileSearch", students);
            }

            return View(students);
        }

        /// <summary>
        /// Displays a page to upload images of the student's face
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <param name="studentId">The id of the student</param>
        [HttpGet]
        public ActionResult Upload(int classId, int studentId)
        {
            var student = _dataAccess.UserManager.GetStudentById(studentId);
            if(student == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.ClassId = classId;

            return View(student);
        }

        /// <summary>
        /// Upload images of the student's face
        /// </summary>
        /// <param name="studentId">The id of the student</param>
        /// <param name="files">The images to upload</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(int studentId, IEnumerable<HttpPostedFileBase> files)
        {
            var student = _dataAccess.UserManager.GetStudentById(studentId);
            if(student == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if(!files.Any() || (files.Count() == 1 && files.ElementAt(0) == null))
            {
                ViewBag.ErrorMessage = "No file selected";
                return View(student);
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
                    return View(student);
                }
                else if(!validImageTypes.Contains(file.ContentType))
                {
                    ViewBag.ErrorMessage = "File type must be either gif, jpeg or png";
                    return View(student);
                }

                images.Add(file.InputStream);
            }

            _dataAccess.UserManager.SetStudentProfile(student.Id, images);
            return RedirectToAction("Upload", new { studentId = student.Id });
        }
    }
}