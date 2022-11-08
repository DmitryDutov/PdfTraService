using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PdfTraService.Models;
using PdfTraService.Models.Parsers;
using PdfTraService.Models.Parsers.Interface;
using PdfTraService.Services.TaskQeue;
using Serilog;

namespace PdfTraService.Services.ParserService
{
    public class ParserManager : BackgroundService
    {
        //Объявляем поля
        //private readonly ILogger<ParserManager> _logger;
        private readonly EqpQueue _queueService;
        //private readonly Settings _settings;

        private readonly List<Type> _parsers = new() //список, в который добавляем типы парсов
        {
            typeof(TestParser),
            typeof(LocalParser),
            typeof(ARL9900_n946_TimerParser),
        };

        //Конструктор
        public ParserManager(/*ILogger<ParserManager> logger, */EqpQueue queueService/*, Settings settings*/)
        {
            //_logger = logger;
            _queueService = queueService;
            //_settings = settings;

            Log.Warning("Инициализирован сервис ParserManager");
        }

        //Метод обязательный для BackgroundService
        protected override Task ExecuteAsync(CancellationToken cancel)
        {
            try
            {
                Log.Warning("Пытаюсь запустить сервис ParserManager");
                Task.Run(() =>
                    {
                        while (!cancel.IsCancellationRequested)
                        {
                            if (_queueService.DeviceQueue.Count > 0)
                            {
                                GetData(cancel);
                            }
                        }
                    }, cancel);
                Log.Warning("Сервис ParserManager успешно запущен");
            }
            catch (Exception e)
            {
                Log.Error($"Ошибка запуска сервиса ParserManager: {e.Message}");
            }
            return Task.CompletedTask;
        }

        //Метод, вынимающий объекты из очереди и отдающих их на обработку парсерам
        private void GetData(CancellationToken stoppingToken)
        {
            try
            {
                Log.Warning("Пытаюсь получить данные из очереди в парсер (сервис ParserManager)");
                Data data = _queueService.Dequeue(stoppingToken);
                var parser = GetParser(data.DataGuid);

                if (parser is not null)
                    parser.Process(data.Line);
                else
                    Log.Error($"DataGuid {data.DataGuid} отсутствует в списке парсеров (сервис ParserManager)");

                var count = _queueService.DeviceQueue.Count;
               Log.Warning( $"PARSER [{parser.Name}] DataGuid: {data.DataGuid} получил данные \n--DATA ==> {data.Line}--");

            }
            catch (Exception e)
            {
                Log.Error($"Ошибка получения данных из очереди в парсер (сервис ParserManager): {e.Message}");
            }
        }

        //Выбираем требуемый парсер по DataGuid и создаём его экземпляр
        private IParser GetParser(Guid guid)
        {
            var eqpSettings = Settings.CurrentSettings;
            var mainPath = Settings.MainPath;
            var path = mainPath + eqpSettings.FirstOrDefault(x => x.EqpGuid == guid)?.Path;
            var encoding = eqpSettings.FirstOrDefault(x => x.EqpGuid == guid)?.Encoding;
            var interval = eqpSettings.FirstOrDefault(x => x.EqpGuid == guid)?.Interval;
            var mask = eqpSettings.FirstOrDefault(x => x.EqpGuid == guid)?.Mask;

            IParser parser = null;
            try
            {
                Log.Warning($"Пытаюсь активировать парсер (сервис ParserManager). DataGuid: {guid}");
                foreach (var item in _parsers)
                {
                    parser = Activator.CreateInstance(item) as IParser;
                    if (parser != null && parser.ParserGuid == guid)
                    {
                        Log.Warning($"Активирован парсер [{parser.Name}], EqpGuid: {parser.ParserGuid}");
                        parser.PathLog = path;
                        parser.Encoding = encoding;
                        if (interval != null) parser.Interval = (int) interval;
                        parser.Mask = mask;

                        return parser;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Ошибка получения данных из очереди в парсер (сервис ParserManager): {e.Message}");
            }
            return parser;
        }
    }
}

