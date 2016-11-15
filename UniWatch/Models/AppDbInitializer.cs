using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RestSharp.Extensions;

namespace UniWatch.Models
{
    public class AppDbInitializer : DropCreateDatabaseIfModelChanges<AppDbContext>
    {
        // TODO: Add the managers here
        private ApplicationUserManager _userManager;

        protected override void Seed(AppDbContext context)
        {
            // TODO: Initialize the managers here
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

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

            // Save the accounts in the database
            //CreateUsers<Teacher>(teachers);
            CreateUsers<Student>(students);

            // TODO: Initiailze database here
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
        private void CreateUsers<T>(IEnumerable<User> users) where T : class
        {
            using (var db = new AppDbContext())
            {
                foreach (var user in users)
                {
                    _userManager.Create(user.Identity);
                    db.Set<T>().Add(user as T);
                }

                db.SaveChanges();
            }
        }
    }
}