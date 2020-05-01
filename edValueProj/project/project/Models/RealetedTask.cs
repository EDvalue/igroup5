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
        int score;
        string note;

        public RealetedTask(Task task, string yearOfStudy, DateTime forDate, DateTime tillDate, DateTime assigmentDate,string note)
        {
            this.task = task;
            this.yearOfStudy = yearOfStudy;
            this.forDate = forDate;
            this.tillDate = tillDate;
            this.assigmentDate = assigmentDate;
            this.note = note;
        }

        public Task Task { get => task; set => task = value; }
        public string YearOfStudy { get => yearOfStudy; set => yearOfStudy = value; }
        public DateTime ForDate { get => forDate; set => forDate = value; }
        public DateTime TillDate { get => tillDate; set => tillDate = value; }
        public DateTime AssigmentDate { get => assigmentDate; set => assigmentDate = value; }
        public string Note { get => note; set => note = value; }
        public DateTime STime { get => sTime; set => sTime = value; }
        public int Score { get => score; set => score = value; }

        public RealetedTask() { }

        public List<RealetedTask> getSTasks(string data)
        {
            StudentDBServices dbs = new StudentDBServices();
            string userEmail = data.Split('_')[0];
            string[] mailArr = userEmail.Split(',');
            userEmail = mailArr[0] + "." + mailArr[1];
            string teamId = data.Split('_')[1];

            return dbs.getSTasks(userEmail,teamId);
        }


         public int changeQ()
         {
            
            Quiz del=new Quiz();
            Quiz add = new Quiz();
            int num = 0;
            foreach (var item in this.Task.QuizList)
            {
                if (item.TaskId == "1")
                {
                   del = item;
                    
                }else if(item.TaskId == "2")
                {
                    add = item;
                }
            }
            del.Title = add.Title;
            del.TaskId = this.Task.TaskId;
            StudentDBServices dbs1 = new StudentDBServices();
            StudentDBServices dbs2 = new StudentDBServices();
            dbs1 = dbs1.updateQansC(del);
            dbs1.dt = deleteRows(dbs1.dt);
            dbs2 = dbs2.updateQansO(del);
            dbs2.dt = deleteRows(dbs2.dt);
            dbs1.update();
            dbs2.update();
            dbs2.deletePQ(del,this.Task.TaskId);
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


        public List<RealetedTask> getTTasks(string data)
        {
            TeacherDBservices dbs = new TeacherDBservices();

        //public List<RealetedTask> getTTasks(string data)
        //{
        //   TeamDBServices dbs = new TeamDBServices();

        //public List<RealetedTask> getTTasks(string data)
        //{
        //    TeacherDBservices dbs = new TeacherDBservices();



            string teamId = data;

            return dbs.getTTasks(teamId);
        }
    }
}