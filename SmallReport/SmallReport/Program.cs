using SmallReport.Tool;
using Topshelf;

namespace SmallReport
{
    static class Program
    {
        static void Main()
        {
            HostFactory.Run(srv =>
            {
                srv.Service<ServiceCenter>(s =>
                {
                    s.ConstructUsing(c => new ServiceCenter());
                    s.WhenStarted(c => c.Start());
                    s.WhenPaused(c => c.Pause());
                    s.WhenContinued(c => c.Continue());
                    s.WhenStopped(c => c.Stop());
                });

                srv.EnablePauseAndContinue();

                srv.RunAsLocalSystem();
                srv.StartAutomatically();

                srv.SetDescription("小小报警服务.定时轮询异常.");
                srv.SetDisplayName("SmallReport");
                srv.SetServiceName("SmallReport");

                srv.AfterInstall(() => LogHelper.Info("服务安装"));
                srv.AfterUninstall(() => LogHelper.Info("服务已卸载"));

                srv.OnException(ex =>
                {
                    LogHelper.Error("服务遇到未处理的异常", ex);
                });
            });
        }
    }
}
