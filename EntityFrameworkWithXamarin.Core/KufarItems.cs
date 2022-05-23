using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EntityFrameworkWithXamarin.Core
{
    public class KufarItems
    {
         
        [JsonProperty("ads")]
        public Ad[] Ads { get; set; }

        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }
    }

    public partial class Ad
    {
        [JsonProperty("account_id")]
        public long AccountId { get; set; }

        [JsonProperty("account_parameters")]
        public AccountParameter[] AccountParameters { get; set; }

        [JsonProperty("ad_id")]
        public long AdId { get; set; }

        [JsonProperty("ad_link")]
        public Uri AdLink { get; set; }

        [JsonProperty("ad_parameters")]
        public AdParameter[] AdParameters { get; set; }

        [JsonProperty("body")]
        public object Body { get; set; }

        [JsonProperty("category")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Category { get; set; }

        [JsonProperty("company_ad")]
        public bool CompanyAd { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("images")]
        public Image[] Images { get; set; }

        [JsonProperty("list_id")]
        public long ListId { get; set; }

        [JsonProperty("list_time")]
        public DateTimeOffset ListTime { get; set; }

        [JsonProperty("paid_services")]
        public PaidServices PaidServices { get; set; }

        [JsonProperty("phone")]
        public object Phone { get; set; }

        [JsonProperty("phone_hidden")]
        public bool PhoneHidden { get; set; }

        [JsonProperty("price_byn")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long PriceByn { get; set; }

        [JsonProperty("price_usd")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long PriceUsd { get; set; }

        [JsonProperty("remuneration_type")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long RemunerationType { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class AccountParameter
    {
        [JsonProperty("pl")]
        public string Pl { get; set; }

        [JsonProperty("vl")]
        public string Vl { get; set; }

        [JsonProperty("p")]
        public string P { get; set; }

        [JsonProperty("v")]
        public string V { get; set; }

        [JsonProperty("pu")]
        public string Pu { get; set; }
    }

    public partial class AdParameter
    {
        [JsonProperty("pl")]
        public string Pl { get; set; }

        [JsonProperty("vl")]
        public Vl Vl { get; set; }

        [JsonProperty("p")]
        public string P { get; set; }

        

        [JsonProperty("pu")]
        public string Pu { get; set; }
    }

    public partial class Image
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("yams_storage")]
        public bool YamsStorage { get; set; }
    }

    public partial class PaidServices
    {
        [JsonProperty("halva")]
        public bool Halva { get; set; }

        [JsonProperty("highlight")]
        public bool Highlight { get; set; }

        [JsonProperty("polepos")]
        public bool Polepos { get; set; }

        [JsonProperty("ribbons")]
        public object Ribbons { get; set; }
    }

    public partial class Pagination
    {
        [JsonProperty("pages")]
        public Page[] Pages { get; set; }
    }

    public partial class Page
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("num")]
        public long Num { get; set; }

        [JsonProperty("token")]
        public object Token { get; set; }
    }

    

    public partial struct Vl
    {
        public string String;
        public string[] StringArray;

        public static implicit operator Vl(string String) => new Vl { String = String };
        public static implicit operator Vl(string[] StringArray) => new Vl { StringArray = StringArray };
    }

   
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
               
                VlConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    

    internal class VlConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Vl) || t == typeof(Vl?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new Vl { String = stringValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<string[]>(reader);
                    return new Vl { StringArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type Vl");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (Vl)untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }
            if (value.StringArray != null)
            {
                serializer.Serialize(writer, value.StringArray);
                return;
            }
            throw new Exception("Cannot marshal type Vl");
        }

        public static readonly VlConverter Singleton = new VlConverter();
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
