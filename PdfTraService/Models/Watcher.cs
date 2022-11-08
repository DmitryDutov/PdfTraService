using System;
using System.IO;
using System.Threading.Tasks;

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

        public Watcher(string name, string current, string target)
        {
            _name = name;
            _current = current;
            _target = target;
        }
        private static async Task MoveFile(string currentDir)
        {
            var name = Path.GetFileName(currentDir);

            var targetDir = $"{Target}{name}";
            File.Move(currentDir, targetDir);

            Console.ReadLine();
        }

        public void WacthFile()
        {
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

            watcher.Filter = "*.txt";
            watcher.EnableRaisingEvents = true;

            Console.ReadLine();
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
            Task.Run(() =>
            {
                MoveFile(e.FullPath);
            });
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
            Task.Run(() =>
            {
                MoveFile(e.FullPath);
            });
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
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
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }

}
