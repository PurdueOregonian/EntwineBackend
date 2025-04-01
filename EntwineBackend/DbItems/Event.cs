using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntwineBackend.DbItems
{
    [Index(nameof(Community), nameof(Start))]
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Community { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int OrganizerId { get; set; }
        public string? Title { get; set; }
        public List<int>? UserIds { get; set; }
        public int MaxParticipants { get; set; }
    }
}
