using SoM.Ipc;
using SoM.Ipc.Utils;

namespace SrvConsole;

public class Worker(ILogger<Worker> logger,
    SoMIpcServices _ipcRepo


    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ipcRepo.Get("MyIpc1");  // <-- initializes and starts the IPC server

        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                //logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            Console.Write(".");
            await Task.Delay(1000, stoppingToken);
        }
    }
}
