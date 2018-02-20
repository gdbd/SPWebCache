using System;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;

namespace SPWebCache.Layouts.SPWebCache
{
    public partial class TestPage2 : LayoutsPageBase
    {
        Cache web;
        Cache webElevated;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (web != null)
            {
                LblHash.Text = "Web: " + web.GetHashCode();
            }

            base.OnPreRender(e);
            if (webElevated != null)
            {
                LblHash.Text = "ElevatedWeb: " + webElevated.GetHashCode();
            }
        }

        protected void TestButtonClick(object sender, EventArgs e)
        {
            web = WebCache.Current.GetWeb(SPContext.Current.Web.Url);
            var  web1 = WebCache.Current.GetWeb(SPContext.Current.Web.Url);

            if(!web.Equals(web1)) throw new Exception();

            LblTest.Text = web.Web.AllowUnsafeUpdates + string.Empty;
        }

        protected void TestButton2Click(object sender, EventArgs e)
        {
            webElevated = WebCache.Current.GetElevatedWeb(SPContext.Current.Web.Url);
            LblTest.Text = webElevated.Web.Author + string.Empty;
        }

        protected void TestButton3Click(object sender, EventArgs e)
        {
            var otherApp = SPFarm.Local.Services.OfType<SPWebService>()
                   .SelectMany(ws => ws.WebApplications)
                   .Where(app => !app.IsAdministrationWebApplication)
                   .FirstOrDefault(app => app.Id != SPContext.Current.Site.WebApplication.Id);

            if (otherApp == null)
            {
                LblTest.Text = "No other apps found ((";
                return;
            }

            using (var s = otherApp.Sites[0])
            {
                using (var w = s.RootWeb)
                {
                    web = WebCache.Current.GetElevatedWeb(w.Url);
                }
            }
        }

        protected void TestButton4Click(object sender, EventArgs e)
        {
            web = WebCache.Current.GetWeb(SPContext.Current.Web.Url);

            LblTest.Text = web.Site.AllowUnsafeUpdates + string.Empty;
        }
    }
}
