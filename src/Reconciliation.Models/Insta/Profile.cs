using System;
using System.Text.Json.Serialization;

[assembly: CLSCompliant(true)]
namespace Reconciliation.Models.Insta
{
    public class Profile
    {
        public string Biography { get; set; }
        [JsonPropertyName("date_joined")]
        public DateTime DateJoined { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Gender { get; set; }
        public bool PrivateAccount { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        [JsonPropertyName("profile_picture_changes")]
        public UploadTimestampObject[] ProfilePictureChanges { get; set; }
    }
}
