using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using project.Models;

namespace project.Controllers
{
    public class StudentInClassController : ApiController
    {
        // GET api/<controller>
        public List<StudentInClass> getStudents()
        {
            StudentInClass s = new StudentInClass();
            return s.getStudents();
        }
        public List<StudentInClass> getStudentByEmail(string email)
        {
            StudentInClass s = new StudentInClass();
            return s.getStudentByEmail(email);
        }
    }
}