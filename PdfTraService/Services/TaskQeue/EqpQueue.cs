using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using PdfTraService.Models;
using Serilog;

namespace PdfTraService.Services.TaskQeue
{
    public class EqpQueue
    {
        public ConcurrentQueue<Data> DeviceQueue = new();
        //private readonly ILogger<EqpQueue> _logger;

        public EqpQueue(/*ILogger<EqpQueue> logger*/)
        {
            //_logger = logger;

            Log.Warning("Инициализирован сервис EqpQueue");
        }

        //Метод, добавляющий данные в очередь
        public async Task PutDataInQueue(CancellationToken cancel, Guid guid, string line)
        {
            await Task.Run(() =>
            {
                Data data = new Data() { DataGuid = guid, Line = line };
                if (!cancel.IsCancellationRequested)
                {
                    var count = DeviceQueue.Count;
                    DeviceQueue.Enqueue(data);
                    Log.Warning($"Строка добавлена в очередь, количество строк =  {DeviceQueue.Count} (сервис EqpQueue)");
                }
            }, cancel);
        }

        //Вынимаем из очереди первый поступивший элемент (далее по этому элементу другой сервер подберёт парсер)
        public Data Dequeue(CancellationToken cancel)
        {
            DeviceQueue.TryDequeue(out Data data);
            Log.Warning($"Строка изъята из очереди, количество строк =  {DeviceQueue.Count} (сервис EqpQueue)");

            return data;
        }
    }
}

