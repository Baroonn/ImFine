namespace ImFine.Client.Models.Exceptions
{
    public class UserUnauthorizedException : Exception
    {
        public UserUnauthorizedException()
           : base("User is unauthorized")
        { }
    }
}
