using System;
using System.Collections.Generic;
using System.Threading;

namespace SPWebCache.Core
{
	public class RequestQueue
	{
        private static volatile RequestQueue _instance;
        private static readonly object InstanceLock = new object();
        
        private readonly object _requestLock = new object();
        private Queue<Request> _queue = new Queue<Request>();
        internal WebFactory WebFactory = new WebFactory();
        
	    internal static RequestQueue Instance
	    {
            get
            {
                if (_instance == null)
                {
                    Create();
                }
                return _instance;
            }
	    }

        private RequestQueue()
        {
            SiteMap.Build();

            var thread = new Thread(ProccessRequests);
            thread.Start();

            var threadGc = new Thread(WebFactory.DisposeLongNotUsed);
            threadGc.Start();
        }

	    public static void Create()
		{
	        lock (InstanceLock)
	        {
	            if (_instance == null)
	            {
                    _instance = new RequestQueue();
	            }
	        }
		}

		internal void QueueRequest(Request request)
		{
            lock (_requestLock)
            {
                _queue.Enqueue(request);
                Monitor.Pulse(_requestLock);
            }
		}
       
	    private void ProccessRequests()
	    {
	        SiteMap.WaitForBuild();

            while (true)
            {
                Request request;
                lock (_requestLock)
                {
                    while (_queue.Count == 0)
                    {
                        Monitor.Wait(_requestLock);
                    }
                    request = _queue.Dequeue();
                }

                try
                {
                    WebFactory.ProccessRequest(request);
                }
                catch(Exception ex)
                {
                    request.CompleteCallback(null, null, ex);
                }
            }
		}
	}
}