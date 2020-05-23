using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using project.Models;
using System.Net;
using System.Net.Mail;
using System.Drawing;
using System.Configuration;

namespace project.Models.DAL
{
    public class TeacherDBservices
    {
        public System.Data.SqlClient.SqlDataAdapter da;

        public DataTable dt;

        public TeacherDBservices()
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

        public List<Quiz> getQuestionnaire(string taskNum)
        {
            SqlConnection con = null;
            List<Quiz> quizList = new List<Quiz>();


            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
                String select1 = "  select q.IntelligenceName,q.creationTime,q.QuestionnaireId,i.EnglishName as QEnglishName,";
                String select2 = " qu.QuestionnaireId,qu.QuestionId,qu.OrderNum,qu.Content,qu.[Type],qu.ImgLink,qu.VideoLink,Count(qu.QuestionId) as num_of_Q,a.Content AS ans_content,a.IsRight,a.AnswerId,Count(a.AnswerId)as num_of_ans ";
                String from1 = " from(select [TaskId],[IntelligenceName],max([QuestionnaireId])as QuestionnaireId,MAX([creationTime])as creationTime ";
                String from2 = "   from Questionnaire  group by [TaskId],[IntelligenceName])q inner join  Intelligence as i on q.IntelligenceName=i.[EnglishName] inner join Question as qu on qu.QuestionnaireId=q.QuestionnaireId left join Answer as a on a.QuestionId=qu.QuestionId ";
                String where = "where q.TaskId='"+taskNum+"'";
                String groupby = "  group by  q.IntelligenceName,q.creationTime,q.QuestionnaireId,i.EnglishName,qu.QuestionnaireId,qu.QuestionId,qu.OrderNum,qu.Content,qu.[Type],qu.ImgLink,qu.VideoLink,a.Content,a.IsRight,a.AnswerId ";
                String selectSTR = select1 + select2 + from1 + from2+where+groupby;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                Quiz q = new Quiz();
                Question que = new Question();
                while (dr.Read())
                {   // Read till the end of the data into a row

                    if (dr["QuestionnaireId"].ToString() != q.QuizID)
                    {
                        if (q.QuizID !=null)
                        {
                            quizList.Add(q);
                        }

                        q = new Quiz();
                        q.FbList = new List<FeedBack>();
                        q.Question = new List<Question>();
                        q.QuizID = dr["QuestionnaireId"].ToString();
                        q.Inteligence = new Inteligence(0, dr["IntelligenceName"].ToString(), dr["QEnglishName"].ToString(),0);
                        q.FbList = getFBbyId(q.QuizID);
                    }
                   
                    if(dr["QuestionId"].ToString() != que.QuestionId)
                    {
                        
                        que =new Question(dr["Type"].ToString(), dr["Content"].ToString(), new List<Answer>(), dr["ImgLink"].ToString(), dr["VideoLink"].ToString(), Convert.ToInt32(dr["OrderNum"]), dr["QuestionId"].ToString());
                        q.Question.Add(que);
                    }

                    if(dr["ans_content"] != DBNull.Value)
                    que.Answer.Add(new Answer(dr["ans_content"].ToString(),Convert.ToBoolean(dr["IsRight"]), Convert.ToInt32(dr["AnswerId"])));
    

                }

                quizList.Add(q);
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


            return quizList;
        }

