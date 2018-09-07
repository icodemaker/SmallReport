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
                        result.Add($"发现未同步需求");
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
                        result.Add($"发现匹配为空数据");
                    }
                }
                //Re Pub Yet
                const string rePub = @"SELECT a.BeginTime,tr.TeacherId, COUNT(1) AS Num
                        FROM [Lks].[study].[ArrangeCourse] a 
                        INNER JOIN Lks.study.TeachRecord tr ON  a.Id = tr.ArrangeCourseId
                        WHERE a.BeginTime >GETDATE() 
                        AND a.Status=101 
                        AND a.CancelStatusType NOT IN (104,105,106) 
                        AND tr.Status=101
                        GROUP BY a.BeginTime,tr.TeacherId HAVING COUNT(*) > 1";
                using (var reader = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, rePub, new List<SqlParameter>()))
                {
                    if (reader.Read())
                    {
                        result.Add($"发现已经重复发布数据");
                    }
                }

                //Stu Req Re
                const string reStuReq = @"SELECT [StudentId] ,[BeginTime], COUNT(1) as Num
                        FROM [LksForICAS].[dbo].[StuReq]
                        WHERE BeginTime > GETDATE() 
                        group by StudentId,[BeginTime]
                        having COUNT(1) >1";
                using (var reader = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, reStuReq, new List<SqlParameter>()))
                {
                    if (reader.Read())
                    {
                        result.Add($"发现学员重复需求数据");
                    }
                }

                //Re Pub Will
                const string rePubWill = @"SELECT A.BeginTime,C.TeacherId,C.Id  AS ReqId,COUNT(1) AS Num FROM LksForICAS.dbo.Matching A
                        INNER JOIN LksForICAS.dbo.TeaReqMathing B ON a.Id = B.MathingId
                        INNER JOIN LksForICAS.dbo.TeaReq C ON B.TeaReqId = C.Id
                        WHERE 1 = 1
                        AND A.Status != 103
                        GROUP BY A.BeginTime,C.TeacherId,C.Id
                        HAVING COUNT(1) > 1";
                using (var reader = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, rePubWill, new List<SqlParameter>()))
                {
                    if (reader.Read())
                    {
                        result.Add($"发现即将重复出课的嫌疑数据");
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
