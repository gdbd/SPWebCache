In memory cache for SharePoint SPWeb and SPSite objects.
Usable for pages intensively used this objects.

if you have 20 webparts on the page, each of them can open own SPWeb
This is may leads high memory preasure and leaks.

With SPWebCache you can open one object per page request, or even per multible requests!

Provides automatic disposing, so you can not to wory about it

Examples:
```

var cache = WebCache.Current.GetWeb(“http://server/sites/1/web1”);  
var cache2 = WebCache.Current.GetWeb(siteId, webId); 

SPSite site = WebCache.Current.GetWeb(siteId, webId); //cast to SPSite
SPWeb = WebCache.Current.GetWeb(siteId, webId);       //cast to SPWeb

using(var cache = WebCache.Current.GetWeb(“http://server/sites/1/web1”)) { ...}

```
