using System;
using System.Linq;
using UniWatch.Models;

namespace UniWatch.DataAccess
{
    public class UserManager : IUserManager
    {
        private bool _disposed = false;
        private AppDbContext _db;

        /// <summary>
        /// Creates a default UserManager
        /// </summary>
        public UserManager() : this(new AppDbContext())
        { }

        /// <summary>
        /// Creates a UserManager with the given context
        /// </summary>
        /// <param name="context">The context to create the manager with</param>
        public UserManager(AppDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Creates a new student
        /// </summary>
        /// <param name="firstName">The student's first name</param>
        /// <param name="lastName">The student's last name</param>
        /// <param name="user">The underlying Identity user object</param>
        /// <returns>The created student</returns>
        public Student CreateStudent(string firstName, string lastName, ApplicationUser user)
        {
            Student student = new Student
            {
                FirstName = firstName,
                LastName = lastName,
                IdentityId = user.Id,
                Profile = new FacialProfile()
            };

            _db.Students.Add(student);
            _db.SaveChanges();

            return student;
        }

        /// <summary>
        /// Creates a new teacher
        /// </summary>
        /// <param name="firstName">The teacher's first name</param>
        /// <param name="lastName">The teacher's last name</param>
        /// <param name="user">The underlying Identity user object</param>
        /// <returns>The created teacher</returns>
        public Teacher CreateTeacher(string firstName, string lastName, ApplicationUser user)
        {
            Teacher teacher = new Teacher
            {
                FirstName = firstName,
                LastName = lastName,
                IdentityId = user.Id,
            };

            _db.Teachers.Add(teacher);
            _db.SaveChanges();

            return teacher;
        }

        /// <summary>
        /// Finds a student with the given id
        /// </summary>
        /// <param name="studentId">The id of the student to search for</param>
        /// <returns>The student with the given id. Returns null if no such student exists</returns>
        public Student GetStudentById(int studentId)
        {
            return _db.Students.Find(studentId);
        }

        /// <summary>
        /// Finds a teacher with the given id
        /// </summary>
        /// <param name="teacherId">The id of the teacher to search for</param>
        /// <returns>The teacher with the given id. Returns null if no such teacher exists</returns>
        public Teacher GetTeacherById(int teacherId)
        {
            return _db.Teachers.Find(teacherId);
        }

        /// <summary>
        /// Returns the user associated with the given identity
        /// </summary>
        /// <param name="identity">The id underlying identity object for the user</param>
        /// <returns>The User associated with the given identity</returns>
        public User GetUser(string identityId)
        {
            var teacher = _db.Teachers.Where(t => t.IdentityId == identityId).FirstOrDefault();

            if(teacher == null)
                return _db.Students.Where(s => s.IdentityId == identityId).FirstOrDefault();

            return teacher;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(_disposed)
                return;

            if(disposing)
            {
                _db.Dispose();
            }

            _disposed = true;
        }
    }
}