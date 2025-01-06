using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Friends5___Backend.DbItems
{
    [Index(nameof(Id), IsUnique = true)]
    public class Community
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Location { get; set; }
        public List<int>? UserIds { get; set; }
    }
}
