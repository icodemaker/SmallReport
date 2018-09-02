using Quartz;
using SmallReport.Assist;
using SmallReport.Assist.WeChat;
using SmallReport.Service;
using SmallReport.Tool;
using System;

namespace SmallReport.Jobs
{
    public class CheckMatchIdNullJob : IJob
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
                    var result = CheckMatchId();
                }
            });
        }

        public bool CheckMatchId()
        {
            var flag = true;
            try
            {
                var HasValue = new ReqSyncService().CheckMatchIdNull();
                if (HasValue)
                {
                    LogHelper.Error("检查空指针问题发现异常数据");
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
