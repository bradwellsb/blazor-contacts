using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorContacts.Shared.Models
{
    public class Contact
    {
        [Key]
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Phone Number")]
        [JsonPropertyName("phonenumber")]
        public string PhoneNumber { get; set; }
    }
}
