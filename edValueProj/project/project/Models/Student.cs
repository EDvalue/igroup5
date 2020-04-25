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

        public Team getStudentsbyGrade(int grade,int scode,string mode)
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

        public Classroom SignRequest(string code)
        {
            Classroom C = new Classroom();
            SystemDBservices dbs = new SystemDBservices();
            dbs = dbs.SignRequest(code);

            if (dbs.dt.Rows.Count > 0)
            {
                C.InSchool = Convert.ToInt32(dbs.dt.Rows[0]["SchoolCode"]);
                C.Grade = Convert.ToInt32(dbs.dt.Rows[0]["Grade"]);
                C.GradeNumber = Convert.ToInt32(dbs.dt.Rows[0]["GradeNumber"]);
                C.Name = dbs.dt.Rows[0]["Title"].ToString();
                C.EdTeacher.Mail= dbs.dt.Rows[0]["TeacherEmail"].ToString();
            }           
                return C;
        }

    }
}