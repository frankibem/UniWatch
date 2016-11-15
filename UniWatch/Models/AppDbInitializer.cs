using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RestSharp.Extensions;

namespace UniWatch.Models
{
    public class AppDbInitializer : DropCreateDatabaseIfModelChanges<AppDbContext>
    {
        private AppDbContext _context;
        private ApplicationUserManager _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private string _pwd;

        protected override void Seed(AppDbContext context)
        {
            _context = context;
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_context));
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));
            _pwd = WebConfigurationManager.AppSettings["TestPassword"];

            // Create user roles if they do not exist
            const string teacherRole = "Teacher";
            const string studentRole = "Student";
            var roles = new List<string> {"Admin", teacherRole, studentRole };
            foreach (var role in roles)
            {
                if (!_roleManager.RoleExists(role))
                {
                    _roleManager.Create(new IdentityRole(role));
                }
            }

            // Create some teachers
            var teacherList = new List<ApplicationUser>
            {
                new ApplicationUser() { UserName = "iteachallclasses", Email = "all@uniwatch.io", PhoneNumber = "5551231234" },
                new ApplicationUser() { UserName = "iteachnoclasses", Email = "no@uniwatch.io", PhoneNumber = "5551231234" },
                new ApplicationUser() { UserName = "iteach", Email = "teach@uniwatch.io", PhoneNumber = "5551231234" }
            };

            //var teachers = new List<Teacher>
            //{
            //    new Teacher() { FirstName = "All", LastName = "Classes", Identity = teacherList[0] },
            //    new Teacher() { FirstName = "No", LastName = "Classes", Identity = teacherList[1] },
            //    new Teacher() { FirstName = "Some", LastName = "Classes", Identity = teacherList[2] },
            //};

            //CreateUsers<Teacher>(teachers, teacherRole);

            // Create some students
            var studentList = new List<ApplicationUser>
            {
                new ApplicationUser() { UserName = "josh", Email = "joshua@uniwatch.io", PhoneNumber = "5551231234" },
                new ApplicationUser() { UserName = "frank", Email = "frank@uniwatch.io", PhoneNumber = "5551231234" },
                new ApplicationUser() { UserName = "claire", Email = "claire@uniwatch.io", PhoneNumber = "5551231234" },
                new ApplicationUser() { UserName = "patrick", Email = "patrick@uniwatch.io", PhoneNumber = "5551231234" }
            };

            var students = new List<Student>
            {
                new Student() { FirstName = "Josh", LastName = "Hernandez", Identity = studentList[0] },
                new Student() { FirstName = "Frank", LastName = "Ibem", Identity = studentList[1] },
                new Student() { FirstName = "Claire", LastName = "Gray", Identity = studentList[2] },
                new Student() { FirstName = "Patrick", LastName = "Tone", Identity = studentList[3] },

            };

            CreateUsersAndAddToRole<Student>(students, studentRole);

            // Create some classes
            var classes = new List<Class>
            {
                new Class() { Name = "Class 0", Number = 0, Section = "All", /*Teacher = teachers[0]*/},
                new Class() { Name = "Class 1", Number = 1, Section = "Some", /*Teacher = teachers[3]*/},
                new Class() { Name = "Class 2", Number = 2, Section = "Another Sect", /*Teacher = teachers[0]*/}
            };

            _context.Classes.AddRange(classes);
            _context.SaveChanges();

            // Enroll some students in some classes
            var yesterday = DateTime.Today.AddDays(-1);
            var lastWeek = DateTime.Today.AddDays(-7);

            var enrollment = new List<Enrollment>
            {
                new Enrollment() { Student = students[0], Class = classes[0], EnrollDate = lastWeek },
                new Enrollment() { Student = students[0], Class = classes[1], EnrollDate = lastWeek },
                new Enrollment() { Student = students[1], Class = classes[0], EnrollDate = yesterday },
                new Enrollment() { Student = students[2], Class = classes[0], EnrollDate = yesterday }
            };

            _context.Enrollments.AddRange(enrollment);
            _context.SaveChanges();

            // Create some facial profiles for the students
            var facialProfiles = new List<FacialProfile>
            {
                new FacialProfile() { Student = students[0], RecognizerTrained = false },
                new FacialProfile() { Student = students[1], RecognizerTrained = false },
                new FacialProfile() { Student = students[2], RecognizerTrained = false },
                new FacialProfile() { Student = students[3], RecognizerTrained = false }
            };

            _context.FacialProfiles.AddRange(facialProfiles);
            _context.SaveChanges();

            // Initiailze database here
            base.Seed(context);
        }

        /// <summary>
        /// A generic method for adding objects to the database
        /// </summary>
        /// <typeparam name="T">
        /// The type of class to add to the database
        /// </typeparam>
        /// <param name="users">
        /// The users to add to the database
        /// </param>
        /// <param name="role">
        /// The user's role
        /// </param>
        private void CreateUsersAndAddToRole<T>(IEnumerable<User> users, string role) where T : class
        {
            foreach (var user in users)
            {
                _userManager.Create(user.Identity, _pwd);
                _userManager.AddToRole(user.Identity.Id, role);
                _context.Set<T>().Add(user as T);
            }

            _context.SaveChanges();
        }
    }
}