using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace HappyFarm.Services
{
    //https://t.me/test_rtest/21
    public class BotServices : IDisposable
    {
        object _lock = new object();

        private DeviceServices deviceServices;
        private ConcurrentDictionary<string, Worker> _workersDictionary = new ConcurrentDictionary<string, Worker>();
        private List<IPost> _posts = new List<IPost>();
        private bool _dispoce;
        private bool _isStart;

        public BotServices(DeviceServices _deviceServices)
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

    public class Worker : IDisposable
    {
        public IDevice Device { private set; get; }

        private List<IWebDriver> _drivers = new List<IWebDriver>();
        private ChromeOptions _options = new ChromeOptions();
        private ChromeDriverService _services = ChromeDriverService.CreateDefaultService();

        public Worker(IDevice device)
        {         
            Device = device;
            _options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
            _options.AddArgument($"--proxy-server=127.0.0.1:{Device.TetharingPort}");
        }

        public void Dispose()
        {
            foreach (var driver in _drivers)
                driver.Dispose();
        }

        public async Task WatchPosts(IEnumerable<IPost> posts)
        {
            List<Task> watchTasks = new List<Task>();

            Device.SwitchIp();
            var count = posts.Count() - _drivers.Count;

            if (posts.Count() > _drivers.Count)            
                foreach (var number in Enumerable.Range(0, posts.Count() - _drivers.Count))                
                    _drivers.Add(new ChromeDriver(_services, _options));
                

            foreach (var driver in _drivers)
                driver.Manage().Cookies.DeleteAllCookies();

            foreach (var index in Enumerable.Range(0,posts.Count()))            
                watchTasks.Add(Task.Run(()=> 
                { 
                    _drivers[index].EnsureGoToUrl(posts.ToArray()[index].Link);
                    Thread.Sleep(600); //Ожидание валидации просмотра
                }));

            Task.WaitAll(watchTasks.ToArray());
        }
    }

    public interface IPost
    {
        Uri Link { get; }
        DateTime Created { get; }
    }

    public class TelegramPost : IPost
    {
        public Uri Link { get; set; }
        public DateTime Created { get; set; }
    }
}
