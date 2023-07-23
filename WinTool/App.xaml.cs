using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WinTool.Internal;
using WinTool.Models.MyMap;

namespace WinTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        void FirstDbOperation()
        {
            Task.Run(() =>
            {
                try
                {
                    using var dbContext = new WinDbContext();
                    dbContext.MyPasswords.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    Log.Error($"FirstDbOperation error, {ex.Message}\n{ex.StackTrace}");
                }
            });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                            .WriteTo.File("logs/log-.log",
                            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                            rollingInterval: RollingInterval.Day)
                            .CreateLogger();
            FirstDbOperation();

            try
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "RegionData", "ChinaBoundary");
                var text = File.ReadAllText(filePath);
                var obj = JsonConvert.DeserializeObject<China>(text);
                Caller.ChinaRegion = obj;
            }
            catch (Exception ex)
            {
                Log.Error($"Load China region data error, {ex.Message}\n{ex.StackTrace}");
            }

            Log.Information($"application startup...");

        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error($"error happened:{e.Exception.Message}\n{e.Exception.StackTrace}");
            e.Handled = true;
        }
    }
}
