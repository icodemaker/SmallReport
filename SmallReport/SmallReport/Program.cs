using SmallReport.Assist.Quartz;
using Topshelf;

namespace SmallReport
{
    internal static class Program
    {
        private static void Main()
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

                srv.SetDescription("SmallReport Exceptions");
                srv.SetDisplayName("SmallReport");
                srv.SetServiceName("SmallReport");

                srv.AfterInstall(() => LogHelper.Info("SmallReport Install"));
                srv.AfterUninstall(() => LogHelper.Info("SmallReport UnInstall"));

                srv.OnException(ex =>
                {
                    LogHelper.Error("SmallReport Unknown Exception", ex);
                });
            });
        }
    }
}
