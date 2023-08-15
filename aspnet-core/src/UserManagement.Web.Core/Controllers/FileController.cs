using Abp.Auditing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Asposes.Dto;
using UserManagement.Storage;

namespace UserManagement.Controllers
{
    public class FileController :UserManagementControllerBase
    {
        private readonly ITempCacheFileManager tempCacheFileManager;

        public FileController(ITempCacheFileManager tempCacheFileManager)
        {
            this.tempCacheFileManager = tempCacheFileManager;
        }
        [DisableAuditing]
        public ActionResult DownloadTempFile(FileDto fileDto)
        {
            var fileBytes = tempCacheFileManager.GetFile(fileDto.FileToken);
            if (fileBytes == null)
            {
                return NotFound(L("RequestedFileDoesNotExists"));
            }
            return File(fileBytes, fileDto.FileType, fileDto.FileToken);
        }
    }
}
