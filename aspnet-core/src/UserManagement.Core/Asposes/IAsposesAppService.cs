using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Asposes.Dto;

namespace UserManagement.Asposes
{
    public interface IAsposesAppService: IApplicationService
    {
        Task<FileDto> GetReport_User(ReportInfoDto info);
    }
}
