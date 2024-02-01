using HappyFarm.Services;

namespace HappyFarm.Models
{
    public class SheduleManager
    {
        public SheduleManager()
        {
        }

        private TpmInterval? _currentInterval;
        private WorkerShedule? _workerShedule;

        public TpmInterval? GetCurrentInterval()
        {
            if (_workerShedule == null)
                return null;

            if (_currentInterval != null)
                return null;

            var dateInequality = (DateTime.Now - _workerShedule.StartDate).TotalMilliseconds;
            if (dateInequality < 0)
                return null;

            long offset = 0;

            foreach (var interval in _workerShedule.Intervals)
            {
                offset += interval.Duration;
                if (offset >= dateInequality)
                    _currentInterval = interval;
            }

            return _currentInterval;
        }

        public void SetTestShedule()
        {
            _workerShedule = new WorkerShedule();
            _workerShedule.StartDate = DateTime.Parse("28.01.2024");
            _workerShedule.Intervals = new List<TpmInterval>
            {
                new TpmInterval
                {
                    Duration = 28800000,
                    TpmCapacity = 300
                },
                new TpmInterval
                {
                    Duration = 28800000,
                    TpmCapacity = 300
                },
                new TpmInterval
                {
                    Duration = 28800000,
                    TpmCapacity = 300
                },
                new TpmInterval
                {
                    Duration = 28800000,
                    TpmCapacity = 300
                },
                new TpmInterval
                {
                    Duration = 28800000,
                    TpmCapacity = 300
                },
                new TpmInterval
                {
                    Duration = 28800000,
                    TpmCapacity = 300
                },
                new TpmInterval
                {
                    Duration = 28800000,
                    TpmCapacity = 300
                },
                new TpmInterval
                {
                    Duration = 28800000,
                    TpmCapacity = 300
                },
                new TpmInterval
                {
                    Duration = 28800000,
                    TpmCapacity = 300
                }
            };
        }
    }
}
