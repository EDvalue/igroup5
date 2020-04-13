using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using project.Models.DAL;

namespace project.Models
{
    public class FeedBack
    {
        string id;
        string content;
        int score;
        string questionnaireId;
        string taskId;

        public FeedBack() { }

        public FeedBack(string id, string content, int score, string questionnaireId, string taskId)
        {
            this.id = id;
            this.content = content;
            this.score = score;
            this.questionnaireId = questionnaireId;
            this.taskId = taskId;
        }

        public string Id { get => id; set => id = value; }
        public string Content { get => content; set => content = value; }
        public int Score { get => score; set => score = value; }
        public string QuestionnaireId { get => questionnaireId; set => questionnaireId = value; }
        public string TaskId { get => taskId; set => taskId = value; }

        public int postFB()
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.postFB(this);
        }
    }
}