using System.Collections.Concurrent;
using HappyFarm.Data.Models;
using HappyFarm.JSONs;
using HappyFarm.Models;
using HappyFarm.Services.Base;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;

namespace HappyFarm.Services
{
    public class WokerTask
    {
        public DateTime Created { get; private set; }
        public IPost Post { get; private set; }
        public ITpmInterval TpmInterval { get; private set; }
        public IViewInterval ViewInterval { get; private set; }

        public WokerTask
            (
                ITpmInterval tpmInterval,
            )
        {
            Created = DateTime.Now;
        }
    }

    public interface IWorkerTaskTicket
    {

    }

    public class WokerTaskTicket : IWorkerTaskTicket
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int RequiredViewCount { get; set; }
        public int ReleaseViewCount { get; set; }
        public IPost Post { get; set; }
    }

    public class WorkerTaskServices : ScopeContextBase
    {
        private SheduleManager _sheduleManager = new SheduleManager();
        private object _locker = new object();
        private WorkerShedule _workerShedule;
        private IEnumerable<IViewInterval> _viewIntervals;
        private List<WokerTaskTicket> _WorekerTsks = new List<WokerTaskTicket>();

        public WorkerTaskServices(IServiceProvider provider) : base(provider) 
        {
            _sheduleManager.SetTestShedule(); //TODO Мок
        }

        public void AddToTrack(IPost post)
        {
            ScopeContext((context) =>
            {
                var ctxPost = context.Posts.FirstOrDefault(_ => _.Link == post.Link.ToString());
                if (ctxPost != null) return;

                var newPost = new Post
                {
                    Id = Guid.NewGuid(),
                    Link = post.Link,
                    Created = DateTime.Now
                };

                context.Posts.Add(newPost);
                context.SaveChanges();
            });
        }
        public IEnumerable<IWorkerTaskTicket> GetCurrentTask()
        {
            throw new NotImplementedException();
        }
        public void SetTpmShedule(IEnumerable<ITpmInterval> tpmIntervals)
        {
            lock(_locker)
                _tpmIntervals = tpmIntervals;
        }
        public void SetViewIntervals(IEnumerable<IViewInterval> viewIntervals)
        {
            lock (_locker)
                _viewIntervals = viewIntervals;
        }

        private void GenerateWokerTasks(IPost post)
        {
            var dateTimeNow = DateTime.Now;
            var tpmInterval = _sheduleManager.GetCurrentInterval();

            if (tpmInterval == null)
                return;

            var offset = (dateTimeNow - post.Created).TotalMicroseconds;
            var intervalOffset = 0L;
            var startDate = post.Created;

            foreach (var interval in _viewIntervals)
            {
                intervalOffset += interval.Duration;
                if (offset > intervalOffset)
                    continue;


                var endDate = startDate.AddMilliseconds(interval.Duration);
                ///
                lock (_locker)                
                    _WorekerTsks.Add(new WokerTaskTicket
                    {
                        Start = startDate,
                        End = endDate,
                        Id = Guid.NewGuid(),
                        Post = post,
                        ReleaseViewCount = (int)Math.Floor(interval.Power * tpmInterval.TpmCapacity)
                    });
                

                startDate = endDate;
            }
        }
    }

    public class BotServices : ScopeContextBase, IDisposable
    {
        private object _locker = new object();
        private BlockingCollection<IPost> _posts = new BlockingCollection<IPost>();
        private WokerServices _workerServices;
        private SheduleManager _sheduleManager;
        private IHostEnvironment _host;
        private IServiceProvider _provider;
        
        private bool _dispocer;

        public BotServices
            (
                WokerServices _bot, 
                IServiceProvider provider,
                IHostEnvironment host
            ) : base(provider)
        {
            _host = host;
            _sheduleManager = new SheduleManager();
            _workerServices = _bot;

            //TODO блок с моками
            LoadViewShedule();
            _sheduleManager.SetTestShedule(); //Создаю тестовый график нагрузок
        }

        public void AddToWatch(IPost post)
        {

        }

        private IViewInterval GetViewInterval(IPost post)
        {
            var offset = (DateTime.Now - post.Created).TotalMicroseconds;
            IViewInterval result = null;
            long intervalDuration = 0;
            foreach (var viewInterval in _viewIntervals)
            {
                intervalDuration += viewInterval.Duration;
                if (offset > intervalDuration)
                    continue;
                else
                    result = viewInterval;
            };

            return result;
        }

        private void Produce()
        {
            var isProduce = true;

            while (isProduce)
            {
                try
                {
                    var interval = _sheduleManager.GetCurrentInterval();
                    ScopeContext((context) =>
                    {                        

                    });
                }
                catch (Exception)
                {


                }

                lock(_locker)
                    isProduce = _dispocer == false;
            }
        }


        public void Dispose()
        {
            lock(_locker)
                _dispocer = true;
        }

        private void LoadViewShedule()
        {
            var viewShedulePath = Path.Combine(_host.ContentRootPath, "viewShedule.json");
            _viewIntervals =  JsonConvert.DeserializeObject<List<IViewInterval>>(File.ReadAllText(viewShedulePath)) ??
                throw new Exception();
        }
    }

    public class WorkerShedule
    {
        public DateTime StartDate { get; set; }
        public List<TpmInterval> Intervals { get; set; }
    }

    public interface ITpmInterval
    {
        int TpmCapacity { get; }
        long Duration { get; }
    }

    public class TpmInterval
    {
        public int TpmCapacity { get; set; }
        public long Duration { get; set; }
    }

    public class ViewInterval
    {
        public float Power { get; set; }
        public long Duration { get; set; }
    }

    public interface ITaskValidator
    {
        Task Validate(ITask post);
    }

    public interface IWatchTask
    {
        public IPost Post { get; set; }
    }

    public interface ITask
    {

    }

    public class WatchPostTask : ITask
    {
        ITaskValidator _validator;
        public WatchPostTask(ITaskValidator validator)
        {
            _validator = validator;
        }

        public IPost Post { get; private set; }

        public void Confirm()
        {
            _validator.Validate(this);
        }
    }
}
