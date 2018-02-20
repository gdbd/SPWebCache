using System;
using NUnit.Framework;

namespace SPWebCache.Tests
{
    public class MemorySizeTests
    {
        [Test]
        public void GetWeb_Test()
        {
            var webUrl = "http://shp2016/";

            for (int i = 0; i < 10; i++)
            {
                var before = GC.GetTotalMemory(true);
                var web = WebCache.Instance.GetWeb(webUrl);
              //  var web = new SPSite(webUrl).OpenWeb().Url;
                var after = GC.GetTotalMemory(true);

                Console.WriteLine((after - before)/(double) 1024/1024 + " MB");
            }
        }
    }
}