using Quartz;
using System;
using System.Collections.Concurrent;

namespace SmallReport.Assist.Quartz
{
    public static class JobHelper
    {
        #region Invoke

        private static readonly ConcurrentDictionary<string, bool> InvokeFlagList = new ConcurrentDictionary<string, bool>();

        public static void Invoke(IJobExecutionContext context, Action action)
        {
            var flag = false;
            var jobKey = context.JobDetail.Key.Name;
            if (InvokeFlagList.ContainsKey(jobKey))
            {
                if (!InvokeFlagList[jobKey])
                {
                    InvokeFlagList[jobKey] = true;
                    flag = true;
                }
            }
            else
            {
                flag = InvokeFlagList.TryAdd(jobKey, true);
            }

            if (!flag) return;
            LogHelper.Debug($"WinService JobKey:{jobKey} Execute Begin Time:{DateTime.Now}");
            try
            {
                action();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"WinService JobKey:{jobKey} Exception:", ex);
            }
            LogHelper.Debug($"WinService JobKey:{jobKey} Execute End Time:{DateTime.Now}");
            InvokeFlagList[jobKey] = false;
        }

        #endregion
    }
}
