using System;
using System.Collections.Generic;

namespace UniWatch.ViewModels.Lecture
{
    public class LectureViewModel
    {
        public int ClassId { get; set; }
        public int LectureId { get; set; }
        public DateTime LectureDate { get; set; }
        public List<UpdateLectureItem> LectureItems { get; set; }

        public LectureViewModel()
        {
            LectureItems = new List<UpdateLectureItem>();
        }
    }

    public class UpdateLectureItem
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public bool Present { get; set; }
    }
}