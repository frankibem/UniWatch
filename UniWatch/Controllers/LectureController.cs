using System.Linq;
using System.Net;
using System.Web.Mvc;
using UniWatch.DataAccess;
using UniWatch.Models;
using UniWatch.ViewModels.Lecture;

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
        [HttpGet]
        public ActionResult Index(int classId)
        {
            var report = new ReportViewModel(classId, _manager);

            // If the class does not exist,
            // then display an error
            if (report.Class == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No class with id {classId}");
            }

            return View(report);
        }

        [HttpGet]
        public ActionResult Override(int lectureId)
        {
            var lecture = new LectureViewModel(lectureId, _manager);

            // If the lecture does not exist,
            // then display an error
            if (lecture.Lecture == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No lecture with id {lectureId}");
            }

            return View(lecture);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Override(LectureViewModel model)
        {
            var lecture = _manager.LectureManager.Get(model.Lecture.Id);

            if (lecture == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"No lecture with id {model.Lecture.Id}");
            }

            foreach (var s in model.Students)
            {
                lecture.Attendance.First(a => a.Student.Id == s.Id).Present =
                    model.Lecture.Attendance.First(a => a.Student.Id == s.Id).Present;
            }

            _manager.LectureManager.Update(lecture);

            return RedirectToAction("Override", new { lectureId = lecture.Id });
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Lecture lecture)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Delete(int lectureId)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(Lecture lecture)
        {
            return View();
        }
    }
}