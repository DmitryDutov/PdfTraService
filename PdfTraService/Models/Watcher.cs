using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;

namespace PdfTraService.Models
{
    public class Watcher
    {
        public static string Name => _name;
        private static string _name;

        private static string _current;
        public static string Current => _current;
        private static string _target;
        public static string Target => _target;
        private static string _filter;
        public static string Filter => _filter;

        public Watcher(string name, string current, string target, string filter)
        {
            _name = name;
            _current = current;
            _target = target;
            _filter = filter;
        }
        private static async Task MoveFile(string currentFilePath)
        {
            var name = Path.GetFileName(currentFilePath);
            var targetDir = $"{Target}{name}";

            File.Move(currentFilePath, targetDir);

            Log.Information($"Watcher {Name} переместил файл в {targetDir}");
        }

        public void WatchFile()
        {
            Log.Information($"Watcher {Name} - запущен");
            using var watcher = new FileSystemWatcher(Current);

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.Filter = Filter;
            watcher.EnableRaisingEvents = true;

            Console.ReadLine();
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Log.Information($"Changed: {e.FullPath}");
            Task.Run(() =>
            {
                MoveFile(e.FullPath);
            });
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Log.Information(value);
            Task.Run(() =>
            {
                MoveFile(e.FullPath);
            });
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Log.Information($"Deleted: {e.FullPath}");

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Log.Information($"Renamed:");
            Log.Information($"    Old: {e.OldFullPath}");
            Log.Information($"    New: {e.FullPath}");
            Task.Run(() =>
            {
                MoveFile(e.FullPath);
            });
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Log.Information($"Message: {ex.Message}");
                Log.Information("Stacktrace:");
                Log.Information(ex.StackTrace);
                PrintException(ex.InnerException);
            }
        }
    }

}
