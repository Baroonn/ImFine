namespace ImFine.Client.DTOs
{
    public class GroupSearchDto
    {
        public string? name { get; set; }
        public int intervalInMinutes { get; set; }
        public string? owner { get; set; }
        public bool following { get; set; }

        public string ActionToTake => following ? "Leave" : "Join";
    }
}
