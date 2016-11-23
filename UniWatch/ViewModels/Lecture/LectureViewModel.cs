using System;
using System.Collections.Generic;
using UniWatch.Models;

namespace UniWatch.ViewModels.Lecture
{
    public class LectureViewModel
    {
        public int ClassId { get; set; }
        public int LectureId { get; set; }
        public DateTime LectureDate { get; set; }
        public List<UpdateLectureItem> LectureItems { get; set; }
        public List<Student> Students { get; set; }

        public LectureViewModel()
        {
            LectureItems = new List<UpdateLectureItem>();
            Students = new List<Student>();
        }
    }
}