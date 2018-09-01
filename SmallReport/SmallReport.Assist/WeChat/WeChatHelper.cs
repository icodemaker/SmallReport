using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;

namespace SmallReport.Assist.WeChat
{
    public class WeChatHelper
    {
        //Dev Key
        public static readonly string OpenAppId = ConfigurationManager.AppSettings["OpenAppId"].ToString();
        public static readonly string OpenAppSecret = ConfigurationManager.AppSettings["OpenAppSecret"].ToString();
        public static readonly string AccessTokenUrl = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";

        private static readonly string Token = ConfigurationManager.AppSettings["globalToken"].ToString();

        #region Valid

        public static bool Valid()
        {
            string echoStr = HttpContext.Current.Request.QueryString["echoStr"];
            if (CheckSignature())
            {
                if (!string.IsNullOrEmpty(echoStr))
                {
                    HttpContext.Current.Response.Write(echoStr);
                    HttpContext.Current.Response.End();

                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckSignature()
        {
            string signature = HttpContext.Current.Request.QueryString["signature"];
            string timestamp = HttpContext.Current.Request.QueryString["timestamp"];
            string nonce = HttpContext.Current.Request.QueryString["nonce"];
            string[] ArrTmp = { Token, timestamp, nonce };
            Array.Sort(ArrTmp);     //字典排序   
            string tmpStr = string.Join("", ArrTmp);
            tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            tmpStr = tmpStr.ToLower();
            if (tmpStr == signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Msg

        public static MessageModel GetXmlMessage()
        {
            Stream inputStream = HttpContext.Current.Request.InputStream;
            if (inputStream.Length > 0)
            {
                byte[] buffer = new byte[inputStream.Length];
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
            XElement xdoc = XElement.Parse(messageText);
            var type = typeof(T);
            var t = Activator.CreateInstance<T>();
            foreach (XElement element in xdoc.Elements())
            {
                var pr = type.GetProperty(element.Name.ToString());
                if (pr != null)
                {
                    if (element.HasElements)
                    {
                        foreach (var ele in element.Elements())
                        {
                            pr = type.GetProperty(ele.Name.ToString());
                            pr.SetValue(t, Convert.ChangeType(ele.Value, pr.PropertyType), null);
                        }
                        continue;
                    }
                    pr.SetValue(t, Convert.ChangeType(element.Value, pr.PropertyType), null);
                }
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
        
        public static string GetExistAccessToken()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/Config/AuthToken.config");
            StreamReader str = new StreamReader(filePath, System.Text.Encoding.UTF8);
            XmlDocument xml = new XmlDocument();
            xml.Load(str);
            str.Close();
            str.Dispose();
            string token = xml.SelectSingleNode("xml").SelectSingleNode("AccessToken").InnerText;
            DateTime AccessExpires = Convert.ToDateTime(xml.SelectSingleNode("xml").SelectSingleNode("AccessExpires").InnerText);

            if (DateTime.Now >= AccessExpires)
            {
                AccessTokenModel model = GetAccessToken();
                xml.SelectSingleNode("xml").SelectSingleNode("AccessToken").InnerText = model.access_token;
                DateTime accessToken = DateTime.Now.AddSeconds(model.expires_in);
                xml.SelectSingleNode("xml").SelectSingleNode("AccessExpires").InnerText = accessToken.ToString();
                xml.Save(filePath);
                token = model.access_token;
            }
            return token;
        }

        public static AccessTokenModel GetAccessToken()
        {
            string assecc_url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + OpenAppId + "&secret=" + OpenAppSecret;
            AccessTokenModel atoken = new AccessTokenModel();
            string content = HttpGet(assecc_url);
            AccessTokenModel token = new AccessTokenModel();
            token = ParseFromJson<AccessTokenModel>(content);
            atoken.access_token = token.access_token;
            atoken.expires_in = token.expires_in;
            return atoken;
        }
        #endregion

        #region JSAPI

        public static string GetExistTicket()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/Config/JsTicket.config");
            StreamReader str = new StreamReader(filePath, System.Text.Encoding.UTF8);
            XmlDocument xml = new XmlDocument();
            xml.Load(str);
            str.Close();
            str.Dispose();
            string token = xml.SelectSingleNode("xml").SelectSingleNode("AccessToken").InnerText;
            DateTime AccessExpires = Convert.ToDateTime(xml.SelectSingleNode("xml").SelectSingleNode("AccessExpires").InnerText);
            if (DateTime.Now >= AccessExpires)
            {
                var tkm = GetTicket();
                xml.SelectSingleNode("xml").SelectSingleNode("AccessToken").InnerText = tkm.ticket;
                DateTime accessToken = DateTime.Now.AddSeconds(tkm.expires_in);
                xml.SelectSingleNode("xml").SelectSingleNode("AccessExpires").InnerText = accessToken.ToString();
                xml.Save(filePath);
                token = tkm.ticket;
            }
            return token;
        }

        public static JsTicketModel GetTicket()
        {
            try
            {
                string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + WeChatHelper.GetExistAccessToken() + "&type=jsapi";
                string tt = WeChatHelper.HttpGet(url);
                if (!string.IsNullOrEmpty(tt))
                {
                    return JSONHelper.Decode<JsTicketModel>(tt);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static SignatureModel getJsSignMap(string jsapi_ticket, string url)
        {
            long ticks = (DateTime.Now.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000;
            SignatureModel map = new SignatureModel();
            string nonce_str = Guid.NewGuid().ToString().Replace("-", "");
            string timestamp = ticks.ToString();
            string sign = "jsapi_ticket=" + jsapi_ticket + "&noncestr=" + nonce_str + "&timestamp=" + timestamp + "&url=" + url;
            string signature = FormsAuthentication.HashPasswordForStoringInConfigFile(sign, "SHA1");
            map.url = url;
            map.jsapi_ticket = jsapi_ticket;
            map.nonceStr = nonce_str;
            map.timestamp = timestamp;
            map.signature = signature;
            return map;
        }
        #endregion

        #region Public

        public static string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;
            WebResponse response = null;
            string responseStr = null;
            try
            {
                response = request.GetResponse();
                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                response = null;
            }
            return responseStr;
        }

        public static T ParseFromJson<T>(string szJson)
        {
            return JSONHelper.Decode<T>(szJson);
        }
        #endregion
    }
}
