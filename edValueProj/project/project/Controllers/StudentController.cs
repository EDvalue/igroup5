using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using project.Models;
using project.Models.DAL;

namespace project.Controllers
{
    public class StudentController : ApiController
    {
        [HttpPost]
        [Route("api/Student/postQ")]
        public int postQ([FromBody] RealetedTask rt)
        {
            rt.STime = DateTime.Now;
            return rt.postQ();
        }



        // POST api/<controller>
        [HttpPost]
        [Route("api/Student/IntQuiz")]

        public int IntQuiz(Dictionary<string, string> qresults)
        {
            Student s = new Student();
            return s.IntQuiz(qresults);


        }

        [HttpGet]
        [Route("api/Student/getIntelliByMail/{val}")]
        public List<Inteligence> getIntelliByMail(string val)
        {
            Student s = new Student();
            List<Inteligence> c = new List<Inteligence>();
            string[] mailArr = val.Split(',');
            val = mailArr[0] + "." + mailArr[1];

            c =s.getIntelligence(val);
            return c;
        }

        [HttpGet]
        [Route("api/Student/getSTasks/{data}")]
        public List<RealetedTask> getSTasks(string data)
        {
            RealetedTask rt = new RealetedTask();
            return rt.getSTasks(data);
        }

        [HttpPut]
        [Route("api/Student/getSTeams")]
        public List<Team> getSTeams([FromBody]User u)
        {
            Team t = new Team();
            return t.getSTeams(u.Mail);
        }

        [HttpPut]
        [Route("api/Student/updateQ")]
        public int updateQ([FromBody] RealetedTask rt)
        {
            Quiz q = new Quiz();
            return q.updateQ(rt);
        }

        [HttpPut]
        [Route("api/Student/validateTime")]
        public int validateTime(RealetedTask rt)
        {
            
            return rt.validateTime();
        }

        [HttpPut]
        [Route("api/Student/changeQ")]
        public int changeQ([FromBody] RealetedTask rt)
        {
            return rt.changeQ();
        }
    }

}
