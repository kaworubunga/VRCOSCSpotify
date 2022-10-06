using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharpOSC;

namespace OSC_Funnies
{
    class Program
    {
        public static UDPSender oscSender;

        //public static PerformanceCounter cpuCounter;
        //public static PerformanceCounterCategory gpuCounterCategory;
        //public static ManagementObjectSearcher mgmtObjectsearcher;

        //public static void InitPerformaceCounters()
        //{
        //    cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        //    gpuCounterCategory = new PerformanceCounterCategory("GPU Engine");

        //    ObjectQuery wql = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
        //    mgmtObjectsearcher = new ManagementObjectSearcher(wql);
        //}

        //public static double GetCPUUsage()
        //{
        //    return Math.Round(cpuCounter.NextValue());
        //}

        //public static double GetGPUUsage()
        //{ 
        //    var gpuUsage = gpuCounterCategory
        //            .GetInstanceNames()
        //            .Where(counterName => counterName.EndsWith("engtype_3D"))
        //            .SelectMany(counterName => gpuCounterCategory.GetCounters(counterName))
        //            .Where(counter => counter.CounterName.Equals("Utilization Percentage"))
        //            .ToList()
        //            .Sum(x => x.NextValue());

        //    return Math.Round(gpuUsage);
        //}

        //public static float GetMemoryUsage()
        //{
        //    ManagementObjectCollection results = mgmtObjectsearcher.Get();
        //    var total = results["TotalVisibleMemorySize"];
        //    var free = results["FreePhysicalMemory"];
        //    var inuse = total - free;
            
        //    return Math.Round(inuse / total)
        //}


        public static string GetSpotifyMessage(bool flipFlop)
        {
            var SpotifyProcess = Process.GetProcessesByName("Spotify").FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle));
            if (SpotifyProcess == null)
            {
                LogUtils.Error("[Error] Spotify is not opened");
            }

            var songName = SpotifyProcess.MainWindowTitle;

            if(songName == "Spotify Free" || songName == "Spotify Premium" || songName == "Spotify" || songName == "Advertisement")
            {
                return "";
            }

            var pre = flipFlop ? '+' : '*';
            var suf = flipFlop ? '+' : '*';

            return $"{pre}{songName}{suf}";
        }

        //public static string GetPCStatsMessage()
        //{
        //    var cpuUsage = GetCPUUsage();
        //    var gpuUsage = GetGPUUsage();
        //    var memoryUsage = GetMemoryUsage();

        //    return $"CPU: {cpuUsage}% || GPU: {gpuUsage}% || MEM: {memoryUsage}%";
        //}

        public static Task Spotify()
        {
            string songName = "";
            var flipFlop = true;
            while (true)
            {
                flipFlop = !flipFlop;
                songName = GetSpotifyMessage(flipFlop);
                //if(songName.Length > 0)
                //{
                    oscSender.Send(new OscMessage("/chatbox/input", songName, true));
                //}
                LogUtils.Log(songName);
                Thread.Sleep(1500);

            }
        }

        //public static Task PCStats()
        //{
        //    while(true)
        //    {
        //        oscSender.Send(new OscMessage("/chatbox/input", GetPCStatsMessage(), true, true));
        //        LogUtils.Log("Sent Stats");
        //        Thread.Sleep(500);
        //    }
        //}

        public static Task TextScroll(string message)
        {
            while (true)
            {
                oscSender.Send(new OscMessage("/chatbox/input", message, true));
                LogUtils.Log(message);
                message = String.Concat(message[1..], message.Substring(0,1));
                Thread.Sleep(1500);
            }
        }

        public static Task TextAnim(string message)
        {
            int symbolIdx = 0;
            string[] symbols = new string[4] {"!", "*", "<3", "+"};
            while (true)
            {
                symbolIdx = (symbolIdx + 1) % symbols.Length;
                string symbol = symbols[symbolIdx];
                oscSender.Send(new OscMessage("/chatbox/input", $"{symbol} {message} {symbol}", true));
                Thread.Sleep(1500);
            }
        }

        static void Main(string[] args)
        {
            LogUtils.Logo();
            oscSender = new UDPSender("127.0.0.1", 9000);

            LogUtils.Log("Options:\n1. Spotify || Text scroller");
            var userInput = Console.ReadLine();
            switch (userInput)
            {
                case "1":
                    Spotify();
                    break;
                default:
                    TextAnim(userInput);
                    break;
            }
        }
    }
}
