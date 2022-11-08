using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PdfTraService.Models;
using PdfTraService.Services.TaskQeue;
using Serilog;

namespace PdfTraService.Services.Listener
{
    public class ListenerFactory : BackgroundService
    {
        //Объявляем поля
        private readonly EqpQueue _queueService;
        //private readonly ILogger<ListenerFactory> _logger;
        //private readonly Settings _settings;

        //Получаем в конструктор нужные сервисы
        public ListenerFactory(EqpQueue queueService/*, ILogger<ListenerFactory> logger*//*, Settings settings*/)
        {
            _queueService = queueService;
            //_logger = logger;
            //_settings = settings;

            Log.Warning("Инициализирован сервис ListenerFactory");
        }

        //Метод, обязательный для BackgroundService
        protected override Task ExecuteAsync(CancellationToken cancel)
        {
            return Task.Factory.StartNew(() =>
            {
                GetSettings(cancel); //Запускаем метод считывающий конфиг-файл и запускающий экземпляры слушателей
            });
        }

        //Метод считывающий конфиг-файл и запускающий экземпляры слушателей
        public void GetSettings(CancellationToken cancel)
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
                            await RunInstance(cancel, item.Address, item.Port, item.EqpGuid, item.Name, item.Path,
                                item.Encoding);
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

        //Метод запускающий слушатели
        private async Task RunInstance(CancellationToken cancel, string address, int port, Guid guid, string name, string path, string encoding)
        {
            IPAddress ip = IPAddress.Parse(address);
            var server = new TcpListener(ip, port);
            try
            {
                server.Start();
                Log.Warning($"Запущен слушатель [{name}] ==> Address: {address}, Port: {port}, DataGuid: {guid}");

                while (!cancel.IsCancellationRequested)
                {
                    var client = await server.AcceptTcpClientAsync();
                    var stream = new StreamReader(client.GetStream(), Encoding.GetEncoding(encoding));
                    var line = await stream.ReadToEndAsync();

                    if (line != String.Empty)
                    {
                        Log.Warning($"LISTENER [{name}] принял данные: \n--LINE ==> {line}--");
                        await _queueService.PutDataInQueue(cancel, guid, line);
                    }
                    Log.Warning("Подключение закрыто. Ожидаю следующее ... (сервис ListenerFactory)");
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message.ToString());
            }
        }
    }
}

