using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using SmallReport.Assist.Quartz;
using SmallReport.Assist.WeChat;
using SmallReport.Service;

namespace SmallReport.Jobs
{
    public class ServiceExceptions : IJob
    {
        #region Attr
        private static readonly object LockObj = new object();

        #endregion
        public void Execute(IJobExecutionContext context)
        {
            JobHelper.Invoke(context, () =>
            {
                using (var lockHelper = new LockHelper(LockObj))
                {
                    if (lockHelper.IsTimeout) return;
                    var result = QueryException();
                }
            });
        }

        private static bool QueryException()
        {
            var flag = true;
            try
            {
                var result = QueryExceptionService.ServiceTimeout();
                if (result == -1)
                {
                    MessageHelper.SendExpMsg($"请求ICAS服务器无响应 ...");
                }
                else if(result > 3000)
                {
                    MessageHelper.SendExpMsg($"服务器响应时间{result},已经超出接受范围");
                }
                else if(result > 10000)
                {
                    MessageHelper.SendExpMsg($"服务器响应时间{result},已经严重影响业务");
                }
                else
                {
                    LogHelper.Info($"ICAS服务器响应时间{result}");
                }
            }
            catch (Exception e)
            {
                LogHelper.Error("Task Exception", e);
                flag = false;
            }

            return flag;
        }
    }
}
