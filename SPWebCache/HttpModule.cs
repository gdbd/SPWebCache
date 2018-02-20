using System;
using System.Web;

namespace SPWebCache
{
    public class HttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            var app = (Application) context;
            app.SharePointEndRequest += OnEndRequest;
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            var req = ((HttpApplication) sender).Request;

            if (!req.Path.EndsWith(".aspx") || !req.Path.EndsWith(".asmx") || !req.Path.EndsWith(".svc") || !req.Path.EndsWith(".ashx")) return;

            int maxretries = 10;
            RecursiveDispose(ref maxretries);
        }

        private void RecursiveDispose(ref int maxretries)
        {
            try
            {
                maxretries--;
                WebCache.CleanCurrent();
            }
            catch
            {
                if (maxretries <= 0) return;
                try
                {
                    RecursiveDispose(ref maxretries);
                }
                catch
                {
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
