using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using UniWatch.Models;
using Microsoft.AspNet.Identity;

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
        /// <returns>The User associated with the given identity</returns>
        public User GetUser()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId<string>();
            var teacher = _db.Teachers.Where(t => t.IdentityId == userId).FirstOrDefault();

            if(teacher == null)
                return _db.Students.Where(s => s.IdentityId == userId).FirstOrDefault();

            return teacher;
        }

        /// <summary>
        /// Returns a list of all students whose id matches the given id
        /// </summary>
        /// <param name="id">The search string</param>
        /// <remarks>Returns an empty list if given a null or empty search string</remarks>
        public IEnumerable<Student> SearchStudent(string id)
        {
            if(string.IsNullOrEmpty(id))
                return new List<Student>();

            return _db.Students.Where(s => s.Id.ToString().Contains(id));
        }

        /// <summary>
        /// Sets the images that make up the facial profile for a student
        /// </summary>
        /// <param name="studentId">The id of the student</param>
        /// <param name="images">The images that make up the profile</param>
        /// <returns>The uploaded images</returns>
        public IEnumerable<UploadedImage> SetStudentProfile(int studentId, IEnumerable<Stream> images)
        {
            var student = GetStudentById(studentId);
            if(student == null)
                throw new InvalidOperationException("Error setting student profile");

            // Store the images
            var storageManager = new StorageManager();
            List<UploadedImage> result = storageManager.SaveImages(images).Result;

            // Update the facial profile
            foreach(var image in result)
            {
                student.Profile.Images.Add(image);
            }

            _db.SaveChanges();
            return result;
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