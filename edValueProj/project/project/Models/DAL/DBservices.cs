using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace project.Models
{
    public class DBservices
    {
        public System.Data.SqlClient.SqlDataAdapter da;

        public DataTable dt;

        public DBservices()
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

        //------------CityInsert----------------------//
        public int loadCity(List<City> city)
        {


            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }

            foreach (City item in city)
            {


                String cStr = BuildCityInsertCommand(item);

                // helper method to build the insert string

                cmd = CreateCommand(cStr, con);             // create the command

                try
                {
                    numEffected += cmd.ExecuteNonQuery(); // execute the command

                }
                catch (Exception ex)
                {
                    if (con != null)
                    {
                        // close the db connection
                        con.Close();
                    }
                    // write to log
                    throw (ex);
                }

            }

            if (con != null)
            {
                // close the db connection
                con.Close();
            }

            return numEffected;

        }

        private String BuildCityInsertCommand(City c)
        {
            String command;


            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values({0},'{1}','{2}','{3}')", c.Code, c.Area, c.Hname, c.Ename);

            String prefix = "INSERT INTO City" + "(CityCode,Area,Hname,Ename)";
            command = prefix + sb.ToString();

            return command;
        }
        //-------------------------------------------------------//

        public string getcityCode(string city)
        {
            string code="";
            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String selectSTR = "select CityCode from City Where city.Hname='"+city+"'";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                
                while (dr.Read())
                {   // Read till the end of the data into a row
                  

                    code = (string)dr["CityCode"];
                    

                    
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

            return code;
        }

        //-----------SchoolInsert----------------------------------//

        public int loadschool(List<School> sc)
        {


            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }

            foreach (School item in sc)
            {


                String cStr = BuildSchoolInsertCommand(item);

                // helper method to build the insert string

                cmd = CreateCommand(cStr, con);             // create the command

                try
                {
                    numEffected += cmd.ExecuteNonQuery(); // execute the command

                }
                catch (Exception ex)
                {
                    if (con != null)
                    {
                        // close the db connection
                        con.Close();
                    }
                    // write to log
                    throw (ex);
                }

            }

            if (con != null)
            {
                // close the db connection
                con.Close();
            }

            return numEffected;

        }

        private String BuildSchoolInsertCommand(School s)
        {
            String command;

            
            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values({0},'{1}','{2}')", s.SchoolCode, s.CityCode, s.Name);

            String prefix = "INSERT INTO School" + "(SchoolCode,CityCode,Name)";
            command = prefix + sb.ToString();

            return command;
        }

        public int postSchool(School s)
        {


            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }


            String cStr = BuildSchoolInsertCommand(s);

            // helper method to build the insert string

            cmd = CreateCommand(cStr, con);             // create the command

            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command

            }
            catch (Exception ex)
            {
                if (con != null)
                {
                    // close the db connection
                    con.Close();
                }
                // write to log
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    // close the db connection
                    con.Close();
                }

                
            }
            return numEffected;
        }


        //--------------------------------------------------------------//
        


        public int updateSchool(School s)
        {


            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("UPDATE School SET  [CityCode]='{1}', [Name]='{2}' where [SchoolCode]={0}", s.SchoolCode, s.CityCode, s.Name);
            String cStr = sb.ToString();

            

            cmd = CreateCommand(cStr, con);             // create the command

            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command

            }
            catch (Exception ex)
            {
                if (con != null)
                {
                    // close the db connection
                    con.Close();
                }
                // write to log
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    // close the db connection
                    con.Close();
                }


            }
            return numEffected;
        }
        //------------------------users create--------------------------------------//

        //------------------post------------------------------//

        public int postnEd(List<Teacher> list)
        {


            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;
            SqlCommand cmd1;
            List<Dictionary<string, string>> report = new List<Dictionary<string, string>>();
            Dictionary<string, string> reportRow = new Dictionary<string, string>();


            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }

            foreach (var t in list)
            {
                String cStr = BuildUserInsertCommand(t);
                cmd = CreateCommand(cStr, con);
                try
                {
                    reportRow = new Dictionary<string, string>();
                    numEffected += cmd.ExecuteNonQuery(); // execute the command
                    reportRow.Add("type", "user");
                    reportRow.Add("id", t.IdNumber);
                    reportRow.Add("message", "success");

                    report.Add(reportRow);


                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2467)
                    {
                        reportRow = new Dictionary<string, string>();
                        reportRow.Add("type", "user");
                        reportRow.Add("id", t.Mail);
                        reportRow.Add("message", "already in DB");

                        report.Add(reportRow);
                        continue;
                    }
                    if (con != null)
                    {
                        // close the db connection
                        con.Close();
                    }

                    // write to log
                    throw (ex);
                }


                String cStr1 = BuildTeacherInsertCommand(t);
                cmd1 = CreateCommand(cStr1, con);

                try
                {
                    reportRow = new Dictionary<string, string>();
                    numEffected += cmd1.ExecuteNonQuery(); // execute the command
                    reportRow.Add("type", "teacher");
                    reportRow.Add("id", t.IdNumber);
                    reportRow.Add("message", "success");

                    report.Add(reportRow);

                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2467)
                    {
                        reportRow = new Dictionary<string, string>();
                        reportRow.Add("type", "teacher");
                        reportRow.Add("id", t.Mail);
                        reportRow.Add("message", "already in DB");

                        report.Add(reportRow);
                        continue;
                    }
                    if (con != null)
                    {
                        // close the db connection
                        con.Close();
                    }

                    // write to log
                    throw (ex);
                }


            }

            if (con != null)
            {
                // close the db connection
                con.Close();
            }

            return numEffected;



        }
        public List<Dictionary<string, string>> postnEdlFile(List<Teacher> list)
        {

            
            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;
            SqlCommand cmd1;
            List<Dictionary<string, string>> report = new List<Dictionary<string, string>>();
            Dictionary<string, string> reportRow = new Dictionary<string, string>();


            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }

            foreach(var t in list)
            {
                String cStr = BuildUserInsertCommand(t);
                cmd = CreateCommand(cStr, con);
                try
                {
                    reportRow = new Dictionary<string, string>();
                    numEffected += cmd.ExecuteNonQuery(); // execute the command
                    reportRow.Add("type", "user");
                    reportRow.Add("id", t.Mail);
                    reportRow.Add("message", "success");
                    reportRow.Add("details", "user of student succeed");

                    report.Add(reportRow);


                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627)
                    {
                        reportRow = new Dictionary<string, string>();
                        reportRow.Add("type", "user");
                        reportRow.Add("id", t.Mail);
                        reportRow.Add("message", "failed");
                        reportRow.Add("details", "user of teacher is in DB");
                        report.Add(reportRow);
                        continue;
                    }
                    else
                    {
                        reportRow = new Dictionary<string, string>();
                        reportRow.Add("type", "user");
                        reportRow.Add("id", t.Mail);
                        reportRow.Add("message", "failed");
                        reportRow.Add("details", "user of teacher created failed");
                        report.Add(reportRow);
                        continue;

                    }
             
                }


                String cStr1 = BuildTeacherInsertCommand(t);
                cmd1 = CreateCommand(cStr1, con);

                try
                {
                    reportRow = new Dictionary<string, string>();
                    numEffected += cmd1.ExecuteNonQuery(); // execute the command
                    reportRow.Add("type", "teacher");
                    reportRow.Add("id", t.IdNumber);
                    reportRow.Add("message", "success");
                    reportRow.Add("details", "student succeed");

                    report.Add(reportRow);

                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627)
                    {
                        reportRow = new Dictionary<string, string>();
                        reportRow.Add("type", "teacher");
                        reportRow.Add("id", t.Mail);
                        reportRow.Add("message", "failed");
                        reportRow.Add("details", "teacher  is already in DB");
                        report.Add(reportRow);

                        continue;
                    }
                    else
                    {
                        deleteUser(t.Mail);
                        reportRow = new Dictionary<string, string>();
                        reportRow.Add("type", "teacher");
                        reportRow.Add("id", t.Mail);
                        reportRow.Add("message", "failed");
                        reportRow.Add("details", "teacher created failed(his user deleted)");
                        report.Add(reportRow);

                        continue;

                    }
                }
                    


                }
          
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
            
                return report;
   
        }

        public int postNewStudent(List<Dictionary<string, string>> slist)
        {
            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;
            SqlCommand cmd1;
           

            try
            {
                con = connect("DBConnectionString"); // create the connection

            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }

            foreach (var item in slist)
            {


                User u = new User(item["Name"], item["LastName"], item["Password"], item["Mail"], item["IdNumber"], Convert.ToInt32(item["SCode"]));

                String cStr = BuildUserInsertCommand(u);
                cmd = CreateCommand(cStr, con);
                try
                {

                    numEffected += cmd.ExecuteNonQuery(); // execute the command
      
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2467)
                    {
                      
                        continue;
                    }
                    if (con != null)
                    {
                        // close the db connection
                        con.Close();
                    }

                    // write to log
                    throw (ex);
                }


                String cStr1 = BuildStudentInsertCommand(item);
                cmd1 = CreateCommand(cStr1, con);

                try
                {

                    numEffected += cmd1.ExecuteNonQuery(); // execute the command

                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2467)
                    {

                        continue;
                    }
                    if (con != null)
                    {
                        // close the db connection
                        con.Close();
                    }

                    // write to log
                    throw (ex);
                }

                if (con != null)
                {
                    // close the db connection
                    con.Close();
                }


            }

            return numEffected;
        }

        public List<Dictionary<string, string>> postStudentFile (List<Dictionary<string,string>> slist)
        {
            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;
            SqlCommand cmd1;
            List<Dictionary<string, string>> report = new List<Dictionary<string, string>>();
            Dictionary<string, string> reportRow = new Dictionary<string, string>();

            try
            {
                con = connect("DBConnectionString"); // create the connection

            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }

            foreach (var item in slist)
            {
                

                    User u = new User(item["Name"],item["LastName"],item["Password"],item["Mail"],item["IdNumber"],Convert.ToInt32(item["SCode"]));

                    String cStr = BuildUserInsertCommand(u);
                    cmd = CreateCommand(cStr, con);
                try
                {
                    reportRow = new Dictionary<string, string>();

                    numEffected += cmd.ExecuteNonQuery(); // execute the command
                    reportRow.Add("type", "user");
                    reportRow.Add("id", u.Mail);
                    reportRow.Add("message", "success");
                    reportRow.Add("details", "user of student succeed");
                    report.Add(reportRow);


                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627)
                    {
                        reportRow = new Dictionary<string, string>();
                        reportRow.Add("type", "user");
                        reportRow.Add("id", u.Mail);
                        reportRow.Add("message", "already in DB");
                        reportRow.Add("details", "user of student is already in DB");

                        report.Add(reportRow);
                        continue;
                    }
                    else if (ex.Number == 547)
                    {
                        reportRow = new Dictionary<string, string>();
                        reportRow.Add("type", "user");
                        reportRow.Add("id", u.Mail);
                        reportRow.Add("message", "failed");
                        reportRow.Add("details", "one of the row data is not valid");

                        report.Add(reportRow);
                        continue;
                    }
                    else
                    {
                        reportRow = new Dictionary<string, string>();
                        reportRow.Add("type", "user");
                        reportRow.Add("id", u.Mail);
                        reportRow.Add("message", "failed");
                        reportRow.Add("details", "user of student created failed");

                        report.Add(reportRow);
                        continue;

                    }


                }


                String cStr1 = BuildStudentInsertCommand(item);
                    cmd1 = CreateCommand(cStr1, con);

                try
                {

                    reportRow = new Dictionary<string, string>();
                    numEffected += cmd1.ExecuteNonQuery(); // execute the command
                    reportRow.Add("type", "student");
                    reportRow.Add("id", u.Mail);
                    reportRow.Add("message", "success");
                    reportRow.Add("details", "student succeed");

                    report.Add(reportRow);

                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627)
                    {
                        reportRow = new Dictionary<string, string>();
                        reportRow.Add("type", "student");
                        reportRow.Add("id", u.Mail);
                        reportRow.Add("message", "already in DB");
                        reportRow.Add("details", "student is already in DB(the user does open)");
                        report.Add(reportRow);
                        continue;
                    }
                    else if (ex.Number == 547)
                    {
                        reportRow = new Dictionary<string, string>();
                        reportRow.Add("type", "student");
                        reportRow.Add("id", u.Mail);
                        reportRow.Add("message", "failed");
                        reportRow.Add("details", "probably class or classNumber incorrect(his user deleted)");
                        deleteUser(u.Mail);
                        report.Add(reportRow);
                        continue;
                    }
                    else
                    {

                        reportRow = new Dictionary<string, string>();
                        reportRow.Add("type", "student");
                        reportRow.Add("id", u.Mail);
                        reportRow.Add("message", "failed");
                        reportRow.Add("details", "failed created student his user deleted");
                        deleteUser(u.Mail);
                        report.Add(reportRow);
                        continue;

                    }
               
                }


            }

            if (con != null)
            {
                // close the db connection
                con.Close();
            }
            return report;
        }

        public void deleteUser(string mail)
        {
            int numEffected=0;
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }


            StringBuilder sb = new StringBuilder();


            sb.AppendFormat("DELETE from [User] WHERE [User].[Email]={0}",mail);
            String cStr = sb.ToString();
            // helper method to build the insert string

            cmd = CreateCommand(cStr, con);             // create the command

            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command

            }
            catch (Exception ex)
            {
                if (con != null)
                {
                    // close the db connection
                    con.Close();
                }
                // write to log
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    // close the db connection
                    con.Close();
                }
            }

        }

        public List<Dictionary<string, string>> postNewClasses(Dictionary<int, Dictionary<int,Classroom>> c)
        {
            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;
            SqlCommand cmd1;
            SqlCommand cmd2;
            List<Dictionary<string, string>> report = new List<Dictionary<string, string>>();
            Dictionary<string, string> reportRow = new Dictionary<string, string>();

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
          

            
            foreach (var entry in c)
            {

                foreach (var arg in entry.Value)
                {
                    String cStr = BuildUserInsertCommand(arg.Value.EdTeacher);
                    cmd = CreateCommand(cStr, con);
                    try
                    {
                        reportRow = new Dictionary<string, string>();
                        numEffected += cmd.ExecuteNonQuery(); // execute the command
                        reportRow.Add("type", "user");
                        reportRow.Add("id", arg.Value.EdTeacher.Mail);
                        reportRow.Add("message", "success");
                        reportRow.Add("details", "user of teacher succeed");

                        report.Add(reportRow);

                    }
                    catch (SqlException ex)
                    {

                        if (ex.Number == 2627)
                        {
                            reportRow = new Dictionary<string, string>();
                            reportRow.Add("type", "user");
                            reportRow.Add("id", arg.Value.EdTeacher.Mail);
                            reportRow.Add("message", "failed");
                            reportRow.Add("details", "user of teacher with this mail already in DB");

                            report.Add(reportRow);

                        }

                        else
                        {

                            reportRow = new Dictionary<string, string>();
                            reportRow.Add("type", "user");
                            reportRow.Add("id", arg.Value.EdTeacher.Mail);
                            reportRow.Add("message", "failed");
                            reportRow.Add("details", "user of teacher failed");

                            report.Add(reportRow);


                        }
                    }

                        String cStr1 = BuildTeacherInsertCommand(arg.Value.EdTeacher);
                    cmd1 = CreateCommand(cStr1, con);

                    try
                    {
                        reportRow = new Dictionary<string, string>();
                        numEffected += cmd1.ExecuteNonQuery(); // execute the command
                        reportRow.Add("type", "teacher");
                        reportRow.Add("id", arg.Value.EdTeacher.Mail);
                        reportRow.Add("message", "Success");
                        reportRow.Add("details", "teacher succeed");
                        report.Add(reportRow);

                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627)
                        {
                            reportRow = new Dictionary<string, string>();
                            reportRow.Add("type", "teacher");
                            reportRow.Add("id", arg.Value.EdTeacher.Mail);
                            reportRow.Add("message", "failed");
                            reportRow.Add("details", "teacher email is in DB");

                            report.Add(reportRow);
                           
                        }

                        else
                        {
                           
                            //delete from user
                            deleteUser(arg.Value.EdTeacher.Mail);
                            reportRow.Add("type", "teacher");
                            reportRow.Add("id", arg.Value.EdTeacher.Mail);
                            reportRow.Add("message", "failed");
                            reportRow.Add("details", "teacher created failed(his user deleted),class does not open");
                            report.Add(reportRow);
                            continue;
                        }
                    }

                    String cStr2 = BuildClassInsertCommand(arg.Value);
                    cmd2 = CreateCommand(cStr2, con);

                    try
                    {
                        reportRow = new Dictionary<string, string>();
                        numEffected += cmd2.ExecuteNonQuery(); // execute the command
                        reportRow.Add("type", "class");
                        reportRow.Add("id", arg.Value.Grade.ToString()+"`"+arg.Value.GradeNumber.ToString());
                        reportRow.Add("message", "Succes");
                        reportRow.Add("details", "class succeed");
                        report.Add(reportRow);

                    }
                    catch (SqlException ex)
                    {

                        if (ex.Number == 2627)
                        {
                            reportRow.Add("type", "Class");
                            reportRow.Add("id", arg.Value.Grade + "`" + arg.Value.GradeNumber);
                            reportRow.Add("message", "failed");
                            reportRow.Add("details", "class for this school is in DB");

                            report.Add(reportRow);
                            continue;
                        }
                        else {
                            reportRow.Add("type", "Class");
                            reportRow.Add("id", arg.Value.Grade + "`" + arg.Value.GradeNumber);
                            reportRow.Add("message", "failed");
                            reportRow.Add("details", "class created failed");
                            continue;
                        }

                    }
                }


            }
         

            if (con != null)
            {
                // close the db connection
                con.Close();
            }
            return report;
        }

        //------------------------------------------------------------------------------//

            //----Builder----------//
        private String BuildUserInsertCommand(User u)
        {
            String command;


            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}','{1}','{2}','{3}','{4}',{5})", u.Name ,u.LastName ,u.Mail,u.IdNumber,u.Password,u.SCode);

            String prefix = "INSERT INTO [User]" + "(Fname,Lname,Email,IdNumber,[Password],SchoolCode)";
            command = prefix + sb.ToString();

            return command;
        }

        private String BuildTeacherInsertCommand(Teacher t)
        {
            String command;


            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}',{1},{2})", t.Mail,0,0);

            String prefix = "INSERT INTO Teacher" + "(TeacherEmail,isEditor,SchoolAdmin)";
            command = prefix + sb.ToString();

            return command;
        }

        private String BuildStudentInsertCommand(Dictionary<string,string> s)
        {
            String command;


            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}',{1},{2},{3})",s["Mail"],s["SCode"],s["Grade"],s["ClassNumber"]); 

            String prefix = "INSERT INTO Student" + "(StudentEmail,ClassSchoolCode,GradeClass,ClassNumber)";
            command = prefix + sb.ToString();

            return command;
        }

        private String BuildClassInsertCommand(Classroom c)
        {

            String command;


            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}',{1},{2},{3},'{4}')", c.Name, c.Grade, c.GradeNumber,c.InSchool,c.EdTeacher.Mail);

            String prefix = "INSERT INTO [Class]" + "(Title,Grade,GradeNumber,SchoolCode,TeacherEmail)";
            command = prefix + sb.ToString();

            return command;

        }

        //----------------------------------------------------------------//

        //---------------------------------------------------------------------------------------//

        public Dictionary<string, string> getUserByMail(string mail)
        {
            int flag = 0;
            SqlConnection con = null;
            Dictionary<string,string> list = new Dictionary<string, string>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String selectSTR = "select * from [User] inner join Student on StudentEmail=[user].Email where [User].Email='"+mail+"'";


                SqlCommand cmd = new SqlCommand(selectSTR, con);

               
                
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (!dr.HasRows)
                {
                    con.Close();
                    dr = null;
                    cmd.Connection = null;
                    con = connect("DBConnectionString");
                    flag = 1;
                    selectSTR = "select * from [User] inner join Teacher on TeacherEmail=[user].Email where [User].Email='" +mail+"'" ;
                    cmd = new SqlCommand(selectSTR, con);
                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                }

                while (dr.Read())
                {   // Read till the end of the data into a row

                    list.Add("IdNumber", dr["IdNumber"].ToString());
                    list.Add("Email", dr["Email"].ToString());
                    list.Add("Name", dr["Fname"].ToString());
                    list.Add("LastName", dr["Lname"].ToString());
                    list.Add("SCode", dr["SchoolCode"].ToString());
                   


                    if (flag==0)
                    {
                        list.Add("Type","Student");
                        list.Add("Grade", dr["GradeClass"].ToString());
                        list.Add("ClassNumber", dr["ClassNumber"].ToString());
                    }
                    else
                    {
                        list.Add("Type", "Teacher");
                        if(dr["schoolAdmin"].ToString() == "True")
                        list.Add("SchoolAdmin","1"); 
                        else
                            list.Add("SchoolAdmin", "0");

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

            return list;


        }


        public List<City> getAllcities()
        {
            
            SqlConnection con = null;
            List<City> list = new List<City>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String selectSTR = "select * from City";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    City c = new City();

                    c.Area = dr["Area"].ToString();
                    c.Hname = dr["Hname"].ToString();
                    c.Ename = dr["Ename"].ToString();
                    c.Code = Convert.ToInt32(dr["CityCode"]);

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

        //------------schoolbycity---------------------------------//
        public List<School> getSchoolByCity(int code)
        {
            SqlConnection con = null;
            List<School> list = new List<School>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String selectSTR = "select * from School where CityCode="+code;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    School s = new School();

                
                    s.Name = dr["Name"].ToString();
                    s.SchoolCode = Convert.ToInt32(dr["SchoolCode"]);
                    s.CityCode= Convert.ToInt32(dr["CityCode"]);

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

        //------------------------------------------------------------//

        //------------schoolbycode---------------------------------//
        public School getSchoolByID(int Scode)
        {
            SqlConnection con = null;
            School s = new School();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String selectSTR = "select * from School where SchoolCode=" + Scode;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                 


                    s.Name = dr["Name"].ToString();
                    s.SchoolCode = Convert.ToInt32(dr["SchoolCode"]);
                    s.CityCode = Convert.ToInt32(dr["CityCode"]);

                  

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

            return s;


        }

        //------------------------------------------------------------//

        //------------ALLschool---------------------------------//
        public List<School> getAllschool()
        {
            SqlConnection con = null;
            List<School> list = new List<School>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String selectSTR = "select * from School";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    School s = new School();


                    s.Name = dr["Name"].ToString();
                    s.SchoolCode = Convert.ToInt32(dr["SchoolCode"]);
                    s.CityCode = Convert.ToInt32(dr["SchoolCode"]);

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

        //------------------------------------------------------------//

        //---------getClassBySchool----------------------------------//

        public List<Classroom> getClassBySchool(int code)
        {
            SqlConnection con = null;
            List<Classroom> list = new List<Classroom>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String spart = "select  c.Grade,c.GradeNumber,c.Title,c.TeacherEmail,ut.Fname as tFname,ut.Lname as tLname,ut.IdNumber as tId,us.Fname as sFname,us.Lname as sLname,us.IdNumber as [sId],st.StudentEmail,c.SchoolCode ";
                String fpart = " from Class as c left join Student as st on st.GradeClass=c.Grade and st.ClassNumber=c.GradeNumber and c.SchoolCode=st.ClassSchoolCode left join [User] as us on us.Email=st.StudentEmail left join [User] as ut on ut .Email=c.TeacherEmail ";
                String wpart = "where c.SchoolCode="+code;
                String opart = " order By c.Grade,c.GradeNumber";
                String selectSTR = spart+fpart+wpart+opart;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                Classroom c = new Classroom();
                while (dr.Read())
                {   // Read till the end of the data into a row

                    Student s = new Student();

                    if (c.Grade!= Convert.ToInt32(dr["Grade"]) || c.GradeNumber!= Convert.ToInt32(dr["GradeNumber"]))
                    {
                        c = new Classroom();
                        c.EdTeacher = new Teacher();
                        c.InSchool = Convert.ToInt32(dr["SchoolCode"]);
                        c.EdTeacher.IdNumber = dr["tId"].ToString();
                        c.Grade = Convert.ToInt32(dr["Grade"]);
                        c.GradeNumber = Convert.ToInt32(dr["GradeNumber"]);
                        c.Name = dr["Title"].ToString();
                        c.EdTeacher.Mail = dr["TeacherEmail"].ToString();
                        c.EdTeacher.Name = dr["tFname"].ToString();
                        c.EdTeacher.LastName = dr["tLname"].ToString();
                       
                        
                      
                        list.Add(c);
                    }


                    if (dr["sFname"].ToString() != "")
                    {


                        s.Name = (dr["sFname"].ToString());
                        s.LastName = dr["sLname"].ToString();
                        s.Mail = dr["StudentEmail"].ToString();
                        s.IdNumber = dr["sId"].ToString();

                        foreach (var c1 in list)
                        {
                            if (c1.Grade == Convert.ToInt32(dr["Grade"]) && c1.GradeNumber == Convert.ToInt32(dr["GradeNumber"]))
                            {
                                c1.Slist.Add(s);
                            }
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

            return list;
        }

        public string getSchoolName(int code)
        {
            SqlConnection con = null;
            string name="";


            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String selectSTR = "select Name from School where SchoolCode="+code;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                
                while (dr.Read())
                {   // Read till the end of the data into a row

                    name = dr["Name"].ToString();
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

            return name;
        }

        //------------------------------------------------------------------//

        //------------------------teacherBySchool--------------------------------//

        public List<Teacher> getTeacherByScode(int scode)
        {
            SqlConnection con = null;
            List<Teacher> list = new List<Teacher>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String selectSTR = "select * from [User] inner join Teacher on TeacherEmail=[user].Email where SchoolCode=" + scode;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                Classroom c = new Classroom();
                while (dr.Read())
                {   // Read till the end of the data into a row
                    Teacher t = new Teacher();
                    t.IdNumber = dr["IdNumber"].ToString();
                    t.Mail = dr["TeacherEmail"].ToString();
                    t.Name = dr["Fname"].ToString();
                    t.LastName = dr["Lname"].ToString();

                    list.Add(t);


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

        //------------------------------------------------------------------//

        public int postNewClass(Classroom c)
        {

            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }


            String cStr = BuildClassInsertCommand(c);

            // helper method to build the insert string

            cmd = CreateCommand(cStr, con);             // create the command

            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command

            }
            catch (Exception ex)
            {
                if (con != null)
                {
                    // close the db connection
                    con.Close();
                }
                // write to log
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    // close the db connection
                    con.Close();
                }


            }
            return numEffected;
        }


        public List<Teacher> getAllteachers()
        {
            SqlConnection con = null;
            List<Teacher> list = new List<Teacher>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String selectSTR = "select * from [User] inner join Teacher on TeacherEmail=[user].Email";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                Classroom c = new Classroom();
                while (dr.Read())
                {   // Read till the end of the data into a row
                    Teacher t = new Teacher();

                    t.Mail = dr["TeacherEmail"].ToString();
                    t.Name = dr["Fname"].ToString();
                    t.LastName = dr["Lname"].ToString();
                    t.IsEditor = Convert.ToBoolean(dr["isEditor"]);

                    list.Add(t);


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

        public int updateEditors(List<Teacher> tlist)
        {
            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
            foreach (var t in tlist)
            {



                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("UPDATE Teacher  SET  [isEditor]={0} where [TeacherEmail]='{1}'", Convert.ToInt32(t.IsEditor), t.Mail);
                String cStr = sb.ToString();
                // helper method to build the insert string

                cmd = CreateCommand(cStr, con); // create the command

                try
                {
                    numEffected += cmd.ExecuteNonQuery(); // execute the command

                }
                catch (Exception ex)
                {
                    if (con != null)
                    {
                        // close the db connection
                        con.Close();
                    }
                    // write to log
                    throw (ex);
                }
            }

            if (con != null)
            {
                // close the db connection
                con.Close();
            }



            return numEffected;
        }
        public int UpdateStudent(List<Dictionary<string, string>> userDict)
        {
            int num = 0;
            num += UpdateUser(userDict);
            return num;
        }

        public int UpdateTeacher(List<Dictionary<string, string>> userDict)
        {
            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;
            numEffected+= UpdateUser(userDict);
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
            foreach (var u in userDict)
            {

                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("UPDATE [Teacher]  SET [schoolAdmin]={1} where [TeacherEmail]='{0}'", u["Mail"], u["SchoolAdmin"]);
                String cStr = sb.ToString();
                // helper method to build the insert string

                cmd = CreateCommand(cStr, con); // create the command

                try
                {
                    numEffected += cmd.ExecuteNonQuery(); // execute the command

                }
                catch (Exception ex)
                {
                    if (con != null)
                    {
                        // close the db connection
                        con.Close();
                    }
                    // write to log
                    throw (ex);
                }
            }

            if (con != null)
            {
                // close the db connection
                con.Close();
            }



            return numEffected;
           
         
        }

        public int UpdateUser(List<Dictionary<string, string>> userDict)
        {
            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;
            SqlCommand cmd2;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
            foreach (var u in userDict)
            {

                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("UPDATE [User]  SET [IdNumber]='{0}',[Email]='{1}',[Fname]='{2}',[Lname]='{3}',[SchoolCode]={4} where [Email]='{5}'", u["IdNumber"], u["Mail"], u["Name"], u["LastName"], u["SCode"], u["orginal_mail"]);
                String cStr = sb.ToString();
                // helper method to build the insert string

                cmd = CreateCommand(cStr, con); // create the command

                try
                {
                    numEffected += cmd.ExecuteNonQuery(); // execute the command

                }
                catch (Exception ex)
                {
                    if (con != null)
                    {
                        // close the db connection
                        con.Close();
                    }
                    // write to log
                    throw (ex);
                }
                if (u["Type"] == "תלמיד")
                {


                    sb.Clear();
                    sb.AppendFormat("UPDATE[Student]  SET [GradeClass]={0},[ClassNumber]={1} where [StudentEmail]='{2}'", u["Grade"], u["ClassNumber"], u["Mail"]);
                    String cStr1 = sb.ToString();
                    cmd2 = CreateCommand(cStr1, con);

                    try
                    {
                        numEffected += cmd2.ExecuteNonQuery(); // execute the command

                    }
                    catch (Exception ex)
                    {
                        if (con != null)
                        {
                            // close the db connection
                            con.Close();
                        }
                        // write to log
                        throw (ex);
                    }
                }


            }

            if (con != null)
            {
                // close the db connection
                con.Close();
            }



            return numEffected;
        }


        public int updateclass(Classroom c)
        {
            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
           

                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("Update [Class] SET [Title]='{0}',[TeacherEmail]='{1}' where [Grade]={2} and [SchoolCode]={3} and [GradeNumber]={4}",c.Name ,c.EdTeacher.Mail,c.Grade,c.InSchool,c.GradeNumber);
                String cStr = sb.ToString();
                // helper method to build the insert string

                cmd = CreateCommand(cStr, con); // create the command

                try
                {
                    numEffected += cmd.ExecuteNonQuery(); // execute the command

                }
                catch (Exception ex)
                {
                    if (con != null)
                    {
                        // close the db connection
                        con.Close();
                    }
                    // write to log
                    throw (ex);
                }

                finally
                {
                   if (con != null)
                   {
                    // close the db connection
                    con.Close();
                   }
                }
         



            return numEffected;
        }

        public List<int> getnumbers(int grade,int scode)
        {
            List<int> numList = new List<int>();
            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String selectSTR = "select GradeNumber from Class where SchoolCode="+ scode + " and Grade="+grade;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row


                    numList.Add(Convert.ToInt32(dr["GradeNumber"]));

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

            return numList;
        }

        public DBservices readClass(string mail)
        {
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString");
                da = new SqlDataAdapter("select * from Class where TeacherEmail='"+mail+"'", con);
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


        public void update()
        {
            da.Update(dt);
        }
    }
}










