using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Asposes.Dto;

namespace UserManagement.Asposes
{
    public interface IReportExporter :ITransientDependency
    {
        Task<MemoryStream> CreateExcelFileUser(ReportInfoDto info);
        Task<dynamic> CreateExcelFileAndDesignUser(ReportInfoDto info);
    }
}
