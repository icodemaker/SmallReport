namespace SmallReport.Assist.WeChat.Model
{

    public class AccessTokenModel
    {
        public string AccessToken { get; set; }

        public int ExpiresIn { get; set; }

        public string RefreshToken { get; set; }

        public string Openid { get; set; }

        public string Scope { get; set; }
    }
}
