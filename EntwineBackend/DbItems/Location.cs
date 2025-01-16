using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntwineBackend.DbItems
{
    [Index(nameof(Id), IsUnique = true)]
    public class Location
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
