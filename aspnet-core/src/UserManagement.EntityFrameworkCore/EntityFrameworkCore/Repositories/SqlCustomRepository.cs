using Abp.Application.Editions;
using Abp.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Asposes.Dto;
using UserManagement.CustomRepository;

namespace UserManagement.EntityFrameworkCore.Repositories
{
    public class SqlCustomRepository : SqlRepositoryBase<Edition>, ISqlCustomRepository
    {
        private readonly string connString = ClientConnection.DefaultConnection;

        public SqlCustomRepository(IDbContextProvider<UserManagementDbContext> dbContextProvider):base (dbContextProvider)
        {

        }

        public Task<DataSet> GetDataUser(ReportInfoDto info)
        {
            DataSet results = new DataSet();
            using(SqlConnection sqlConnection=new SqlConnection(connString))
            {
                SqlCommand objCmd = new SqlCommand();
                DataSet dataSet = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                objCmd.Connection = sqlConnection;
                objCmd.CommandText = info.StoreName;
                objCmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    sqlConnection.Open();
                    if (objCmd.Connection.State == ConnectionState.Open && objCmd != null)
                    {
                        SqlDataReader objReader = objCmd.ExecuteReader();
                        objReader.Close();
                    }
                    //objCmd.ExecuteNonQuery();
                    da.SelectCommand = objCmd;
                    da.Fill(results);
                }
                catch(Exception ex)
                {
                    return null;
                }
            }
            return Task.FromResult(results);
        }
    }
}
