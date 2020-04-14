using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace project.Models
{
    public class Inteligence
    {
        int points;
        string name;
        string ename;
        

        
        public string Name { get => name; set => name = value; }
        public int Points { get => points; set => points = value; }
        public string Ename { get => ename; set => ename = value; }

      

       public Inteligence()
        {

        }

        public Inteligence(int points, string name, string ename)
        {
            this.points = points;
            this.name = name;
            this.ename = ename;
        }
    }
}