using System;
using System.Text;

namespace SmallReport.Assist.WeChat
{
    public static class MessageHelper
    {

        public static string ReturnTextMsg(string ToUserName, string FromUserName, string Content)
        {
            int num = CommonHelper.ConvertDateTimeInt(DateTime.Now);
            StringBuilder builder = new StringBuilder();
            builder.Append("<xml>");
            builder.Append("<ToUserName><![CDATA[" + ToUserName + "]]></ToUserName>");
            builder.Append("<FromUserName><![CDATA[" + FromUserName + "]]></FromUserName>");
            builder.Append("<CreateTime>" + num.ToString() + "</CreateTime>");
            builder.Append("<MsgType><![CDATA[text]]></MsgType>");
            builder.Append("<Content><![CDATA[" + Content + "]]></Content>");
            builder.Append("</xml>");
            return builder.ToString();
        }

        public static string ReturnImageMsg(string ToUserName, string FromUserName, string media_id)
        {
            int createdt = CommonHelper.ConvertDateTimeInt(DateTime.Now);
            StringBuilder sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<ToUserName><![CDATA[" + ToUserName + "]]></ToUserName>");
            sb.Append("<FromUserName><![CDATA[" + FromUserName + "]]></FromUserName>");
            sb.Append("<CreateTime>" + createdt.ToString() + "</CreateTime>");
            sb.Append("<MsgType><![CDATA[image]]></MsgType>");
            sb.Append("<Image>");
            sb.Append("<MediaId><![CDATA[" + media_id + "]]></MediaId>");
            sb.Append("</Image>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        public static string ReturnVideoMsg(string ToUserName, string FromUserName, string media_id, string title, string description)
        {
            int createdt = CommonHelper.ConvertDateTimeInt(DateTime.Now);
            StringBuilder sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<ToUserName><![CDATA[" + ToUserName + "]]></ToUserName>");
            sb.Append("<FromUserName><![CDATA[" + FromUserName + "]]></FromUserName>");
            sb.Append("<CreateTime>" + createdt.ToString() + "</CreateTime>");
            sb.Append("<MsgType><![CDATA[video]]></MsgType>");
            sb.Append("<Video>");
            sb.Append("<MediaId><![CDATA[" + media_id + "]]></MediaId>");
            sb.Append("<Title><![CDATA[" + title + "]]></Title>");
            sb.Append("<Description><![CDATA[" + description + "]]></Description>");
            sb.Append("</Video> ");
            sb.Append("</xml>");
            return sb.ToString();
        }

        public static string ReturnMusic(string toUser, string FromUserName, string TITLE, string DESCRIPTION, string MUSIC_Url, string HQ_MUSIC_Url)
        {
            int createdt = CommonHelper.ConvertDateTimeInt(DateTime.Now);
            StringBuilder sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<ToUserName><![CDATA[" + toUser + "]]></ToUserName>");
            sb.Append("<FromUserName><![CDATA[" + FromUserName + "]]></FromUserName>");
            sb.Append("<CreateTime>" + createdt.ToString() + "</CreateTime>");
            sb.Append("<MsgType><![CDATA[music]]></MsgType>");
            sb.Append("<Music>");
            sb.Append("<Title><![CDATA[" + TITLE + "]]></Title>");
            sb.Append("<Description><![CDATA[" + DESCRIPTION + "]]></Description>");
            sb.Append("<MusicUrl><![CDATA[" + MUSIC_Url + "]]></MusicUrl>");
            sb.Append("<HQMusicUrl><![CDATA[" + HQ_MUSIC_Url + "]]></HQMusicUrl>");
            sb.Append("</Music>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        public static string ReturnVoice(string toUser, string fromUser, string media_id)
        {
            int createdt = CommonHelper.ConvertDateTimeInt(DateTime.Now);
            StringBuilder sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<ToUserName><![CDATA[" + toUser + "]]></ToUserName>");
            sb.Append("<FromUserName><![CDATA[" + fromUser + "]]></FromUserName>");
            sb.Append("<CreateTime>" + createdt.ToString() + "</CreateTime>");
            sb.Append("<MsgType><![CDATA[voice]]></MsgType>");
            sb.Append("<Voice>");
            sb.Append("<MediaId><![CDATA[" + media_id + "]]></MediaId>");
            sb.Append("</Voice>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        public static string AccessService(string toUser, string fromUser)
        {
            int createdt = CommonHelper.ConvertDateTimeInt(DateTime.Now);
            StringBuilder builder = new StringBuilder();
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
            string text = string.Format(@"
				{{ 
				    ""touser"": ""{0}"",
				    ""template_id"": ""{1}"",
                    ""topcolor"":""#FF0000"",
                    ""url"":"""",
				    ""data"": {{
						""first"":{{""value"":""{2}"",""color"":""#173177""}},
						""keyword1"":{{""value"":""{3}"",""color"":""#173177""}},
						""keyword2"":{{""value"":""{4}"",""color"":""#173177""}},
                        ""keyword3"":{{""value"":""{5}"",""color"":""#173177""}},
                        ""remark"":{{""value"":""{6}"",""color"":""#173177""}}
					}}
				}}", "oc0Qp0m9wYecC0cBihpsDdsS-4FU",
                   "u6slk3ft-7x4jRR8lgy-ZlvNtGbMzpP3zeOyX4dGN_0",
                   "数据发现异常",
                   "同步异常或空指针",
                   DateTime.Now,
                   "严重错误",
                   "调度发现异常数据，请速度修复并排查日志");
            WeChatHelper.SendTemplateMsg(text);
        }

        #endregion
    }
}
