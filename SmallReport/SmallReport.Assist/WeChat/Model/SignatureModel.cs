namespace SmallReport.Assist.WeChat.Model
{
    public class SignatureModel
    {
        public string url { get; set; }
        public string jsapi_ticket { get; set; }
        public string nonceStr { get; set; }
        public string timestamp { get; set; }
        public string signature { get; set; }
    }
}
