using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UniWatch.DataAccess;
using UniWatch.Services;

namespace UniWatch.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private IDataAccess _dataAccess;

        public AdminController() : this(new DataAccess.DataAccess()) { }

        public AdminController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        /// <summary>
        /// Returns home page for administrative actions
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Clears all data in the identified item
        /// </summary>
        /// <param name="item">The asset whose data contents are to be deleted</param>
        public ActionResult Clear(string item)
        {
            item = item.ToLowerInvariant();

            switch(item)
            {
                case "db":
                    _dataAccess.DbContext.Database.Delete();
                    break;
                case "cognitive":                    
                    RecognitionService.ClearAll();
                    break;
                case "storage":
                    new StorageService().ClearAll();
                    break;
                case "all":
                    _dataAccess.DbContext.Database.Delete();
                    RecognitionService.ClearAll();
                    new StorageService().ClearAll();
                    break;
                default:
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Message = "Deletion Successful";
            return View("Index");
        }
    }
}