using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallReport.Assist
{

    public static class JSONHelper
    {
        #region 编码

        public static string Encode<T>(T t)
        {
            return Encode(t, Formatting.None);
        }
        #endregion

        #region 编码

        public static string Encode<T>(T t, Formatting format)
        {
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            BigintConverter bigintConverter = new BigintConverter();         
            timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";

            return JsonConvert.SerializeObject(t, format, timeConverter, bigintConverter);
        }
        #endregion

        #region 解码

        public static T Decode<T>(string json)
        {
            BigintConverter bigintConverter = new BigintConverter();
            return (T)JsonConvert.DeserializeObject(json, typeof(T));
        }
        #endregion
    }

    #region Bigint转换成字符串

    public class BigintConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(System.Int64)
                || objectType == typeof(System.UInt64);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return 0;
            }
            else
            {
                IConvertible convertible = reader.Value as IConvertible;
                if (objectType == typeof(System.Int64))
                {
                    return convertible.ToInt64(CultureInfo.InvariantCulture);
                }
                else if (objectType == typeof(System.UInt64))
                {
                    return convertible.ToUInt64(CultureInfo.InvariantCulture);
                }
                return 0;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue("0");
            }
            else if (value is Int64 || value is UInt64)
            {
                writer.WriteValue(value.ToString());
            }
            else
            {
                throw new Exception("Expected Bigint value");
            }
        }
    }
    #endregion
}
