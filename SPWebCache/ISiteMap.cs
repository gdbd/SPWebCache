using System;

namespace SPWebCache
{
    public interface ISiteMap
    {
        Guid GetSiteId(string url);
        Guid GetWebId(string url);
        string GetSiteUrl(Guid siteId);
        string GetWebUrl(Guid webId);
    }
}