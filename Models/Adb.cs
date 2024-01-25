using System.Diagnostics;
using System.Text.RegularExpressions;

namespace HappyFarm.Models
{
    public static class Adb
    {
        private static Process GetProcess()
        {
            Process process = new Process();
            process.StartInfo.FileName = "adb";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            return process;
        }

        public static IEnumerable<string> Devices()
        { 
            List<string> deviceIds = new List<string>();

            using (var process = GetProcess())
            {
                process.StartInfo.Arguments = "devices";
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                var withoutDescription = output.Split(new char[] { '\r', }).Skip(1);

                foreach (var line in withoutDescription)
                {
                    var match = Regex.Match(line, @"([\d|\w]*).*device");

                    if (match.Success)
                        deviceIds.Add
                            (
                                Regex.Match(line, @"([\d|\w]*).*device").Groups[1].Value
                            );
                }

            }

            return deviceIds;
        }

        public static void SetDeviceOwner(string deviceId, string packageName, string adminReciver)
        {
            using (var process = GetProcess())
            {
                process.StartInfo.Arguments = $"-s {deviceId} shell dpm set-device-owner {packageName}/.{adminReciver}";
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                var t = output;
            }
        }

        public static List<ForwardPorts> GetForwardPorts(string deviceId)
        {
            var result = new List<ForwardPorts>();

            using (var process = GetProcess())
            {
                process.StartInfo.Arguments = $"-s {deviceId} forward --list";
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                var lines = output.Split(new char[] { '\r' });

                foreach (var line in lines)
                {
                    var math = Regex.Match(line, @"tcp:(\d+).*tcp:(\d+)");

                    if (!math.Success) continue;

                    result.Add(new ForwardPorts
                    {
                        Host = int.Parse(math.Groups[1].Value),
                        Client = int.Parse(math.Groups[2].Value)
                    });
                }

                process.WaitForExit();
            }

            return result;
        }

        public static void SetAirplaneMode(bool isEnable, string deviceId = null)
        {
            using (var process = GetProcess())
            {
                var mode = isEnable ? "enable" : "disable";
                var deviceLink = deviceId != null ? $"-s {deviceId}" : "";
                process.StartInfo.Arguments =
                    $"{deviceLink} shell cmd connectivity airplane-mode {mode}";
                process.Start();

                string output = process.StandardOutput.ReadToEnd(); //Success
                process.WaitForExit();
            }
        }

        public static void ForwardPort(int hostPort, int clientPort, string deviceId)
        {
            using (var process = GetProcess())
            {
                process.StartInfo.Arguments = $"-s {deviceId} forward tcp:{hostPort} tcp:{clientPort}";
                process.Start();

                string output = process.StandardOutput.ReadToEnd(); //Success
                process.WaitForExit();
            }
        }
    }

    public class ForwardPorts
    {
        public int Host { get; set; }
        public int Client { get; set; }
    }
}
