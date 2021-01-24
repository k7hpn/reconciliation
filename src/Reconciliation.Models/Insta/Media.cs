using System;
using System.Text.Json.Serialization;

namespace Reconciliation.Models.Insta
{
    public class Media
    {
        public string Caption { get; set; }
        [JsonPropertyName("taken_at")]
        public DateTime TakenAt { get; set; }
        public string Location { get; set; }
        public string Path { get; set; }
    }
}
