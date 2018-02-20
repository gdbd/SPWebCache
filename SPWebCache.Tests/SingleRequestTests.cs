using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.SharePoint;
using NUnit.Framework;
using SPWebCache.Common;
using SPWebCache.Core;


namespace SPWebCache.Tests
{
    public class SingleRequestTests
    {
        [Test]
        public void GetWeb_Throws_On_Incorrect_Parameter_Test()
        {
            Assert.Throws<Exception>(() => WebCache.Current.GetWeb("asd-zxc"));
            Assert.Throws<Exception>(() => WebCache.Current.GetWeb(Guid.NewGuid(), Guid.NewGuid()));
        }
        
        [Test]
        public void GetWeb_Test() 
        {
            var webUrl = "http://shp2016/sites/1";
            var web = WebCache.Current.GetWeb(webUrl);
            var web1 = WebCache.Current.GetWeb(webUrl);
            Assert.AreEqual(web, web1);

            var web8 = WebCache.Current.GetWeb(web.Site.ID, web.Web.ID);
            Assert.AreEqual(web, web8);

            var webUrl2 = "http://shp2016/sites/2";
            var web4 = WebCache.Current.GetWeb(webUrl2);
            var web5 = WebCache.Current.GetWeb(webUrl2);
            Assert.AreEqual(web4, web5);
            Assert.AreNotEqual(web, web4);

            var webUrl3 = "http://shp2016/sites/1/w1";
            var web6 = WebCache.Current.GetWeb(webUrl3);
            var web7 = WebCache.Current.GetWeb(webUrl3);
            Assert.AreEqual(web6, web7);
            Assert.AreNotEqual(web, web6);
            Assert.AreNotEqual(web4, web6);

            var web2 = WebCache.Current.GetElevatedWeb(webUrl);
            var web3 = WebCache.Current.GetElevatedWeb(webUrl);
            Assert.AreEqual(web2, web3);

            var rc = (SingleRequestCache)WebCache.Current;
            

            Assert.That(rc.WebFactory.Webs.Count, Is.EqualTo(3), "Webs");
            Assert.That(rc.WebFactory.Sites.Count, Is.EqualTo(2), "Sites");

            using (new ImpersonatedScope("userone@test.ru"))
            {
                var web9 = WebCache.Current.GetWeb(webUrl);
                Assert.AreNotEqual(web9, web);
            }

            Assert.That(rc.WebFactory.Webs.Count, Is.EqualTo(4), "Webs(2)");
            Assert.That(rc.WebFactory.Sites.Count, Is.EqualTo(3), "Sites(2)");
        }

        [Test]
        public void GetWeb_Compare_With_Direct_Opened_Objects_Test()
        {
            var webUrl1 = "http://shp2016/sites/1/w1";
            var site1 = new SPSite(webUrl1);
            var web1 = site1.OpenWeb();

            var web11 = WebCache.Current.GetWeb(webUrl1);

            Assert.AreEqual(web1.ID, web11.Web.ID);


            var webUrl2 = "http://shp2016/sites/1/w1/w11";
            var site2 = new SPSite(webUrl2);
            var web2 = site2.OpenWeb();

            var web22 = WebCache.Current.GetWeb(webUrl2);

            Assert.AreEqual(web2.ID, web22.Web.ID);
        }

        [Test]
        public void Performance_Compare_Test()
        {
            int testsCount = 10000;
            var times = new long[testsCount];
            var times2 = new long[testsCount];

            var webUrl1 = "http://shp2016/sites/1/w1";

            var sw = new Stopwatch();

            for (int i = 0; i < testsCount; i++)
            {
                sw.Reset();
                sw.Start();

                var site1 = new SPSite(webUrl1);
                var web1 = site1.OpenWeb();
                var url = web1.Url;
                web1.Dispose();
                site1.Dispose();

                times[i] = sw.ElapsedMilliseconds;
            }

            for (int i = 0; i < testsCount; i++)
            {
                sw.Reset();
                sw.Start();

                var web11 = WebCache.Current.GetWeb(webUrl1);
                var url = web11.Web.Url;
                times2[i] = sw.ElapsedMilliseconds;
            }

            var average1 = times.Average();
            var average2 = times2.Average();

            Console.WriteLine("open new web avg: " + average1);
            Console.WriteLine("open cached web avg: " + average2);
            Console.WriteLine("factor: " + average1/average2);
        }

        [Test]
        public void Concurent_Operation_Test()
        {

            Func<Exception> a = () =>
            {
                try
                {
                    var web = WebCache.Current.GetWeb("http://shp2016/sites/1");
                    lock (web.Web)
                    {
                        web.Web.GetList("sites/1/lists/contract");
                    }
                    return null;
                }
                catch (Exception e)
                {
                    return e;
                }
            };

            var ars = new List<IAsyncResult>();

            for (int i = 0; i < 10; i++)
            {
                var r = a.BeginInvoke(ar =>{}, null);
                ars.Add(r);
            }

            foreach (var asyncResult in ars)
            {
                asyncResult.AsyncWaitHandle.WaitOne();
            }
            

            foreach (var asyncResult in ars)
            {
                var ex = a.EndInvoke(asyncResult);
                if (ex != null) throw ex;
            }
        }
    }
}
