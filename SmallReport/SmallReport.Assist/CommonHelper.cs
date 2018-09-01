using System;

namespace SmallReport.Assist
{
    public class CommonHelper
    {
        #region WeChat

        public static int ConvertDateTimeInt(DateTime time)
        {
            DateTime time2 = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan span = (TimeSpan)(time - time2);
            return (int)span.TotalSeconds;
        }
        #endregion
    }
}
