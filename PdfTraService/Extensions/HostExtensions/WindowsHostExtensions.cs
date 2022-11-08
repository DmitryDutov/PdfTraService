using Microsoft.Extensions.Hosting;
using Severstal.LIMS.DeviceMonitoring.WindowsService;
using System;
using System.Threading.Tasks;

namespace Severstal.LIMS.DeviceMonitoring.Extensions.HostExtensions
{
    //Выбор режима запуска программы в зависимости от наличия интерактива
    public static class WindowsHostExtensions
    {
        public static async Task RunService(this IHostBuilder hostBuilder)
        {
            if (!Environment.UserInteractive)
            {
                await hostBuilder.RunAsServiceAsync();
            }
            else
            {
                await hostBuilder.RunConsoleAsync();
            }
        }
    }
}

