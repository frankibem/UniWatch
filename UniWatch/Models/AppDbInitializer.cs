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
                new ApplicationUser() { UserName = "Temple", Email = "witman@uniwatch.com", PhoneNumber = "5551231234" },
                new ApplicationUser() { UserName = "Daniel", Email = "payne@uniwatch.com", PhoneNumber = "5551231234" },
                new ApplicationUser() { UserName = "Elton", Email = "grice@uniwatch.com", PhoneNumber = "5551231234" }
            };
            CreateUsersAndAddToRole(teacherList, teacherRole);

            var teachers = new List<Teacher>();
            teachers.Add(_dataAccess.UserManager.CreateTeacher("Temple", "Witman", teacherList[0]));
            teachers.Add(_dataAccess.UserManager.CreateTeacher("Daniel", "Payne", teacherList[1]));
            teachers.Add(_dataAccess.UserManager.CreateTeacher("Elton", "Grice", teacherList[2]));

            // Create some students
            var studentList = new List<ApplicationUser>
            {
                new ApplicationUser() { UserName = "Josh", Email = "jos@uniwatch.com", PhoneNumber = "5551231234" },
                new ApplicationUser() { UserName = "Frank", Email = "frank@uniwatch.com", PhoneNumber = "5551231234" },
                new ApplicationUser() { UserName = "Claire", Email = "claire@uniwatch.com", PhoneNumber = "5551231234" },
                new ApplicationUser() { UserName = "Patrick", Email = "patrick@uniwatch.com", PhoneNumber = "5551231234" }
            };
            CreateUsersAndAddToRole(studentList, studentRole);

            var students = new List<Student>();
            students.Add(_dataAccess.UserManager.CreateStudent("Joshua", "Hernandez", studentList[0]));
            students.Add(_dataAccess.UserManager.CreateStudent("Frank", "Ibem", studentList[1]));
            students.Add(_dataAccess.UserManager.CreateStudent("Claire", "Gray", studentList[2]));
            students.Add(_dataAccess.UserManager.CreateStudent("Patrick", "Tone", studentList[3]));

            // Create some classes
            _dataAccess.ClassManager.CreateClass("Data Structures", 2413, "001", Semester.Fall, 2016, teachers[0].Id);
            _dataAccess.ClassManager.CreateClass("Senior Capstone", 4366, "001", Semester.Spring, 2016, teachers[1].Id);
            _dataAccess.ClassManager.CreateClass("Theory of Automata", 3383, "001", Semester.Summer1, 2017, teachers[0].Id);
            var classes = _context.Classes.ToList();

            // Enroll some students
            _dataAccess.ClassManager.EnrollStudent(classes[0].Id, students[0].Id);
            _dataAccess.ClassManager.EnrollStudent(classes[0].Id, students[1].Id);
            _dataAccess.ClassManager.EnrollStudent(classes[1].Id, students[0].Id);
            _dataAccess.ClassManager.EnrollStudent(classes[2].Id, students[0].Id);

            _context.SaveChanges();
            base.Seed(context);
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