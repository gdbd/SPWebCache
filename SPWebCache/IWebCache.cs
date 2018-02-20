using System;

namespace SPWebCache
{
	public interface IWebCache : IDisposable
	{
		Cache GetWeb(string url);
        Cache GetElevatedWeb(string url);

        Cache GetWeb(Guid siteId, Guid webId);
        Cache GetElevatedWeb(Guid siteId, Guid webId);
	}
}