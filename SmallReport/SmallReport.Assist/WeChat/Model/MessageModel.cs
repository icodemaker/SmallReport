namespace SmallReport.Assist.WeChat.Model
{
    public abstract class MessageModel
    {
        #region Public

        public string MsgId { get; set; }

        public string MsgType { get; set; }

        public string FromUserName { get; set; }

        public string ToUserName { get; set; }

        public string CreateTime { get; set; }
        #endregion

        #region Msg
        #region Text

        public string Content { get; set; }
        #endregion

        #region Media

        public string MediaId { get; set; }

        public string ThumbMediaId { get; set; }
        #endregion

        #region Pic

        public string PicUrl { get; set; }
        #endregion

        #region Audio

        public string Format { get; set; }
        #endregion

        #region Local

        public string Location_X { get; set; }

        public string Location_Y { get; set; }

        public string Scale { get; set; }

        public string Label { get; set; }
        #endregion

        #region Link

        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }
        #endregion
        #endregion
        #region Event

        public string Event { get; set; }

        public string EventKey { get; set; }

        public string Ticket { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Precision { get; set; }
        #endregion
    }
}
