using System;
using System.Text.Json.Serialization;

namespace Reconciliation.Models.Insta
{
    public class UploadTimestampObject
    {
        [JsonPropertyName("upload_timestamp")]
        public DateTime UploadTimestamp { get; set; }
    }
}
