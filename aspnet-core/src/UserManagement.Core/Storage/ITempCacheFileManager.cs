using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Storage
{
    public interface ITempCacheFileManager:ITransientDependency
    {
        void SetFile(string token, byte[] content);
        byte[] GetFile(string token);
    }
}
