using System;
using System.Linq;
using System.Security.Principal;
using Microsoft.SharePoint;
using SPWebCache.Common;

namespace SPWebCache.Core
{
    internal class ByGuidRequest : Request
    {
        public Guid SiteId;
        public Guid WebId;

        public override string RequestKey
        {
            get { return string.Format("SiteId:{0}, WebId:{1}", SiteId, WebId); }
        }

        public override string SiteUrlOfRequest()
        {
            return SiteMap.Sites
                .Where(su => su.Id == SiteId)
                .Select(h => h.Url)
                .FirstOrDefault();
        }

        public override string WebUrlOfRequest()
        {
            return SiteMap.Webs
                .Where(su => su.Id == WebId)
                .Select(h => h.Url)
                .FirstOrDefault();
        }

        public override SPSite OpenNewSite()
        {
            if (Identity.IsAnonymous)
            {
                return new SPSite(SiteId, SPUserToken.SystemAccount);
            }
            else
            {
                using (new ImpersonatedScope(Identity))
                {
                    return new SPSite(SiteId);
                }
            }
        }

        public override SPWeb OpenNewWeb(SPSite site)
        {
            if (Identity.IsAnonymous)
            {
                return site.OpenWeb(WebId);
            }
            else
            {
                using (new ImpersonatedScope(Identity))
                {
                    return site.OpenWeb(WebId);
                }
            }
        }

        public override bool IsCurrentWebRequest()
        {
            return SPContext.Current != null && Identity != WindowsIdentity.GetAnonymous() &&
                   SPContext.Current.Site.ID == SiteId && SPContext.Current.Web.ID == WebId;
        }
    }
}