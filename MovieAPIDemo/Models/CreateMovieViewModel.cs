using Microsoft.OpenApi.MicrosoftExtensions;
using MovieAPIDemo.Entities;
using System.ComponentModel.DataAnnotations;

namespace MovieAPIDemo.Models
{
    public class CreateMovieViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<int> Actors { get; set; }
        public string Language { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string CoverImage { get; set; }
    }

}
