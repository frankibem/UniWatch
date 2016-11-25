using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniWatch.DataAccess;
using UniWatch.Models;

namespace UniWatch.Controllers
{
    public class ClassController : Controller
    {
        private IClassManager _classManager;
        private IStudentManager _studentManager;
       

        public ClassController()
        {
            _classManager = new ClassManager();
            _studentManager = new StudentManager();
          //  var classes = _classManager.GetClassesForTeacher(teacherId);
        }

        //GET: Index
        public ActionResult Index(int teacherId)
        {
            ViewBag.TeacherId = teacherId;
            return View(_classManager.GetClassesForTeacher(teacherId));
        }


        // GET: Create
        public ActionResult Create(int teacherId)
        {
            var teacher=_studentManager.GetTeacher(teacherId);
            return View(new Class() { Teacher= teacher});
        }

        //POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Number,Section,Semester,Year,Teacher")] Class @class)
        {
            //if (ModelState.IsValid)
            //{
            //    _classManager.CreateClass(@class.Name, @class.Number, @class.Section, @class.Semester, @class.Year, @class.Teacher.Id);
                
            //    return RedirectToAction("Index");
            //}
            _classManager.CreateClass(@class.Name, @class.Number, @class.Section, @class.Semester, @class.Year,@class.Teacher.Id);
            return RedirectToAction("Index", new { teacherId = @class.Teacher.Id});
            //return View(@class);
        }

        //GET: Delete
        public ActionResult Delete(int classId)
        {
            var @class = _classManager.GetById(classId);
            return View(@class);
        }

        //POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int classId)
        {
            var teacherId =_classManager.DeleteClass(classId);
            return RedirectToAction("Index", new { teacherId = teacherId });
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);
        }
    }
}