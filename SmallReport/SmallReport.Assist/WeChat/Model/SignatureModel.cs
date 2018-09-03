namespace SmallReport.Assist.WeChat.Model
{
    public class SignatureModel
    {
        public string Url { get; set; }
        public string JsapiTicket { get; set; }
        public string NonceStr { get; set; }
        public string Timestamp { get; set; }
        public string Signature { get; set; }
    }
}
