using System.ComponentModel.DataAnnotations;

namespace anitracker_minimal_api
{
    public class TitleItem
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public required string Title { get; set; }
    }
}
