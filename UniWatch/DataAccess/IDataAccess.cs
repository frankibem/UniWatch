using System;
using UniWatch.Models;

namespace UniWatch.DataAccess
{
    /// <summary>
    /// Provides central access to data managers
    /// </summary>
    public interface IDataAccess : IDisposable
    {
        /// <summary>
        /// The manager for class related functionality
        /// </summary>
        IClassManager ClassManager { get; }

        /// <summary>
        /// The manager for lecture related functionality
        /// </summary>
        ILectureManager LectureManager { get; }

        /// <summary>
        /// The manager for user related functionality
        /// </summary>
        IUserManager UserManager { get; }
    }
}