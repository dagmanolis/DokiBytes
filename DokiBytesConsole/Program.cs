using DokiBytesCommon.Models;
using DokiBytesCore.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
namespace DokiBytesConsole
{
    class Program
    {

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);

            IConfiguration configuration = builder.Build();

            var services = new ServiceCollection()
                .AddSingleton(configuration)
                .AddSingleton<IFtpClient, FluentFtpClient>()
                .AddSingleton<IFolderMonitor, DefaultFolderMonitor>()
                .AddSingleton<IRemoteDev, RemoteDevService>()
                .BuildServiceProvider();

            var mainService = services.GetService<IRemoteDev>();
            mainService.FtpOperationFinished += OnFtpOperationFinished;
            mainService.Run();


            Console.WriteLine("Type 'q' to quit");
            while (Console.ReadLine() != "q")
            { }
            mainService.Dispose();
            services.Dispose();
        }

        private static void OnFtpOperationFinished(object sender, FtpOperationEventArgs e)
        {
            Console.WriteLine("{0} {1} {2}", DateTime.Now, e.Operation, e.RemoteFullPath);
        }
    }
}
