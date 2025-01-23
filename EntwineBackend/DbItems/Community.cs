using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntwineBackend.DbItems
{
    // 1 to 1 with Locations for now but we may want to store other info like extra chatrooms
    // in a community, which wouldn't make sense on Location.
    [Index(nameof(Id), IsUnique = true)]
    public class Community
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? Location { get; set; }
        public List<int>? UserIds { get; set; }
    }
}
