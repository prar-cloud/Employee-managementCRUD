namespace EmployeeCRUD
{
    /// <summary>
    /// Factory class for creating repository instances
    /// Always returns LocalStorageRepository (JSON-based storage)
    /// </summary>
    public static class RepositoryFactory
    {
        public static LocalStorageRepository CreateRepository()
        {
            return new LocalStorageRepository();
        }
    }
}
