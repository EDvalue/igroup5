using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using project.Models.DAL;
using System.Data;

namespace project.Models
{
    public class EDsystem
    {

        Dictionary<int, string> gradesDict = new Dictionary<int, string>
        {
             {7,"z"},
             {8,"ח"},
             {9,"ט"},
             {10,"י"}
        };

        public int sysUpdates()
        {
            int grade = 10;

            while (grade >= 7)
            {
                SystemDBservices dbs1 = new SystemDBservices();
                SystemDBservices dbs2 = new SystemDBservices();
                SystemDBservices dbs3 = new SystemDBservices();
                SystemDBservices dbs4 = new SystemDBservices();
                dbs1 = dbs1.getAllClassesByGrade(grade);
                dbs4=dbs4.getAllClassesByGrade(grade);
                dbs2 = dbs2.getAllTeamsByGrade(grade);
                dbs3 = dbs3.getAllStudentbyGrade(grade);
                if (grade == 10)
                {
                    dbs3.dt=deleteIgradeSt(dbs3.dt);
                    dbs2.dt = DeleteTeamsOrClass(dbs2.dt);
                    dbs1.dt = DeleteTeamsOrClass(dbs1.dt);

                    dbs2.update();
                    dbs1.update();
                }
                else
                {
                    createClasses(dbs1.dt, dbs1.dt);
                    DeleteTeamsOrClass(dbs4.dt);
                    raiseSt(dbs3.dt);
                    raiseTeams(dbs2.dt);
                    dbs1.update();
                    dbs3.update();
                    dbs4.update();
                    dbs2.update();
                }

                grade--;
            }

            return 1;
        }

        private DataTable raiseTeams(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {
                dr["Grade"] = Convert.ToInt32(dr["Grade"]) + 1;
            }
            return dt;
        }

        private DataTable raiseSt(DataTable dt)
        {

            foreach(DataRow dr in dt.Rows)
            {
                dr["GradeClass"] = Convert.ToInt32(dr["GradeClass"]) + 1;
            }
            return dt;
        }

        private DataTable createClasses(DataTable dt, DataTable dtHolder)
        {
           int len = dt.Rows.Count-1;
   
           while (len>=0)
           {
                DataRow workRow = dt.NewRow();
                int c = Convert.ToInt32(dt.Rows[len]["GradeNumber"]);
                int g = Convert.ToInt32(dt.Rows[len]["Grade"]) + 1;
                workRow["Grade"] = g;
                workRow["GradeNumber"] = c;
                workRow["Title"] = gradesDict[g] + "`" + c;
                workRow["TeacherEmail"] = dt.Rows[len]["TeacherEmail"].ToString();
                workRow["SchoolCode"] = Convert.ToInt32(dt.Rows[len]["SchoolCode"]);

                dt.Rows.Add(workRow);
                len--;
            }


            return dt;
        }

        private DataTable deleteIgradeSt(DataTable dt)
        {
            SystemDBservices dbs = new SystemDBservices();
            foreach(DataRow dr in dt.Rows)
            {

                string mail = dr["StudentEmail"].ToString();
                dbs.deleteSt(mail);
                dbs.deleteUser(mail);
            }

            return dt;
        }

        private DataTable DeleteTeamsOrClass(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                dr.Delete();
            }

            return dt;
        }
    }
}