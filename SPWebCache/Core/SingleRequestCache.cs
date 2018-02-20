using System;
using System.Security.Principal;
using System.Web;
using Microsoft.SharePoint;

namespace SPWebCache.Core
{
    internal class SingleRequestCache : MultiRequestCache
    {
        public SingleRequestCache()
        {
            SiteMap.Build();
        }

        internal WebFactory WebFactory = new WebFactory();

        internal override Cache GetWebCore(Request request)
        {
            if (IsRunedUnderElevetedPrivilegies(request))
            {
                request.Identity = WindowsIdentity.GetAnonymous();
            }

            SPSite site = null;
            SPWeb web = null;
            Exception ex = null;

            // при использовании кеша на один запрос нужно различать объекты созданные предыдущим запросом
            // для этого к хешу объекта добавляется хеш запроса
            if (HttpContext.Current != null)
            {
                request.RequestHash = HttpContext.Current.Request.GetHashCode();
            }

            request.CompleteCallback = (s, w, e) =>
            {
                site = s;
                web = w;
                ex = e;
            };
           
            SiteMap.WaitForBuild();
            WebFactory.ProccessRequest(request);

            if (ex != null) throw ex;

            return new Cache(site, web);
        }

        public override void Dispose()
        {
            WebFactory.Dispose();
        }
    }
}
