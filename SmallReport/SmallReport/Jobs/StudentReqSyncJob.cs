using Quartz;
using SmallReport.Assist;
using SmallReport.Assist.WeChat;
using SmallReport.Service;
using SmallReport.Tool;
using System;

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
                    var result = ReqSync();
                }
            });
        }

        public bool ReqSync()
        {
            var flag = true;
            try
            {
                var HasValue = new ReqSyncService().CheckReqSync();
                if (HasValue)
                {
                    LogHelper.Error("检查需求同步问题发现异常数据");
                    MessageHelper.SendExpMsg();
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
