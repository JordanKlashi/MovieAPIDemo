using System.ComponentModel.DataAnnotations;

namespace MovieAPIDemo.Entities
{
    public class Person
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        public ICollection<Movies>  Movies { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }
    }
}
