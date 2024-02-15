using Caesar_decoder_encoder.Infrastructure.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Caesar_decoder_encoder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost? _host;
        public static IHost HostInstance
        {
            get
            {
                return _host ??= CreateHost();
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            HostInstance.StartAsync();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            HostInstance.StopAsync();
            _host = null;
        }
        private static IHost CreateHost()
        {
            var builder = Host.CreateDefaultBuilder(Environment.GetCommandLineArgs());
            builder.UseContentRoot(Environment.CurrentDirectory)
                .ConfigureServices((context, services) =>
                {
                    services.Configure();
                });
            return builder.Build();
        }
    }
}
