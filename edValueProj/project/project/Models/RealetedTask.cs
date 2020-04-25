using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using project.Models.DAL;

namespace project.Models
{
    public class RealetedTask
    {
        Task task;
        string yearOfStudy;
        DateTime forDate;
        DateTime tillDate;
        DateTime assigmentDate;
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
        //public List<RealetedTask> getTTasks(string data)
        //{
        //    TeamDBServices dbs = new TeamDBServices();
            
        //    string teamId = data;

        //    return dbs.getTTasks(teamId);
        //}
    }
}