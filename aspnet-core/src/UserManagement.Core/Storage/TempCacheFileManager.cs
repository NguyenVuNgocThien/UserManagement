using Abp.Runtime.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Storage
{
    public class TempCacheFileManager:ITempCacheFileManager
    {
        public const string TempFileCacheName = "TempFileCacheName";
        private readonly ICacheManager _cacheManager;
        public TempCacheFileManager(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }
        public void SetFile(string token, byte[] content)
        {
            _cacheManager.GetCache(TempFileCacheName).Set(token, content, new TimeSpan(0, 0, 1, 0));
        }
        public byte[] GetFile (string token)
        {
            return _cacheManager.GetCache(TempFileCacheName).Get(token, ep => ep) as byte[];
        }
    }
}
