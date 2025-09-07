using System.IO;
using Microsoft.Maui.Storage;

namespace Unutmaz.Services
{
    public static class DatabaseConfig
    {
        public static string DatabasePath => Path.Combine(
            FileSystem.AppDataDirectory, "UnutmazDatabase.db3");
    }
}
