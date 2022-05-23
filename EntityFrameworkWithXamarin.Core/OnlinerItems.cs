using System;
using System.Collections.Generic;
using System.Text;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EntityFrameworkWithXamarin.Core
{
    public class OnlinerItems
    {
        [JsonProperty("tasks")]
        public Taski[] Tasks { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("page")]
        public Page Page { get; set; }
    }

    public partial class Page
    {
        [JsonProperty("limit")]
        public long Limit { get; set; }

        [JsonProperty("items")]
        public long Items { get; set; }

        [JsonProperty("current")]
        public long Current { get; set; }

        [JsonProperty("last")]
        public long Last { get; set; }
    }

    public partial class Taski
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }

        [JsonProperty("price")]
        public Price Price { get; set; }

        [JsonProperty("executor_id")]
        public long? ExecutorId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("proposals_qty")]
        public long ProposalsQty { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("deadline")]
        public DateTimeOffset Deadline { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("deleted")]
        public object Deleted { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("html_url")]
        public Uri HtmlUrl { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("section")]
        public Section Section { get; set; }

        [JsonProperty("permissions")]
        public Permissions Permissions { get; set; }
    }

    public partial class Author
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("photo")]
        public Uri Photo { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("html_url")]
        public Uri HtmlUrl { get; set; }
    }

    public partial class Image
    {
        [JsonProperty("original")]
        public Uri Original { get; set; }

        [JsonProperty("280x280")]
        public Uri The280X280 { get; set; }

        [JsonProperty("640x320")]
        public Uri The640X320 { get; set; }

        [JsonProperty("2100x1200")]
        public Uri The2100X1200 { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("geo_town_id")]
        public long GeoTownId { get; set; }

        [JsonProperty("region_id")]
        public long RegionId { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("town")]
        public string Town { get; set; }

        [JsonProperty("formatted_locality")]
        public string FormattedLocality { get; set; }

        [JsonProperty("street_address")]
        public string StreetAddress { get; set; }
    }

    public partial class Permissions
    {
        [JsonProperty("admin")]
        public bool Admin { get; set; }

        [JsonProperty("accept")]
        public bool Accept { get; set; }

        [JsonProperty("close")]
        public bool Close { get; set; }

        [JsonProperty("delete")]
        public bool Delete { get; set; }

        [JsonProperty("propose")]
        public bool Propose { get; set; }
    }

    public partial class Price
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public partial class Section
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }
    }
}
