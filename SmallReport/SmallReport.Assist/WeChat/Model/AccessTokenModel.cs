namespace SmallReport.Assist.WeChat.Model
{

    public class AccessTokenModel
    {
        public string access_token { get; set; }

        public int expires_in { get; set; }

        public string RefreshToken { get; set; }

        public string Openid { get; set; }

        public string Scope { get; set; }
    }
}