        public int PostQuiz( Quiz q)
        {
            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;
            SqlCommand cmd1; 
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


            String cStr = BuildQuizInsertCommand(q);

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
            foreach (var qu in q.Question)
            {
                String cStr1 = BuildQuestionInsertCommand(qu,q.TaskId,q.QuizID);

                // helper method to build the insert string

                cmd1 = CreateCommand(cStr1, con);

                try
                {
                    numEffected += cmd1.ExecuteNonQuery(); // execute the command

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

                foreach(var ans in qu.Answer)
                {
                    String cStr2 = BuildAnswerInsertCommand(ans,q.QuizID,q.TaskId, qu.QuestionId);
                  

                    // helper method to build the insert string

                    cmd2 = CreateCommand(cStr2, con);

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

        public List<Classroom> getClassesbyMail(string mail)
        {
            SqlConnection con = null;
            List<Classroom> list = new List<Classroom>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String spart = "select  c.Grade,c.GradeNumber,c.Title,c.TeacherEmail,ut.Fname as tFname,ut.Lname as tLname,ut.IdNumber as tId,us.Fname as sFname,us.Lname as sLname,us.IdNumber as [sId],st.StudentEmail,c.SchoolCode ";
                String fpart = " from Class as c left join Student as st on st.GradeClass=c.Grade and st.ClassNumber=c.GradeNumber and c.SchoolCode=st.ClassSchoolCode left join [User] as us on us.Email=st.StudentEmail left join [User] as ut on ut .Email=c.TeacherEmail ";
                String wpart = "where c.TeacherEmail='"+mail+"'" ;
                String opart = " order By c.Grade,c.GradeNumber";
                String selectSTR = spart + fpart + wpart + opart;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                Classroom c = new Classroom();
                while (dr.Read())
                {   // Read till the end of the data into a row

                    Student s = new Student();

                    if (c.Grade != Convert.ToInt32(dr["Grade"]) || c.GradeNumber != Convert.ToInt32(dr["GradeNumber"]))
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

        public List<Team> getTeamsbyMail(string mail)
        {
            SqlConnection con = null;
            List<Team> list = new List<Team>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String part1 = "select t.TeamId,t.Title,t.Grade,t.SubjectName,t.[status],s.ImgLink,sit.StudentEmail,IdNumber,Fname,Lname ";
                String part2 = "from Team as t inner join [Subject] as s on s.[Name]=t.SubjectName  left join StudentInTeam as sit on sit.TeamId=t.TeamId left join [User] on [User].Email=sit.StudentEmail where TeacherEmail='"+mail+ "' order by TeamId";
                String selectSTR = part1 + part2;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                Team t = new Team();
                while (dr.Read())
                {   // Read till the end of the data into a row
                   

                    if(t.Id!= dr["TeamId"].ToString())
                    {
                        if (t.Id != null)
                        {
                            list.Add(t);
                        }
                       
                        t = new Team();
                        t.StudentList = new List<Student>();
                        t.Title = dr["Title"].ToString();
                        t.Status = Convert.ToChar(dr["status"]);
                        t.Id = dr["TeamId"].ToString();
                        t.Grade = Convert.ToInt32(dr["Grade"]);
                        t.Subject = new Subject(dr["SubjectName"].ToString(), dr["ImgLink"].ToString());

                    }

                    if (!(dr["StudentEmail"] == DBNull.Value))
                    {
                        Student s = new Student();
                        s.Name = dr["Fname"].ToString();
                        s.LastName = dr["Lname"].ToString();
                        s.IdNumber = dr["IdNumber"].ToString();
                        s.Mail = dr["StudentEmail"].ToString();
                        t.StudentList.Add(s);
                    }
                  

                    

                }
                list.Add(t);


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

        public Team getStudentsbyGrade(int grade,int scode,string mode)
        {
            int count = 0;
            SqlConnection con = null;
            Team t = new Team();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String select1 = "select [User].IdNumber,[User].Fname,[User].Lname,Student.ClassNumber,Student.GradeClass,[User].Email,case when sit.StudentEmail=NULL Then 1 Else 1 END AS INTEAM,team.Title,team.SubjectName,team.Grade";
                String from1 = " from [User] inner join Student on StudentEmail=[user].Email inner join [dbo].[StudentInTeam] as sit on sit.[StudentEmail]=[user].Email inner join Team on team.TeamId=sit.TeamId ";
                String where1 = " where [Student].GradeClass='"+grade+"' and [User].SchoolCode='"+scode+"'  and sit.TeamId='"+mode+"'";
                String select2 = " select distinct [User].IdNumber,[User].Fname,[User].Lname,Student.ClassNumber,Student.GradeClass,[User].Email, case when StudentEmail=StudentEmail Then 0 Else 0 END AS INTEAM, case when StudentEmail=StudentEmail Then '' Else '' END AS Title, case when StudentEmail=StudentEmail Then '' Else '' END AS [SubjectName], case when StudentEmail=StudentEmail Then '' Else '' END AS Grade ";
                String from2 = " from [User] inner join Student on StudentEmail=[user].Email  ";
                String where2 = " where [Student].GradeClass='"+grade+"' and [User].SchoolCode='460154' and Student.StudentEmail not in (select sit1.StudentEmail from StudentInTeam as sit1 where sit1.TeamId='"+mode+"')";
                String selectSTR = select1 + from1 + where1 + " union " + select2 + from2 + where2;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    Student s = new Student();

                    if(count==0 && Convert.ToInt32(dr["INTEAM"])==1)
                    {
                        count++;
                        t.Title = dr["Title"].ToString();
                        t.Subject = new Subject(dr["SubjectName"].ToString(), "");
                        t.Grade = Convert.ToInt32(dr["Grade"]);
                    }

                    s.SCode = Convert.ToInt32(dr["INTEAM"]);
                    s.Mail = dr["Email"].ToString();
                    s.IdNumber= dr["IdNumber"].ToString();
                    s.Name= dr["Fname"].ToString();
                    s.LastName= dr["Lname"].ToString();
                    s.ClassNumber= Convert.ToInt32(dr["ClassNumber"]);
                    t.StudentList.Add(s);
                    



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

            return t;

        }

        public int postTeam(Team t)
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


            String cStr = BuildTeamInsertCommand(t);

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
            foreach(var s in t.StudentList)
            {
                String cStr1 = BuildSITInsertCommand(t,s);

                // helper method to build the insert string

                cmd1 = CreateCommand(cStr1, con);

                try
                {
                    numEffected += cmd1.ExecuteNonQuery(); // execute the command

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

        private String BuildAnswerInsertCommand(Answer ans,string QuestionnaireId,string TaskId,string QuestionId)
        {
            String command;


            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}','{1}','{2}','{3}',{4})", QuestionnaireId,TaskId, QuestionId,ans.Content,Convert.ToInt32(ans.IsRight));

            String prefix = "INSERT INTO Answer" + "(QuestionnaireId,TaskId, QuestionId,Content,IsRight)";
            command = prefix + sb.ToString();

            return command;

        }
        private String BuildQuestionInsertCommand(Question qu, string taskID,string questionnaireCode)
        {
            String command;


            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}','{1}',{2},'{3}','{4}','{5}','{6}','{7}')", questionnaireCode,qu.Content,qu.OrderNum,qu.Type,qu.ImgLink,qu.VideoLink,taskID,qu.QuestionId);

            String prefix = "INSERT INTO Question" + "(QuestionnaireId,Content,OrderNum,Type,ImgLink,VideoLink,TaskId,QuestionId)";
            command = prefix + sb.ToString();

            return command;

        }

        private String BuildQuizInsertCommand(Quiz q)
        {
            String command;


            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}','{1}','{2}')", q.Inteligence.Ename,q.TaskId,q.QuizID);

            String prefix = "INSERT INTO Questionnaire" + "(IntelligenceName,TaskId,QuestionnaireId)";
            command = prefix + sb.ToString();

            return command;
        }

        private String BuildSITInsertCommand(Team t,Student s)
        {
            String command;


            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}','{1}',{2})", t.Id,s.Mail,t.Scode);

            String prefix = "INSERT INTO StudentInTeam" + "(TeamId,StudentEmail,SchoolCode)";
            command = prefix + sb.ToString();

            return command;
        }

        private String BuildTeamInsertCommand(Team t)
        {
            String command;


            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}',{1},'{2}',{3},'{4}', '{5}')", t.Title, t.Grade, t.Teacher.Mail, t.Scode, t.Subject.Name, t.Id);

            String prefix = "INSERT INTO Team" + "(Title,Grade,TeacherEmail,SchoolCode,SubjectName,TeamId)";
            command = prefix + sb.ToString();

            return command;
        }

        public List<Task> getTasksBySubject(string subName)
        {

            SqlConnection con = null;
            List<Task> taskList = new List<Task>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String selectSTR ="select * from Task where SubjectName='"+subName+"' and Active=1";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    Task t = new Task();
                    t.Sub = new Subject();

                    t.TaskId = (dr["TaskId"]).ToString();
                    t.Title = dr["Title"].ToString();
                    t.Grade= Convert.ToInt32(dr["Grade"]);
                    t.Sub.Name = dr["SubjectName"].ToString();
                    List<Quiz> li = new List<Quiz>();
                    li = getQuestionnaire(t.TaskId);
                    if (li[0].QuizID!=null)
                    {
                        t.QuizList = li; 
                    }
                    
                   

                    taskList.Add(t);
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

            return taskList;

        }

        public int postTask(Task t)
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


            String cStr = BuildTaskInsertCommand(t);

            // helper method to build the insert string

            cmd = CreateCommand(cStr, con);             // create the command

            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command

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
                    // close the db connection
                    con.Close();
                }
            }

            return numEffected;
        }

        private String BuildTaskInsertCommand(Task t)
        {
            String command;


            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}',{1},'{2}','{3}')", t.Title, t.Grade,t.Sub.Name,t.TaskId);

            String prefix = "INSERT INTO Task" + "(Title,Grade,SubjectName,TaskId)";
            command = prefix + sb.ToString();

            return command;
        }

