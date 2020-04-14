using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace project.Models
{
    public class Answer
    {
        string content;
        bool isRight;


        public Answer() { }
        public Answer(string content, bool isRight)
        {
            this.content = content;
            this.isRight = isRight;
        }

        public string Content { get => content; set => content = value; }
        public bool IsRight { get => isRight; set => isRight = value; }
    }
}