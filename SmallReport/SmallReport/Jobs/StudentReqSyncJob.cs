using Quartz;
using SmallReport.Assist;
using SmallReport.Tool;
using System;

namespace SmallReport.Jobs
{
    public class StudentReqSyncJob: IJob
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
                    LogHelper.Info("Task Start");
                    var result = ReqSync();
                    LogHelper.Info(result ? "Task Success" : "Task Failed");
                }
            });
        }

        public bool ReqSync()
        {
            var flag = true;
            try
            {
                LogHelper.Info("Finished Once");
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
