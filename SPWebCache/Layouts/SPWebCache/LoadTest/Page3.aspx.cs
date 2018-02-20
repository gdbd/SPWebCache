using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SPWebCache.Layouts.SPWebCache.LoadTest
{
    public partial class Page3 : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
          /*  var context = HttpContext.Current;
            var currentUrl = SPUtility.ConcatUrls("http://",
                SPUtility.ConcatUrls(context.Request.Url.Authority, context.Request.RawUrl));


            using (var site = new SPSite(currentUrl))
            {
                using (var web = site.OpenWeb())
                {
                    Lbl1.Text = web.Url;
                }
            }*/

          //  SPRequestModule

            var web = SPContext.Current.Web;


        }
    }
}
