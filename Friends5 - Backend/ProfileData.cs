using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Friends5___Backend
{
    [Index(nameof(Username), IsUnique = true)]
    public class ProfileData
    {
        [Key]
        public string? Username { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
    }
}
