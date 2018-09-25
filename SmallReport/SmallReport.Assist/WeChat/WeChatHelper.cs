using SmallReport.Assist.Quartz;
using SmallReport.Assist.WeChat.Model;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;

namespace SmallReport.Assist.WeChat
{
    public static class WeChatHelper
    {
        //Dev Key
        private static readonly string OpenAppId = ConfigurationManager.AppSettings["OpenAppId"];

        private static readonly string OpenAppSecret = ConfigurationManager.AppSettings["OpenAppSecret"];
        private static readonly string AccessTokenUrl = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";

        private static readonly string Token = ConfigurationManager.AppSettings["globalToken"];

        #region Valid

        public static bool Valid()
        {
            var echoStr = HttpContext.Current.Request.QueryString["echoStr"];
            if (CheckSignature())
            {
                if (string.IsNullOrEmpty(echoStr)) return true;
                HttpContext.Current.Response.Write(echoStr);
                HttpContext.Current.Response.End();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckSignature()
        {
            var signature = HttpContext.Current.Request.QueryString["signature"];
            var timestamp = HttpContext.Current.Request.QueryString["timestamp"];
            var nonce = HttpContext.Current.Request.QueryString["nonce"];
            string[] arrTmp = { Token, timestamp, nonce };
            Array.Sort(arrTmp);
            var tmpStr = string.Join("", arrTmp);
            tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            tmpStr = tmpStr.ToLower();
            return tmpStr == signature;
        }
        #endregion

        #region Msg

        public static MessageModel GetXmlMessage()
        {
            var inputStream = HttpContext.Current.Request.InputStream;
            if (inputStream.Length > 0)
            {
                var buffer = new byte[inputStream.Length];
                inputStream.Read(buffer, 0, (int)inputStream.Length);
                return ConvertObj<MessageModel>(Encoding.UTF8.GetString(buffer));
            }
            else
            {
                return null;
            }
        }

        private static T ConvertObj<T>(string messageText)
        {
            var xdoc = XElement.Parse(messageText);
            var type = typeof(T);
            var t = Activator.CreateInstance<T>();
            foreach (var element in xdoc.Elements())
            {
                var pr = type.GetProperty(element.Name.ToString());
                if (pr == null) continue;
                if (element.HasElements)
                {
                    foreach (var ele in element.Elements())
                    {
                        pr = type.GetProperty(ele.Name.ToString());
                        if (pr != null) pr.SetValue(t, Convert.ChangeType(ele.Value, pr.PropertyType), null);
                    }
                    continue;
                }
                pr.SetValue(t, Convert.ChangeType(element.Value, pr.PropertyType), null);
            }
            return t;
        }
        #endregion

        #region Data

        public static AccessTokenModel GetOpenIdByCode(string code)
        {
            var jsonAccess = HttpGet(string.Format(AccessTokenUrl, OpenAppId, OpenAppSecret, code));
            var accessToken = ParseFromJson<AccessTokenModel>(jsonAccess);
            return accessToken;
        }

        private static string GetExistAccessToken()
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\AuthToken.config";
            var str = new StreamReader(filePath, Encoding.UTF8);
            var xml = new XmlDocument();
            xml.Load(str);
            str.Close();
            str.Dispose();
            var token = xml.SelectSingleNode("xml")?.SelectSingleNode("AccessToken")?.InnerText;
            var accessExpires = Convert.ToDateTime(xml.SelectSingleNode("xml")?.SelectSingleNode("AccessExpires")?.InnerText);

            if (DateTime.Now < accessExpires) return token;
            var model = GetAccessToken();
            var selectSingleNode = xml.SelectSingleNode("xml");
            var singleNode = selectSingleNode?.SelectSingleNode("AccessToken");
            if (singleNode != null)
                singleNode.InnerText = model.access_token;
            var accessToken = DateTime.Now.AddSeconds(model.expires_in);
            var xmlNode = xml.SelectSingleNode("xml");
            var node = xmlNode?.SelectSingleNode("AccessExpires");
            if (node != null)
                node.InnerText = accessToken.ToString(CultureInfo.InvariantCulture);
            xml.Save(filePath);
            token = model.access_token;
            return token;
        }

        private static AccessTokenModel GetAccessToken()
        {
            var asseccUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + OpenAppId + "&secret=" + OpenAppSecret;
            var atoken = new AccessTokenModel();
            var content = HttpGet(asseccUrl);
            LogHelper.Info($"获取微信token结果：{content}");
            var token = ParseFromJson<AccessTokenModel>(content);
            atoken.access_token = token.access_token;
            atoken.expires_in = token.expires_in;
            return atoken;
        }
        #endregion

        #region JSAPI

        public static string GetExistTicket()
        {
            var filePath = HttpContext.Current.Server.MapPath("~/Config/JsTicket.config");
            var str = new StreamReader(filePath, Encoding.UTF8);
            var xml = new XmlDocument();
            xml.Load(str);
            str.Close();
            str.Dispose();
            var token = xml.SelectSingleNode("xml")?.SelectSingleNode("AccessToken")?.InnerText;
            var accessExpires = Convert.ToDateTime(xml.SelectSingleNode("xml")?.SelectSingleNode("AccessExpires")?.InnerText);
            if (DateTime.Now < accessExpires) return token;
            var tkm = GetTicket();
            var selectSingleNode = xml.SelectSingleNode("xml");
            var singleNode = selectSingleNode?.SelectSingleNode("AccessToken");
            if (singleNode != null)
                singleNode.InnerText = tkm.ticket;
            var accessToken = DateTime.Now.AddSeconds(tkm.expires_in);
            var xmlNode = xml.SelectSingleNode("xml");
            var node = xmlNode?.SelectSingleNode("AccessExpires");
            if (node != null)
                node.InnerText = accessToken.ToString(CultureInfo.InvariantCulture);
            xml.Save(filePath);
            token = tkm.ticket;
            return token;
        }

        private static JsTicketModel GetTicket()
        {
            try
            {
                var url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + GetExistAccessToken() + "&type=jsapi";
                var tt = HttpGet(url);
                return !string.IsNullOrEmpty(tt) ? JsonHelper.Decode<JsTicketModel>(tt) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static SignatureModel GetJsSignMap(string jsapiTicket, string url)
        {
            var ticks = (DateTime.Now.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000;
            var map = new SignatureModel();
            var nonceStr = Guid.NewGuid().ToString().Replace("-", "");
            var timestamp = ticks.ToString();
            var sign = "jsapi_ticket=" + jsapiTicket + "&noncestr=" + nonceStr + "&timestamp=" + timestamp + "&url=" + url;
            var signature = CommonHelper.Md5Hash(sign);
            map.url = url;
            map.jsapi_ticket = jsapiTicket;
            map.nonceStr = nonceStr;
            map.timestamp = timestamp;
            map.signature = signature;
            return map;
        }
        #endregion

        #region Public

        private static string HttpGet(string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;
            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            var responseStr = reader.ReadToEnd();
            reader.Close();
            return responseStr;
        }

        private static T ParseFromJson<T>(string szJson)
        {
            return JsonHelper.Decode<T>(szJson);
        }

        public static void SendTemplateMsg(string postContent)
        {
            try
            {
                var token = GetExistAccessToken();
                var url = $"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={token}";
                Post(url, postContent);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void Post(string url, string param)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            try
            {
                var requestStream = new StreamWriter(request.GetRequestStream());
                requestStream.Write(param);
                requestStream.Close();

                var response = request.GetResponse();
                {
                    var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("post to wechat ex:" + ex.Message + "。time:" + DateTime.Now);
            }
        }
        #endregion
    }
}
