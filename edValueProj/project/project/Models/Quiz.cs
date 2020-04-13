using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using project.Models.DAL;

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
    }
}