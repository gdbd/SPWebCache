using System;
using System.Diagnostics;
using Microsoft.SharePoint;

namespace SPWebCache
{
    [DebuggerDisplay("{Web.Url}")]
    public class Cache : IDisposable
    {
        private readonly SPSite _site;
        private readonly SPWeb _web;

        public Cache(SPSite site, SPWeb web)
        {
            _site = site;
            _web = web;
        }

        public SPSite Site
        {
            get { return _site; }
        }

        public SPWeb Web
        {
            get { return _web; }
        }

        public virtual void Dispose()
        {
        }

        public bool Equals(Cache obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Site.Equals(obj.Site) && Web.Equals(obj.Web);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((Cache)obj);
        }

        public override int GetHashCode()
        {
            return Site.GetHashCode() + Web.GetHashCode();
        }

        public static implicit operator SPWeb(Cache c)
        {
            return c._web;
        }

        public static implicit operator SPSite(Cache c)
        {
            return c._site;
        }
    }
}