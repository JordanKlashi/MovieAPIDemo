using AutoMapper;
using MovieAPIDemo.Entities;
using MovieAPIDemo.Models;

namespace MovieAPIDemo
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Movies, MovieListViewModel>();
            CreateMap<MovieListViewModel, Movies>();
            CreateMap<CreateMovieViewModel, Movies>().ForMember(x => x.Actors, y => y.Ignore());

            CreateMap<Person, ActorViewModel>();
            CreateMap<Person, ActorDetailsViewModel>();
            CreateMap<ActorViewModel, Person>();

        }
    }
}
