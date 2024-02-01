namespace HappyFarm.Models
{
    public interface IPost
    {
        string Link { get; }
        DateTime Created { get; }
    }

    public class TelegramPost : IPost
    {
        public string Link { get; set; }
    }
}