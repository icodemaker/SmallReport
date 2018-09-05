using Quartz;
using SmallReport.Assist.Quartz;
using SmallReport.Assist.WeChat;
using SmallReport.Service;
using System;

namespace SmallReport.Jobs
{
    public class QueryExceptions : IJob
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
                var result = QueryExceptionService.QueryException();
                if (!string.IsNullOrWhiteSpace(result))
                {
                    LogHelper.Error($"返现异常数据:{result}");
                    MessageHelper.SendExpMsg(result);
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
