using System;
using System.Web;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SPWebCache.Layouts.SPWebCache.LoadTest
{
    public partial class Page2 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            var currentUrl = SPUtility.ConcatUrls("http://",SPUtility.ConcatUrls(context.Request.Url.Authority, context.Request.RawUrl + "?fromcache=1"));

            var cache = WebCache.Current.GetWeb(SPContext.Current.Site.ID, SPContext.Current.Web.ID);
          //  var cache = WebCache.Current.GetWeb(currentUrl);

            Lbl1.Text = ((SPWeb) cache).Url;
        }
    }
}
