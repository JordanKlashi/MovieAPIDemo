using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPIDemo.Data;
using MovieAPIDemo.Entities;
using MovieAPIDemo.Models;
using System.Net.Http.Headers;
using System.Reflection;

namespace MovieAPIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieDbContext _context;
        private readonly IMapper _mapper;

        public MovieController(MovieDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = 10)
        {
            BaseResponseModel response = new BaseResponseModel();

            try
            {
                var movieCount = _context.Movie.Count();
                var movieList = _mapper.Map<List<MovieListViewModel>>(_context.Movie.Include(x => x.Actors).Skip(pageIndex * pageSize).Take(pageSize).ToList());

                response.Status = true;
                response.Message = "Success";
                response.Data = new { Count = movieCount, Movies = movieList };

                return Ok(response);
            }
            catch (Exception ex)
            {
                // TODO: Log the exception
                response.Status = false;
                response.Message = "Something went wrong";

                return BadRequest(response);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetMovieId(int id)
        {
            BaseResponseModel response = new BaseResponseModel();

            try
            {
                var movie = _context.Movie.Include(x => x.Actors).FirstOrDefault(x => x.Id == id);

                if (movie == null)
                {
                    response.Status = false;
                    response.Message = "Record Not Exist.";
                    return BadRequest(response);
                }

                var movieData = _mapper.Map<MovieDetailsViewModel>(movie);

                response.Status = true;
                response.Message = "Success";
                response.Data = movieData;

                return Ok(response);
            }
            catch (Exception ex)
            {
                // TODO: Log the exception
                response.Status = false;
                response.Message = "Something went wrong";

                return BadRequest(response);
            }
        }

        [HttpPost]
        public IActionResult Post(CreateMovieViewModel model)
        {
            BaseResponseModel response = new BaseResponseModel();

            try
            {
                if (ModelState.IsValid)
                {
                    var actors = _context.Person.Where(x => model.Actors.Contains(x.Id)).ToList();

                    if (actors.Count != model.Actors.Count)
                    {
                        response.Status = false;
                        response.Message = "Invalid";
                        return BadRequest(response);
                    }

                    var postedModel = _mapper.Map<Movies>(model);
                    postedModel.Actors = actors;

                    _context.Movie.Add(postedModel);
                    _context.SaveChanges();

                    var responseData = _mapper.Map<MovieDetailsViewModel>(postedModel);

                    response.Status = true;
                    response.Message = "Success";
                    response.Data = responseData;

                    return Ok(response);
                }
                else
                {
                    response.Status = false;
                    response.Message = "Erreur dans l'ajout d'un film.";
                    response.Data = ModelState;

                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Une erreur est survenue";
                return BadRequest(response);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, CreateMovieViewModel model)
        {
            BaseResponseModel response = new BaseResponseModel();

            try
            {
                if (ModelState.IsValid)
                {
                    if (id <= 0)
                    {
                        response.Status = false;
                        response.Message = "Invalid Movie Record.";
                        return BadRequest(response);
                    }

                    var actors = _context.Person.Where(x => model.Actors.Contains(x.Id)).ToList();
                    if (actors.Count != model.Actors.Count)
                    {
                        response.Status = false;
                        response.Message = "Invalid";
                        return BadRequest(response);
                    }

                    var movieDetails = _context.Movie.Include(x => x.Actors).FirstOrDefault(x => x.Id == id);

                    if (movieDetails == null)
                    {
                        response.Status = false;
                        response.Message = "Record Not Exist.";
                        return BadRequest(response);
                    }

                    movieDetails.Title = model.Title;
                    movieDetails.Description = model.Description;
                    movieDetails.Language = model.Language;
                    movieDetails.ReleaseDate = model.ReleaseDate;
                    movieDetails.CoverImage = model.CoverImage;

                    // Find removed actors
                    var removedActors = movieDetails.Actors.Where(x => !model.Actors.Contains(x.Id)).ToList();
                    foreach (var actor in removedActors)
                    {
                        movieDetails.Actors.Remove(actor);
                    }

                    // Find new actors
                    var newActors = actors.Except(movieDetails.Actors).ToList();
                    foreach (var actor in newActors)
                    {
                        movieDetails.Actors.Add(actor);
                    }

                    _context.SaveChanges();

                    var responseData = new MovieDetailsViewModel
                    {
                        Id = movieDetails.Id,
                        Title = movieDetails.Title,
                        Actors = movieDetails.Actors.Select(x => new ActorViewModel
                        {
                            Id = x.Id,
                            Name = x.Name,
                            DateOfBirth = x.DateOfBirth
                        }).ToList(),
                        CoverImage = movieDetails.CoverImage,
                        Language = movieDetails.Language,
                        ReleaseDate = movieDetails.ReleaseDate,
                        Description = movieDetails.Description
                    };

                    response.Status = true;
                    response.Message = "Updated Successfully";
                    response.Data = responseData;

                    return Ok(response);
                }
                else
                {
                    response.Status = false;
                    response.Message = "Invalid";
                    response.Data = ModelState;

                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Invalid";
                return BadRequest(response);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            BaseResponseModel response = new BaseResponseModel();

            try
            {
                var movie = _context.Movie.Where(x => x.Id == id).FirstOrDefault();

                if (movie == null)
                {
                    response.Status = false;
                    response.Message = "Record Not Exist.";
                    return BadRequest(response);
                }

                _context.Movie.Remove(movie);
                _context.SaveChanges();

                response.Status = true;
                response.Message = "Deleted Successfully";

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Invalid";
                return BadRequest(response);
            }
        }

        [HttpPost]
        [Route("upload-movie-poster")]
        public async Task<IActionResult> UploadMoviePoster(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest(new BaseResponseModel
                {
                    Status = false,
                    Message = "No file uploaded"
                });
            }

            try
            {
                var filename = ContentDispositionHeaderValue.Parse(imageFile.ContentDisposition)?.FileName?.Trim('"');
                string newPath = @"C:\Users\jorda\Desktop\formation developpeur web\CSharp\New Project ASP + React\StaticImage";

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                string[] allowedImageExtensions = new string[] { ".jpg", ".jpeg", ".png" };

                if (!allowedImageExtensions.Contains(Path.GetExtension(filename)?.ToLower()))
                {
                    return BadRequest(new BaseResponseModel
                    {
                        Status = false,
                        Message = "Invalid Image Format"
                    });
                }

                string newFileName = Guid.NewGuid() + Path.GetExtension(filename);
                string fullFilePath = Path.Combine(newPath, newFileName);

                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                return Ok(new { ProfileImage = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/StaticImage/{newFileName}" });
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here for further analysis
                return BadRequest(new BaseResponseModel
                {
                    Status = false,
                    Message = "Error Image Upload"
                });
            }
        }
    }
}