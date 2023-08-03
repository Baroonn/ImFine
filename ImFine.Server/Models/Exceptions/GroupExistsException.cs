namespace ImFine.Server.Models.Exceptions
{
    public sealed class GroupExistsException : BadRequestException
    {
        public GroupExistsException()
            : base("Group already exists.")
        { }
    }
}
