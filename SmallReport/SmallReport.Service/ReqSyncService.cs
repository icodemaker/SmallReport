using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using YiTu.DBUtility;

namespace SmallReport.Service
{
    public static class ReqSyncService
    {
        private static string ConnectionString => ConfigurationManager.ConnectionStrings["Lks"].ConnectionString;

        public static bool CheckReqSync()
        {
            const string sql = @"SELECT a.* FROM [Lks].[stu].[StudentRequirement] a 
                        LEFT JOIN [LksForICAS].[dbo].[StuReq] b  
                        ON  a.StudentId=b.StudentId AND a.BeginTime=b.BeginTime
                        WHERE a.CancelStatusType=101 AND a.RequirementStatusType= 102 
                        AND a.BeginTime > GETDATE() 
                        AND b.Id IS NULL 
                        ORDER BY a.BeginTime DESC";
            using (var reader = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sql, new List<SqlParameter>()))
            {
                if (reader.Read())
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckMatchIdNull()
        {
            const string sql = @"SELECT * FROM LksForICAS.dbo.StuReq WHERE 1=1 
                        AND MathingId IS NULL
                        ORDER BY BeginTime DESC";
            using (var reader = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sql, new List<SqlParameter>()))
            {
                if (reader.Read())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
