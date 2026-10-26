namespace ChessMaze.Persistence;

public static class DatabasePath
{
    public static string Resolve()
    {
        var configured = Environment.GetEnvironmentVariable("CHESSMAZE_DB_PATH");
        if (!string.IsNullOrWhiteSpace(configured))
            return Path.GetFullPath(configured);
        var root = FindSolutionRoot(AppContext.BaseDirectory) ?? Directory.GetCurrentDirectory();
        return Path.Combine(root, "data", "saved-games.db");
    }

    static string? FindSolutionRoot(string start)
    {
        var d = new DirectoryInfo(start);
        while (d is not null)
        {
            if (d.GetFiles("BCIT615.A2.EFCore.Support.sln").Length > 0)
                return d.FullName;
            d = d.Parent;
        }

        return null;
    }
}
