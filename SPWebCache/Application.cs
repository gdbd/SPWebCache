using System;
using System.Threading;
using System.Web;
using SPWebCache.Core;

namespace SPWebCache
{
    public class Application : Microsoft.SharePoint.ApplicationRuntime.SPHttpApplication
    {
        private static IHttpModule _perRequestCacheModule = new HttpModule();

        protected void Application_Start(object sender, EventArgs e)
        {
            var thread = new Thread(RequestQueue.Create);
            thread.Start();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            RequestQueue.Instance.WebFactory.Dispose();
        }

        public override void Init()
        {
            base.Init();
            _perRequestCacheModule.Init(this);
        }
    }
}