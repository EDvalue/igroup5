using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using project.Models.DAL;

namespace project.Models
{
    public class Student :User
    {

        int grade;
        int classNumber;
        bool isCompleteIquizz;
        Inteligence inteligence;

        public int Grade { get => grade; set => grade = value; }
        public int ClassNumber { get => classNumber; set => classNumber = value; }
        public bool IsCompleteIquizz { get => isCompleteIquizz; set => isCompleteIquizz = value; }
        public Inteligence Inteligence { get => inteligence; set => inteligence = value; }

        public Student(string name, string lastName, string password, string mail,int scode, string idNumber) 
            : base(name, lastName, password, mail,idNumber,scode)
        {

          
        }
     

        public Student():base() { }

        public int postNewStudent(List<Dictionary<string,string>> userDict)
        {
            DBservices dbs = new DBservices();
            return dbs.postNewStudent(userDict);
        }

        public Team getStudentsbyGrade(int grade,int scode,int mode)
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.getStudentsbyGrade(grade,scode,mode);
        }

        public int IntQuiz(Dictionary<string, string> qresults)
        {

            StudentDBServices dbs = new StudentDBServices();
            return dbs.postStudentIntelli(qresults);

        }
        public List<Inteligence> getIntelligence(string mail)
        {
            StudentDBServices dbs = new StudentDBServices();
            return dbs.getStudentIntelli(mail);
        }

    }
}