using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace News.Models
{
    public class Source
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Article
    {
        public Source Source { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string UrlToImage { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string Content { get; set; }
    }

    public class FavoriteEntry
    {
        public Article Article { get; set; }
        public DateTime FavoritedAt { get; set; }
    }

    public class ArticlesResult
    {
        public string Status { get; set; }
        public int TotalResults { get; set; }
        public List<Article> Articles { get; set; }
    }

    public enum Countries
    {
        AE,
        /// <summary>
        /// Argentina
        /// </summary>
        AR,
        /// <summary>
        /// Austria
        /// </summary>
        AT,
        /// <summary>
        /// Australia
        /// </summary>
        AU,
        /// <summary>
        /// Belgium
        /// </summary>
        BE,
        BG,
        BR,
        /// <summary>
        /// Canada
        /// </summary>
        CA,
        CH,
        /// <summary>
        /// China
        /// </summary>
        CN,
        CO,
        CU,
        /// <summary>
        /// Czech Republic
        /// </summary>
        CZ,
        /// <summary>
        /// Germany
        /// </summary>
        DE,
        /// <summary>
        /// Egypt
        /// </summary>
        EG,
        /// <summary>
        /// France
        /// </summary>
        FR,
        /// <summary>
        /// United Kingdom
        /// </summary>
        GB,
        /// <summary>
        /// Greece
        /// </summary>
        GR,
        /// <summary>
        /// Hong Kong
        /// </summary>
        HK,
        /// <summary>
        /// Hungary
        /// </summary>
        HU,
        ID,
        /// <summary>
        /// Ireland
        /// </summary>
        IE,
        IL,
        IN,
        /// <summary>
        /// Italy
        /// </summary>
        IT,
        /// <summary>
        /// Japan
        /// </summary>
        JP,
        /// <summary>
        /// South Korea
        /// </summary>
        KR,
        LT,
        LV,
        MA,
        /// <summary>
        /// Mexico
        /// </summary>
        MX,
        MY,
        NG,
        /// <summary>
        /// Netherlands
        /// </summary>
        NL,
        /// <summary>
        /// Norway
        /// </summary>
        NO,
        /// <summary>
        /// New Zealand
        /// </summary>
        NZ,
        PH,
        PL,
        /// <summary>
        /// Portugal
        /// </summary>
        PT,
        RO,
        RS,
        /// <summary>
        /// Russia
        /// </summary>
        RU,
        SA,
        SE,
        SG,
        SI,
        SK,
        TH,
        TR,
        TW,
        UA,
        /// <summary>
        /// United States
        /// </summary>
        US,
        VE,
        ZA
    }

    public enum Languages
    {
        /// <summary>
        /// Afrikaans (South Africa)
        /// </summary>
        AF,
        AN,
        AR,
        AZ,
        BG,
        BN,
        BR,
        BS,
        CA,
        CS,
        CY,
        DA,
        /// <summary>
        /// German
        /// </summary>
        DE,
        EL,
        /// <summary>
        /// English
        /// </summary>
        EN,
        EO,
        /// <summary>
        /// Spanish
        /// </summary>
        ES,
        ET,
        EU,
        FA,
        FI,
        FR,
        GL,
        HE,
        HI,
        HR,
        HT,
        HU,
        HY,
        ID,
        IS,
        /// <summary>
        /// Italian
        /// </summary>
        IT,
        /// <summary>
        /// Japanese
        /// </summary>
        JP,
        JV,
        KK,
        KO,
        LA,
        LB,
        LT,
        LV,
        MG,
        MK,
        ML,
        MR,
        MS,
        /// <summary>
        /// Dutch
        /// </summary>
        NL,
        NN,
        NO,
        OC,
        PL,
        /// <summary>
        /// Portuguese
        /// </summary>
        PT,
        RO,
        RU,
        SH,
        SK,
        SL,
        SQ,
        SR,
        SV,
        SW,
        TA,
        TE,
        TH,
        TL,
        TR,
        UK,
        UR,
        VI,
        VO,
        /// <summary>
        /// Chinese
        /// </summary>
        ZH
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Categories
    {
        Business,
        Entertainment,
        Health,
        Science,
        Sports,
        Technology
    }

    public class ArticlesRequest
    {
        public string Query { get; set; }
        public Countries? Country { get; set; }
        public Languages? Language { get; set; }
        public Categories? Category { get; set; }
        public List<string> Sources { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
