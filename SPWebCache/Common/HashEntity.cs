using System;

namespace SPWebCache.Common
{
    internal class HashEntity
    {
        public string Url;
        public Guid Id;

        public HashEntity(string url, Guid id)
        {
            Url = url;
            Id = id;
        }
    }
}
