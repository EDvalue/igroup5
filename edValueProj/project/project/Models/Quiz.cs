using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using project.Models.DAL;
using System.Data;

namespace project.Models
{
    public class Quiz
    {
        string taskId;
        string quizID;
        string title;
        string sub;
        Inteligence inteligence;
        List<Question> question;
        List<FeedBack> fbList;
        

        public Quiz() { }

        public Quiz(string taskId, string quizID, string title, string sub, Inteligence inteligence, List<Question> question, List<FeedBack> fbList)
        {
            this.taskId = taskId;
            this.quizID = quizID;
            this.title = title;
            this.sub = sub;
            this.inteligence = inteligence;
            this.question = question;
            this.fbList = fbList;
        }

        public string TaskId { get => taskId; set => taskId = value; }
        public string QuizID { get => quizID; set => quizID = value; }
        public string Title { get => title; set => title = value; }
        public string Sub { get => sub; set => sub = value; }
        public Inteligence Inteligence { get => inteligence; set => inteligence = value; }
        public List<Question> Question { get => question; set => question = value; }
        public List<FeedBack> FbList { get => fbList; set => fbList = value; }

        public List<Quiz> getQuestionnaire(string taskNum)
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.getQuestionnaire(taskNum);
        }

        public int PostQuiz()
        {
            
           TeacherDBservices dbs = new TeacherDBservices();
            return dbs.PostQuiz(this);
        }


        public int updateQ(RealetedTask rt)
        {
            StudentDBServices dbs1 = new StudentDBServices();
            StudentDBServices dbs2 = new StudentDBServices();
            dbs1 = dbs1.updateQansC(rt);
            dbs2 = dbs2.updateQansO(rt);
            dbs1.dt = closeQ(rt.Task.QuizList[0], dbs1.dt,rt.YearOfStudy);
            dbs2.dt = openQ(rt.Task.QuizList[0], dbs2.dt, rt.YearOfStudy);

            dbs1.update();
            dbs2.update();

            dbs1.updateisWaitingPQ(rt);
            return 1;
        }

        private DataTable closeQ(Quiz q, DataTable dt,string tId)
        {
            int flag = 0;
            foreach (var item in q.Question)
            {
                if (item.Type == "A" || item.Type == "M")
                {
                    foreach (var ans in item.Answer)
                    {
                        flag = 0;
                        foreach (DataRow dr in dt.Rows)
                        {

                            if (dr.RowState != DataRowState.Deleted && dr.RowState != DataRowState.Added)
                            {
                                string quId = dr["QuestionId"].ToString();
                                int ansId = Convert.ToInt32(dr["AnswerId"]);


                                if (quId == item.QuestionId && ansId == ans.AnsId)
                                {
                                    if (ans.IsPicked == false)
                                    {
                                        flag = 1;
                                        dr.Delete();
                                        break;
                                    }
                                    else
                                    {
                                        flag = 1;
                                        break;
                                    }

                                }
                            }

                        }

                        if (flag!=1)
                        {
                            if (ans.IsPicked == true)
                            {
                                DataRow workRow = dt.NewRow();

                                workRow["StudentEmail"] = q.Title;
                                workRow["QuestionnaireId"] = q.QuizID;
                                workRow["TaskId"] = q.taskId;
                                workRow["AnswerId"] = ans.AnsId;
                                workRow["QuestionId"] = item.QuestionId;
                                workRow["TeamId"] = tId;

                                dt.Rows.Add(workRow);
                            }

                        }


                    }
                }
     
            }
            return dt;
        }

        private DataTable openQ(Quiz q, DataTable dt,string tId)
        {
            foreach (var item in q.Question)
            {
                if (item.Type == "O" || item.Type == "U")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr.RowState != DataRowState.Modified)
                        {
                            string quId = dr["QuestionId"].ToString();
                            if (quId == item.QuestionId)
                            {
                                if (item.Type == "O")
                                {
                                    dr["Answer"] = item.AnsContent;
                                }
                                else if (item.Type == "U")
                                {
                                    dr["FileLink"] = item.Content;
                                }
                            }
                        }
                    }
                }
            }

            return dt;
        }

        public Quiz getQ(Dictionary<string,string>d)
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.getQ(d);
        }
    }
}