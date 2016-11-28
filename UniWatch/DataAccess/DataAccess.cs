using System;
using UniWatch.Models;

namespace UniWatch.DataAccess
{
    public class DataAccess : IDataAccess
    {
        private AppDbContext _db;
        private IClassManager _classManager;
        private ILectureManager _lectureManager;
        private IUserManager _userManager;

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
        /// The manager for user related functionality
        /// </summary>
        public IUserManager UserManager
        {
            get
            {
                if(_userManager == null)
                {
                    _userManager = new UserManager(_db);
                }
                return _userManager;
            }

            protected set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// The underlying database context
        /// </summary>
        public AppDbContext DbContext
        {
            get
            {
                return _db;
            }
        }

        /// <summary>
        /// Initializes a default data access object
        /// </summary>
        public DataAccess() : this(new AppDbContext())
        { }

        /// <summary>
        /// Initializes a new data access object using the given context
        /// </summary>
        /// <param name="context">The context to initialize the data access object with</param>
        public DataAccess(AppDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Initializes a new data access object using the given managers
        /// </summary>
        /// <param name="classManager"></param>
        /// <param name="lectureManager"></param>
        /// <param name="userManager"></param>
        public DataAccess(IClassManager classManager, ILectureManager lectureManager, IUserManager userManager)
        {
            _classManager = classManager;
            _lectureManager = lectureManager;
            _userManager = userManager;         
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
                    _userManager = null;
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