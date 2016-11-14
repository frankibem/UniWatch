using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace UniWatch.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        public DbSet<Class> Classes { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<StudentAttendance> Attendance { get; set; }
        public DbSet<UploadedImage> Images { get; set; }
        public DbSet<FacialProfile> FacialProfiles { get; set; }
    }
}