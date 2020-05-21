using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using project.Models.DAL;
using System.Data;

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

        public Team(string title, int grade, Teacher teacher, Subject subject, List<Student> studentList, int scode, string id, char status)
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

        public int updateTeam()
        {
            TeacherDBservices dbs = new TeacherDBservices();
            dbs = dbs.updateTeam(this.Id);
            dbs.dt = teamTBL(this, dbs.dt);
            dbs.update();
            dbs = dbs.updateSIT(this.Id);
            dbs.dt = sitTBL(this, dbs.dt);
            dbs.update();


            return 0;
        }

        private DataTable sitTBL(Team t, DataTable dt)
        {
            DataTable dt1=dt;
            dt1 =addToTeam(t,dt);
            dt.Merge(dt1);
            dt1=removeFromTeam(t,dt);
            dt.Merge(dt1);

            return dt;
        }

        private DataTable removeFromTeam(Team t, DataTable dt)
        {

            bool flag = false;
            foreach ( DataRow dr in dt.Rows)
            {
                flag = false;
                DataRow dr1 = dr;
                foreach (var s in t.StudentList)
                {
                    if (dr["StudentEmail"].ToString() == s.Mail)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    dr1.Delete();
                }
       
            }
            
            return dt;
        }


        private DataTable addToTeam(Team t, DataTable dt)
        {
            bool flag = false;
            foreach (var s in t.StudentList)
            {
                flag = false;
                foreach (DataRow dr in dt.Rows)
                {
                    
                    if (s.Mail == dr["StudentEmail"].ToString())
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    DataRow workRow = dt.NewRow();

                    workRow["StudentEmail"] = s.Mail;
                    workRow["TeamId"] = t.Id;
                    workRow["SchoolCode"] = t.Scode;
                    if (dt.Rows.Count > 0)
                    {
                        dt.Rows.InsertAt(workRow, dt.Rows.Count - 1);
                    }
                    else if(dt.Rows.Count==0)
                    {
                        dt.Rows.InsertAt(workRow, dt.Rows.Count);

                    }
                    
                }
     
            }

            return dt;
        }
 

        private DataTable teamTBL(Team t, DataTable dt)
        {            
                foreach (DataRow dr in dt.Rows)
                {
                  dr["Title"] = t.Title;
                       
                }
             
            return dt;
        }

        public List<Team> getSTeams(string mail)
        {
            StudentDBServices dbs = new StudentDBServices();
            return dbs.getSTeams(mail);
        }

        public List<Dictionary<string, string>> graphDataTeam(string teamId)
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.graphDataTeam(teamId);
        }

        public List<Dictionary<string, string>> intTeamGraph(string teamId)
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.intTeamGraph(teamId);
        }
    }
}
