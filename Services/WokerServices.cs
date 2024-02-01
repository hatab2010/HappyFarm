using HappyFarm.Models;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace HappyFarm.Services
{
    //https://t.me/test_rtest/21
    public class WokerServices : IDisposable
    {
        object _lock = new object();

        private DeviceServices deviceServices;
        private ConcurrentDictionary<string, Worker> _workersDictionary = new ConcurrentDictionary<string, Worker>();
        private List<IPost> _posts = new List<IPost>();
        private bool _dispoce;
        private bool _isStart;

        public WokerServices(DeviceServices _deviceServices)
        {
            _posts.Add(new TelegramPost()
            {
                Link = new Uri("https://t.me/test_rtest/21"),
                Created = DateTime.Now
            });

            _posts.Add(new TelegramPost()
            {
                Link = new Uri("https://t.me/mainchannelforuserbot/425"),
                Created = DateTime.Now
            });

            deviceServices = _deviceServices;
            deviceServices.Attached += OnDeviceAttached;
            deviceServices.Detached += OnDeviceDetached;

            deviceServices.PairDevices();

            _ = Task.Run(Start);
        }

        public void AddPost(IPost telegramPost)
        {
            lock(_lock)
                _posts.Add(telegramPost);
        }

        private void OnDeviceDetached(IDevice device)
        {
            var isWorkerExist = _workersDictionary.ContainsKey(device.Id);

            if (isWorkerExist == false) return;
            Worker worker;
            _workersDictionary.Remove(device.Id, out worker);
            worker.Dispose();
        }

        private void OnDeviceAttached(IDevice device)
        {
            var isWorkerExist = _workersDictionary.ContainsKey(device.Id);

            if (isWorkerExist == true) return;

            var worker = new Worker(device);
            _workersDictionary.TryAdd(device.Id, worker);
        }

        public void Watch(IEnumerable<IPost> posts)
        {
            throw new NotImplementedException();
        }

        public async Task Start()
        {
            if (_isStart)
                return;
            _isStart = true;

            while (_dispoce == false)
            {
                try
                {
                    List<Task> tasks = new List<Task>();

                    foreach (var worker in _workersDictionary)
                    {
                        tasks.Add(worker.Value.WatchPosts(_posts));
                    }

                    Task.WaitAll(tasks.ToArray());

                    await Task.Delay(10);
                }
                catch
                {

                }

            }
        }

        public void Dispose()
        {
            _dispoce = true;
        }
    }
}
