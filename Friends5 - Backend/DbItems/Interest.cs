using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Friends5___Backend.DbItems
{
    [Index(nameof(Id), IsUnique = true)]
    public class Interest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public List<int>? Categories { get; set; }
        public string? Name { get; set; }
    }
}
