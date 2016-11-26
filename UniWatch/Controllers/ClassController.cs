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
    public class ClassController : Controller
    {
        private IDataAccess _dataAccess;

        public ClassController()
        {
            _dataAccess = new DataAccess.DataAccess();
        }
         public ClassController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        //GET: Index
        /// <summary>
        /// Displays all classes taught by the teacher
        /// </summary>
        /// <param name="teacherId">Id of the teacher</param>
        public ActionResult Index(int teacherId)
        {
            var teacher = _dataAccess.UserManager.GetTeacherById(teacherId);
                if (teacher==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.TeacherId = teacherId;
            return View(_dataAccess.ClassManager.GetClassesForTeacher(teacherId));
        }


        // GET: Create
        /// <summary>
        /// Sends user to the create class page
        /// </summary>
        /// <param name="teacherId">Id of the teacher</param>
        public ActionResult Create(int teacherId)
        {
            var teacher = _dataAccess.UserManager.GetTeacherById(teacherId);
            if (teacher == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           // var teacher = _dataAccess.UserManager.GetTeacherById(teacherId);
            return View(new Class() { Teacher = teacher });
        }

        //POST: Create
        /// <summary>
        /// Create class for the teacher
        /// </summary>
        /// <param name="class">Model containing information to create the class</param>
        /// <param name="teacherId">Id of the teacher</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Number,Section,Semester,Year")] Class @class, int teacherId)
        {
            if (!ModelState.IsValid)
            {
                return View(@class);
            }
            try
            {
                var created = _dataAccess.ClassManager.CreateClass(@class.Name, @class.Number, @class.Section, @class.Semester, @class.Year, teacherId);
                return RedirectToAction("Index", new { teacherId = created.Teacher.Id });
            }
            catch (InvalidOperationException)
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
            if (@class == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No class with id {classId}");
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
            if (@class == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No class with id {classId}");
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