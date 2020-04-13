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
    public class StudentDBServices
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
        public int postStudentIntelli(Dictionary<string, string> qresults)
        {
            int done = 1;
            int numEffected = 0;
            string mail = "";
            string intel;
            string points;
            mail = qresults["mail"];
            qresults.Remove("mail");
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String UpdateSIquiz = "UPDATE Student Set CompletedIQuiz= " + done + " WHERE StudentEmail= '" + mail + "'";
                


                SqlCommand cmd = new SqlCommand(UpdateSIquiz, con);
                numEffected += cmd.ExecuteNonQuery();

                foreach (KeyValuePair<string, string> entry in qresults)
                {
                    intel = entry.Key;
                    points = entry.Value;
                    string InsertIpoints = IntelligencePointsInsertCommand(mail, intel, points);
                    SqlCommand cmd1 = new SqlCommand(InsertIpoints, con);
                    numEffected += cmd1.ExecuteNonQuery();
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

            return numEffected; ;


        }

        public List<Inteligence> getStudentIntelli(string mail)
        {

            SqlConnection con = null;
            List<Inteligence> list = new List<Inteligence>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String selectSTR = "select * from PointsInIntelligence where StudentEmail='" + mail + "'";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    Inteligence c = new Inteligence();

                    c.Points = Convert.ToInt32(dr["points"]);
                    c.Name = dr["IntelligenceName"].ToString();


                    list.Add(c);

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



        private String IntelligencePointsInsertCommand(string mail,string intel, string points)
        {
            String command;


            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}','{1}','{2}')", mail, intel, points);
            
            String prefix = "INSERT INTO PointsInIntelligence" + "(StudentEmail,IntelligenceName,points)";
            command = prefix + sb.ToString();

            return command;
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


 