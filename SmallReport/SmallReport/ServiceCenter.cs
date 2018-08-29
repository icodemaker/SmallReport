using Quartz;
using Quartz.Impl;
using SmallReport.Tool;

namespace SmallReport
{
    public class ServiceCenter
    {
        private readonly IScheduler _globalScheduler;

        public ServiceCenter()
        {
            _globalScheduler = StdSchedulerFactory.GetDefaultScheduler();
        }

        public void Start()
        {
            _globalScheduler.Start();
            LogHelper.Info("服务启动");
        }

        public void Pause()
        {
            _globalScheduler.PauseAll();
            LogHelper.Info("服务暂停");
        }

        public void Continue()
        {
            _globalScheduler.ResumeAll();
            LogHelper.Info("服务继续");
        }

        public void Stop()
        {
            _globalScheduler.Shutdown();
            LogHelper.Info("服务结束");
        }
    }
}
