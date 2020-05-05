using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using project.Models.DAL;
using System.Data;

namespace project.Models
{
    public class RealetedTask
    {
        Task task;
        string yearOfStudy;
        DateTime forDate;
        DateTime tillDate;
        DateTime assigmentDate;
        DateTime sTime;
        string note;
        int score;
        Student stPerformer=new Student();


        public Task Task { get => task; set => task = value; }
        public string YearOfStudy { get => yearOfStudy; set => yearOfStudy = value; }
        public DateTime ForDate { get => forDate; set => forDate = value; }
        public DateTime TillDate { get => tillDate; set => tillDate = value; }
        public DateTime AssigmentDate { get => assigmentDate; set => assigmentDate = value; }
        public string Note { get => note; set => note = value; }
        public int Score { get => score; set => score = value; }
        public DateTime STime { get => sTime; set => sTime = value; }
        public Student StPerformer { get => stPerformer; set => stPerformer = value; }

        public RealetedTask() { }

        public RealetedTask(Task task, string yearOfStudy, DateTime forDate, DateTime tillDate, DateTime assigmentDate, DateTime sTime, string note, int score, Student stPerformer)
        {
            this.task = task;
            this.yearOfStudy = yearOfStudy;
            this.forDate = forDate;
            this.tillDate = tillDate;
            this.assigmentDate = assigmentDate;
            this.sTime = sTime;
            this.note = note;
            this.score = score;
            this.stPerformer = stPerformer;
        }

        public int postQ()
        {

            StudentDBServices dbs = new StudentDBServices();
            return dbs.postQ(this);
        }

        public List<RealetedTask> getSTasks(string data)
        {
            StudentDBServices dbs = new StudentDBServices();
            string userEmail = data.Split('_')[0];
            string[] mailArr = userEmail.Split(',');
            userEmail = mailArr[0] + "." + mailArr[1];
            string teamId = data.Split('_')[1];

            return dbs.getSTasks(userEmail, teamId);
        }


        public List<RealetedTask> getTTasks(string data)
        {
            TeacherDBservices dbs = new TeacherDBservices();

            string teamId = data;

            return dbs.getTTasks(teamId);
        }

        public int changeQ()
        {
            Quiz del = new Quiz();
            Quiz add = new Quiz();
            int num = 0;

            foreach (var item in this.Task.QuizList)
            {

                if (item.TaskId == "1")
                {
                    del = item;
                }
                else if (item.TaskId == "2")
                {
                    add = item;
                }
            }

            
            del.TaskId = this.Task.TaskId;
            add.TaskId= this.Task.TaskId;
            RealetedTask rt= new RealetedTask();
            StudentDBServices dbs1 = new StudentDBServices();
            StudentDBServices dbs2 = new StudentDBServices();
            rt.Task = new Task();
            rt.stPerformer = new Student();
            rt.stPerformer = this.stPerformer;
            rt.Task.QuizList = new List<Quiz>();
            rt.Task.QuizList.Add(del);
            rt.YearOfStudy = this.YearOfStudy;
            if(this.sTime>=new DateTime())
            {
                rt.STime = this.sTime;
            }
            else
            {
                rt.STime = new DateTime();
            }
            
            dbs1 = dbs1.updateQansC(rt);
            dbs1.dt = deleteRows(dbs1.dt);
            dbs2 = dbs2.updateQansO(rt);
            dbs2.dt = deleteRows(dbs2.dt);
            dbs1.update();
            dbs2.update();
            dbs2.deletePQ(del, this);
            rt.Task.QuizList.Clear();
            rt.Task.QuizList.Add(add);
            dbs2.postQ(rt);
            return num;


        }
        private DataTable deleteRows(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {


                dr.Delete();
            }

            return dt;

        }

        public int validateTime()
        {
            StudentDBServices dbs = new StudentDBServices();
            return dbs.validateTime(this);
        }

        public List<Dictionary<string,string>> getSQuest(string data)
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.getSQuest(data);

        }

        public int updateQFB(Dictionary<string, string> qfb)
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.updateQFB(qfb);
        }

        public List<Dictionary<string, string>> getStTasksInTeam(Dictionary<string, string> info)
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.getStTasksInTeam(info);
        }
    }
}
