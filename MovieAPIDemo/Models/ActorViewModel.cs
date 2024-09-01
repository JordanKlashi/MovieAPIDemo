using System.ComponentModel.DataAnnotations;

namespace MovieAPIDemo.Models
{
    public class ActorViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
