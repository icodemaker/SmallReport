using System.Web;
using SmallReport.Assist.WeChat.Model;

namespace SmallReport.Assist.WeChat
{
    public static class ResponseHelper
    {

        public static void ResponseMsg(MessageModel msg)
        {

            if (msg.MsgType == MessgeTypeModel.Text)
            {
                var result = MessageHelper.AccessService(msg.FromUserName, msg.ToUserName);
                HttpContext.Current.Response.Write(result);
            }
            else if (msg.MsgType == MessgeTypeModel.Location)
            {
                var result = MessageHelper.ReturnTextMsg(msg.FromUserName, msg.ToUserName, "你的坐标是:" + msg.Location_X + " , " + msg.Location_Y + "   位置是：" + msg.Label);
                HttpContext.Current.Response.Write(result);
            }
            else if (msg.MsgType == MessgeTypeModel.Voice)
            {
                var result = MessageHelper.ReturnVoice(msg.FromUserName, msg.ToUserName, msg.MediaId);
                HttpContext.Current.Response.Write(result);
            }
            else if (msg.MsgType == MessgeTypeModel.Image)
            {
                var result = MessageHelper.ReturnImageMsg(msg.FromUserName, msg.ToUserName, msg.MediaId);
                HttpContext.Current.Response.Write(result);
            }
            else if (msg.MsgType == MessgeTypeModel.Video)
            {
                var result = MessageHelper.ReturnVideoMsg(msg.FromUserName, msg.ToUserName, msg.ThumbMediaId, "Video", "Video Description");
                HttpContext.Current.Response.Write(result);
            }
            else if (msg.MsgType == MessgeTypeModel.Shortvideo)
            {
                var result = MessageHelper.ReturnVideoMsg(msg.FromUserName, msg.ToUserName, msg.ThumbMediaId, "Short Video", "Short Video Description");
                HttpContext.Current.Response.Write(result);
            }
            else if (msg.MsgType == MessgeTypeModel.Link)
            {

            }
            else if (msg.MsgType == MessgeTypeModel.Events)
            {
                switch (msg.Event)
                {
                    #region Follow
                    case MessgeTypeModel.Subscribe:
                        break;

                    case MessgeTypeModel.Scan:
                        
                        break;
                    #endregion

                    #region UnFollow

                    case MessgeTypeModel.Unsubscribe:

                        break;
                    #endregion

                    #region Lacal
                    case MessgeTypeModel.LocationEvnet:
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
