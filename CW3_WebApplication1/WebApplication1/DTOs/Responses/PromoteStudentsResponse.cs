using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.DTOs.Responses
{
	public class PromoteStudentsResponse
	{
        public int Semester { get; set; }

        public string StartDate { get; set; }

        public string StudiesName { get; set; }

        private int Status { get; set;}
        private string Message { get; set;}

        public void setStatus(int v, string m)
        {
           this.Status = v;
           this.Message = m;
        }

        public int getStatus()
        {
            return Status;
        }

        public string getMessage()
        {
           return Message;
        }

	}
}
