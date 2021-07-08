using System;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using CsvHelper;

namespace NetBLogger
{
    class Program
    {
        static int Main(string[] args)
        {                        
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"Logs\\NetBApp_{DateTime.Now.ToString("ddMMyyyy_HHmmss")}.log")
                .CreateLogger();

            Log.Information("Starting NetBLogger now");
            try
            {
                int nloop = 0;
            
                Log.Information("Initializing output CSV file");
                CsvHelper.Configuration.CsvConfiguration csvCfg = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    HasHeaderRecord = true,
                };

                var writer = new StreamWriter($"Logs\\SpeedLog_{DateTime.Now.ToString("ddMMyyyy_HHmmss")}.csv");
                var csv = new CsvHelper.CsvWriter(writer, csvCfg);
                csv.WriteHeader<SpeedTestResult>();

                Log.Information("Getting system interfaces");
                SystemInterfaces();
                
                for (nloop = 0; true; nloop++)
                {
                    csv.NextRecord();
                    Log.Information($"Performing speed tests at loop {nloop}");
                    var resTemp = PerformSpeedTest();
                    SpeedTestResult resultObj = new SpeedTestResult()
                    {
                        TimeStamp = resTemp.Item1,
                        Speed = resTemp.Item2
                    };
                    csv.WriteRecord(resultObj);
                    csv.Flush();
                    
                    Thread.Sleep(60000); //It is trash, i know. But it works for now.
                }
            }
            catch(Exception ex)
            {
                Log.Fatal("Exception Caught. Exiting application.");
                Log.Fatal(ex, ex.Message);
                return -1;
            }
            finally
            {
                Log.Information("Application closing");
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            throw new NotImplementedException();
        }

        public static (DateTime, double) PerformSpeedTest()
        {
            Log.Information("Running Fast.com");
            var taskSpeedFast = Task.Run(() => SpeedTest.Net.FastClient.GetDownloadSpeed(SpeedTest.Net.Enums.SpeedTestUnit.MegaBitsPerSecond));
            taskSpeedFast.Wait();
            var resultFast = taskSpeedFast.Result;
            Log.Information("Fast.com done");

            //Log.Information("Running SpeedTest.com");
            //var taskSpeedSTest = Task.Run(() => SpeedTest.Net.SpeedTestClient.GetDownloadSpeed(unit: SpeedTest.Net.Enums.SpeedTestUnit.MegaBitsPerSecond));
            //taskSpeedSTest.Wait();
            //var resultSTest = taskSpeedSTest.Result;
            //Log.Information("SpeedTest.com done");

            Log.Information($"Fast.com: {resultFast.Speed} {resultFast.Unit}");
            return (DateTime.Now, resultFast.Speed);
        }

        public static void SystemInterfaces()
        {
            Log.Information("Capturing all network interfaces");
            NetworkInterface[] pcInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            if (pcInterfaces.Length == 0)
            {
                Log.Information("There is no avaiable network interfaces.");
                throw new NoNetworkInterfacesException();
            }

            Log.Information($"Found {pcInterfaces.Length} network interfaces");

            foreach (var intf in pcInterfaces)
            {
                Log.Information($"\tInterface Name: {intf.Name}");
                Log.Information($"\t\tInterface ID: {intf.Id}");
                Log.Information($"\t\tInterface Type: {intf.NetworkInterfaceType}");
                Log.Information($"\t\tInterface Status: {intf.OperationalStatus}");
            }

            return;
        }

        public class SpeedTestResult
        {
            public DateTime TimeStamp { get; init; }
            public double Speed { get; init; }
        }
    }
}
