using System;
using System.Security.Principal;
using Microsoft.SharePoint;

namespace SPWebCache.Core
{
    internal abstract class Request
    {
        public Action<SPSite, SPWeb, Exception> CompleteCallback;
        public WindowsIdentity Identity;
        public int? RequestHash;

        public abstract string RequestKey { get; }

        public abstract string SiteUrlOfRequest();
        public abstract string WebUrlOfRequest();
        public abstract SPSite OpenNewSite();
        public abstract SPWeb OpenNewWeb(SPSite site);
        public abstract bool IsCurrentWebRequest();
    }
}