namespace WebApplication1.Cache
{
    public interface ICacheable
    {
        public void AddToCache(string key, object value);
        public void UpdateCacheItem(string key, object value);
        public object GetCacheItem(string key);
    }
}
