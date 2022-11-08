using System;
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
            var eqpSettings = Settings.CurrentSettings;

            try
            {
                Log.Warning("Пытаюсь активировать слушатели согласно файла настроек (сервис ListenerFactory)");
                foreach (var item in eqpSettings)
                {
                    if (item.Active)
                    {
                        //Запуск слушателей производим в отдельном потоке
                        new Thread(async () =>
                        {
                            await RunInstance(
                                cancel
                                //, 
                                //item.Address, 
                                //item.Port, 
                                //item.EqpGuid, 
                                //item.Name, 
                                //item.Path,
                                //item.Encoding
                                );
                        }).Start();
                    }
                }
                Log.Warning("Слушатели успешно активированы");
            }
            catch (Exception e)
            {
                Log.Error($"Ошибка активации слушателей: {e.Message}");
            }
        }

        private async Task RunInstance(CancellationToken cancel)
        {
            string name   = string.Empty;
            string current= string.Empty;
            string target = string.Empty;

            var watcher = new Models.Watcher(name, current, target);
            watcher.WacthFile();
        }
    }
}
