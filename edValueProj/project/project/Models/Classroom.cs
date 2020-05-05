using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using project.Models.DAL;

namespace project.Models
{
    public class Classroom
    {
        string name;
        int grade;
        int gradeNumber;
        Teacher edTeacher;
        int inSchool;
        List<Student> slist = new List<Student>();

        public Classroom() { }

        public Classroom(string name, int grade, int gradeNumber, Teacher edTeacher, int inSchool, List<Student> slist)
        {
            this.slist = slist;
            this.name = name;
            this.grade = grade;
            this.gradeNumber = gradeNumber;
            this.edTeacher = edTeacher;
            this.inSchool = inSchool;
        }

        public string Name { get => name; set => name = value; }
        public int Grade { get => grade; set => grade = value; }
        public int GradeNumber { get => gradeNumber; set => gradeNumber = value; }
        public Teacher EdTeacher { get => edTeacher; set => edTeacher = value; }
        public int InSchool { get => inSchool; set => inSchool = value; }
        public List<Student> Slist { get => slist; set => slist = value; }

        public List<Classroom> getClassesbyMail(string mail)
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.getClassesbyMail(mail);
        }


        public List<Classroom> getClassBySchool(int code)
        {
            DBservices dbs = new DBservices();
            return dbs.getClassBySchool(code);
        }

        public int postNewClass()
        {
            DBservices dbs = new DBservices();
            return dbs.postNewClass(this);
        }

        public int updateclass()
        {
            DBservices dbs = new DBservices();
            return dbs.updateclass(this);
        }

        public int openSI(Dictionary<string, string> dict)
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.openSI(dict);
        }

        public List<Dictionary<string, string>> getGraphClassData(Dictionary<string, string> info)
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.graphDataClass(info);
        }
    }
}
            
   
