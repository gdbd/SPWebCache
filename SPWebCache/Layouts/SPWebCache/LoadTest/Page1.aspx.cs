using System;
using System.Web;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;

namespace SPWebCache.Layouts.SPWebCache.LoadTest
{
    public partial class Page1 : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            var currentUrl = SPUtility.ConcatUrls("http://",
                SPUtility.ConcatUrls(context.Request.Url.Authority, context.Request.RawUrl));

            using (var web = WebCache.Instance.GetWeb(currentUrl))
            {
                var lc = web.Web.Lists.Count - 1;
                var r = new Random();
                var list = web.Web.Lists[r.Next(0, lc)];

                Lbl1.Text = list.Title;
            }
        }
    }
}
