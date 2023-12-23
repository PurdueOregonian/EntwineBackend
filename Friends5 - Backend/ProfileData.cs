using System.ComponentModel.DataAnnotations;

namespace Friends5___Backend
{
    public class ProfileData
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Interest { get; set; }
    }
}
