using Quartz;
using SmallReport.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallReport.Jobs
{
    public class StudentReqSyncJob: IJob
    {
        #region 属性
        private static readonly object LockObj = new object();

        #endregion
        public void Execute(IJobExecutionContext context)
        {
            JobHelper.Invoke(context, () =>
            {
                using (var lockHelper = new LockHelper(LockObj))
                {
                    if (lockHelper.IsTimeout) return;
                    LogHelper.Info("任务开始");
                    var result = ReqSync();
                    LogHelper.Info(result ? "任务成功" : "任务失败");
                }
            });
        }

        public bool ReqSync()
        {
            var flag = true;
            try
            {
                LogHelper.Info("任务执行一次");
            }
            catch (Exception e)
            {
                LogHelper.Error("任务出现异常", e);
                flag = false;
            }

            return flag;
        }
    }
}
