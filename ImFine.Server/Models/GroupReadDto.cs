namespace ImFine.Server.Models
{
    public class GroupReadDto
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public int intervalInMinutes { get; set; }
        public string? status { get; set; }
        public string? owner { get; set; }
        public string? members { get; set; }
        //public string? role { get; set; }
        public string? currentUser { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
