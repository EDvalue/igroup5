using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace project.Models
{
    public class Question
    {
        string type;
        string content;
        List<Answer> answer;
        string imgLink;
        string videoLink;
        int orderNum;
        string questionId;
        string ansContent;
        

        
        public Question(){}

        public Question(string type, string content, List<Answer> answer, string imgLink, string videoLink, int orderNum, string questionId)
        {
            this.type = type;
            this.content = content;
            this.answer = answer;
            this.imgLink = imgLink;
            this.videoLink = videoLink;
            this.orderNum = orderNum;
            this.questionId = questionId;
        }

        public string Type { get => type; set => type = value; }
        public string Content { get => content; set => content = value; }
        public List<Answer> Answer { get => answer; set => answer = value; }
        public string ImgLink { get => imgLink; set => imgLink = value; }
        public string VideoLink { get => videoLink; set => videoLink = value; }
        public int OrderNum { get => orderNum; set => orderNum = value; }
        public string QuestionId { get => questionId; set => questionId = value; }
        public string AnsContent { get => ansContent; set => ansContent = value; }
    }
}