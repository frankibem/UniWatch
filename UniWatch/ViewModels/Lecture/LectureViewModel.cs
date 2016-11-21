using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniWatch.DataAccess;
using UniWatch.Models;

namespace UniWatch.ViewModels.Lecture
{
    public class LectureViewModel
    {
        public Models.Lecture Lecture { get; set; }
        public List<Student> Students { get; set; }
    }
}