        public List<FeedBack> getFBbyId(string id)
        {
            List<FeedBack> fbList = new List<FeedBack>();
            SqlConnection con = null;
            List<Task> taskList = new List<Task>();

            try
            {

                con = connect("DBConnectionString");

                String selectSTR = "select * from FeedBack where QuestionnaireId='" + id + "'";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    FeedBack fb= new FeedBack();

                    fb.Content = dr["Content"].ToString();
                    fb.Score = Convert.ToInt32(dr["Score"]);
                

                    fbList.Add(fb);
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

            return fbList;
        }

        public int postFB( FeedBack fb)
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


            String cStr = BuildFBInsertCommand(fb);

            // helper method to build the insert string

            cmd = CreateCommand(cStr, con);             // create the command

            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command

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
                    // close the db connection
                    con.Close();
                }
            }

            return numEffected;

        }

        private String BuildFBInsertCommand(FeedBack fb)
        {
            String command;


            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}','{1}','{2}',{3},'{4}')",fb.TaskId,fb.QuestionnaireId,fb.Content,fb.Score,fb.Id);

            String prefix = "INSERT INTO FeedBack" + "(TaskId,QuestionnaireId,Content,Score,FeedBackId)";
            command = prefix + sb.ToString();

            return command;
        }

        public int postAssigment(Dictionary<string, string> assigment)
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


            String cStr = BuilAssignmentInsertCommand(assigment);

            // helper method to build the insert string

            cmd = CreateCommand(cStr, con);             // create the command

            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command

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
                    // close the db connection
                    con.Close();
                }
            }

            return numEffected;
        }

        private String BuilAssignmentInsertCommand(Dictionary<string,string> assigment)
        {
            String command;


            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}','{1}','{2}','{3}','{4}',{5})", assigment["TaskId"], assigment["TeamId"],assigment["classTime"]+":00",assigment["submitionTime"] + ":00", assigment["YearOfStudy"],assigment["TeamSchoolCode"]);

            String prefix = "INSERT INTO RealatedTo" + "(TaskId,TeamId,ForDate,OpenTill,YearOfStudy,TeamSchoolCode)";
            command = prefix + sb.ToString();

            return command;
        }

        public TeacherDBservices updateTeam(string id)
        {
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString");
                da = new SqlDataAdapter("select * from Team where Teamid='"+id+"'", con);
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

        public TeacherDBservices updateSIT(string id)
        {
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString");
                da = new SqlDataAdapter("select * from StudentInTeam where TeamId='"+id+"'", con);
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

        public int hideTask(string id)
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


            String cStr = "UPDATE Task SET Active = 0 WHERE TaskId='"+id+"' ";



            // helper method to build the insert string

            cmd = CreateCommand(cStr, con);             // create the command

            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command

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
                    // close the db connection
                    con.Close();
                }
            }

            return numEffected;
        }

        public int openSI(Dictionary<string, string> dict)
        {

            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;

            var datetime = Convert.ToDateTime(dict["Time"])+":00";
            var date = datetime.Split(' ')[0];
            var time = datetime.Split(' ')[1];
            var fixDate = date.Split('/');
            var finTime = fixDate[2] + "-" + fixDate[1] + "-" + fixDate[0] + " " + time;





            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }


            String cStr = "UPDATE Class SET SignIn_Code ='"+dict["Code"]+ "',SignIn_Time='"+ finTime + "' WHERE Grade=" +dict["Grade"]+ " and GradeNumber="+dict["GradeNumber"]+ " and SchoolCode="+dict["School"];



            // helper method to build the insert string

            cmd = CreateCommand(cStr, con);             // create the command

            try
            {
                numEffected += cmd.ExecuteNonQuery(); // execute the command

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
                    // close the db connection
                    con.Close();
                }
            }
            sendCodeMail(dict);
            return numEffected;

        }

        public void sendCodeMail(Dictionary<string,string> d)
        {
            MailMessage mm = new MailMessage("morptao@gmail.com", d["Mail"]);
            mm.Subject = "Code for registration";
            mm.Body = string.Format("Hi,<br /><br />Your code is {0} for class {1}.<br /><br />Thank You.", d["Code"], d["Title"]);
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


        }

        public void update()
        {
            da.Update(dt);
            
        
        }
        public List<Dictionary<string, string>> getTTasks(string teamId)
        {


            SqlConnection con = null;

        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String part1 = "select  tbl.taskId,tbl.Title,tbl.SubjectName,tbl.Date_Assignment,tbl.ForDate,tbl.OpenTill,tbl.YearOfStudy,tbl.YearOfStudy, ";
                String part2 = "Case when sum(CAST(pq.isWaiting as Int))>0 then sum(CAST(pq.isWaiting as Int)) else 0 END AS numWaiting ";
                String part3 = "from(select rt.TeamId, t.taskId, t.Title, t.SubjectName, rt.Date_Assignment, rt.ForDate, rt.OpenTill, rt.YearOfStudy ";
                String part4 = " from task as t inner join RealatedTo rt on rt.TaskId=t.TaskId and rt.TeamId= '"+teamId+"' ";
                string part5 = "group by t.taskId,t.Title,t.SubjectName,rt.Date_Assignment,rt.ForDate,rt.OpenTill,rt.YearOfStudy,rt.TeamId)as tbl ";
                string part6 = "left join [dbo].[PerformQuestionnaire] as pq on pq.TeamId=tbl.TeamId and pq.TaskId=tbl.TaskId ";
                string part7 = " group by pq.TaskId,tbl.taskId,tbl.Title,tbl.SubjectName,tbl.Date_Assignment,tbl.ForDate,tbl.OpenTill,tbl.YearOfStudy";
                String selectSTR = part1 + part2 + part3 + part4 + part5 +part6 +part7;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                             

                while (dr.Read())
                {   // Read till the end of the data into a row
                    Dictionary<string, string> srt = new Dictionary<string, string>();
                    DateTime d=Convert.ToDateTime(dr["ForDate"]);
                    srt.Add("taskId", Convert.ToString(dr["taskId"]));
                    srt.Add("Title", Convert.ToString(dr["Title"]));
                    srt.Add("Date_Assignment", Convert.ToString(dr["Date_Assignment"]));
                    srt.Add("ForDate", Convert.ToString(dr["ForDate"]));
                    srt.Add("OpenTill", Convert.ToString(dr["OpenTill"]));
                    srt.Add("numWaiting", Convert.ToString(dr["numWaiting"]));
                    srt.Add("YearOfStudy", Convert.ToString(dr["YearOfStudy"]));


                    list.Add(srt);

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

        public List<Dictionary<string,string>> getSQuest(string data)
        {
            SqlConnection con = null;
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            string[] data1;
            data1 = data.Split(',');
            string taskId = data1[0];
            string teamId = data1[1];

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file



                string select1 = "select st.StudentEmail,u.IdNumber,u.Fname,u.Lname,pq.QuestionnaireId,pq.Ptime,pq.Grade,pq.Note,q.IntelligenceName,Case when pq.isWaiting is Null Then 0 Else pq.isWaiting END AS isWaiting,";
                string select2 = "pq.SubmissionDate,pq.Ptime,Case When q.QuestionnaireId is null Then 0 Else 1 END AS isperform";
                string from = " from(";
                string select3 = "select * ";
                string from1 = " from StudentInTeam sit ";
                string from2 = "where sit.TeamId='" + teamId + "')as st left join [dbo].[PerformQuestionnaire] as pq";
                string from3 = " on pq.[StudentId] = st.StudentEmail and pq.TaskId = '" + taskId + "' and pq.TeamId='" + teamId + "'";
                string from4 = "left join[User] as u on u.Email = st.StudentEmail";
                string from5 = " left join Questionnaire as q on q.QuestionnaireId = pq.QuestionnaireId";
                string from6 = " left join[dbo].[Intelligence] as i on i.[EnglishName]=q.IntelligenceName";

                String selectSTR = select1 + select2 + from + select3 + from1 + from2 + from3 + from4 + from5 + from6;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    Dictionary<string,string> srt = new Dictionary<string,string>();
                    srt.Add("Mail",Convert.ToString(dr["StudentEmail"]));
                    srt.Add("IdNumber", Convert.ToString(dr["IdNumber"]));
                    srt.Add("QuizID",Convert.ToString(dr["QuestionnaireId"]));
                    srt.Add("Fname", Convert.ToString(dr["Fname"]));
                    srt.Add("Lname",Convert.ToString(dr["Lname"]));
                    srt.Add("IntelligenceName", dr["IntelligenceName"].ToString());
                    srt.Add("isperform", Convert.ToString(dr["isperform"]));
                    srt.Add("isWaiting", Convert.ToInt32(dr["isWaiting"]).ToString());
                    if (dr["Grade"] != DBNull.Value)
                    {
                        srt.Add("Grade",Convert.ToString(dr["Grade"]));
                    }
                    else
                    {
                        srt.Add("Grade","0");
                    }
                    if (dr["Note"] != DBNull.Value)
                    {
                        srt.Add("Note", Convert.ToString(dr["Note"]));
                    }
                    else
                    {
                        srt.Add("Note","");
                    }

                    list.Add(srt);

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

        public Quiz getQ(Dictionary<string, string> d)
        {
            SqlConnection con = null;
            Quiz q = new Quiz();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
                String select1 = "   select t.Title,t.TaskId,t.SubjectName,q.IntelligenceName,q.QuestionnaireId ,quest.QuestionId,quest.Content,quest.[Type],quest.OrderNum,quest.ImgLink,quest.VideoLink, ";
                String select2 = "    a.Content AS ans_content,a.IsRight,a.AnswerId ,ac.[AnswerId] as picked,ao.[FileLink],ao.[Answer],pq.[Note],pq.Ptime,pq.Grade ";
                String select3 = "  from PerformQuestionnaire as pq INNER JOIN Questionnaire as q on q.TaskId=pq.TaskId inner join Question as quest on quest.QuestionnaireId=q.QuestionnaireId  ";
                String select4 = " left join Answer AS a on a.QuestionId=quest.QuestionId left join AnsOpen as ao on  quest.QuestionId=ao.QuestionId and ao.StudentEmail='" + d["Mail"] + "'";
                String select5 = "  left join AnsClose as ac on ac.StudentEmail='" + d["Mail"] + "' and a.AnswerId=ac.AnswerId inner join Task as t on t.TaskId=pq.TaskId ";
                String select6 = " where pq.TaskId='" + d["TaskId"] + "' and q.QuestionnaireId='" + d["QuizID"] + "' and pq.StudentId='" + d["Mail"]+"' and pq.TeamId='"+d["TeamId"]+"'";
                String selectSTR = select1 + select2 + select3 + select4 + select5 + select6;



                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
               
                Question que = new Question();
                while (dr.Read())
                {   // Read till the end of the data into a row


                    
                    if (dr["QuestionnaireId"].ToString() != q.QuizID)
                    {
  
                        q = new Quiz();
                        q.Question = new List<Question>();
                        q.QuizID = dr["QuestionnaireId"].ToString();
                        q.TaskId = dr["TaskId"].ToString();
                        q.Title = dr["Title"].ToString();
                        q.Inteligence = new Inteligence(0, dr["IntelligenceName"].ToString(), dr["IntelligenceName"].ToString(),0);

                    }

                    if (dr["QuestionId"].ToString() != que.QuestionId)
                    {

                        que = new Question(dr["Type"].ToString(), dr["Content"].ToString(), new List<Answer>(), dr["ImgLink"].ToString(), dr["VideoLink"].ToString(), Convert.ToInt32(dr["OrderNum"]), dr["QuestionId"].ToString());
                        que.Answer = new List<Answer>();
                        q.Question.Add(que);
                    }

                    if (dr["ans_content"] != DBNull.Value)
                    {
                        que.Answer.Add(new Answer(dr["ans_content"].ToString(), Convert.ToBoolean(dr["IsRight"]), Convert.ToInt32(dr["AnswerId"])));

                        if (dr["picked"] != DBNull.Value)
                        {
                            que.Answer[que.Answer.Count - 1].IsPicked = true;
                        }
                        else
                        {
                            que.Answer[que.Answer.Count - 1].IsPicked = false;
                        }
                    }
                    else if (dr["Answer"] != DBNull.Value || dr["FileLink"] != DBNull.Value)
                    {
                        if (dr["FileLink"] != DBNull.Value)
                        {
                            que.AnsContent = dr["FileLink"].ToString();
                        }
                        else
                        {
                            que.AnsContent = dr["Answer"].ToString();
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


            return q;
        }

        public int updateQFB(Dictionary<string,string> qfb)
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

            sb.AppendFormat("UPDATE [dbo].[PerformQuestionnaire] SET  [Grade]='{0}', [Note]='{1}',[isWaiting]={4} where [StudentId]='{2}' and [QuestionnaireId]='{3}'", qfb["grade"],qfb["text"],qfb["mail"],qfb["qid"],0);
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

        public List<Dictionary<string,string>> getStTasksInTeam(Dictionary<string,string>info)
        {
            SqlConnection con = null;
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                string select1 = "select rt.Date_Assignment,rt.ForDate,rt.OpenTill,rt.[YearOfStudy],t.title,pq.TaskId,pq.Ptime,";
                string select2 = "u.Email,u.IdNumber,u.Fname,u.Lname,pq.QuestionnaireId,pq.Ptime,pq.Grade,pq.Note,q.IntelligenceName,";
                string from = "i.[ImgLink],Case When q.QuestionnaireId is null Then 0 Else 1 END AS isperform ";
                string select3 = "from(";
                string from1 = "select*";
                string from2 = " from [dbo].[RealatedTo] as rt where rt.TeamId='"+info["TeamId"]+"')as rt left join ";
                string from3 = " [dbo].[PerformQuestionnaire] as pq on pq.[StudentId]='" + info["Mail"] + "' and rt.TaskId=pq.TaskId and pq.TeamId='" + info["TeamId"] + "' ";
                string from4 = " left join [User] as u on u.Email='" + info["Mail"] + "' left join Questionnaire as q on q.QuestionnaireId=pq.QuestionnaireId ";
                string from5 = " left join [dbo].[Intelligence] as i on i.[EnglishName]=q.IntelligenceName inner join Task as t on t.TaskId=rt.TaskId";
              

                String selectSTR = select1 + select2 + from + select3 + from1 + from2 + from3 + from4 + from5;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    Dictionary<string, string> srt = new Dictionary<string, string>();
                    srt.Add("Mail", Convert.ToString(dr["Email"]));
                    srt.Add("IdNumber", Convert.ToString(dr["IdNumber"]));
                    srt.Add("QuizID", Convert.ToString(dr["QuestionnaireId"]));
                    srt.Add("Fname", Convert.ToString(dr["Fname"]));
                    srt.Add("Lname", Convert.ToString(dr["Lname"]));
                    srt.Add("IntelligenceName", dr["IntelligenceName"].ToString());
                    srt.Add("isperform", Convert.ToString(dr["isperform"]));
                    srt.Add("title", Convert.ToString(dr["title"]));
                    srt.Add("TaskId", Convert.ToString(dr["TaskId"]));
                    if (dr["Grade"] != DBNull.Value)
                    {
                        srt.Add("Grade", Convert.ToString(dr["Grade"]));
                    }
                    else
                    {
                        srt.Add("Grade", "0");
                    }
                    if (dr["Note"] != DBNull.Value)
                    {
                        srt.Add("Note", Convert.ToString(dr["Note"]));
                    }
                    else
                    {
                        srt.Add("Note", "");
                    }
                    if (dr["Ptime"] != DBNull.Value)
                    {
                        srt.Add("Ptime", Convert.ToString(dr["Ptime"]));
                    }
                    else
                    {
                        srt.Add("Ptime", "");
                    }

                    list.Add(srt);

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

        public List<Dictionary<string,string>> graphDataTeam(string teamId)
        {
            SqlConnection con = null;
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                string select1 = " select sit.StudentEmail,u.Fname,u.Lname,u.IdNumber,rt.TaskId,t.Title,q.QuestionnaireId,q.IntelligenceName,pq.Grade, ";
                string select2 = " Case When q.QuestionnaireId is null Then 0 Else 1 END AS isperform from RealatedTo as rt left join StudentInTeam as sit  ";
                string from = " on sit.TeamId=rt.TeamId inner join [User] as u on u.Email=sit.StudentEmail left join PerformQuestionnaire as pq on pq.StudentId=sit.StudentEmail ";
                string select3 = " and pq.TeamId=rt.TeamId and pq.TaskId=rt.TaskId left join Questionnaire as q on q.QuestionnaireId=pq.QuestionnaireId ";
                string from1 = " inner join Task as T on t.TaskId=rt.TaskId where rt.TeamId='"+teamId+"'";
     

                String selectSTR = select1 + select2 + from + select3 + from1;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    Dictionary<string, string> srt = new Dictionary<string, string>();
                    srt.Add("Mail", Convert.ToString(dr["StudentEmail"]));
                    srt.Add("IdNumber", Convert.ToString(dr["IdNumber"]));
                    srt.Add("Fname", Convert.ToString(dr["Fname"]));
                    srt.Add("Lname", Convert.ToString(dr["Lname"]));
                    srt.Add("QuizID", Convert.ToString(dr["QuestionnaireId"]));
                    srt.Add("IntelligenceName", dr["IntelligenceName"].ToString());
                    srt.Add("isperform", Convert.ToString(dr["isperform"]));
                    srt.Add("TaskId", Convert.ToString(dr["TaskId"]));
                    srt.Add("TaskTitle", Convert.ToString(dr["Title"]));
                    if (dr["Grade"] != DBNull.Value)
                    {
                        srt.Add("Grade", Convert.ToString(dr["Grade"]));
                    }
                    else
                    {
                        srt.Add("Grade", "0");
                    }
                   

                    list.Add(srt);

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

        public List<Dictionary<string, string>> graphDataClass(Dictionary<string,string> info)
        {
            SqlConnection con = null;
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                string select1 = " select sit.StudentEmail,u.Fname,u.Lname,u.IdNumber,rt.TaskId,t.Title,t.SubjectName,q.QuestionnaireId,q.IntelligenceName,pq.Grade, ";
                string select2 = " Case When q.QuestionnaireId is null Then 0 Else 1 END AS isperform,s.ClassNumber,S.GradeClass,S.ClassSchoolCode,sit.TeamId  ";
                string from = " from RealatedTo as rt left join StudentInTeam as sit on sit.TeamId=rt.TeamId inner join [User] as u on u.Email=sit.StudentEmail inner join Student as s ";
                string select3 = " on s.StudentEmail=u.Email left join PerformQuestionnaire as pq on pq.StudentId=sit.StudentEmail and pq.TeamId=rt.TeamId and pq.TaskId=rt.TaskId ";
                string from1 = " left join Questionnaire as q on q.QuestionnaireId=pq.QuestionnaireId inner join Task as T on t.TaskId=rt.TaskId where s.ClassNumber="+info["ClassNumber"]+" and s.GradeClass="+info["GradeClass"]+" and s.ClassSchoolCode='"+info["SchoolCode"]+"'";



                String selectSTR = select1 + select2 + from + select3 + from1;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    Dictionary<string, string> srt = new Dictionary<string, string>();
                    srt.Add("Mail", Convert.ToString(dr["StudentEmail"]));
                    srt.Add("IdNumber", Convert.ToString(dr["IdNumber"]));
                    srt.Add("Fname", Convert.ToString(dr["Fname"]));
                    srt.Add("Lname", Convert.ToString(dr["Lname"]));
                    srt.Add("QuizID", Convert.ToString(dr["QuestionnaireId"]));
                    srt.Add("IntelligenceName", dr["IntelligenceName"].ToString());
                    srt.Add("isperform", Convert.ToString(dr["isperform"]));
                    srt.Add("TaskId", Convert.ToString(dr["TaskId"]));
                    srt.Add("TaskTitle", Convert.ToString(dr["Title"]));
                    srt.Add("ClassNumber", Convert.ToString(dr["ClassNumber"]));
                    srt.Add("GradeClass", Convert.ToString(dr["GradeClass"]));
                    srt.Add("ClassSchoolCode", Convert.ToString(dr["ClassSchoolCode"]));
                    srt.Add("TeamId", Convert.ToString(dr["TeamId"]));
                    srt.Add("SubjectName", Convert.ToString(dr["SubjectName"]));
                    if (dr["Grade"] != DBNull.Value)
                    {
                        srt.Add("Grade", Convert.ToString(dr["Grade"]));
                    }
                    else
                    {
                        srt.Add("Grade", "0");
                    }


                    list.Add(srt);

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

        public int updateAssigment(Dictionary<string,string> info)
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

            sb.AppendFormat("Update [RealatedTo] SET [ForDate]='{0}',[OpenTill]='{1}' where [TaskId]='{2}' and [TeamSchoolCode]={3} and [TeamId]='{4}'", info["classTime"], info["submitionTime"], info["TaskId"], info["TeamSchoolCode"], info["TeamId"]);
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

        public int updatePtime(Dictionary<string, string> info)
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

            sb.AppendFormat("Update [PerformQuestionnaire] SET [Ptime]='{0}' where [TaskId]='{1}' and [StudentId]='{2}' and [TeamId]='{3}'", info["Ptime"],  info["TaskId"], info["Mail"], info["TeamId"]);
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

        public int deleteAssigment(Dictionary<string, string> info)
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

            sb.AppendFormat("Delete From [RealatedTo]  where [TaskId]='{0}' and [TeamSchoolCode]={1} and [TeamId]='{2}'", info["TaskId"], info["TeamSchoolCode"], info["TeamId"]);
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

        public List<Dictionary<string,string>> intTeamGraph(string teamId)
        {
            SqlConnection con = null;
            List<Dictionary<string,string>> list = new List<Dictionary<string, string>>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                string select1 = " select s.StudentEmail,pio.IntelligenceName,pio.points,pio.Spoints from Student as s  ";
                string select2 = " inner join StudentInTeam as sit on sit.StudentEmail=s.StudentEmail   ";
                string select3 = " left join PointsInIntelligence as pio on pio.StudentEmail=s.StudentEmail where sit.TeamId='"+teamId+"'";
         
                String selectSTR = select1+select2 +select3;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                Dictionary<string, string> srt = new Dictionary<string, string>();
                srt["Mail"] = "";
                while (dr.Read())
                {   // Read till the end of the data into a row
                    if(srt["Mail"]!= Convert.ToString(dr["StudentEmail"]))
                    {
                        if(srt["Mail"] !="")
                          list.Add(srt);
                        srt = new Dictionary<string, string>();
                        srt.Add("Mail", Convert.ToString(dr["StudentEmail"]));
                    }

                    if (dr["IntelligenceName"] != DBNull.Value)
                    {
                        string Spoints = dr["Spoints"] != DBNull.Value ? Convert.ToString(dr["Spoints"]):"0";
                        srt.Add(Convert.ToString(dr["IntelligenceName"]) + "points", Convert.ToString(dr["points"]));
                        srt.Add(Convert.ToString(dr["IntelligenceName"]) + "Spoints", Spoints);
                    }
                    else
                    {
                        srt.Add("points","none");
                        srt.Add("Spoints","none");
                    }
   
                   

                }
                list.Add(srt);

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

        public List<Dictionary<string, string>> intlClassGraph(Dictionary<string,string>info)
        {
            SqlConnection con = null;
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                string select1 = " select s.StudentEmail,pio.IntelligenceName,pio.points,pio.Spoints  ";
                string select2 = " from Student as s left join PointsInIntelligence as pio on pio.StudentEmail=s.StudentEmail   ";
                string select3 = " where s.ClassNumber="+info["ClassNumber"] +" and s.GradeClass="+info["GradeClass"] +" and s.ClassSchoolCode="+info["SchoolCode"];

                String selectSTR = select1 + select2 + select3;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                Dictionary<string, string> srt = new Dictionary<string, string>();
                srt["Mail"] = "";
                while (dr.Read())
                {   // Read till the end of the data into a row
                    if (srt["Mail"] != Convert.ToString(dr["StudentEmail"]))
                    {
                        if (srt["Mail"] != "")
                            list.Add(srt);
                        srt = new Dictionary<string, string>();
                        srt.Add("Mail", Convert.ToString(dr["StudentEmail"]));
                    }

                    if (dr["IntelligenceName"] != DBNull.Value)
                    {
                        string Spoints = dr["Spoints"] != DBNull.Value ? Convert.ToString(dr["Spoints"]) : "0";
                        srt.Add(Convert.ToString(dr["IntelligenceName"]) + "points", Convert.ToString(dr["points"]));
                        srt.Add(Convert.ToString(dr["IntelligenceName"]) + "Spoints", Spoints);
                    }
                    else
                    {
                        srt.Add("points", "none");
                        srt.Add("Spoints", "none");
                    }



                }
                list.Add(srt);

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

        public int changeClassName(Classroom c)
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
            sb.AppendFormat("Update [Class] SET [Title]='{0}' where [SchoolCode]={1} and [Grade]={2} and [GradeNumber]={3}",c.Name,c.InSchool,c.Grade,c.GradeNumber);
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
    }
}