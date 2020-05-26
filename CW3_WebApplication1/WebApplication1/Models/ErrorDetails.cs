using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
	public class ErrorDetails
	{
		public int StatusCode { get; set; }

		public string Message { get; set; }

		public override string ToString()
		{ 
			return Message;
		}


	}
}
