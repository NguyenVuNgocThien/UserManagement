using Abp.Application.Services;
using Abp.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Asposes.Dto;
using UserManagement.CustomRepository;
using UserManagement.Storage;
using UserManagement.Net.MimeTypes;


namespace UserManagement.Asposes
{
    [AbpAuthorize]
    public class AsposesAppService : ApplicationService, IAsposesAppService
    {
        
        private readonly IReportExporter _customReportFile;
        private readonly ISqlCustomRepository _storeProcedureProvider;
        private readonly ITempCacheFileManager _tempCacheFileManager;

        public AsposesAppService(IReportExporter customReportFile, ISqlCustomRepository storeProcedureProvider, ITempCacheFileManager tempCacheFileManager)
        {
            _customReportFile = customReportFile;
            _storeProcedureProvider = storeProcedureProvider;
            _tempCacheFileManager = tempCacheFileManager;
        }

        public async Task<FileDto> GetReport_User(ReportInfoDto info)
        {
            try
            {
                var reportByteArray = await _customReportFile.CreateExcelFileUser(info);
                FileDto file = new FileDto();
                var fileName = info.PathName.Substring(info.PathName.IndexOf("/") + 1);
                fileName = "List User " + DateTime.Now.Month + "/" + DateTime.Now.Year;
                switch (info.TypeExport.ToLower())
                {
                    case FileTypeConst.Excel:
                        file = new FileDto(fileName + ".xlsx", MimeTypeNames.ApplicationVndMsExcel);
                        break;
                    case FileTypeConst.Pdf:
                        file = new FileDto(fileName + ".pdf", MimeTypeNames.ApplicationPdf);
                        break;
                    case FileTypeConst.Word:
                        file = new FileDto(fileName + ".docx", MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentWordprocessingmlDocument);
                        break;
                }
                _tempCacheFileManager.SetFile(file.FileToken, reportByteArray.ToArray());
                return file;
            }
            catch (Exception ex)
            {
                throw new Abp.UI.UserFriendlyException(message: ex.Message);
            }
        }
    }
}
