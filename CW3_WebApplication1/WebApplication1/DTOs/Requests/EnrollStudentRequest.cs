using System;
using System.ComponentModel.DataAnnotations;


namespace WebApplication1.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        [Required(ErrorMessage = "Musisz podać numer indeksu")]
        [RegularExpression("^s[0-9]{4,6}")]
        public string IndexNumber { get; set; }
        
        [Required(ErrorMessage ="Musisz podać imię")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Musisz podać nazwisko")]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Musisz podać datę urodzenia (w formacie dd.MM.yyyy)")]
        public string BirthDate { get; set; }

        [Required(ErrorMessage = "Musisz podać nazwę studiów")]
        public string Studies { get; set; }
 
    }
}
