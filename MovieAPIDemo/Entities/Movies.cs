using MovieAPIDemo.Models;

namespace MovieAPIDemo.Entities
{
    public class Movies
    {

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public ICollection<Person> Actors { get; set; }

        public string Language { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string CoverImage { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        internal object Select(Func<object, MovieListViewModel> value)
        {
            throw new NotImplementedException();
        }
    }
}
