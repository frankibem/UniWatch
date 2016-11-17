using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniWatch.Models;

namespace UniWatch.Controllers
{
    public class LectureController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        public ActionResult Create(Lecture lecture)
        {
            return View();
        }

        [Authorize]
        public ActionResult Delete()
        {
            return View();
        }

        [Authorize]
        public ActionResult Delete(int lectureId)
        {
            return View();
        }

        [Authorize]
        public ActionResult Override()
        {
            return View();
        }

        [Authorize]
        public ActionResult Override(int lectureId)
        {
            return View();
        }
    }
}