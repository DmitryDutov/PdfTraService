using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PdfTraService.Models;
using PdfTraService.Services.Listener;
using PdfTraService.Services.ParserService;
using PdfTraService.Services.TaskQeue;
using Serilog;
using PdfTraService.Extensions.HostExtensions;

namespace PdfTraService
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var builder = new HostBuilder()
                .ConfigureLogging(confLogging =>
                {
                    //Для Serilog создаём отдельную конфигурацию и настраиваем чтение настроек из файла JSON
                    var configBuilder = new ConfigurationBuilder()
                        .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "config.json"), false, true)//Нужно для чтения файла конфигурации при работе в режиме сервиса
                        .AddCommandLine(args)
                        .Build(); 

                    //Настраиваем logger и передаём ему конфигурацию Serilog
                    Log.Logger = new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .ReadFrom.Configuration(configBuilder)
                        .CreateLogger();

                    confLogging.ClearProviders(); //Отключаем все логгеры
                    confLogging.AddSerilog(Log.Logger); //Включаем Serilog
                })
                .ConfigureServices(services =>
                {
                    Settings.Init();
                    //Регистрируем все сервисы приложения
                    services.AddHostedService<ListenerFactory>();
                    services.AddHostedService<ParserManager>();

                    //services.AddSingleton<Settings>();
                    services.AddSingleton<EqpQueue>();
                });

            await builder.RunService();
        }
    }
}

