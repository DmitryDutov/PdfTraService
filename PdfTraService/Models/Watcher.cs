using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;

namespace PdfTraService.Models
{
    public class Watcher
    {
        public string Name => _name;
        private string _name;

        private string _current;
        public string Current => _current;

        private string _target;
        public string Target => _target;

        private string _filter;
        public string Filter => _filter;

        private bool _checkFolder;
        public bool CheckFolder => _checkFolder;

        public Watcher(string name, string current, string target, string filter, bool checkFolder)
        {
            _name = name;
            _current = current;
            _target = target;
            _filter = filter;
            _checkFolder = checkFolder;
        }
        private async Task MoveFile(string currentFilePath)
        {
            var name = Path.GetFileName(currentFilePath);
            var targetDir = $"{Target}{name}";

            File.Move(currentFilePath, targetDir);

            Log.Information($"Watcher {Name} переместил файл в {targetDir}");
        }

        private void CheckDir()
        {
            Log.Information($"Запускаю проверку папки {Current}");
            var dir = Directory.GetFiles(Current);
            if (dir.Length != 0)
            {
                foreach (var item in dir)
                {
                    if (item.Contains(Filter.Replace("*", string.Empty)))
                    {
                        Console.WriteLine(item);
                        Task.Run(() =>
                        {
                            MoveFile(item);
                        });
                    }
                }
            }
        }

        public void WatchFile()
        {
            if (CheckFolder)
            {
                CheckDir();
            }

            Log.Information($"Watcher {Name} - запущен");
            try
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

                watcher.Filter = Filter;
                watcher.EnableRaisingEvents = true;

                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Log.Error($"{Name} ==> Ошибка указания пути: {ex.Message}");
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
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

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Log.Information(value);
            Task.Run(() =>
            {
                MoveFile(e.FullPath);
            });
        }

        private void OnDeleted(object sender, FileSystemEventArgs e) =>
            Log.Information($"Deleted: {e.FullPath}");

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            Log.Information($"Renamed:");
            Log.Information($"    Old: {e.OldFullPath}");
            Log.Information($"    New: {e.FullPath}");
            Task.Run(() =>
            {
                MoveFile(e.FullPath);
            });
        }

        private void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private void PrintException(Exception? ex)
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

