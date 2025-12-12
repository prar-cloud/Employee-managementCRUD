using System;

namespace EmployeeCRUD
{
    /// <summary>
    /// Simple helper class for managing data storage location
    /// All data is stored as JSON files in local storage
    /// </summary>
    public class DatabaseHelper
    {
        private static string _storagePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EmployeeCRUD"
        );

        public static string StoragePath
        {
            get => _storagePath;
            set => _storagePath = value;
        }

        /// <summary>
        /// Ensures the storage directory exists
        /// </summary>
        public static void EnsureStorageDirectory()
        {
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        /// <summary>
        /// Gets the full path for a specific data file
        /// </summary>
        public static string GetDataFilePath(string fileName)
        {
            EnsureStorageDirectory();
            return Path.Combine(_storagePath, fileName);
        }
    }
}
