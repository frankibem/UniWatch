using System;
using UniWatch.Models;

namespace UniWatch.Managers
{
    public class DataAccess : IDataAccess
    {
        private AppDbContext _db;
        private IClassManager _classManager;
        private ILectureManager _lectureManager;
        private IStudentManager _studentManager;

        /// <summary>
        /// The class manager for class related functionality
        /// </summary>
        public IClassManager ClassManager
        {
            get
            {
                if(_classManager == null)
                {
                    _classManager = new ClassManager(_db);
                }
                return _classManager;
            }

            protected set
            {
                _classManager = value;
            }
        }

        /// <summary>
        /// The class manager for lecture related functionality
        /// </summary>
        public ILectureManager LectureManager
        {
            get
            {
                if(_lectureManager == null)
                {
                    _lectureManager = new LectureManager(_db);
                }
                return _lectureManager;
            }

            protected set
            {
                _lectureManager = value;
            }
        }

        /// <summary>
        /// The class manager for student related functionality
        /// </summary>
        public IStudentManager StudentManager
        {
            get
            {
                if(_studentManager == null)
                {
                    _studentManager = new StudentManager(_db);
                }
                return _studentManager;
            }

            protected set
            {
                _studentManager = value;
            }
        }

        #region IDisposable Support
        private bool disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
                {
                    _db.Dispose();
                    _lectureManager = null;
                    _classManager = null;
                    _studentManager = null;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}