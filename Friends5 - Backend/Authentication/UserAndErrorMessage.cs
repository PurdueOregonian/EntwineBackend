using Friends5___Backend.UserId;

namespace Friends5___Backend.Authentication
{
    public class UserAndErrorMessage
    {
        public ApplicationUser? User { get; set; }
        // Set if unexpected exception
        public string? ErrorMessage { get; set; }
    }
}
