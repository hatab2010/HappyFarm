using OpenQA.Selenium;
using System.Diagnostics;

namespace HappyFarm
{
    public static class SeleniumExtensions
    {
        public static void EnsureGoToUrl(this IWebDriver driver, Uri uri, int second = 20)
        {
            var timer = Stopwatch.StartNew();
            bool isFirst = true;

            while (true)
            {
                try
                {
                    if (isFirst)
                        driver.Navigate().GoToUrl(uri);
                    else
                        driver.Navigate().Refresh();

                    isFirst = false;

                    break;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(200);

                    if (timer.ElapsedMilliseconds > second * 1000)
                    {
                        timer.Stop();
                        throw ex;
                    }
                }
            }
        }
    }
}
