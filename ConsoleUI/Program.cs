using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ConsoleUI.Services;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ConsoleUI
{
    class Program
    {
        public static ILogger log;
        private static IConfiguration _config;

        static void Main(string[] args)
        {
            //startup
            Startup();

            var rootPath = _config["VeamJobPath"];
            //var externalDrives = FileService.GetAllExternalDrives();
            var paths = FileService.GetPaths();
            //log.Information("External Drives Count:{count}",externalDrives.Count);
            Stopwatch sw = Stopwatch.StartNew();
            log.Information("Checking Path: {path}",$"{rootPath}");
            DirectoryInfo folder = new DirectoryInfo(rootPath);
            if(folder != null) log.Information("folder exist:{folder}",folder);
            var files = Directory.GetFiles(rootPath);
            foreach (var file in files)
            {
                log.Information("found file: {file}",file);
            }


            try
            {
                //delete everything from the drives
                //FileService.FormatExternalDrives(externalDrives);

                //Copy over files
                //FileService.CopyOverVeamFilesToDrives(externalDrives, rootPath);

                FileService.CopyOverVeamFilesToDrives(paths,rootPath);


                //finish up
                sw.Stop();
                log.Information($"Took {(sw.ElapsedMilliseconds / 1000) / 60} minutes to backup");
                log.Information($"Time Stop: {DateTime.Now}");
                log.Information("BACKUP WAS SUCCESSFUL");
                Thread.Sleep(10000);
            }
            catch(Exception ex)
            {
                log.Error(ex,"Something went wrong");
                Thread.Sleep(10000);
            }
            
        }




        private static void Startup()
        {
            var appStartDate = DateTime.Now;
            _config = CreateConfiguration();
            SetupLogger();
            log.Information("------------------------------------------------------------------------------------");
            log.Information("Veam Backup To External Drive Software");
            log.Information("Time Start: {now}", appStartDate);
        }
        private static void SetupLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("log.txt")
                .CreateLogger();
            log = Log.Logger;
        }
        private static IConfiguration CreateConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath((GetBasePath()))
                .AddJsonFile("appsettings.json", true, true);
            return configuration.Build();
        }
        private static string GetBasePath()
        {
            using var processModule = Process.GetCurrentProcess().MainModule;
            return Path.GetDirectoryName(processModule?.FileName);
        }
    }
    
}
