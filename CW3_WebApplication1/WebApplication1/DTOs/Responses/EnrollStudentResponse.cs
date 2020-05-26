using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.DTOs.Responses
{
    public class EnrollStudentResponse
    {
        private int Status { get; set; }

        private string message = "";
        public string LastName { get; set; }
        public int Semester { get; set; }
        public string StartDate { get; set; }
        public string Studies { get; set; }

        public void setStatus(int v, string m)
        {
           this.Status = v;
           this.message = m;
        }

        public int getStatus()
        {
            return Status;
        }

        public string getMessage()
        {
           return message;
        }
    }
}
