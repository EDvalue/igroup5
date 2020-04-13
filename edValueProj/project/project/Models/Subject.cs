using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace project.Models
{
    public class Subject
    {
        string name;
        string imgLink;



        public string Name { get => name; set => name = value; }
        public string ImgLink { get => imgLink; set => imgLink = value; }

        public Subject() { }

      

        public Subject(string name, string imgLink)
        {
            this.name = name;
            this.imgLink = imgLink;
        }
    }
}