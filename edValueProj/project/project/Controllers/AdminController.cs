using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using project.Models;

namespace project.Controllers
{
    public class AdminController : ApiController
    {
        [HttpGet]
        [Route("api/Admin/getAllcities")]

        public List<City> getAllcities()
        {
            City c = new City();
            return c.getAllcities();
        }

        [HttpGet]
        [Route("api/Admin/getSchoolByCity/{code}")]

        public List<School> getSchoolByCity(int code)
        {
            School s = new School();
            return s.getSchoolByCity(code);
        }

        [HttpGet]
        [Route("api/Admin/getUserByMail/{mail}")]
        public Dictionary<string,string> getUserByMail(string mail)
        {
            User u = new User();
            return u.getUserByMail(mail);

        }

        [HttpGet]
        [Route("api/Admin/getAllschool")]

        public List<School> getAllschool()
        {
            School s = new School();
            return s.getAllschool();
        }

        [HttpGet]
        [Route("api/Admin/getClassBySchool/{code}")]

        public List<Classroom> getClassBySchool(int code)
        {
            Classroom c = new Classroom();
            return c.getClassBySchool(code);
        }

        [HttpGet]
        [Route("api/Admin/getSchoolName/{code}")]

        public string getSchoolName(int code)
        {
            Classroom c = new Classroom();
            return c.getSchoolName(code);
        }

        [HttpPost]
        [Route("api/Admin/postSchool")]

        public int PostSchool([FromBody]School sc)
        {
            return sc.postSchool();
        }

        [HttpPost]
        [Route("api/Admin/updateSchool")]

        public int updateSchool([FromBody]School sc)
        {
            return sc.updateSchool();
        }

        [HttpPost]
        [Route("api/Admin/postNewUser")]

        public int postNewUser([FromBody]List<Dictionary<string, string>> userDict)
        {
            User u = new User();
            return u.postNewUser(userDict);
        }

        

        [HttpGet]
        [Route("api/Admin/getSchoolByID/{id}")]
        public School getSchoolByID(int id)
        {
            School s = new School();
            return s.getSchoolByID(id);
        }

        [HttpGet]
        [Route("api/Admin/getTeacherByScode/{scode}")]

        public List<Teacher> getTeacherByScode(int scode)
        {
            Teacher t = new Teacher();
            return t.getTeacherByScode(scode);
        }

        [HttpPost]
        [Route("api/Admin/postNewClass")]

        public int postNewClass([FromBody] Classroom c)
        {
            return c.postNewClass();
        }

        [HttpGet]
        [Route("api/Admin/getAllteachers")]

        public List<Teacher> getAllteachers()
        {
            Teacher t = new Teacher();
            return t.getAllteachers();
        }

        [HttpPost]
        [Route("api/Admin/updateEditors")]

        public int updateEditors([FromBody] List<Teacher> tlist)
        {
            Teacher t = new Teacher();
            return t.updateEditors(tlist);

        }

        [HttpPut]
        [Route("api/Admin/UpdateUser")]

        public int UpdateUser([FromBody] List<Dictionary<string, string>> userDict)
        {
            User u = new User();
            return u.UpdateUser(userDict);
        }

        [HttpPut]
        [Route("api/Admin/updateclass")]

        public int updateclass([FromBody] Classroom c)
        {
           return c.updateclass();
        }

        [HttpGet]
        [Route("api/Admin/getnumbers/{val}")]

        public List<int> getnumbers(string val)
        {
            string[] val1 = val.Split(',');
            int grade = Convert.ToInt32(val1[1]);
            int scode = Convert.ToInt32(val1[0]);

            DBservices dbs = new DBservices();
            return dbs.getnumbers(grade, scode);
        }
    }  
}