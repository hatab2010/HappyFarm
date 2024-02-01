namespace HappyFarm.JSONs
{
    public interface IViewInterval
    {
        long Duration { get; }
        float Power { get; }
    }
    

    public class ViewIntervalJson : IViewInterval
    {
        public long Duration { get; set; }
        public float Power { get; set; }
    }
}
