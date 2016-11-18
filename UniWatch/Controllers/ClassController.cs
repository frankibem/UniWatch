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

        public ClassController()
        {
            _classManager = new ClassManager();
        }

        //GET: Index
        public ActionResult Index()
        {
            return View();
        }


        // GET: Create
        public ActionResult Create()
        {
            return View();
        }

        //POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Number,Section,Semester,Year,Teacher")] Class @class)
        {
            _classManager.CreateClass(@class.Name, @class.Number, @class.Section, @class.Semester, @class.Year,@class.Teacher.Id);
            return View(@class);
        }

        //GET: Delete
        public ActionResult Delete()
        {
            return View();
        }

        //POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int classId)
        {
            _classManager.DeleteClass(classId);
            return RedirectToAction("Index");
        }


    }
}