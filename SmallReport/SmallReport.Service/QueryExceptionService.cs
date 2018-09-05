using SmallReport.Entity;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SmallReport.Service
{
    public static class QueryExceptionService
    {
        private static string ConnectionString => ConfigurationManager.ConnectionStrings["Lks"].ConnectionString;

        public static string QueryException()
        {
            try
            {
                var result = new List<string>();
                //StuReq Async
                const string sqlAsync = @"SELECT a.* FROM [Lks].[stu].[StudentRequirement] a 
                        LEFT JOIN [LksForICAS].[dbo].[StuReq] b  
                        ON  a.StudentId=b.StudentId AND a.BeginTime=b.BeginTime
                        WHERE a.CancelStatusType=101 AND a.RequirementStatusType= 102 
                        AND a.BeginTime > GETDATE() 
                        AND b.Id IS NULL 
                        ORDER BY a.BeginTime DESC";
                using (var reader = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlAsync, new List<SqlParameter>()))
                {
                    if (reader.Read())
                    {
                        result.Add($"发现未同步需求{reader.FieldCount}条");
                    }
                }
                //Null
                const string sqlNull = @"SELECT * FROM LksForICAS.dbo.StuReq WHERE 1=1 
                        AND MathingId IS NULL
                        ORDER BY BeginTime DESC";
                using (var reader = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlNull, new List<SqlParameter>()))
                {
                    if (reader.Read())
                    {
                        result.Add($"发现匹配为空数据{reader.FieldCount}条");
                    }
                }
                return string.Join(",", result);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
