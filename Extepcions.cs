namespace HappyFarm
{
    public class WithoutOpenPort : Exception
    {
        public WithoutOpenPort(string id) : base($"Отсутствует открытый порт для устройства Id:{id}") { }
    }
}
