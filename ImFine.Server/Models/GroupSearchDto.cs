namespace ImFine.Server.Models
{
    public class GroupSearchDto
    {
        public string? name { get; set; }
        public int intervalInMinutes { get; set; }
        public string? owner { get; set; }
        public bool following { get; set; }
    }
}
