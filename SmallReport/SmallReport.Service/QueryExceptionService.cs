using System;
using SmallReport.Entity;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using SmallReport.Assist.Quartz;

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
                using (var reader = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlAsync,
                    new List<SqlParameter>()))
                {
                    if (reader.Read())
                    {
                        result.Add($"发现学员未同步需求第一类");
                    }
                }
                //Null
                const string sqlNull = @"SELECT * FROM LksForICAS.dbo.StuReq WHERE 1=1 
                        AND MathingId IS NULL
                        ORDER BY BeginTime DESC";
                using (var reader = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlNull,
                    new List<SqlParameter>()))
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
                using (var reader = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, rePub,
                    new List<SqlParameter>()))
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
                using (var reader = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, reStuReq,
                    new List<SqlParameter>()))
                {
                    if (reader.Read())
                    {
                        result.Add($"发现学员重复需求数据");
                    }
                }

                //Re Pub Will
                const string rePubWill =
                    @"SELECT A.BeginTime,C.TeacherId,C.Id  AS ReqId,COUNT(1) AS Num FROM LksForICAS.dbo.Matching A
                        INNER JOIN LksForICAS.dbo.TeaReqMathing B ON a.Id = B.MathingId
                        INNER JOIN LksForICAS.dbo.TeaReq C ON B.TeaReqId = C.Id
                        WHERE 1 = 1
                        AND A.Status != 103
                        GROUP BY A.BeginTime,C.TeacherId,C.Id
                        HAVING COUNT(1) > 1";
                using (var reader = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, rePubWill,
                    new List<SqlParameter>()))
                {
                    if (reader.Read())
                    {
                        result.Add($"发现即将重复出课的嫌疑数据");
                    }
                }

                //New Stu Not Async
                const string newAsync = @"SELECT *
                    FROM [Lks].[stu].[StudentRequirement]
                    WHERE CancelStatusType=101 AND [Status]=101 AND RequirementStatusType=102
                    AND BeginTime>=GETDATE()
                    AND DATEDIFF(HOUR,GETDATE(),BeginTime) <12";
                using (var reader = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, newAsync,
                    new List<SqlParameter>()))
                {
                    if (reader.Read())
                    {
                        result.Add($"发现学员未同步需求第二类");
                    }
                }

                //Tea Not Async
                const string teaAsync = @"SELECT a.* FROM [Lks].tea.TeacherRequirement a 
                    LEFT JOIN [LksForICAS].[dbo].TeaReq b  
                    ON  a.TeacherId=b.TeacherId AND a.BeginTime=b.BeginTime
                    WHERE a.CancelStatusType=101 AND a.RequirementStatusType=102 
                    AND a.BeginTime > GETDATE() 
                    AND b.Id IS NULL 
                    ORDER BY a.BeginTime DESC";
                using (var reader = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, teaAsync,
                    new List<SqlParameter>()))
                {
                    if (reader.Read())
                    {
                        result.Add($"发现教师未同步需求");
                    }
                }
                return string.Join(",", result);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public static int ServiceTimeout()
        {
            try
            {
                var sTime = DateTime.Now.AddDays(1 - Convert.ToInt16(DateTime.Now.DayOfWeek)).Date;
                var eTime = DateTime.Now.AddDays(8 - Convert.ToInt16(DateTime.Now.DayOfWeek)).Date;
                var url = $"https://s.likeshuo.com/Book/General/GetTimeListJson?begintDate={sTime}&endDate={eTime}";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Accept = "*/*";
                request.Timeout = 12000;//12s超时
                request.AllowAutoRedirect = false;
                request.Headers.Add("Cookie", "ReturnUrlKey=; ABCHomeStudentContractCookie643203=false; ABCHomeStudentContractCookie575980=false; ABCHomeStudentContractCookie576130=false; ABCHomeStudentContractCookie664107=false; ABCHomeStudentContractCookie439572=false; ABCHomeStudentContractCookie296519=false; ABCHomeStudentContractCookie451560=false; ABCHomeStudentContractCookie631879=false; ABCHomeStudentContractCookie574657=false; ABCHomeStudentContractCookie650060=false; ABCHomeStudentContractCookie1031=false; ABCHomeStudentContractCookie646776=false; ABCHomeStudentContractCookie610016=false; ABCHomeStudentContractCookie617706=false; ABCHomeStudentContractCookie289838=false; ABCHomeStudentContractCookie669751=false; ABCHomeStudentContractCookie615268=false; ABCHomeStudentContractCookie632483=false; ABCHomeStudentContractCookie633644=false; ABCHomeStudentContractCookie246904=false; ABCHomeStudentContractCookie674917=false; ABCHomeStudentContractCookie618047=false; ABCHomeStudentContractCookie650480=false; ABCHomeStudentContractCookie605969=false; ABCHomeStudentContractCookie661941=false; ABCHomeStudentContractCookie677864=false; ABCHomeStudentContractCookie674828=false; StudentGuideCookieKey={%22Studentbook%22:true%2C%22StudentBook/General/Time%22:true%2C%22Student%22:true%2C%22StudentStudy/Detail/Index%22:true%2C%22StudentBook/General/Course%22:true%2C%22StudentBook/General/Teacher%22:true%2C%22StudentBook/General/Recommend%22:true%2C%22StudentBook/Home/Index%22:true%2C%22StudentBook/General/Package%22:true%2C%22Studentbook/overseas/time%22:true%2C%22StudentBook/Overseas/Teacher%22:true}; ABCHomeStudentContractCookie671181=false; ABCHomeStudentContractCookie669475=%7b%0d%0a++%22Rdata%22%3a+null%2c%0d%0a++%22Rstatus%22%3a+false%2c%0d%0a++%22Rcode%22%3a+0%2c%0d%0a++%22Rmsg%22%3a+null%0d%0a%7d; ABCHomeStudentContractCookie652470=false; ABCHomeStudentContractCookie676551=false; Lks_Treaty_676551=true; ABCHomeStudentContractCookie659884=false; Lks_Treaty_659884=true; ABCHomeStudentContractCookie232648=false; Lks_Treaty_232648=true; ABCHomeStudentContractCookie548610=false; Lks_Treaty_548610=true; Lks_Treaty_646776=true; UM_distinctid=165f50e1710154-0a914b6ed689f9-50432518-1fa400-165f50e1712987; ABCHomeStudentContractCookie401988=false; Lks_Treaty_401988=true; LoginUserKey=OpenId=7688EA496D5BD76FB2CA9E25C4DA00FC&XbOpenId=&UserId=55453A89B61099FC&StudentNo=S152586360358666&TeacherNo=T152589377581919&UserType=1038&CName=%e6%9d%8e%e6%81%a9%e5%8d%8e&EName=Vic&Photo=5c68b7d2-ded5-4f6c-94d7-459dcd6452a3.jpg&PhotoUrl=https%3a%2f%2fattachment.likeshuo.com%2fuser%2f5c68b7d2-ded5-4f6c-94d7-459dcd6452a3.jpg%3fExpires%3d1568965901%26OSSAccessKeyId%3dLTAIIOYxwINNCCOS%26Signature%3dUxNzBOMST3u7MJe8B6SwP0vUPGo%253D%26x-oss-process%3dstyle%252Fuser-small&TimeZone=UTC%2b08%3a00+China&FromType=101&StudentStageType=4&EmployeeType=102&EmployeeLeave=False&ProxyType=1&PlineType=&DeputyId=576130&OaUserName=&576130=rrttas1okq2ckzgfvvjiofjv; LanguageThemeKey=Language=101&Theme=1&Layout=1; TimeZoneKey=TimeZone=8&TimeZoneName=55BFA6768EE99690633F37C02A944CB9; LoginIsProxyKey=False; Lks_StudentServiceSign_576130={%22PlineType%22:1%2C%22IsHasXbOpenId%22:false%2C%22IsCtraining%22:false%2C%22IsOverseas%22:false%2C%22IsGeneral%22:true%2C%22IsJunior%22:false}; StudentBookCommonInfoKeyOfLks=%7b%0d%0a++%22StudentId%22%3a+576130%2c%0d%0a++%22LevelType%22%3a+128%2c%0d%0a++%22ApplyLevel%22%3a+384%2c%0d%0a++%22CourseTypeIds%22%3a+%5b%0d%0a++++1%2c%0d%0a++++2%2c%0d%0a++++3%2c%0d%0a++++4%2c%0d%0a++++5%2c%0d%0a++++6%2c%0d%0a++++7%2c%0d%0a++++8%2c%0d%0a++++9%2c%0d%0a++++10%2c%0d%0a++++11%2c%0d%0a++++12%2c%0d%0a++++15%2c%0d%0a++++19%2c%0d%0a++++31%2c%0d%0a++++32%0d%0a++%5d%2c%0d%0a++%22BlackTeacherIds%22%3a+%5b%5d%2c%0d%0a++%22CollectionTeacherIds%22%3a+%5b%0d%0a++++428523%0d%0a++%5d%2c%0d%0a++%22BlackCourseIds%22%3a+%5b%0d%0a++++1284%0d%0a++%5d%0d%0a%7d; StudentBookClassTypeInfoKeyOfLks=%7b%0d%0a++%22ClassType%22%3a+4%2c%0d%0a++%22PackageId%22%3a+null%2c%0d%0a++%22PackageName%22%3a+null%2c%0d%0a++%22CourseTypeId%22%3a+null%2c%0d%0a++%22CourseTypeName%22%3a+null%2c%0d%0a++%22BeginDate%22%3a+%222018-09-01+00%3a00%3a00%22%2c%0d%0a++%22EndDate%22%3a+%222018-09-30+00%3a00%3a00%22%2c%0d%0a++%22ContractTypeIds%22%3a+%5b%0d%0a++++12%0d%0a++%5d%2c%0d%0a++%22CourseId%22%3a+null%2c%0d%0a++%22CourseName%22%3a+null%2c%0d%0a++%22CourseCover%22%3a+null%2c%0d%0a++%22MaxBookNum%22%3a+4%2c%0d%0a++%22IsSelectCourse%22%3a+false%2c%0d%0a++%22IsSelectTeacher%22%3a+false%2c%0d%0a++%22IsSelectTime%22%3a+true%2c%0d%0a++%22IsRecommond%22%3a+true%0d%0a%7d; Hm_lvt_109d93afcbf0a2817930fe7cc3485474=1535359611,1535361134; Hm_lpvt_109d93afcbf0a2817930fe7cc3485474=1537861509; CNZZDATA1260331220=1226226382-1537413268-https%253A%252F%252Fmanager.likeshuo.com%252F%7C1537861300");
                var startTime = DateTime.Now;
                var webresponce = (HttpWebResponse)request.GetResponse();
                var endTime = DateTime.Now;
                if (webresponce.StatusCode != HttpStatusCode.OK) return -1;
                webresponce.Close();
                return Convert.ToInt32((endTime - startTime).TotalMilliseconds);//获取相应时间 
            }
            catch (Exception ex)
            {
                LogHelper.Error("获取ICAS服务器响应时间报错", ex);
                return -1;
            }
        }
    }
}
