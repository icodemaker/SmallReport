using Quartz;
using SmallReport.Assist;
using SmallReport.Assist.WeChat;
using SmallReport.Service;
using System;
using SmallReport.Assist.Quartz;

namespace SmallReport.Jobs
{
    public class StudentReqSyncJob : IJob
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
                    ReqSync();
                }
            });
        }

        private static void ReqSync()
        {
            try
            {
                var hasValue = ReqSyncService.CheckReqSync();
                if (!hasValue) return;
                LogHelper.Error("检查需求同步问题发现异常数据");
                MessageHelper.SendExpMsg();
            }
            catch (Exception e)
            {
                LogHelper.Error("Task Exception", e);
            }
        }
    }
}
