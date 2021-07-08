using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Serilog;

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

            //Evaluating startup arguments
            //if (args.Length > 0)
            //{
                
            //}

            Log.Information("Starting NetBLogger now");
            try
            {
                Log.Information("Getting system interfaces");
                SystemInterfaces();
                Log.Information("Perfming speed tests");
                PerformSpeedTest();
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

            return 0;
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

            Log.Information($"Fast.com: {resultFast.Speed}");
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
    }
}
