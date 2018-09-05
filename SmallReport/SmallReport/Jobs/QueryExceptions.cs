using Quartz;
using SmallReport.Assist;
using SmallReport.Assist.Quartz;
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
                var hasValue = QueryExceptionService.CheckMatchIdNull();
                if (hasValue)
                {
                    LogHelper.Error("返现异常数据，开始调用消息推送");
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
