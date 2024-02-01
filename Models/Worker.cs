using HappyFarm.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace HappyFarm.Models
{
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

            foreach (var index in Enumerable.Range(0, posts.Count()))
                watchTasks.Add(Task.Run(() =>
                {
                    _drivers[index].EnsureGoToUrl(posts.ToArray()[index].Link);
                    Thread.Sleep(600); //Ожидание валидации просмотра
                }));

            Task.WaitAll(watchTasks.ToArray());
        }
    }
}
