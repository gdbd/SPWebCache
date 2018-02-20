using Microsoft.SharePoint;
using NUnit.Framework;

namespace SPWebCache.Tests
{
    public class SiteMapTest
    {
        [Test]
        public void GetWebId_Test()
        {
            var webUrl1 = "http://shp2016/sites/1/w1";
            var site1 = new SPSite(webUrl1);
            var web1 = site1.OpenWeb();

            var webId = WebCache.SiteMap.GetWebId(webUrl1);

            Assert.That(webId, Is.EqualTo(web1.ID));
        }

        [Test]
        public void GetSiteId_Test()
        {
            var webUrl1 = "http://shp2016/sites/1/w1";
            var site1 = new SPSite(webUrl1);
            var web1 = site1.OpenWeb();

            var siteId = WebCache.SiteMap.GetSiteId(webUrl1);

            Assert.That(siteId, Is.EqualTo(site1.ID));
        }

        [Test]
        public void GetWebUrl_Test()
        {
            var webUrl1 = "http://shp2016/sites/1/w1";
            var site1 = new SPSite(webUrl1);
            var web1 = site1.OpenWeb();

            var url = WebCache.SiteMap.GetWebUrl(web1.ID);

            Assert.That(url, Is.EqualTo(web1.Url));
        }

        [Test]
        public void GetSiteUrl_Test()
        {
            var webUrl1 = "http://shp2016/sites/1/w1";
            var site1 = new SPSite(webUrl1);
            var web1 = site1.OpenWeb();

            var url = WebCache.SiteMap.GetSiteUrl(site1.ID);

            Assert.That(url, Is.EqualTo(site1.Url));
        }
    }
}
