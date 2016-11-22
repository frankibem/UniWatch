using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using Microsoft.Ajax.Utilities;
using UniWatch.Models;
using UniWatch.Services;

namespace UniWatch.DataAccess
{
    /// <summary>
    /// Provides central access to Lecture related information
    /// </summary>
    public class LectureManager : ILectureManager
    {
        private bool disposed = false;
        private AppDbContext _db;

        /// <summary>
        /// Creates a default LectureManager
        /// </summary>
        public LectureManager()
        {
            _db = new AppDbContext();
        }

        /// <summary>
        /// Creates a LectureManager with the given context
        /// </summary>
        /// <param name="context">The context to create the manager with</param>
        public LectureManager(AppDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Returns the lecture with the given id
        /// </summary>
        /// <param name="lectureId">The id of the lecture</param>
        /// <returns>The lecture with the given id</returns>
        public Lecture Get(int lectureId)
        {
            return _db.Lectures.Find(lectureId);
        }

        /// <summary>
        /// Returns a list of all lectures for the given class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <returns>A list of all lectures for the class</returns>
        public IEnumerable<Lecture> GetTeacherReport(int classId)
        {
            var lectures = _db.Lectures.Where(lecture => lecture.Class.Id == classId).ToList();
            foreach(var lecture in lectures)
            {
                var attendance = _db.Attendance
                    .Where(a => a.Lecture.Id == lecture.Id)
                    .Include(a => a.Student);
                lecture.Attendance = attendance.ToList();
            }

            return lectures;
        }

        /// <summary>
        /// Returns a list of attendance for a student in a class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <param name="studentId">The id of the student</param>
        /// <returns>The list of attendance for the student in the class</returns>
        public IEnumerable<StudentAttendance> GetStudentReport(int classId, int studentId)
        {
            return _db.Attendance
                .Where(a => a.Lecture.Class.Id == classId && a.Student.Id == studentId);
        }

        /// <summary>
        /// Updates/Overrides the values in a lecture
        /// </summary>
        /// <param name="lectureId">The id of the lecture to update</param>
        /// <param name="updates">The list of updates to make</param>
        /// <returns>The updated lecture</returns>
        public Lecture Update(int lectureId, IEnumerable<UpdateLectureItem> updates)
        {
            var lecture = _db.Lectures.Find(lectureId);

            // Doesn't exist
            if(lecture == null)
                throw new InvalidOperationException("Error updating lecture. Id not found.");

            var attendanceMap = new Dictionary<int, StudentAttendance>(lecture.Attendance.Count);
            foreach(var attendance in lecture.Attendance)
                attendanceMap.Add(attendance.Student.Id, attendance);

            foreach(var item in updates)
            {
                StudentAttendance attendance;
                if(attendanceMap.TryGetValue(item.StudentId, out attendance))
                {
                    if(attendance.Present != item.Present)
                    {
                        // Only update if changed
                        attendance.Present = item.Present;
                        _db.Entry(attendance).State = EntityState.Modified;
                    }
                }

                // Assumption: we do not create new attendance objects for students who were not enrolled
                // at the time the lecture was recorded.
            }


            _db.Lectures.Attach(lecture);
            _db.Entry(lecture).State = EntityState.Modified;
            _db.SaveChanges();

            return lecture;
        }

        /// <summary>
        /// Deletes the matched record and all related information
        /// </summary>
        /// <param name="lectureId">The id of the lecture to delete</param>
        /// <returns>The deleted lecture</returns>
        public Lecture Delete(int lectureId)
        {
            var existing = _db.Lectures.Find(lectureId);

            if(existing == null)
                throw new InvalidOperationException("Error deleting lecture.");

            return _db.Lectures.Remove(existing);

            // TODO: Delete all other lecture related information (Images (and blobs), Attendance)
        }

        /// <summary>
        /// Record a new lecture for the given class using the given images
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <param name="images">The images to detect students from</param>
        public Lecture RecordLecture(int classId, IEnumerable<Stream> images)
        {
            var @class = _db.Classes.Find(classId);

            if(@class == null)
                throw new InvalidOperationException("Error training recognizer");
            else if(@class.TrainingStatus != TrainingStatus.Trained)
                throw new InvalidOperationException("Cannot record using untrained recognizer");

            var lecture = new Lecture()
            {
                Class = @class,
                RecordDate = DateTime.Now
            };

            // Save the images in Azure Storage
            var storageManager = new StorageManager();
            var uploadedImages = storageManager.SaveImages(images).Result;

            foreach(var image in uploadedImages)
                lecture.Images.Add(image);

            // Detect the faces in the images
            var recognitionService = new RecognitionService();
            var personIds = recognitionService.DetectStudents(classId.ToString(), uploadedImages).Result;

            // Create StudentAttendance for each student in class
            var enrollments = _db.Enrollments.Where(e => e.Class.Id == classId)
                .Include(e => e.Student);
            Dictionary<Guid, StudentAttendance> attendanceMap = new Dictionary<Guid, StudentAttendance>();
            foreach(var enrollment in enrollments)
            {
                var attendance = new StudentAttendance
                {
                    Lecture = lecture,
                    Student = enrollment.Student,
                    Present = false
                };
                lecture.Attendance.Add(attendance);
                attendanceMap.Add(enrollment.PersonId, attendance);
            }

            // Mark detected students as present
            foreach(var personId in personIds)
                attendanceMap[personId].Present = true;

            _db.Lectures.Add(lecture);
            _db.SaveChanges();

            // Alert absent students
            var absent = lecture.Attendance.Where(a => !a.Present)
                .Select(a => a.Student);
            AlertAbsentStudents(absent, @class);

            return lecture;
        }

        /// <summary>
        /// Alert the given students via email and sms of their absence
        /// </summary>
        /// <param name="students">The students to alert</param>
        /// <param name="class">The class to alert students for</param>
        private void AlertAbsentStudents(IEnumerable<Student> students, Class @class)
        {
            EmailService emailService = new EmailService();
            SmsService smsService = new SmsService();
            List<Task> tasks = new List<Task>();

            var fromEmail = WebConfigurationManager.AppSettings["EmailFrom"];
            var fromSms = WebConfigurationManager.AppSettings["TwilioSmsNumber"];
            string subject = $"Attendance Information for for \"{@class.Name}\"";
            string message = $"You have been marked absent for \"{@class.Name}\" on {DateTime.Now.ToShortDateString()}";

            foreach(var student in students)
            {
                var user = _db.Users.Find(student.IdentityId);
                if(user != null)
                {
                    tasks.Add(emailService.SendEmail(fromEmail, user.Email, "Attendance Information", message));
                    smsService.SendMessage("", user.PhoneNumber, message);
                }
            }
            Task.WaitAll(tasks.ToArray());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposed)
                return;

            if(disposing)
            {
                _db.Dispose();
            }

            disposed = true;
        }
    }
}