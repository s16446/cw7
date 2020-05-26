using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.DAL;
using WebApplication1.DTOs.Requests;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/students")]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private IConfiguration  Configuration { get; set; }

        private IDbService _dbService;

        public StudentsController(IDbService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            Configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(string id)
        {
            return Ok(_dbService.GetStudent(id));
        }

        [HttpGet("{id}/{semester}")] // student's ID for whom we want to get enrollments
        public IActionResult GetEnrollments(string id, int semester)
        {
            return Ok(_dbService.GetEnrollments(id, semester));
        }

        [HttpPost] // add
        public IActionResult CreateStudent(Student student)
        {
            _dbService.AddStudent(student);
            return Ok(student);
        }

        [HttpPut("{id}")] // update
        public IActionResult UpdateStudent(string id)
        {
            return Ok("Aktualizacja zakonczona: " + id);
        }

        [HttpDelete("{id}")] // delete
        public IActionResult DeleteStudent(string id)
        {
            Student student = _dbService.FindStudent(id);
            if (student != null) 
            {
                _dbService.DeleteStudent(student);
                return Ok("Usuwanie zakonczone" + id);
            }
        else
            return NotFound("Nie znaleziono studenta o indeksie: " + id);
        }

       

    }
}