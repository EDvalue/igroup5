using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using project.Models.DAL;

namespace project.Models
{
    public class Team
    {
        string id;
        string title;
        int grade;
        char status;
        Teacher teacher;
        Subject subject;
        List<Student> studentList = new List<Student>();
        int scode;
      
        public Team() { }

        public Team(string title, int grade, Teacher teacher, Subject subject, List<Student> studentList, int scode,string id, char status)
        {
            this.id = id;
            this.title = title;
            this.grade = grade;
            this.teacher = teacher;
            this.subject = subject;
            this.studentList = studentList;
            this.scode = scode;
            this.status = status;
        }

        public string Title { get => title; set => title = value; }
        public int Grade { get => grade; set => grade = value; }
        public Teacher Teacher { get => teacher; set => teacher = value; }
        public Subject Subject { get => subject; set => subject = value; }
        public List<Student> StudentList { get => studentList; set => studentList = value; }
        public int Scode { get => scode; set => scode = value; }
        public string Id { get => id; set => id = value; }
        public char Status { get => status; set => status = value; }

        public List<Team> getTeamsbyMail(string mail)
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.getTeamsbyMail(mail);
        }

        public int postTeam()
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.postTeam(this);
        }
    }
}