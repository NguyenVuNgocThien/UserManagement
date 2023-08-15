using Abp.Application.Editions;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Asposes.Dto;

namespace UserManagement.CustomRepository
{
    public interface ISqlCustomRepository:IRepository<Edition>
    {
        Task<DataSet> GetDataUser(ReportInfoDto info);
    }
}
