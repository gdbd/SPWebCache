using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SPWebCache.Features.WebCache
{
    [Guid("f16ccded-e993-43f5-ae7d-98e27f47997a")]
    public class WebCacheEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var webApp = (SPWebApplication)properties.Feature.Parent;

            foreach (SPSite site in webApp.Sites)
            {
                try
                {
                    AddStatusLinkCustomAction(site);
                }
                finally
                {
                    site.Dispose();
                }
            }

            foreach (var zone in webApp.IisSettings.Keys)
            {
                var setting = webApp.IisSettings[zone];
                var path = setting.Path.FullName;

                var origFilePath = Path.Combine(path, "global.asax");
                var bakFilePath = Path.Combine(path, "global.asax.bak");
                File.Copy(origFilePath, bakFilePath, true);
                using (var sw = File.CreateText(origFilePath))
                {
                    sw.WriteLine("<%@ Assembly Name=\"{0}\" %> ", Assembly.GetExecutingAssembly().FullName);
                    sw.WriteLine("<%@ Application Inherits=\"{0}\" %>", typeof(Application).FullName);
                }
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var webApp = (SPWebApplication)properties.Feature.Parent;

            foreach (SPSite site in webApp.Sites)
            {
                try
                {
                    RemoveStatusLinkCustomAction(site);
                }
                finally
                {
                    site.Dispose();
                }
            }

            foreach (var zone in webApp.IisSettings.Keys)
            {
                var setting = webApp.IisSettings[zone];
                var path = setting.Path.FullName;

                var modifiedFilePath = Path.Combine(path, "global.asax");
                var origFilePath = Path.Combine(path, "global.asax.bak");

                File.Copy(origFilePath, modifiedFilePath, true);
            }
        }

        private static void AddStatusLinkCustomAction(SPSite site)
        {
            var caStatus = site.UserCustomActions.FirstOrDefault(ca => ca.Name == "WebCacheStatus");
            if(caStatus != null) return;

            caStatus = site.UserCustomActions.Add();
            caStatus.Name = "WebCacheStatus";
            caStatus.Location = "Microsoft.SharePoint.SiteSettings";
            caStatus.Group = "CAG_SPWebCache";
            caStatus.Url = "/_layouts/SPWebCache/status.aspx";
            caStatus.Title = "Статус кэша объектов";
            caStatus.Update();
        }

        private static void RemoveStatusLinkCustomAction(SPSite site)
        {
            var caStatus = site.UserCustomActions.FirstOrDefault(ca => ca.Name == "WebCacheStatus");

            if (caStatus != null) caStatus.Delete();
        }
    }
}
