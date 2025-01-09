using EntwineBackend.UserId;

namespace EntwineBackend.Authentication
{
    public class UserAndErrorMessage
    {
        public ApplicationUser? User { get; set; }
        // Set if unexpected exception
        public string? ErrorMessage { get; set; }
    }
}
