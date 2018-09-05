using System;
using System.Text;

namespace SmallReport.Assist.WeChat
{
    public static class MessageHelper
    {

        public static string ReturnTextMsg(string toUserName, string fromUserName, string content)
        {
            var num = CommonHelper.ConvertDateTimeInt(DateTime.Now);
            var builder = new StringBuilder();
            builder.Append("<xml>");
            builder.Append("<ToUserName><![CDATA[" + toUserName + "]]></ToUserName>");
            builder.Append("<FromUserName><![CDATA[" + fromUserName + "]]></FromUserName>");
            builder.Append("<CreateTime>" + num.ToString() + "</CreateTime>");
            builder.Append("<MsgType><![CDATA[text]]></MsgType>");
            builder.Append("<Content><![CDATA[" + content + "]]></Content>");
            builder.Append("</xml>");
            return builder.ToString();
        }

        public static string ReturnImageMsg(string toUserName, string fromUserName, string mediaId)
        {
            var createdt = CommonHelper.ConvertDateTimeInt(DateTime.Now);
            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<ToUserName><![CDATA[" + toUserName + "]]></ToUserName>");
            sb.Append("<FromUserName><![CDATA[" + fromUserName + "]]></FromUserName>");
            sb.Append("<CreateTime>" + createdt.ToString() + "</CreateTime>");
            sb.Append("<MsgType><![CDATA[image]]></MsgType>");
            sb.Append("<Image>");
            sb.Append("<MediaId><![CDATA[" + mediaId + "]]></MediaId>");
            sb.Append("</Image>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        public static string ReturnVideoMsg(string toUserName, string fromUserName, string mediaId, string title, string description)
        {
            var createdt = CommonHelper.ConvertDateTimeInt(DateTime.Now);
            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<ToUserName><![CDATA[" + toUserName + "]]></ToUserName>");
            sb.Append("<FromUserName><![CDATA[" + fromUserName + "]]></FromUserName>");
            sb.Append("<CreateTime>" + createdt.ToString() + "</CreateTime>");
            sb.Append("<MsgType><![CDATA[video]]></MsgType>");
            sb.Append("<Video>");
            sb.Append("<MediaId><![CDATA[" + mediaId + "]]></MediaId>");
            sb.Append("<Title><![CDATA[" + title + "]]></Title>");
            sb.Append("<Description><![CDATA[" + description + "]]></Description>");
            sb.Append("</Video> ");
            sb.Append("</xml>");
            return sb.ToString();
        }

        public static string ReturnMusic(string toUser, string fromUserName, string title, string description, string musicUrl, string hqMusicUrl)
        {
            var createdt = CommonHelper.ConvertDateTimeInt(DateTime.Now);
            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<ToUserName><![CDATA[" + toUser + "]]></ToUserName>");
            sb.Append("<FromUserName><![CDATA[" + fromUserName + "]]></FromUserName>");
            sb.Append("<CreateTime>" + createdt.ToString() + "</CreateTime>");
            sb.Append("<MsgType><![CDATA[music]]></MsgType>");
            sb.Append("<Music>");
            sb.Append("<Title><![CDATA[" + title + "]]></Title>");
            sb.Append("<Description><![CDATA[" + description + "]]></Description>");
            sb.Append("<MusicUrl><![CDATA[" + musicUrl + "]]></MusicUrl>");
            sb.Append("<HQMusicUrl><![CDATA[" + hqMusicUrl + "]]></HQMusicUrl>");
            sb.Append("</Music>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        public static string ReturnVoice(string toUser, string fromUser, string mediaId)
        {
            var createdt = CommonHelper.ConvertDateTimeInt(DateTime.Now);
            var sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<ToUserName><![CDATA[" + toUser + "]]></ToUserName>");
            sb.Append("<FromUserName><![CDATA[" + fromUser + "]]></FromUserName>");
            sb.Append("<CreateTime>" + createdt.ToString() + "</CreateTime>");
            sb.Append("<MsgType><![CDATA[voice]]></MsgType>");
            sb.Append("<Voice>");
            sb.Append("<MediaId><![CDATA[" + mediaId + "]]></MediaId>");
            sb.Append("</Voice>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        public static string AccessService(string toUser, string fromUser)
        {
            var createdt = CommonHelper.ConvertDateTimeInt(DateTime.Now);
            var builder = new StringBuilder();
            builder.Append("<xml>");
            builder.Append("<ToUserName><![CDATA[" + toUser + "]]></ToUserName>");
            builder.Append("<FromUserName><![CDATA[" + fromUser + "]]></FromUserName>");
            builder.Append("<CreateTime>" + createdt.ToString() + "</CreateTime>");
            builder.Append("<MsgType><![CDATA[transfer_customer_service]]></MsgType>");
            builder.Append("</xml>");
            return builder.ToString();
        }

        #region Template Msg
        public static void SendExpMsg()
        {
            var text = $@"
				{{ 
				    ""touser"": ""{"oc0Qp0m9wYecC0cBihpsDdsS-4FU"}"",
				    ""template_id"": ""{"u6slk3ft-7x4jRR8lgy-ZlvNtGbMzpP3zeOyX4dGN_0"}"",
                    ""topcolor"":""#FF0000"",
                    ""url"":"""",
				    ""data"": {{
						""first"":{{""value"":""{"数据发现异常"}"",""color"":""#173177""}},
						""keyword1"":{{""value"":""{"学员需求未同步至ICAS库"}"",""color"":""#173177""}},
						""keyword2"":{{""value"":""{DateTime.Now}"",""color"":""#173177""}},
                        ""keyword3"":{{""value"":""{"严重错误"}"",""color"":""#173177""}},
                        ""remark"":{{""value"":""{"小程序发现异常数据，请速度修复并排查日志"}"",""color"":""#173177""}}
					}}
				}}";
            WeChatHelper.SendTemplateMsg(text);
        }

        #endregion
    }
}
