using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using Microsoft.SharePoint;
using NUnit.Framework;
using SPWebCache.Common;
using SPWebCache.Core;


namespace SPWebCache.Tests
{
    public class MultiRequestTests
    {
        [Test]
        public void GetWeb_Throws_On_Incorrect_Parameter_Test()
        {
            Assert.Throws<Exception>(() => WebCache.Instance.GetWeb("asd-zxc"));
            Assert.Throws<Exception>(() => WebCache.Instance.GetWeb(Guid.NewGuid(), Guid.NewGuid()));
        }
        
        [Test]
        public void GetWeb_Test() 
        {
            //для очистки кеша от запусков предыдущих тестов
            RequestQueue.Instance.WebFactory.Dispose();

            var webUrl = "http://shp2016/sites/1";
            var web = WebCache.Instance.GetWeb(webUrl);
            var web1 = WebCache.Instance.GetWeb(webUrl);
            Assert.AreEqual(web, web1);
            
            var web8 = WebCache.Instance.GetWeb(web.Site.ID, web.Web.ID);
            Assert.AreEqual(web, web8);

            var webUrl2 = "http://shp2016/sites/2";
            var web4 = WebCache.Instance.GetWeb(webUrl2);
            var web5 = WebCache.Instance.GetWeb(webUrl2);
            Assert.AreEqual(web4, web5);
            Assert.AreNotEqual(web, web4);

            var webUrl3 = "http://shp2016/sites/1/w1";
            var web6 = WebCache.Instance.GetWeb(webUrl3);
            var web7 = WebCache.Instance.GetWeb(webUrl3);
            Assert.AreEqual(web6, web7);
            Assert.AreNotEqual(web, web6);
            Assert.AreNotEqual(web4, web6);
            
            var web2 = WebCache.Instance.GetElevatedWeb(webUrl);
            var web3 = WebCache.Instance.GetElevatedWeb(webUrl);
            Assert.AreEqual(web2, web3);
        //    Assert.AreNotEqual(web, web2);

            Assert.That(RequestQueue.Instance.WebFactory.Webs.Count, Is.EqualTo(3), "Webs");
            Assert.That(RequestQueue.Instance.WebFactory.Sites.Count, Is.EqualTo(2), "Sites");

            var userToken = System.Security.Principal.WindowsIdentity.GetCurrent().UserClaims
                .Where(c => c.Value.Contains("\\"));

            using (new ImpersonatedScope("userone@test.ru"))
            {
                var web9 = WebCache.Instance.GetWeb(webUrl);
                Assert.AreNotEqual(web9, web);
            }

            Assert.That(RequestQueue.Instance.WebFactory.Webs.Count, Is.EqualTo(4), "Webs(2)");
            Assert.That(RequestQueue.Instance.WebFactory.Sites.Count, Is.EqualTo(3), "Sites(2)");
        }

        [Test]
        public void GetWeb_Compare_With_Direct_Opened_Objects_Test()
        {
            var webUrl1 = "http://shp2016/sites/1/w1";
            var site1 = new SPSite(webUrl1);
            var web1 = site1.OpenWeb();

            var web11 = WebCache.Instance.GetWeb(webUrl1);

            Assert.AreEqual(web1.ID, web11.Web.ID);


            var webUrl2 = "http://shp2016/sites/1/w1/w11";
            var site2 = new SPSite(webUrl2);
            var web2 = site2.OpenWeb();

            var web22 = WebCache.Instance.GetWeb(webUrl2);

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

                var web11 = WebCache.Instance.GetWeb(webUrl1);
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
                    using (var web = WebCache.Instance.GetWeb("http://shp2016/sites/1"))
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
