using EntwineBackend;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EntwineBackend.DbItems
{
    [Index(nameof(Id), IsUnique = true)]
    public class ProfileData
    {
        [Key]
        public int Id { get; set; }
        public string? Username { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public List<int>? Interests { get; set; }
        public int? Location { get; set; }
    }
}
