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

            return numEffected; 


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

        public List<RealetedTask>getSTasks(string userEmail,string teamId)
        {
            

            SqlConnection con = null;

            List<RealetedTask> rtList = new List<RealetedTask>();
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String part1 = " select  tbl.taskId,tbl.Title,tbl.SubjectName,tbl.Date_Assignment,tbl.ForDate,tbl.OpenTill,tbl.YearOfStudy,tbl.IntelligenceName,tbl.[Name],tbl.creationTime,tbl.QuestionnaireId";
                String part2 = " ,pio.points,pio.Spoints,quest.QuestionId,quest.Content,quest.[Type],quest.OrderNum,quest.ImgLink,quest.VideoLink,a.Content AS ans_content,a.IsRight,a.AnswerId ,ac.[AnswerId] as picked,ao.[FileLink],ao.[Answer],pq.[Note],pq.Ptime,pq.Grade, ";
                String part3 = " case when pq.[StudentId]='"+userEmail+"' then 1 else 0 END as IsChoose ";
                String part4 = "  from (select t.taskId,t.Title,t.SubjectName,rt.Date_Assignment,rt.ForDate,rt.OpenTill,rt.YearOfStudy,q.IntelligenceName,i.[Name],max(q.creationTime)as creationTime,max(q.QuestionnaireId) as QuestionnaireId ";
                String part5 = " from task as t inner join RealatedTo rt on rt.TaskId=t.TaskId and rt.TeamId='"+teamId+"' inner join Questionnaire as q on q.TaskId=rt.TaskId and rt.Date_Assignment>q.creationTime ";
                String part6 = "  inner join  Intelligence as i on q.IntelligenceName=i.[EnglishName] group by t.taskId,t.Title,t.SubjectName,q.IntelligenceName,i.[Name],rt.Date_Assignment,rt.ForDate,rt.OpenTill,rt.YearOfStudy)as tbl  ";
                String part7 = "  inner join Question as quest on quest.QuestionnaireId=tbl.QuestionnaireId inner join PointsInIntelligence as pio on pio.IntelligenceName=tbl.IntelligenceName and pio.StudentEmail='"+userEmail+"' ";
                String part8 = "  left join Answer as a on a.QuestionId=quest.QuestionId left join [dbo].[PerformQuestionnaire] as pq on pq.[QuestionnaireId]=tbl.[QuestionnaireId] and pq.[StudentId]='" + userEmail + "' and  pq.TeamId='"+teamId+"' ";
                String part9 = " left join [dbo].[AnsClose] as ac on ac.[AnswerId]=a.[AnswerId] and ac.StudentEmail='" + userEmail + "' left join [dbo].[AnsOpen] as ao on ao.[QuestionId]=quest.[QuestionId] and ao.StudentEmail='" + userEmail + "' order by tbl.ForDate,tbl.TaskId,tbl.IntelligenceName,points ";
                String selectSTR = part1+part2+part3+part4+part5+part6+part7+part8+part9;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                RealetedTask rt = new RealetedTask();
                rt.Task = new Task();
                rt.Task.QuizList = new List<Quiz>();
                Quiz q = new Quiz();
                Question que = new Question();
                while (dr.Read())
                {   // Read till the end of the data into a row

                    if (dr["TaskId"].ToString() != rt.Task.TaskId)
                    {
                        if (rt.Task.TaskId != null)
                        {
                            rt.Task.QuizList.Add(q);
                            rtList.Add(rt);
                        }
                        rt = new RealetedTask();
                        q = new Quiz();
                        rt.Task = new Task();
                        rt.Task.QuizList = new List<Quiz>();
                        rt.Task.Grade = Convert.ToInt32(dr["IsChoose"]);
                        rt.Task.TaskId = dr["TaskId"].ToString();
                        if (dr["Grade"] != DBNull.Value)
                        rt.Score= Convert.ToInt32(dr["Grade"]);
                        rt.Task.Title = dr["Title"].ToString();
                        rt.Task.Grade =0;
                        if(dr["Note"]!=DBNull.Value)
                        rt.Note = dr["Note"].ToString();
                        rt.Task.Sub = new Subject(dr["SubjectName"].ToString(), "");
                        rt.YearOfStudy = dr["YearOfStudy"].ToString();
                        rt.ForDate = Convert.ToDateTime(dr["ForDate"]);
                        rt.TillDate = Convert.ToDateTime(dr["OpenTill"]);
                        rt.AssigmentDate = Convert.ToDateTime(dr["Date_Assignment"]);
                    }
                    if (dr["QuestionnaireId"].ToString() != q.QuizID)
                    {
                        if (q.QuizID != null )
                        {
                            rt.Task.QuizList.Add(q);
                        }
                          
                        q = new Quiz();
                        q.Question = new List<Question>();
                        q.QuizID = dr["QuestionnaireId"].ToString();
                        q.TaskId = dr["IsChoose"].ToString();
                        if (q.TaskId == "1") {
                            if (dr["Ptime"] != DBNull.Value)
                                rt.STime = Convert.ToDateTime(dr["Ptime"]);
                            rt.Task.Grade =1;
                        }
                        q.Inteligence=new Inteligence(Convert.ToInt32(dr["points"]),dr["Name"].ToString(),dr["IntelligenceName"].ToString(),0);
                        if (dr["Spoints"] != DBNull.Value)
                            q.Inteligence.Spoints = Convert.ToInt32(dr["Spoints"]);
                    }
   
                    if (dr["QuestionId"].ToString() != que.QuestionId)
                    {

                        que = new Question(dr["Type"].ToString(), dr["Content"].ToString(), new List<Answer>(), dr["ImgLink"].ToString(), dr["VideoLink"].ToString(), Convert.ToInt32(dr["OrderNum"]), dr["QuestionId"].ToString());
                        que.Answer = new List<Answer>();
                        q.Question.Add(que);
                    }

                    if (dr["ans_content"] != DBNull.Value)
                    {
                        que.Answer.Add(new Answer(dr["ans_content"].ToString(), Convert.ToBoolean(dr["IsRight"]),Convert.ToInt32(dr["AnswerId"])));

                        if (dr["picked"] != DBNull.Value)
                        {
                            que.Answer[que.Answer.Count - 1].IsPicked = true;
                        }
                        else
                        {
                            que.Answer[que.Answer.Count - 1].IsPicked = false;
                        }
                    }else if (dr["Answer"] != DBNull.Value|| dr["FileLink"] != DBNull.Value)
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
                if (rt.Task.TaskId != null)
                {
                    rt.Task.QuizList.Add(q);
                    rtList.Add(rt);
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

        public List<Team> getSTeams(string mail)
        {
            List<Team> tList = new List<Team>();
            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
                String sl1 = " select t.TeamId,t.Title,t.SubjectName,s.ImgLink,t.TeacherEmail,rt.TaskId, case when pq.StudentId IS NULL and rt.TaskId is not NULL then 1 else 0 end as isPerform  ";
                String sl2 = " from [dbo].[StudentInTeam] as sit inner join Team as t on t.TeamId=sit.TeamId and sit.SchoolCode=t.SchoolCode and  sit.StudentEmail='"+mail+ "' inner join Subject as s on s.[Name]=t.SubjectName left  join [dbo].[RealatedTo] as rt ";
                String sl3 = " on rt.TeamId=t.TeamId left join [dbo].[PerformQuestionnaire] as pq on pq.TaskId=rt.TaskId and pq.StudentId='"+mail+"'";
                String selectSTR = sl1+sl2+sl3;

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                Team t = new Team();
                while (dr.Read())
                {   // Read till the end of the data into a row
                    if(t.Id != dr["TeamId"].ToString())
                    {
                        if (t.Id != null)
                        {
                            tList.Add(t);
                        }
                        t = new Team();
                        t.Subject = new Subject();
                        t.Teacher = new Teacher();
                        t.Id = dr["TeamId"].ToString();
                        t.Title = dr["Title"].ToString();
                        t.Subject.Name = dr["SubjectName"].ToString();
                        t.Subject.ImgLink = dr["ImgLink"].ToString();
                        t.Teacher.Mail = dr["TeacherEmail"].ToString();
                        t.Scode = 0;

                    }
                   

                    t.Scode += Convert.ToInt32(dr["isPerform"]);


                }
                tList.Add(t);

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



            return tList;
        }

        public int postQ(RealetedTask rt)
        {
            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;
            String cStr="";
            numEffected+=insertperformQuestionnaire(rt);
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }

            foreach(var item in rt.Task.QuizList[0].Question)
            {
                if (item.Type == "A" || item.Type == "M")
                {
                    foreach(var a in item.Answer)
                    {
                        if (a.IsPicked)
                        {
                            cStr = BuildAnsCInsertCommand(rt.Task.QuizList[0],item,a,rt.YearOfStudy,rt.StPerformer);

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
                    }
                    
                }
                else
                {
                    cStr = BuildAnsOInsertCommand(rt.Task.QuizList[0],item, rt.YearOfStudy,rt.StPerformer);

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

                
              
            }
            if (con != null)
            {
                // close the db connection
                con.Close();
            }


            return numEffected;
            
        }

        public int insertperformQuestionnaire(RealetedTask rt)
        {

            int numEffected = 0;
            SqlConnection con;
            SqlCommand cmd;
            String cStr = "";
            string sqlFormattedDate = rt.STime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }


            cStr = BuildPQInsertCommand(rt.Task.QuizList[0],rt.StPerformer,rt.YearOfStudy, sqlFormattedDate);
            cmd = CreateCommand(cStr, con);    // create the command

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
        private String BuildPQInsertCommand(Quiz q,Student t,string tId,string sqlFormattedDate)
        {
            String command;


            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}','{1}','{2}',{3},{4},'{5}','{6}','{7}',{8})", t.Mail, q.TaskId, q.QuizID,"GetDate()",0,t.SCode,tId, sqlFormattedDate,1);

            String prefix = "INSERT INTO PerformQuestionnaire" + "(StudentId,TaskId,QuestionnaireId,SubmissionDate,Grade,TeamSchoolCode,TeamId,Ptime,isWaiting)";
            command = prefix + sb.ToString();

            return command;
        }
        private String BuildAnsCInsertCommand(Quiz q,Question qu,Answer a,string tId,Student s)
        {
            String command;


            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}','{1}','{2}','{3}',{4},'{5}')",s.Mail,q.TaskId,q.QuizID,qu.QuestionId,a.AnsId,tId);

            String prefix = "INSERT INTO AnsClose" + "(StudentEmail,TaskId,QuestionnaireId,QuestionId,AnswerId,TeamId)";
            command = prefix + sb.ToString();

            return command;
        }
        private String BuildAnsOInsertCommand(Quiz q,Question qu,string tId,Student s)
        {
            String command;


            StringBuilder sb = new StringBuilder();
            String prefix = "";
            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}','{1}','{2}','{3}','{4}','{5}')", s.Mail,q.TaskId,q.QuizID,qu.QuestionId,qu.AnsContent,tId);
            if (qu.Type == "O")
            {
                prefix = "INSERT INTO AnsOpen" + "(StudentEmail,TaskId,QuestionnaireId,QuestionId,Answer,TeamId)";
            }
            else
            {
                prefix = "INSERT INTO AnsOpen" + "(StudentEmail,TaskId,QuestionnaireId,QuestionId,Answer,TeamId)";
            }
            
            command = prefix + sb.ToString();

            return command;
        }

        public StudentDBServices updateQansO(RealetedTask rt)
        {
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString");
                da = new SqlDataAdapter("select * from AnsOpen AS ac  where ac.TaskId='"+ rt.Task.QuizList[0].TaskId+"' and ac.StudentEmail='"+ rt.StPerformer.Mail+"' and ac.QuestionnaireId='"+ rt.Task.QuizList[0].QuizID+"' and ac.TeamId='"+rt.YearOfStudy+"'", con);
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

        public StudentDBServices updateQansC(RealetedTask rt)
        {
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString");
                da = new SqlDataAdapter("select * from AnsClose AS ac  where ac.TaskId='" + rt.Task.QuizList[0].TaskId + "' and ac.StudentEmail='" + rt.StPerformer.Mail + "' and ac.QuestionnaireId='" + rt.Task.QuizList[0].QuizID + "' and ac.TeamId='"+ rt.YearOfStudy + "'", con);
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

        public int validateTime(RealetedTask rt)
        {
            SqlConnection con = null;
            List<Inteligence> list = new List<Inteligence>();
            int num = 0;

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                int qnum = getNum(rt);
                String selectSTR = "select* from [dbo].[PerformQuestionnaire] as pq inner join RealatedTo as rt on pq.TaskId=rt.TaskId and rt.TaskId='"+rt.Task.TaskId+"' and pq.StudentId='"+rt.Task.Title+"' and pq.[QuestionnaireId]='"+rt.Task.QuizList[qnum].QuizID+"' where (getDate()>=rt.ForDate) and (getDate()<rt.[OpenTill] or getDate()<pq.Ptime)";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.HasRows)
                {
                    if (con != null)
                    {
                        con.Close();
                    }
                    return 1;
                }
                else
                {
                    con.Close();
                    dr = null;
                    cmd.Connection = null;
                    con = connect("DBConnectionString");

                    String selectSTR2 = "select* from  RealatedTo as rt where (getDate()>=rt.ForDate) and (getDate()<rt.[OpenTill]) and rt.TaskId='" + rt.Task.TaskId + "' ";
                    SqlCommand cmd2 = new SqlCommand(selectSTR2, con);
                    SqlDataReader dr2 = cmd2.ExecuteReader();

                    if (dr2.HasRows)
                    {
                        if (con != null)
                        {
                            con.Close();
                        }
                        return 1;
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

            return num;
        }

        private int getNum(RealetedTask arg)
        {
            int i= -1;
            int num=0;
            foreach(var a in arg.Task.QuizList)
            {
                i++;
                if (a.TaskId == "1")
                {

                    num=i;
                    break;
                }
            }

            return num;
        }

        public void deletePQ(Quiz del,RealetedTask rt)
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

            sb.AppendFormat("Delete from PerformQuestionnaire where  [QuestionnaireId]='{0}' and [TaskId]='{1}'  and [StudentId]='{2}' and [TeamId]='{3}'",del.QuizID,rt.Task.TaskId,rt.StPerformer.Mail,rt.YearOfStudy); ;
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
        }

        public StudentDBServices getQ(string mail)
        {
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString");
                da = new SqlDataAdapter("select pq.StudentId,q.IntelligenceName,pq.Grade from [dbo].[PerformQuestionnaire] as pq inner join  Questionnaire as q on q.QuestionnaireId=pq.QuestionnaireId where pq.StudentId='"+mail+"' and datediff(MONTH,GETDate(),pq.SubmissionDate)<=3", con);
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
        public StudentDBServices getPIN(string mail)
        {
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString");
                da = new SqlDataAdapter("select* from PointsInIntelligence as pi  where pi.StudentEmail='"+mail+"'", con);
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

        public int updateisWaitingPQ(RealetedTask rt) {

           
            int numEffected = 0;
           
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file


                String UpdateSIquiz = "UPDATE PerformQuestionnaire  Set isWaiting=1  where TaskId='" + rt.Task.QuizList[0].TaskId + "' and StudentId='" + rt.StPerformer.Mail + "' and QuestionnaireId='" + rt.Task.QuizList[0].QuizID + "' and TeamId='" + rt.YearOfStudy + "'";



                SqlCommand cmd = new SqlCommand(UpdateSIquiz, con);
                numEffected += cmd.ExecuteNonQuery();

   
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

        public void update()
        {
            da.Update(dt);
        }
    }
}


 