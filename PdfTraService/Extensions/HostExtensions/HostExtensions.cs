using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace PdfTraService.Extensions.HostExtensions
{
    //Замена стадартного метода расширения, чтобы можно было реализовать выбор режима запуска
    public static class HostExtensions
    {
        public static Task RunService(this IHostBuilder hostBuilder)
        {
            return hostBuilder.RunConsoleAsync();
        }
    }
}

