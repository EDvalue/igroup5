using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using project.Models;

namespace project.Models.DAL
{
    public class TeamDBServices
    {
        public System.Data.SqlClient.SqlDataAdapter da;

        public DataTable dt;
        public SqlConnection connect(String conString)
        {

            // read the connection string from the configuration file
            string cStr = WebConfigurationManager.ConnectionStrings[conString].ConnectionString;
            SqlConnection con = new SqlConnection(cStr);
            con.Open();
            return con;
        }
        public List<RealetedTask> getTTasks(string teamId)
        {


            SqlConnection con = null;

            List<RealetedTask> rtList = new List<RealetedTask>();
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String part1 = " select  tbl.taskId,tbl.Title,tbl.SubjectName,tbl.Date_Assignment,tbl.ForDate,tbl.OpenTill,tbl.YearOfStudy";
                String part2 = "  from (select t.taskId,t.Title,t.SubjectName,rt.Date_Assignment,rt.ForDate,rt.OpenTill,rt.YearOfStudy ";
                String part3 = " from task as t inner join RealatedTo rt on rt.TaskId=t.TaskId and rt.TeamId= '"+teamId+ "' ";
                String part4 = "  group by t.taskId,t.Title,t.SubjectName,rt.Date_Assignment,rt.ForDate,rt.OpenTill,rt.YearOfStudy)as tbl  ";
                
                String selectSTR = part1 + part2 + part3 + part4 ;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                RealetedTask rt = new RealetedTask();
                rt.Task = new Task();
                
                while (dr.Read())
                {   // Read till the end of the data into a row

                    if (dr["TaskId"].ToString() != rt.Task.TaskId)
                    {
                        if (rt.Task.TaskId != null)
                        {
                            //rt.Task.QuizList.Add(q);
                            rtList.Add(rt);
                        }
                        rt = new RealetedTask();
                        rt.Task = new Task();
                        //rt.Task.QuizList = new List<Quiz>();
                        //rt.Task.Grade = Convert.ToInt32(dr["IsChoose"]);
                        rt.Task.TaskId = dr["TaskId"].ToString();
                        rt.Task.Title = dr["Title"].ToString();
                        rt.Task.Grade = 0;
                        
                        //rt.Task.Sub = new Subject(dr["SubjectName"].ToString(), "");
                        rt.YearOfStudy = dr["YearOfStudy"].ToString();
                        rt.ForDate = Convert.ToDateTime(dr["ForDate"]);
                        rt.TillDate = Convert.ToDateTime(dr["OpenTill"]);
                        rt.AssigmentDate = Convert.ToDateTime(dr["Date_Assignment"]);
                    }
                    //if (dr["QuestionnaireId"].ToString() != q.QuizID)
                    //{
                    //    if (q.QuizID != null)
                    //    {
                    //        rt.Task.QuizList.Add(q);
                    //    }

                    //q = new Quiz();
                    //q.Question = new List<Question>();
                    //q.QuizID = dr["QuestionnaireId"].ToString();
                    //q.TaskId = dr["IsChoose"].ToString();
                    //if (q.TaskId == "1")
                    //{
                    //    rt.Task.Grade = 1;
                    //}
                    //q.Inteligence = new Inteligence(Convert.ToInt32(dr["points"]), dr["Name"].ToString(), dr["IntelligenceName"].ToString());

                    //}



                    //if (dr["QuestionId"].ToString() != que.QuestionId)
                    //{

                    //    que = new Question(dr["Type"].ToString(), dr["Content"].ToString(), new List<Answer>(), dr["ImgLink"].ToString(), dr["VideoLink"].ToString(), Convert.ToInt32(dr["OrderNum"]), dr["QuestionId"].ToString());
                    //    que.Answer = new List<Answer>();
                    //    q.Question.Add(que);
                    //}

                    //if (dr["ans_content"] != DBNull.Value)
                    //{
                    //    que.Answer.Add(new Answer(dr["ans_content"].ToString(), Convert.ToBoolean(dr["IsRight"]), Convert.ToInt32(dr["AnswerId"])));

                    //    if (dr["picked"] != DBNull.Value)
                    //    {
                    //        que.Answer[que.Answer.Count - 1].IsPicked = true;
                    //    }
                    //    else
                    //    {
                    //        que.Answer[que.Answer.Count - 1].IsPicked = false;
                    //    }
                    //}
                    //else if (dr["Answer"] != DBNull.Value || dr["FileLink"] != DBNull.Value)
                    //{
                    //    if (dr["FileLink"] != DBNull.Value)
                    //    {
                    //        que.AnsContent = dr["FileLink"].ToString();
                    //    }
                    //    else
                    //    {
                    //        que.AnsContent = dr["Answer"].ToString();
                    //    }

                    //}




                    }
                
                rtList.Add(rt);

            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }



            return rtList;
        }


        private SqlCommand CreateCommand(String CommandSTR, SqlConnection con)
        {

            SqlCommand cmd = new SqlCommand(); // create the command object

            cmd.Connection = con;              // assign the connection to the command object

            cmd.CommandText = CommandSTR;      // can be Select, Insert, Update, Delete 

            // cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

            cmd.CommandType = System.Data.CommandType.Text; // the type of the command, can also be stored procedure

            return cmd;
        }

    }
}
 