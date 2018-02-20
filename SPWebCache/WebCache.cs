using System.Collections.Generic;
using System.Web;
using SPWebCache.Core;

namespace SPWebCache
{
    //http://ru.wikipedia.org/wiki/Double_checked_locking
	public class WebCache
	{
        private static readonly object Sync = new object();
        private static readonly object Sync2 = new object();
        private static readonly object Sync3 = new object();

	    private static volatile ISiteMap _siteMap;
        private static volatile IWebCache _instance;
        private static volatile Dictionary<int, IWebCache> _instancesPerRequest = new Dictionary<int, IWebCache>();

        /// <summary>
        /// Получение кешированных объектов. Время жизни кешированного объекта - несколько запросов
        /// Должен использоваться в using!
        /// IDisposable нужен для синхронизации доступа к кешу и не вызывает Dispose SPSite и SPWeb.
        /// На возвращаемых объектах SPSite и SPWeb Dispose не вызывать!
        /// </summary>
		public static IWebCache Instance
		{
			get
			{
			    if (_instance == null)
			    {
			        lock (Sync)
			        {
			            if (_instance == null)
			            {
			                _instance = new MultiRequestCache();
			            }
			        }
			    }

			    return _instance;
			}
		}
        
        /// <summary>
        /// Получение кешированных объектов. Время жизни кешированного объекта - один запрос.
        /// Запрошенные объекты уничтожаются автоматически в конце запроса.
        /// Можно использовать без using.
        /// На возвращаемых объектах SPSite и SPWeb Dispose не вызывать!
        /// </summary>
        public static IWebCache Current
        {
            get
            {
                int requestHash = HttpContext.Current != null ? HttpContext.Current.Request.GetHashCode() : -1;

                IWebCache instance;
                lock (Sync2)
                {
                    if (!_instancesPerRequest.TryGetValue(requestHash, out instance))
                    {
                        instance = new SingleRequestCache();
                        _instancesPerRequest.Add(requestHash, instance);
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Позволяет получить ID объектов SPSite и SPWeb зная их URL
        /// а также получить URL зная их ID
        /// </summary>
	    public static ISiteMap SiteMap
        {
	        get
	        {
                if (_siteMap == null)
                {
                    lock (Sync3)
                    {
                        if (_siteMap == null)
                        {
                            _siteMap = new SiteMapImpl();
                        }
                    }
                }

                return _siteMap;
	        }
	    }

	    internal static void CleanCurrent()
	    {

            int requestHash = HttpContext.Current != null ? HttpContext.Current.Request.GetHashCode() : -1;
            
            lock (Sync2)
            {
                IWebCache instance;
                if (_instancesPerRequest.TryGetValue(requestHash, out instance))
                {
                    instance.Dispose();
                    _instancesPerRequest.Remove(requestHash);
                }
            }
	    }
	}
}