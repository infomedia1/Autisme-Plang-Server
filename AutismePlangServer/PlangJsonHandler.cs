// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using NSPlang;
//
//    var plang = Plang.FromJson(jsonString);

namespace NSPlang
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using J = Newtonsoft.Json.JsonPropertyAttribute;
    using N = Newtonsoft.Json.NullValueHandling;

    public class Response
    {
        [J("message", NullValueHandling = N.Ignore)] public string Message { get; set; }
        [J("sender", NullValueHandling = N.Ignore)] public string Sender { get; set; }
        [J("info", NullValueHandling = N.Ignore)] public string Info { get; set; }
        [J("concernuserid", NullValueHandling = N.Ignore)] public int Concernuserid { get; set; }
        [J("settoserviceid", NullValueHandling = N.Ignore)] public int Settoserviceid { get; set; }
        [J("userdata", NullValueHandling = N.Ignore)] public Personal Userdata { get; set; }
    }

    public partial class JsonRes
    {
        [J("type")] public string Type { get; set; }
        [J("content", NullValueHandling = N.Ignore)] public Content Content { get; set; }
        [J("response", NullValueHandling = N.Ignore)] public Response Response { get; set; }
    }

    public partial class Content
    {
        [J("Date", NullValueHandling = N.Ignore)] public DateTime? Date { get; set; }
        [J("Services", NullValueHandling = N.Ignore)] public List<Service> Services { get; set; }
        [J("user", NullValueHandling = N.Ignore)] public Personal Personal { get; set; }
    }

    public partial class Service
    {
        [J("name", NullValueHandling = N.Ignore)] public string Name { get; set; }
        [J("id", NullValueHandling = N.Ignore)] public int Id { get; set; }
        [J("c", NullValueHandling = N.Ignore)] public long? C { get; set; }
        [J("auerzait", NullValueHandling = N.Ignore)] public string Auerzait { get; set; }
        [J("encadrantsMoies", NullValueHandling = N.Ignore)] public List<Personal> EncadrantsMoies { get; set; }
        [J("usagersMoies", NullValueHandling = N.Ignore)] public List<Personal> UsagersMoies { get; set; }
        [J("encadrantsMettes", NullValueHandling = N.Ignore)] public List<Personal> EncadrantsMettes { get; set; }
        [J("usagersMettes", NullValueHandling = N.Ignore)] public List<Personal> UsagersMettes { get; set; }
        [J("Moies", NullValueHandling = N.Ignore)] public List<Personal> Moies { get; set; }
        [J("Mettes", NullValueHandling = N.Ignore)] public List<Personal> Mettes { get; set; }
        [J("Dag", NullValueHandling = N.Ignore)] public List<Personal> Dag { get; set; }
    }

    public partial class Personal
    {
        [J("id", NullValueHandling = N.Ignore)] public long Id { get; set; }
        [J("numm")] public string Numm { get; set; }
        [J("virnumm")] public string Virnumm { get; set; }
        [J("isencadrant", NullValueHandling = N.Ignore)] public Boolean Isencadrant { get; set; }
        [J("online", NullValueHandling = N.Ignore)] public Boolean Online { get; set; }
        [J("bday", NullValueHandling = N.Ignore)] public DateTime? BDay { get; set; }
        [J("timings", NullValueHandling = N.Ignore)] public Timings Timings { get; set; }
        [J("photo", NullValueHandling = N.Ignore)] public string Photo { get; set; }
        [J("partial", NullValueHandling = N.Ignore)] public string Partial { get; set; }
        [J("info", NullValueHandling = N.Ignore)] public Info Info { get; set; }
        [J("iessenclass", NullValueHandling = N.Ignore)] public IessenClass IessenClass { get; set; }
    }

    public partial class IessenClass
    {
        [J("iessenauswiel", NullValueHandling = N.Ignore)] public Iessen Auswiel { get; set; }
        [J("service", NullValueHandling = N.Ignore)] public string Service { get; set; }
    }

    public partial class Iessen
    {
        [J("menu", NullValueHandling = N.Ignore)] public bool? Menu { get; set; }
        [J("al1", NullValueHandling = N.Ignore)] public bool? Alternative1 { get; set; }
        [J("al2", NullValueHandling = N.Ignore)] public bool? Alternative2 { get; set; }
        [J("br", NullValueHandling = N.Ignore)] public string Breitchen { get; set; }
    }

    public class Timings
    {
        [J("event", NullValueHandling = N.Ignore)] public string Event { get; set; }
        [J("timestamp", NullValueHandling = N.Ignore)] public DateTime? Timestamp { get; set; }
    }

    public class Info
    {
        [J("info", NullValueHandling = N.Ignore)] public string InfoString { get; set; }
        [J("value", NullValueHandling = N.Ignore)] public string Value { get; set; }
    }

    public class SonySystem
    {
        [J("result", NullValueHandling = N.Ignore)] public List<SonyResult> Result { get; set; }
        [J("id", NullValueHandling = N.Ignore)] public long? Id { get; set; }
        [J("method", NullValueHandling = N.Ignore)] public string Method { get; set; }
        [J("version", NullValueHandling = N.Ignore)] public string Version { get; set; }
        [J("params", NullValueHandling = N.Ignore)] public List<SonyResult> Params { get; set; }
    }

    public partial class SonyResult
    {
        [J("status", NullValueHandling = N.Ignore)] public string Status { get; set; }
        [J("uri", NullValueHandling = N.Ignore)] public string Uri { get; set; }
        [J("data", NullValueHandling = N.Ignore)] public string Data { get; set; }
    }

    public partial class JsonRes
    {
        public static JsonRes FromJson(string json) => JsonConvert.DeserializeObject<JsonRes>(json, NSPlang.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this JsonRes self) => JsonConvert.SerializeObject(self, NSPlang.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeLocal }
            },
        };
    }
}