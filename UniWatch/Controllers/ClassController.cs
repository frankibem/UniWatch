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
    [Authorize(Roles = "Teacher")]
    public class ClassController : Controller
    {
        private IDataAccess _dataAccess;

        public ClassController() : this(new DataAccess.DataAccess())
        {
        }

        public ClassController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        //GET: Index
        /// <summary>
        /// Displays all classes taught by the teacher
        /// </summary>
        [OverrideAuthorization]
        [Authorize(Roles = "Teacher, Student")]
        public ActionResult Index()
        {
            // TODO: modify view to show appropriate information for either user type
            var user = _dataAccess.UserManager.GetUser();
            if(user is Teacher)
            {
                Teacher teacher = user as Teacher;
                return View("TeacherIndex", _dataAccess.ClassManager.GetClassesForTeacher(teacher.Id));
            }
            else
            {
                Student student = user as Student;
                return View("StudentIndex", _dataAccess.ClassManager.GetClassesForStudent(student.Id));
            }
        }

        // GET: Create
        /// <summary>
        /// Returns the view to create a new class
        /// </summary>
        public ActionResult Create()
        {
            return View();
        }

        //POST: Create
        /// <summary>
        /// Create class for the teacher
        /// </summary>
        /// <param name="class">Model containing information to create the class</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Number,Section,Semester,Year")] Class @class)
        {
            if(!ModelState.IsValid)
            {
                return View(@class);
            }
            try
            {
                var teacher = _dataAccess.UserManager.GetUser() as Teacher;
                var created = _dataAccess.ClassManager.CreateClass(@class.Name, @class.Number, @class.Section, @class.Semester, @class.Year, teacher.Id);
                return RedirectToAction("Index");
            }
            catch(InvalidOperationException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
        }

        //GET: Delete
        /// <summary>
        /// Sends user to the delete class page
        /// </summary>
        /// <param name="classId">Id of the class</param>
        public ActionResult Delete(int classId)
        {
            var @class = _dataAccess.ClassManager.GetById(classId);
            if(@class == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(@class);
        }

        //POST: Delete
        /// <summary>
        /// Deletes the class
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int classId)
        {
            var @class = _dataAccess.ClassManager.GetById(classId);
            if(@class == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var teacherId = @class.Teacher.Id;

            _dataAccess.ClassManager.DeleteClass(classId);
            return RedirectToAction("Index", new { teacherId = teacherId });
        }

        protected override void Dispose(bool dispose)
        {
            _dataAccess.Dispose();
        }
    }
}