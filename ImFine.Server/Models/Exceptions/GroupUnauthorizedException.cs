namespace ImFine.Server.Models.Exceptions
{
    public sealed class GroupUnauthorizedException : UnauthorizedException
    {
        public GroupUnauthorizedException()
            : base("You are not authorized to perform this action on this group.")
        { }
    }
}
