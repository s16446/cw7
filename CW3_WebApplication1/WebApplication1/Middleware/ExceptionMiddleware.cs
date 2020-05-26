using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Middleware
{
	public class ExceptionMiddleware
	{

		
	private Task HandleExceptionAsync(HttpContext context, Exception exc)
	{ 
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = StatusCodes.Status500InternalServerError;
		return context.Response.WriteAsync(new ErrorDetails
		{ 
			StatusCode = StatusCodes.Status500InternalServerError,
			Message = "Wystąpił jakiś błąd..."
		}.ToString());
	}
	}

}
