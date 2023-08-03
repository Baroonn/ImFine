using System.ComponentModel.DataAnnotations;

namespace ImFine.Server.Models
{
    public class Group
    {
        public string? id { get; set; } = Guid.NewGuid().ToString();
        [Required(ErrorMessage = "Group name is required")]
        public string? name { get; set; }
        public int intervalInMinutes { get; set; } = 15;
        public string? status { get; set; } = "stop";
        public string? owner { get; set; }
        public string? members { get; set; } = "";
        public string? currentUser { get; set; } = "";
        public string? lastSeen { get; set; } = "";
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
