
using SoM.Ipc;
using System.Diagnostics.Contracts;

namespace SrvConsole;
class Program
{


    public static void Main(string[] args)
    {
        Console.WriteLine("hi");
        //NovitusDto.DemoIpcB msg2 = new NovitusDto.DemoIpcB();

        var builder = Host.CreateApplicationBuilder(args);
        // builder.Services.AddWatsonTcpConnector();
        // builder.Services.AddKeyedTransient<SoM.Ipc.IServerConnector, SoM.Ipc.WatsonTcpServerConnector>("WatsonTcp");

        builder.Services.AddKeyedSoMIpcWatsonTcpService("MyIpc1",
            (options) =>
            {
                options.Uri = "127.0.0.1:50543";
            });
        
        builder.Services.AddHostedService<Worker>();
        

        var host = builder.Build();
        host.Run();

    }
}