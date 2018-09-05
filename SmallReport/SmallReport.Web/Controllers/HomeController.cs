using SmallReport.Assist;
using SmallReport.Assist.Quartz;
using SmallReport.Assist.WeChat;
using System.Web.Mvc;

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
            MessageHelper.SendExpMsg();
            return Json(null);
        }
    }
}