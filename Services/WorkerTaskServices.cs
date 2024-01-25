namespace HappyFarm.Services
{
    public class WorkerTaskServices
    {
        public List<IPost> _posts = new List<IPost>();

        public WorkerTaskServices()
        {
            
        }

        public void AddToTrack(IPost post) => _posts.Add(post);

        public IEnumerable<IPost> Take()
        {
            throw new NotImplementedException();
        }
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
