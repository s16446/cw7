using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.DTOs.Requests
{
	public class PromoteStudentsRequest
	{
		[Required(ErrorMessage = "Nazwa studiów jest wymagana")]
		public string Studies { get; set; }

		[Required(ErrorMessage = "Numer semestru jest wymagany")]
		[RegularExpression("[1-9][0-9]{0,1}")]
		public int Semester { get; set; }
	}
}
