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


       [HttpPut]
       [Route("api/Teacher/getClassesbyEmail")]

       public List<Classroom> getClassesbyMail(Dictionary<string,string> mail)
       {
            //mail = mail.Replace(',', '.').Replace('~', '@');
            string m = mail["Mail"];
            Classroom c = new Classroom();
           return c.getClassesbyMail(m);
       }

        [HttpPut]
        [Route("api/Teacher/getTeamsbyMail")]

        public List<Team> getTeamsbyMail(Dictionary<string, string> mail)
        {
            //mail = mail.Replace(',', '.').Replace('~', '@');
            string m = mail["Mail"];
            Team t = new Team();
            return t.getTeamsbyMail(m);
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
        public List<Dictionary<string, string>> getTTasks(string data)
        {
            RealetedTask rt = new RealetedTask();
            return rt.getTTasks(data);
        }
      



        [HttpPut]
        [Route("api/Teacher/openSI")]
        public int openSI([FromBody] Dictionary<string, string> dict)
        {
            Classroom c = new Classroom();
            return c.openSI(dict);
        }
        [HttpPut]
        [Route("api/Teacher/getQ")]
        public Quiz getQ([FromBody]Dictionary<string,string> d)
        {
            Quiz q = new Quiz();
            return q.getQ(d);
        }
        [HttpPut]
        [Route("api/Teacher/updateFB")]
        public int updateQFB([FromBody] Dictionary<string,string> qfb)
        {
            RealetedTask rt = new RealetedTask();
            return rt.updateQFB(qfb);
        }

        [HttpPut]
        [Route("api/Teacher/getStTasksInTeam")]
        public List<Dictionary<string, string>> getStTasksInTeam(Dictionary<string,string> info)
        {
            RealetedTask rt = new RealetedTask();
            return rt.getStTasksInTeam(info);
        }

        [HttpGet]
        [Route("api/Teacher/getGraphTeamData/{teamId}")]
        public List<Dictionary<string,string>> graphDataTeam(string teamId)
        {
            Team t = new Team();
            return t.graphDataTeam(teamId);
        }
        [HttpPut]
        [Route("api/Teacher/getGraphClassData")]
        public List<Dictionary<string, string>> getGraphClassData(Dictionary<string, string> info)
        {
            Classroom c= new Classroom();
            return c.getGraphClassData(info);
        }

        [HttpPut]
        [Route("api/Teacher/updateAssigment")]
        public int updateAssigment([FromBody]Dictionary<string,string> info)
        {
            RealetedTask rt = new RealetedTask();
            return rt.updateAssigment(info);
        }

        [HttpPut]
        [Route("api/Teacher/Ptime")]
        public int updatePtime([FromBody]Dictionary<string, string> info)
        {
            StudentInClass st = new StudentInClass();
            return st.updatePtime(info);
        }

        [HttpDelete]
        [Route("api/Teacher/deleteAssigment")]
        public int deleteAssigment([FromBody]Dictionary<string, string> info)
        {
            RealetedTask rt = new RealetedTask();
            return rt.deleteAssigment(info);
        }


        [HttpGet]
        [Route("api/Teacher/intTeamGraph/{teamId}")]
        public List<Dictionary<string, string>> intTeamGraph(string teamId)
        {
            Team t = new Team();
            return t.intTeamGraph(teamId);
        }

        [HttpPut]
        [Route("api/Teacher/intlClassGraph")]
        public List<Dictionary<string, string>> intlClassGraph(Dictionary<string,string> info)
        {
            Classroom c = new Classroom();
            return c.intlClassGraph(info);
        }

        [HttpPut]
        [Route("api/Teacher/changeClassName")]
        public int changeClassName([FromBody] Classroom c)
        {
           
            return c.changeClassName();
        }

    }
}