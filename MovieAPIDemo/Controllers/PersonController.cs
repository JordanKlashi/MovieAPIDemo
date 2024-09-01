using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPIDemo.Data;
using MovieAPIDemo.Entities;
using MovieAPIDemo.Models;
using System.Linq.Expressions;

namespace MovieAPIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly MovieDbContext _context;
        private readonly IMapper _mapper;

        public PersonController(MovieDbContext context, IMapper mapper)
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
                var actorCount = _context.Person.Count();
                var actorList = _mapper.Map<List<ActorViewModel>>(_context.Person.Skip(pageIndex * pageSize).Take(pageSize).ToList());

                response.Status = true;
                response.Message = "Success";
                response.Data = new { Count = actorCount, Person = actorList };

                return Ok(response);
            }
            catch (Exception)
            {
                // TODO: Log the exception
                response.Status = false;
                response.Message = "Something went wrong";

                return BadRequest(response);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetPersonById(int id)
        {
            BaseResponseModel response = new BaseResponseModel();

            try
            {
                var person = _context.Person.Where(x => x.Id == id).FirstOrDefault();

                if (person == null)
                {
                    response.Status = false;
                    response.Message = "Record Not Exist.";
                    return BadRequest(response);
                }
                var personData = new ActorDetailsViewModel
                {
                    Id = person.Id,
                    Name = person.Name,
                    DateOfBirth = person.DateOfBirth,
                    Movies = _context.Movie.Where(x => x.Actors.Contains(person)).Select(x => x.Title).ToArray()
                };

                response.Status = true;
                response.Message = "Success";
                response.Data = personData;

                return Ok(response);
            }
            catch (Exception)
            {
                // TODO: Log the exception
                response.Status = false;
                response.Message = "Something went wrong";

                return BadRequest(response);
            }
        }

        [HttpGet]
        [Route("search/{searchText}")]
        public IActionResult Get(string searchText)
        {
            BaseResponseModel response = new BaseResponseModel();
            try
            {
                var searchedPerson = _context.Person.Where(x => x.Name.Contains(searchText)).Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.DateOfBirth
                }).ToList();

                response.Status = true;
                response.Message = "Success";
                response.Data = searchedPerson;

                return Ok(response);
            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Something went wrong";
                return BadRequest(response);
            }
        }

        [HttpPost]
        public IActionResult Post(ActorViewModel model)
        {
            BaseResponseModel response = new BaseResponseModel();

            try
            {
                if (ModelState.IsValid)
                {
                    var postedModel = new Person()
                    {
                        Name = model.Name,
                        DateOfBirth = model.DateOfBirth
                    };

                    _context.Person.Add(postedModel);
                    _context.SaveChanges();

                    model.Id = postedModel.Id;

                    response.Status = true;
                    response.Message = "Success";
                    response.Data = model;

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
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Une erreur est survenue";
                return BadRequest(response);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, ActorViewModel model)
        {
            BaseResponseModel response = new BaseResponseModel();

            try
            {
                if (ModelState.IsValid)
                {
                    var postedModel = _mapper.Map<Person>(model);

                    if (model.Id <= 0)
                    {
                        response.Status = false;
                        response.Message = "Id is required";

                        return BadRequest(response);
                    }

                    var personDetails = _context.Person.Where(x => x.Id == model.Id).AsNoTracking().FirstOrDefault();
                    if (personDetails != null)
                    {
                        personDetails.Name = model.Name;
                        personDetails.DateOfBirth = model.DateOfBirth;

                        _context.Person.Update(postedModel);
                        _context.SaveChanges();

                        response.Status = true;
                        response.Message = "Success";
                        response.Data = postedModel;

                        return Ok(response);
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Record Not Exist.";
                        return BadRequest(response);
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = "Erreur dans la mise à jour d'un film.";
                    response.Data = ModelState;

                    return BadRequest(response);
                }
            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Something went wrong";
                return BadRequest(response);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            BaseResponseModel response = new BaseResponseModel();

            try
            {
                var person = _context.Person.Where(x => x.Id == id).FirstOrDefault();

                if (person == null)
                {
                    response.Status = false;
                    response.Message = "Record Not Exist.";
                    return BadRequest(response);
                }

                _context.Person.Remove(person);
                _context.SaveChanges();

                response.Status = true;
                response.Message = "Deleted Successfully";

                return Ok(response);
            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Invalid";
                return BadRequest(response);
            }
        }
    }
}
