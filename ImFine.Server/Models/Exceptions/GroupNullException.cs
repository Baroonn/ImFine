namespace ImFine.Server.Models.Exceptions
{
    public sealed class GroupNullException : BadRequestException
    {
        public GroupNullException()
            : base("No group was provided to create")
        { }
    }
}
