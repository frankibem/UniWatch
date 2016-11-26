using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using UniWatch.DataAccess;

namespace UniWatch.Models
{
    public class AppDbInitializer : DropCreateDatabaseIfModelChanges<AppDbContext>
    {
        private AppDbContext _context;
        private ApplicationUserManager _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IDataAccess _dataAccess;
        private string _pwd;

        protected override void Seed(AppDbContext context)
        {
            _context = context;
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_context));
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));
            _dataAccess = new DataAccess.DataAccess(context);
            _pwd = WebConfigurationManager.AppSettings["TestPassword"];

            // Create user roles if they do not exist
            const string teacherRole = "Teacher";
            const string studentRole = "Student";
            var roles = new List<string> { "Admin", teacherRole, studentRole };
            foreach(var role in roles)
            {
                if(!_roleManager.RoleExists(role))
                {
                    _roleManager.Create(new IdentityRole(role));
                }
            }

            // Create some teachers
            var teacherList = new List<ApplicationUser>
            {
                new ApplicationUser() { UserName = "witman@uniwatch.com", Email = "witman@uniwatch.com", PhoneNumber = "5551231234" },
                new ApplicationUser() { UserName = "payne@uniwatch.com", Email = "payne@uniwatch.com", PhoneNumber = "5551231234" },
                new ApplicationUser() { UserName = "grice@uniwatch.com", Email = "grice@uniwatch.com", PhoneNumber = "5551231234" }
            };
            CreateUsersAndAddToRole(teacherList, teacherRole);

            var teachers = new List<Teacher>
            {
                _dataAccess.UserManager.CreateTeacher("Temple", "Witman", teacherList[0]),
                _dataAccess.UserManager.CreateTeacher("Daniel", "Payne", teacherList[1]),
                _dataAccess.UserManager.CreateTeacher("Elton", "Grice", teacherList[2])
            };

            // Create some students
            var studentList = new List<ApplicationUser>
            {
                new ApplicationUser() { UserName = "jos@uniwatch.com", Email = "jos@uniwatch.com", PhoneNumber = "5551231234" },
                new ApplicationUser() { UserName = "frank@uniwatch.com", Email = "frank@uniwatch.com", PhoneNumber = "5551231234" },
                new ApplicationUser() { UserName = "claire@uniwatch.com", Email = "claire@uniwatch.com", PhoneNumber = "5551231234" },
                new ApplicationUser() { UserName = "patrick@uniwatch.com", Email = "patrick@uniwatch.com", PhoneNumber = "5551231234" }
            };
            CreateUsersAndAddToRole(studentList, studentRole);

            var students = new List<Student>
            {
                _dataAccess.UserManager.CreateStudent("Joshua", "Hernandez", studentList[0]),
                _dataAccess.UserManager.CreateStudent("Frank", "Ibem", studentList[1]),
                _dataAccess.UserManager.CreateStudent("Claire", "Gray", studentList[2]),
                _dataAccess.UserManager.CreateStudent("Patrick", "Tone", studentList[3])
            };

            // Create some classes
            var classes = new List<Class>
            {
                new Class() { Name = "Data Structures", Number = 2413, Section = "001", Semester = Semester.Fall, Year = 2016, Teacher = teachers[0], TrainingStatus = TrainingStatus.UnTrained },
                new Class() { Name = "Senior Capstone", Number = 4366, Section = "001", Semester = Semester.Spring, Year = 2016, Teacher = teachers[1], TrainingStatus = TrainingStatus.UnTrained },
                new Class() { Name = "Theory of Automata", Number = 3383, Section = "001", Semester = Semester.Summer1, Year = 2016, Teacher = teachers[2], TrainingStatus = TrainingStatus.UnTrained }
            };
            _context.Classes.AddRange(classes);
            _context.SaveChanges();

            // Enroll some students
            var enrollments = new List<Enrollment>
            {
                new Enrollment() { Class = classes[0], Student = students[0], EnrollDate = DateTime.Now, PersonId = Guid.NewGuid() },
                new Enrollment() { Class = classes[0], Student = students[1], EnrollDate = DateTime.Now, PersonId = Guid.NewGuid() },
                new Enrollment() { Class = classes[1], Student = students[0], EnrollDate = DateTime.Now, PersonId = Guid.NewGuid() },
                new Enrollment() { Class = classes[2], Student = students[0], EnrollDate = DateTime.Now, PersonId = Guid.NewGuid() }
            };
            _context.Enrollments.AddRange(enrollments);
            _context.SaveChanges();

            // Hold some lectures
            var lectures = new List<Lecture>
            {
                new Lecture() { Class = classes[0], RecordDate = DateTime.Today.AddDays(-1) },
                new Lecture() { Class = classes[1], RecordDate = DateTime.Today.AddDays(-2) },
                new Lecture() { Class = classes[0], RecordDate = DateTime.Today.AddDays(-7) },
                new Lecture() { Class = classes[0], RecordDate = DateTime.Today.AddDays(-9) }
            };

            lectures[0].Images = new List<UploadedImage>
            {
                new UploadedImage { BlobName = "Dummy", CreationTime = DateTime.Now, Url = "http://images.freeimages.com/images/premium/previews/3547/3547972-close-up-of-students-studying.jpg" },
                new UploadedImage { BlobName = "Dummy", CreationTime = DateTime.Now, Url = "http://images.freeimages.com/images/premium/previews/5892/5892035-interesting-book.jpg" },
                new UploadedImage { BlobName = "Dummy", CreationTime = DateTime.Now, Url = "http://images.freeimages.com/images/premium/previews/8312/8312548-graduation-students.jpg" }
            };

            lectures[1].Images = new List<UploadedImage>
            {
                new UploadedImage { BlobName = "Dummy", CreationTime = DateTime.Now, Url = "http://images.freeimages.com/images/premium/previews/5892/5892035-interesting-book.jpg" },
                new UploadedImage { BlobName = "Dummy", CreationTime = DateTime.Now, Url = "http://images.freeimages.com/images/premium/previews/8312/8312548-graduation-students.jpg" },
                new UploadedImage { BlobName = "Dummy", CreationTime = DateTime.Now, Url = "http://images.freeimages.com/images/premium/previews/3547/3547972-close-up-of-students-studying.jpg" },

            };

            lectures[2].Images = new List<UploadedImage>
            {
                new UploadedImage { BlobName = "Dummy", CreationTime = DateTime.Now, Url = "http://images.freeimages.com/images/premium/previews/3547/3547972-close-up-of-students-studying.jpg" },
                new UploadedImage { BlobName = "Dummy", CreationTime = DateTime.Now, Url = "http://images.freeimages.com/images/premium/previews/8312/8312548-graduation-students.jpg" },
                new UploadedImage { BlobName = "Dummy", CreationTime = DateTime.Now, Url = "http://images.freeimages.com/images/premium/previews/5892/5892035-interesting-book.jpg" },
            };

            _context.Lectures.AddRange(lectures);
            _context.SaveChanges();

            // Take some attendance
            var attendance = new List<StudentAttendance>
            {
                new StudentAttendance() { Lecture = lectures[0], Student = students[0], Present = true },
                new StudentAttendance() { Lecture = lectures[0], Student = students[1], Present = false },
                new StudentAttendance() { Lecture = lectures[2], Student = students[0], Present = true },
                new StudentAttendance() { Lecture = lectures[2], Student = students[1], Present = false },
                new StudentAttendance() { Lecture = lectures[3], Student = students[0], Present = false },
                new StudentAttendance() { Lecture = lectures[3], Student = students[1], Present = true },
                new StudentAttendance() { Lecture = lectures[1], Student = students[0], Present = false}
            };
            _context.Attendance.AddRange(attendance);
            _context.SaveChanges();

            base.Seed(_context);
        }

        /// <summary>
        /// Creates the given users and assigns them the given role
        /// </summary>
        /// <param name="users">
        /// The users to create
        /// </param>
        /// <param name="role">
        /// The role to assign
        /// </param>
        private void CreateUsersAndAddToRole(IEnumerable<ApplicationUser> users, string role)
        {
            foreach(var user in users)
            {
                _userManager.Create(user, _pwd);
                _userManager.AddToRole(user.Id, role);
            }

            _context.SaveChanges();
        }
    }
}