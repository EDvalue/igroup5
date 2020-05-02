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
    public class StudentInClassDBservices
    {
        public System.Data.SqlClient.SqlDataAdapter da;

        public DataTable dt;

        public StudentInClassDBservices()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public SqlConnection connect(String conString)
        {

            // read the connection string from the configuration file
            string cStr = WebConfigurationManager.ConnectionStrings[conString].ConnectionString;
            SqlConnection con = new SqlConnection(cStr);
            con.Open();
            return con;
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
        public List<StudentInClass> getStudents()
        {
            SqlConnection con = null;
            List<StudentInClass> list = new List<StudentInClass>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String selectSTR = "select * from zzzTest";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    StudentInClass s = new StudentInClass();
                    s.StudentEmail = Convert.ToString(dr["StudentId"]);
                    s.QuestionnaireId = Convert.ToString(dr["QuestionnaireId"]);
                    s.TaskId = Convert.ToString(dr["TaskId"]);
                    s.SubjectName = Convert.ToString(dr["SubjectName"]);
                    s.IntelligenceName = Convert.ToString(dr["IntelligenceName"]);
                    s.Grade = Convert.ToInt32(dr["Grade"]);

                    list.Add(s);

                }


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

            return list;
        }

        public List<StudentInClass> getStudentByEmail(string email)
        {
            SqlConnection con = null;
            List<StudentInClass> list = new List<StudentInClass>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String selectSTR = "select * from Student as s inner join [User] as u on u.[Email]=s.StudentEmail where StudentEmail='" + email + "'";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    StudentInClass s = new StudentInClass();




                    list.Add(s);

                }


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

            return list;
        }
        public List<StudentInClass> getSQuest(string data)
        {
            SqlConnection con = null;
            List<StudentInClass> list = new List<StudentInClass>();
            string[] data1;
            data1 = data.Split(',');
            string taskId = data1[0];
            string teamId = data1[1];

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                
                string select1 = "select st.StudentEmail,u.IdNumber,u.Fname,u.Lname,pq.QuestionnaireId,pq.Ptime,pq.Grade,pq.Note,q.IntelligenceName,";
                string select2 = "i.[ImgLink],Case When q.QuestionnaireId is null Then 0 Else 1 END AS isperform";
                string from = " from(";
                string select3 = "select * ";
                string from1 = " from StudentInTeam sit ";
                string from2 = "where sit.TeamId='"+teamId+"')as st left join [dbo].[PerformQuestionnaire] as pq";
                string from3 = " on pq.[StudentId] = st.StudentEmail and pq.TaskId = '" + taskId + "' ";
                string from4 = "left join[User] as u on u.Email = st.StudentEmail";
                string from5 = " left join Questionnaire as q on q.QuestionnaireId = pq.QuestionnaireId";
                string from6 = " left join[dbo].[Intelligence] as i on i.[EnglishName]=q.IntelligenceName";

                String selectSTR = select1 + select2+ from+select3+ from1 + from2 + from3+ from4+ from5+ from6;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    StudentInClass s = new StudentInClass();
                    s.StudentEmail = Convert.ToString(dr["StudentEmail"]);
                    s.QuestionnaireId = Convert.ToString(dr["QuestionnaireId"]);
                    s.Fname= Convert.ToString(dr["Fname"]);
                    s.Lname = Convert.ToString(dr["Lname"]);
                    s.IntelligenceName= Convert.ToString(dr["IntelligenceName"]);
                    s.Isperform= Convert.ToInt32(dr["isperform"]);
                    if (dr["Grade"] != DBNull.Value)
                    {
                        s.Grade = Convert.ToInt32(dr["Grade"]);
                    }

                    list.Add(s);

                }


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

            return list;
        }
    }
}