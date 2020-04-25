using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using project.Models;


namespace project.Controllers
{
    public class TeacherController : ApiController
    {


        [HttpGet]
        [Route("api/Teacher/getTasksBySubject/{subName}")]

        public List<Task> getTasksBySubject(string subName)
        {
            Task t = new Task();
            return t.getTasksBySubject(subName);
        }

        [HttpGet]
        [Route("api/Teacher/getQuestionnaire/{TaskId}")]

        public List<Quiz> getQuestionnaire(string TaskId)
        {
            Quiz q = new Quiz();
            return q.getQuestionnaire(TaskId);
        }

        [HttpPost]
        [Route("api/Teacher/PostQuiz")]

        public int PostQuiz([FromBody] Quiz q)
        {
            return q.PostQuiz();
        }

        [HttpPost]
        [Route("api/Teacher/postTask")]

        public int postTask([FromBody] Task t)
        {

            return t.postTask();
        }

        [HttpGet]
        [Route("api/Teacher/getUserByMail/{mail}")]
        public Dictionary<string, string> getUserByMail(string mail)
        {

            string[] mailArr = mail.Split(',');
            mail = mailArr[0] + "." + mailArr[1];
            User u = new User();
            return u.getUserByMail(mail);

        }


        [HttpGet]
       [Route("api/Teacher/getClassesbyEmail/{mail}")]

       public List<Classroom> getClassesbyMail(string mail)
       {
            mail = mail.Replace(',', '.').Replace('~', '@');
            Classroom c = new Classroom();
           return c.getClassesbyMail(mail);
       }

        [HttpGet]
        [Route("api/Teacher/getTeamsbyMail/{mail}")]

        public List<Team> getTeamsbyMail(string mail)
        {
            mail = mail.Replace(',', '.').Replace('~', '@');
            Team t = new Team();
            return t.getTeamsbyMail(mail);
        }

        [HttpGet]
        [Route("api/Teacher/getStudentsbyGrade/{val}")]

        public  Team getStudentsbyGrade(string val)
        {
            Student s = new Student();
            string[] val1 = val.Split(',');
            int grade = Convert.ToInt32(val1[0]);
            int scode = Convert.ToInt32(val1[1]);
            string mode = Convert.ToString(val1[2]);
            Team arg= s.getStudentsbyGrade(grade, scode, mode); ;

            return arg;
        }

        [HttpPost]
        [Route("api/Teacher/postTeam")]

        public int postTeam([FromBody] Team t)
        {
           return t.postTeam();
        }


        [HttpPut]
        [Route("api/Teacher/updateTeam")]

        public int updateTeam([FromBody] Team t)
        {
            return t.updateTeam();
        }

        [HttpPut]
        [Route("api/Teacher/hideTask/{id}")]
        public int hideTask(string id)
        {
            Task t = new Task();
            return t.hideTask(id);
        }

        [HttpPost]
        [Route("api/Teacher/postFB")]
        public int postFB([FromBody] FeedBack fb)
        {
            return fb.postFB();
        }
     
        [HttpPost]
        [Route("api/Teacher/postAssigment")]
        public int postAssigment([FromBody] Dictionary<string, string> assigment)
        {
            Task t = new Task();
            return t.postAssigment(assigment);
        }
        [HttpGet]
        [Route("api/Teacher/getTTasks/{data}")]
        public List<RealetedTask> getTTasks(string data)
        {
            RealetedTask rt = new RealetedTask();
            return rt.getTTasks(data);
        }

    }
}