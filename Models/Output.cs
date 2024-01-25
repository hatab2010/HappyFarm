namespace HappyFarm.Models
{
    public static class Output
    {
        private static ConsoleColor _defaultColor = ConsoleColor.White;

        public static void SetDefaultColor(ConsoleColor color)
        {
            _defaultColor = color;
        }

        public static void WriteLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = _defaultColor;
        }
    }
}
