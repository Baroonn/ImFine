namespace ImFine.Server.Models.Exceptions
{
    public class GroupNotFoundException : NotFoundException
    {
        public GroupNotFoundException(string name)
            : base($"Group {name} does not exist.")
        { }
    }
}
