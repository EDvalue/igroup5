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
                String select2 = " qu.QuestionnaireId,qu.QuestionId,qu.OrderNum,qu.Content,qu.[Type],qu.ImgLink,qu.VideoLink,Count(qu.QuestionId) as num_of_Q,a.Content AS ans_content,a.IsRight,Count(a.AnswerId)as num_of_ans ";
                String from1 = " from(select [TaskId],[IntelligenceName],max([QuestionnaireId])as QuestionnaireId,MAX([creationTime])as creationTime ";
                String from2 = "   from Questionnaire  group by [TaskId],[IntelligenceName])q inner join  Intelligence as i on q.IntelligenceName=i.[EnglishName] inner join Question as qu on qu.QuestionnaireId=q.QuestionnaireId left join Answer as a on a.QuestionId=qu.QuestionId ";
                String where = "where q.TaskId='"+taskNum+"'";
                String groupby = "  group by  q.IntelligenceName,q.creationTime,q.QuestionnaireId,i.EnglishName,qu.QuestionnaireId,qu.QuestionId,qu.OrderNum,qu.Content,qu.[Type],qu.ImgLink,qu.VideoLink,a.Content,a.IsRight ";
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
                        q.Inteligence = new Inteligence(0, dr["IntelligenceName"].ToString(), dr["QEnglishName"].ToString());
                        q.FbList = getFBbyId(q.QuizID);
                    }
                   
                    if(dr["QuestionId"].ToString() != que.QuestionId)
                    {
                        
                        que =new Question(dr["Type"].ToString(), dr["Content"].ToString(), new List<Answer>(), dr["ImgLink"].ToString(), dr["VideoLink"].ToString(), Convert.ToInt32(dr["OrderNum"]), dr["QuestionId"].ToString());
                        q.Question.Add(que);
                    }

                    if(dr["ans_content"] != DBNull.Value)
                    que.Answer.Add(new Answer(dr["ans_content"].ToString(),Convert.ToBoolean(dr["IsRight"])));
    

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
                String part2 = "from Team as t inner join [Subject] as s on s.[Name]=t.SubjectName  inner join StudentInTeam as sit on sit.TeamId=t.TeamId inner join[User] on [User].Email=sit.StudentEmail where TeacherEmail='"+mail+ "' order by TeamId";
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

                    Student s = new Student();
                    s.Name = dr["Fname"].ToString();
                    s.LastName = dr["Lname"].ToString();
                    s.IdNumber = dr["IdNumber"].ToString();
                    s.Mail = dr["StudentEmail"].ToString();
                    t.StudentList.Add(s);

                    

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

        public void update()
        {
            da.Update(dt);
            
        }
    }
}