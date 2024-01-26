using System.Collections.Concurrent;

namespace HappyFarm.Services
{
    public class WorkerTaskServices
    {
        public BlockingCollection<IPost> _posts = new BlockingCollection<IPost>();
        public List<TelegramShedule> Shedules { get; set; }

        private BotServices _botServices;
        private int _dayCountInterval { get; set; }

        public WorkerTaskServices(BotServices _bot)
        {
            _botServices = _bot;
        }

        public void AddToWatch(IPost post) => _posts.Add(post);
        public void LoadShedule()
        {
            throw new NotImplementedException();
        }

        private IEnumerable<TpmDivision>? GetExistDevision()
        {
            throw new NotImplementedException();
        }

        private void Produce()
        {
            var divisions = GetExistDevision();

            if (divisions == null || divisions.Count() == 0)
                return;

            foreach (var division in divisions) 
            { 

            }
        }

        private void CalculateTpm()
        {
        }
    }

    public class DivisionMeter
    {
        public int MyProperty { get; set; }
    }

    public class TelegramShedule
    {
        public string ChannelId { get; set; }
        public List<TpmDivision> Divisions { get; set; }
    }

    public class TpmDivision
    {
        TelegramShedule _shedulel;
        public TpmDivision(TelegramShedule shedule)
        {
            
        }

        public int TpmCapacity { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
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

    public class WatchPost
    {
        public int Count { get; set; }
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
