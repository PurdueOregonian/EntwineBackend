using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntwineBackend.DbItems
{
    [Index(nameof(ChatId))]
    public class CommunityChatMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ChatId { get; set; }
        public required ProfileData Sender { get; set; }
        public string? Content { get; set; }
        public DateTime? TimeSent { get; set; }
    }
}
