using System.Web;

namespace SmallReport.Assist.WeChat
{
    public class ResponseHelper
    {

        public static void ResponseMsg(MessageModel msg)
        {

            if (msg.MsgType == MessgeTypeModel.text)
            {
                string result = MessageHelper.AccessService(msg.FromUserName, msg.ToUserName);
                HttpContext.Current.Response.Write(result);
            }
            else if (msg.MsgType == MessgeTypeModel.location)
            {
                string result = MessageHelper.ReturnTextMsg(msg.FromUserName, msg.ToUserName, "你的坐标是:" + msg.Location_X + " , " + msg.Location_Y + "   位置是：" + msg.Label);
                HttpContext.Current.Response.Write(result);
            }
            else if (msg.MsgType == MessgeTypeModel.voice)
            {
                string result = MessageHelper.ReturnVoice(msg.FromUserName, msg.ToUserName, msg.MediaId);
                HttpContext.Current.Response.Write(result);
            }
            else if (msg.MsgType == MessgeTypeModel.image)
            {
                string result = MessageHelper.ReturnImageMsg(msg.FromUserName, msg.ToUserName, msg.MediaId);
                HttpContext.Current.Response.Write(result);
            }
            else if (msg.MsgType == MessgeTypeModel.video)
            {
                string result = MessageHelper.ReturnVideoMsg(msg.FromUserName, msg.ToUserName, msg.ThumbMediaId, "Video", "Video Description");
                HttpContext.Current.Response.Write(result);
            }
            else if (msg.MsgType == MessgeTypeModel.shortvideo)
            {
                string result = MessageHelper.ReturnVideoMsg(msg.FromUserName, msg.ToUserName, msg.ThumbMediaId, "Short Video", "Short Video Description");
                HttpContext.Current.Response.Write(result);
            }
            else if (msg.MsgType == MessgeTypeModel.link)
            {

            }
            else if (msg.MsgType == MessgeTypeModel.events)
            {
                switch (msg.Event)
                {
                    #region Follow
                    case MessgeTypeModel.subscribe:
                        break;

                    case MessgeTypeModel.scan:
                        
                        break;
                    #endregion

                    #region UnFollow

                    case MessgeTypeModel.unsubscribe:

                        break;
                    #endregion

                    #region Lacal
                    case MessgeTypeModel.locationEvnet:
                        string location = MessageHelper.ReturnTextMsg(msg.FromUserName, msg.ToUserName, "Lat：" + msg.Latitude + " Long：" + msg.Longitude + " Pre：" + msg.Precision);
                        HttpContext.Current.Response.Write(location);
                        break;

                    #endregion

                    #region  Other
                    default:
                        
                        break;
                    #endregion
                }
            }
        }
    }
}
