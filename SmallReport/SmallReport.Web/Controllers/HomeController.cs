using SmallReport.Assist;
using SmallReport.Assist.Quartz;
using SmallReport.Assist.WeChat;
using System.Web.Mvc;
using SmallReport.Service;

namespace SmallReport.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public void Msg()
        {
            if (!WeChatHelper.Valid()) return;
            var wmsg = WeChatHelper.GetXmlMessage();
            LogHelper.Debug($"msg come in, msgId:{wmsg.MediaId}");
            ResponseHelper.ResponseMsg(wmsg);
        }

        public ActionResult SendTemplateMsg()
        {
            var result = QueryExceptionService.QueryException();
            if (string.IsNullOrWhiteSpace(result))
            {
                result = "没有异数据";
            }
            MessageHelper.SendExpMsg(result);
            return Json(new { code = 0, msg = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckServiceMsg()
        {
            var result = QueryExceptionService.ServiceTimeout();
            if (result == -1)
            {
                MessageHelper.SendExpMsg($"请求ICAS服务器无响应 ...");
            }
            else if (result > 3000)
            {
                MessageHelper.SendExpMsg($"服务器响应时间{result},已经超出接受范围");
            }
            else if (result > 10000)
            {
                MessageHelper.SendExpMsg($"服务器响应时间{result},已经严重影响业务");
            }
            else
            {
                MessageHelper.SendExpMsg($"ICAS服务器响应时间{result}");
            }
            return Json(new { code = 0, msg = result }, JsonRequestBehavior.AllowGet);
        }
    }
}