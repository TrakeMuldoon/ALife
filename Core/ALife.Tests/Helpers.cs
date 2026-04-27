namespace ALife.Tests;

public static class Helpers
{
    private static string? _solutionRoot = null;
    
    /// <summary>
    /// Gets the root solution directory by starting from TestContext.TestRunDirectory
    /// and traversing upward until a .sln or .slnx file is found.
    /// </summary>
    public static string GetSolutionRootFromTestContext(TestContext testContext)
    {
        if(_solutionRoot != null)
        {
            return _solutionRoot;
        }
        
        if (string.IsNullOrWhiteSpace(testContext.TestRunDirectory))
            throw new InvalidOperationException("TestContext.TestRunDirectory is null or empty.");

        DirectoryInfo dir = new(testContext.TestRunDirectory);

        // Traverse upward until we find a .sln or .slnx file
        while (dir != null && dir.Exists)
        {
            bool hasSolutionFile =
                dir.GetFiles("*.sln", SearchOption.TopDirectoryOnly).Length > 0 ||
                dir.GetFiles("*.slnx", SearchOption.TopDirectoryOnly).Length > 0;

            if (hasSolutionFile)
            {
                _solutionRoot = dir.FullName;
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException("Solution root (.sln or .slnx) not found from TestContext.");
    }
}
