using Quartz;
using Quartz.Impl;
using SmallReport.Assist.Quartz;

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
            LogHelper.Info("Service Start");
        }

        public void Pause()
        {
            _globalScheduler.PauseAll();
            LogHelper.Info("Service Pause");
        }

        public void Continue()
        {
            _globalScheduler.ResumeAll();
            LogHelper.Info("Service Continue");
        }

        public void Stop()
        {
            _globalScheduler.Shutdown();
            LogHelper.Info("Service Stop");
        }
    }
}
