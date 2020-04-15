using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace project.Models
{
    public class Answer
    {
        int ansId;
        string content;
        bool isRight;
        bool isPicked;


        public Answer() { }
        public Answer(string content, bool isRight,int ansId)
        {
            this.content = content;
            this.isRight = isRight;
            this.ansId = ansId;
        }

        public string Content { get => content; set => content = value; }
        public bool IsRight { get => isRight; set => isRight = value; }
        public int AnsId { get => ansId; set => ansId = value; }
        public bool IsPicked { get => isPicked; set => isPicked = value; }
    }
}