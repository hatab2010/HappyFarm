using HappyFarm.Models;
using OpenQA.Selenium.DevTools.V118.Network;
using System.Management;

namespace HappyFarm.Services
{

    public class SheduleServices
    {
    }

    public class WorkerServices
    {
    }

    public class DeviceServices
    {
        public int freePort = 9055;
        public event Action<Device> Attached;
        public event Action<Device> Detached;

        public List<Device> Devices { private set; get; } = new List<Device>();

        public DeviceServices()
        {
            ManagementEventWatcher attachWatcher =
                new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2"));
            ManagementEventWatcher detachWatcher =
                new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3"));

            attachWatcher.EventArrived += new EventArrivedEventHandler(OnUSBDeviceConnected);
            detachWatcher.EventArrived += new EventArrivedEventHandler(OnUSBDeviceDisconnected);


            attachWatcher.Start();
            detachWatcher.Start();

            //PairDevices();
        }

        private void OnUSBDeviceDisconnected(object sender, EventArrivedEventArgs e)
        {
            Thread.Sleep(1000); //TODO Исправить триггер
            RefrashActiveDevices();
        }

        private void OnUSBDeviceConnected(object sender, EventArrivedEventArgs e)
        {
            Thread.Sleep(1000); //TODO Исправить триггер
            PairDevices();
        }

        public void PairDevices()
        {
            var devicesIds = Adb.Devices();

            foreach (var deviceId in devicesIds)
            {
                if (Devices.FirstOrDefault(_ => _.Id == deviceId) == null)
                {
                    //Output.WriteLine($"Найдено новое устройство id:{deviceId}", ConsoleColor.Green);
                    var device = new Device { Id = deviceId };
                    Devices.Add(device);
                    Adb.ForwardPort(freePort, 9077, device.Id); //TODO добавить проверку на свободный порт
                    device.TetharingPort = freePort;
                    freePort++;
                    Attached?.Invoke(device);
                }
            }
        }

        public void RefrashActiveDevices()
        {
            var devicesIds = Adb.Devices();
            var removeDevices = new List<Device>();

            foreach (var device in Devices)
            {
                if (devicesIds.FirstOrDefault(_ => _ == device.Id) == null)
                {
                    //Output.WriteLine($"Устройство отключено id:{device.Id}", ConsoleColor.Red);
                    Detached?.Invoke(device);
                    removeDevices.Add(device);
                }
            }

            foreach (var device in removeDevices)
                Devices.Remove(device);
        }

        internal void NetworkReconnect(string id)
        {
            var device = Devices.FirstOrDefault(x => x.Id == id);

            if (device == null)
            {
                //Output.WriteLine($"Устройство с {id} не обноружено", ConsoleColor.Red);
                return;
            }

            Adb.SetAirplaneMode(true, id);
            Adb.SetAirplaneMode(false, id);
        }
    }

    public interface IDevice
    {
        string Id { get; }
        public int? TetharingPort { get; }

        void SwitchIp();
    }

    public class Device : IDevice
    {
        public string Id { get; set; }
        public int? TetharingPort { get; set; }
        //public string ProxyServer => $"127.0.0.1:{TetharingPort ?? throw new WithoutOpenPort(Id)}";

        public void SwitchIp()
        {
            Adb.SetAirplaneMode(true, Id);
            Adb.SetAirplaneMode(false, Id);
        }
    }
}
