using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 0;
            bool searchByPort = args.FirstOrDefault() is string portStr && int.TryParse(portStr, out port);
            int? searchPort = searchByPort ? port : null;

            var result = YieldPortInfos(searchPort).OrderBy(x => x.LocalPort);
            foreach (var info in result)
                Console.WriteLine($@"""{info.LocalPort}"", ""{info.ProcessName}"", ""{info.Protocol}"", ""{info.State}"", ""{info.ForeignAddress}""");
        }

        private static IEnumerable<PortInfo> YieldPortInfos(int? port = null)
        {
            var processLookup = Process.GetProcesses().ToLookup(x => x.Id);
            foreach (var result in YieldNetstatPortInfos().Where(x => !port.HasValue || x.LocalPort == port.Value))
            {
                result.ProcessName = processLookup[result.ProcessId].FirstOrDefault()?.ProcessName ?? "Unknow";
                yield return result;
            }
        }

        private static string GetNetstatResult()
        {
            using Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "netstat",
                    Arguments = "-ano",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        private static IEnumerable<PortInfo> YieldNetstatPortInfos()
        {
            string output = GetNetstatResult();
            string[] lines = output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

            // 跳過標頭
            foreach (string line in lines.Skip(4))
            {
                string[] parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 1)
                    continue;

                (int localPort, string localAddress) = TryParseLocalEndPoint(parts[1]);
                yield return new PortInfo
                {
                    Protocol = parts[0],
                    LocalAddress = localAddress,
                    LocalPort = localPort,
                    ForeignAddress = parts[2],
                    State = parts[0] == "TCP" ? parts[3] : string.Empty,
                    ProcessId = int.TryParse(parts.Last(), out int pid) ? pid : 0
                };
            }
        }

        private static (int, string) TryParseLocalEndPoint(string localEndPoint)
        {
            return IPEndPoint.TryParse(localEndPoint, out IPEndPoint? ep)
                ? (ep.Port, ep.Address.ToString())
                : (0, string.Empty);
        }

        private class PortInfo
        {
            public string Protocol { get; set; } = string.Empty;

            public string LocalAddress { get; set; } = string.Empty;

            public int LocalPort { get; set; }

            public string ForeignAddress { get; set; } = string.Empty;

            public string State { get; set; } = string.Empty;

            public int ProcessId { get; set; }

            public string ProcessName { get; set; } = string.Empty;
        }
    }
}
