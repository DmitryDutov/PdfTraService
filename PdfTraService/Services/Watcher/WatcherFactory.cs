using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PdfTraService.Models;
using Serilog;

namespace PdfTraService.Services.Watcher
{
    public class WatcherFactory : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken cancel)
        {
            return Task.Factory.StartNew(() =>
            {
                StartWatchers(cancel); //Запускаем метод считывающий конфиг-файл и запускающий экземпляры слушателей
            });
        }

        public void StartWatchers(CancellationToken cancel)
        {
            var settings = Settings.CurrentSettings;
            var mainPath = Settings.MainPath;

            try
            {
                Log.Information("Пытаюсь активировать экземпляры Wather'ов");
                foreach (var item in settings)
                {
                    if (item.Active)
                    {
                        //Запуск слушателей производим в отдельном потоке
                        new Thread(async () =>
                        {
                            await RunInstance(
                                cancel ,
                                item.Name,
                                Path.Combine(mainPath, item.Current),
                                item.Target,
                                item.Filter,
                                item.CheckFolder
                                );
                        }).Start();
                    }
                }
                Log.Information("Экземпляры активированы");
            }
            catch (Exception e)
            {
                Log.Error($"Ошибка активации экземпляров: {e.Message}");
            }
        }

        private async Task RunInstance(CancellationToken cancel, string name, string current, string target, string filter, bool checkFolder)
        {
            var watcher = new Models.Watcher(name, current, target, filter, checkFolder);
            watcher.WatchFile();
        }
    }
}
