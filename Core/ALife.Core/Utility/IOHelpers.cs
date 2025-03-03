namespace ALife.Core.Utility
{
    /// <summary>
    /// Various IO helpers
    /// </summary>
    public static class IOHelpers
    {
        /// <summary>
        /// Creates the directory if it does not exists.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void CreateDirectoryIfNotExists(string path)
        {
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Deletes the directory if exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="maxDeletionAttempts">The maximum deletion attempts.</param>
        public static void DeleteDirectoryIfExists(string path, uint maxDeletionAttempts = 10)
        {
            // Windows sometimes locks files for a short time (after they are created, etc.). By retrying, we can still
            // delete the directory.
            for(uint i = 0; i < maxDeletionAttempts; i++)
            {
                try
                {
                    if(Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                    return;
                }
                catch
                {
                }
            }
        }

        public static void DeleteFileIfExists(string path, uint maxDeletionAttempts = 10)
        {
            // Windows sometimes locks files for a short time (after they are created, etc.). By retrying, we can still
            // delete the directory.
            for(uint i = 0; i < maxDeletionAttempts; i++)
            {
                try
                {
                    if(File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    return;
                }
                catch
                {
                }
            }
        }
    }
}