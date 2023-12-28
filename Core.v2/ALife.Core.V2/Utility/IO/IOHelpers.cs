using System.IO;

namespace ALife.Core.Utility.IO
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
    }
}
