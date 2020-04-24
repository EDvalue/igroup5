using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Net;
using System.Net.Mail;
using System.Drawing;
using System.Configuration;


namespace project.Models.DAL
{
    public class SystemDBservices
    {

        public System.Data.SqlClient.SqlDataAdapter da;

        public DataTable dt;

        public SystemDBservices()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        //--------------------------------------------------------------------------------------------------
        // This method creates a connection to the database according to the connectionString name in the web.config 
        //--------------------------------------------------------------------------------------------------
        public SqlConnection connect(String conString)
        {

            // read the connection string from the configuration file
            string cStr = WebConfigurationManager.ConnectionStrings[conString].ConnectionString;
            SqlConnection con = new SqlConnection(cStr);
            con.Open();
            return con;
        }

        //---------------------------------------------------------------------------------
        private SqlCommand CreateCommand(String CommandSTR, SqlConnection con)
        {

            SqlCommand cmd = new SqlCommand(); // create the command object

            cmd.Connection = con;              // assign the connection to the command object

            cmd.CommandText = CommandSTR;      // can be Select, Insert, Update, Delete 

            // cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

            cmd.CommandType = System.Data.CommandType.Text; // the type of the command, can also be stored procedure

            return cmd;
        }

        public User conect(Dictionary<string,string> conection)
        {
           
            SqlConnection con = null;
            //Dictionary<string, string> user = new Dictionary<string, string>();
            Teacher t = new Teacher();
            Student s = new Student();
            User u = new User();
            int counteacher = 0;
            int countstudent = 0;

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String selectSTR = "select * from [User] inner join Student on StudentEmail=[User].Email where [User].Email='" + conection["Email"]+"' and "+" [User].Password='"+conection["Password"]+"'" ;


                SqlCommand cmd = new SqlCommand(selectSTR, con);

                

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                while (dr.Read())
                {
                    s.IdNumber = dr["IdNumber"].ToString();
                    s.Name = dr["Fname"].ToString();
                    s.LastName = dr["Lname"].ToString();
                    s.Mail = dr["Email"].ToString();
                    s.SCode = Convert.ToInt32(dr["SchoolCode"]);
                    s.IsCompleteIquizz = Convert.ToBoolean(dr["CompletedIQuiz"]);
                    countstudent++;

                    
                }


                if (countstudent == 0)
                {
                    con.Close();
                    dr = null;
                    cmd.Connection = null;
                    con = connect("DBConnectionString");
                   
                    selectSTR = "select * from [User] inner join Teacher on TeacherEmail=[User].Email where [User].Email='" + conection["Email"] + "' and " + " [User].Password='" + conection["Password"]+"'";
                    cmd = new SqlCommand(selectSTR, con);
                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);




                    while (dr.Read())
                    {   // Read till the end of the data into a row


                        t.IdNumber = dr["IdNumber"].ToString();
                        t.Name = dr["Fname"].ToString();
                        t.LastName = dr["Lname"].ToString();
                        t.Mail = dr["Email"].ToString();
                        t.SCode = Convert.ToInt32(dr["SchoolCode"]);
                        t.SchoolAdmin = Convert.ToBoolean(dr["SchoolAdmin"]);
                        t.IsEditor = Convert.ToBoolean(dr["isEditor"]);
                        counteacher++;

                    }


                    if (counteacher == 0)
                    {
                        con.Close();
                        dr = null;
                        cmd.Connection = null;
                        con = connect("DBConnectionString");

                        selectSTR = "select * from [Admin]  where [Admin].UserName='" + conection["Email"] + "' and " + " [Admin].Password='" + conection["Password"] + "'";
                        cmd = new SqlCommand(selectSTR, con);
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        while (dr.Read())
                        {
                            
                            u.Name = dr["UserName"].ToString();
                         
                        }
                    }
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

            if (countstudent > 0)
            {
                return s;
            }
            else if(counteacher>0)
            {

                return t;
            }
            else
            {
                return u;
            }

        }


        public SystemDBservices updateAPass(string userName,string pass)
        {
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString");
                da = new SqlDataAdapter("select * from Admin where UserName='" + userName + "' and Password='"+pass+"'", con);
                SqlCommandBuilder builder = new SqlCommandBuilder(da);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dt = ds.Tables[0];
            }

            catch (Exception ex)
            {

                throw ex;
            }

            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }


            return this;

        }

        public SystemDBservices updateUPass(string userName, string pass)
        {
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString");
                da = new SqlDataAdapter("select * from [User] where Email='" + userName + "' and Password='" + pass + "'", con);
                SqlCommandBuilder builder = new SqlCommandBuilder(da);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dt = ds.Tables[0];
            }

            catch (Exception ex)
            {

                throw ex;
            }

            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }


            return this;

        }


        public string fp(Dictionary<string, string> conection)
        {
            string msg="";
            SqlConnection con = null;
            string password = "";
            string userName = "";
            string name = "";
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String selectSTR = "select * from [User] where [User].Email='"+conection["userName"] + "' and [User].Lname='" + conection["lastName"]+"'";


                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (!dr.HasRows)
                {
                    msg = "שם המשתמש או המשפחה שגויים,אם הינך משתמש אדמין פנה אל מנהלי המערכת";
                }
                else
                {
                    while (dr.Read())
                    {

                        userName = dr["Email"].ToString();
                        password = dr["Password"].ToString();
                        name= dr["name"].ToString();


                    }

                    MailMessage mm = new MailMessage("morptao@gmail.com", userName);
                    mm.Subject = "Password Recovery";
                    mm.Body = string.Format("Hi {0},<br /><br />Your password is {1}.<br /><br />Thank You.", name, password);
                    mm.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential();
                    NetworkCred.UserName = "morptao@gmail.com";
                    NetworkCred.Password = "MorPinto123";
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);

                    msg = "הסיסמא שלך מחכה לך בחשבון המייל";
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
          
            return msg;
        }

        public void update()
        {
            da.Update(dt);

        }

    }
}