using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace PdfTraService.Models.Parsers
{
    public class ARL9900_n946_TimerParser : BaseParser
    {
        private List<ValueTuple<Guid, string, string>> _list = new();
        private bool TimerFlag { get; set; } = false;
        private Timer _timer;

        public ARL9900_n946_TimerParser()
        {
            ParserGuid = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2 }); //todo: разобраться с нумерацией Guid'ов
            Name = "ARL9900_n946_TimerParser";
        }

        public override bool Process(string data)
        {
            var time = DateTime.Now.ToString(CultureInfo.CurrentCulture); //todo: переделать на "красивый формат"
            var item = (ParserGuid, time, data);
            _list.Add(item);

            if (!TimerFlag)
            {
                TimerFlag = true;
                Log.Information("инициализирую таймер");

                _timer = new Timer
               (
                    (e) => ResultProcessing(),   //запускаемый метод
                    null,                                //состояние переменной "e"
                    TimeSpan.FromSeconds(Interval),           //время с момента создания таймера до запуска
                    TimeSpan.Zero                       //период вызова запускаемого метода
                );
            }

            return true;
        }
        private void ResultProcessing()
        {
            Log.Information("начинаю выполнение задачи по таймеру");
            _list = _list.Distinct().ToList();

            foreach (var item in _list)
            {
                Log.Information($"Номер элемента: {item.Item1}, Элемент: {item.Item3}");
            }
            var match = string.Empty;

            for (int i = 0; i < _list.Count; i++)
            {
                var data = _list[i].Item3;
                Log.Information($"Data = {data}");
                try
                {
                    //match = new Regex(_maskNull, (RegexOptions)531).Matches(data)[0].Groups[1].Value;
                    match = new Regex(Mask, (RegexOptions)531).Match(data).Value;
                    Log.Information(match != string.Empty ? $"Math = {match}" : "Нет совпадений с маской");
                }
                catch (Exception e)
                {
                    Log.Information($"Ошибка разбора маски...\nМаска: {Mask}; \nПараллель: {data};\nТекст ошибки{e.Message}");
                }

                if (match == string.Empty)
                {
                    string str = data + "\n";
                    if (data != string.Empty && data != "" && data != " " && data is not null)
                    {
                        Task.Factory.StartNew(async () =>
                        {
                            //File.AppendAllText(PathLog, str);
                            await Write(str);
                        }, TaskCreationOptions.AttachedToParent);
                        Log.Information("данные записаны в файл");
                    }
                }
            }

            TimerFlag = false;
            _timer.Dispose();
            Log.Information("Таймер остановлен");
            try
            {
                _list.Clear();
            }
            catch (Exception e)
            {
                Log.Error($"Ошибка очистки списка параллелей в парсере {Name}.\nТекст ошибки: {e.Message}");
            }
        }

        private async Task Write(string result)
        {
            using (var writer = new StreamWriter(PathLog, true, System.Text.Encoding.GetEncoding(Encoding)))
            {
                await writer.WriteLineAsync(result);
            }
        }
    }
}

