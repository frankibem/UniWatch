using System;

namespace UniWatch.DataAccess
{
    /// <summary>
    /// Provides central access to data managers
    /// </summary>
    public interface IDataAccess : IDisposable
    {
        /// <summary>
        /// The class manager for class related functionality
        /// </summary>
        IClassManager ClassManager { get; }

        /// <summary>
        /// The class manager for lecture related functionality
        /// </summary>
        ILectureManager LectureManager { get; }

        /// <summary>
        /// The class manager for student related functionality
        /// </summary>
        IStudentManager StudentManager { get; }
    }
}