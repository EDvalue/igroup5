using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using project.Models.DAL;
namespace project.Models
{
    public class StudentInClass
    {
        string studentEmail;
        string fname;
        string lname;
        string questionnaireId;
        string taskId;
        string subjectName;
        string intelligenceName;
        int grade;
        int isperform;

        public StudentInClass(string studentEmail, string questionnaireId, string taskId, string subjectName, string intelligenceName, int grade, string lname,string fname,int isperform)
        {
            this.studentEmail = studentEmail;
            this.QuestionnaireId = questionnaireId;
            this.TaskId = taskId;
            this.subjectName = subjectName;
            this.intelligenceName = intelligenceName;
            this.grade = grade;
            this.Lname = lname;
            this.Fname = fname;
            this.Isperform = isperform;
        }
        public StudentInClass()
        {

        }
        public string StudentEmail { get => studentEmail; set => studentEmail = value; }
        public string SubjectName { get => subjectName; set => subjectName = value; }
        public string IntelligenceName { get => intelligenceName; set => intelligenceName = value; }
        public int Grade { get => grade; set => grade = value; }
        public string QuestionnaireId { get => questionnaireId; set => questionnaireId = value; }
        public string TaskId { get => taskId; set => taskId = value; }
        public string Fname { get => fname; set => fname = value; }
        public string Lname { get => lname; set => lname = value; }
        public int Isperform { get => isperform; set => isperform = value; }

        public List<StudentInClass> getStudents()
        {
            StudentInClassDBservices dbs = new StudentInClassDBservices();
            return dbs.getStudents();
        }
        public List<StudentInClass> getStudentByEmail(string email)
        {
            StudentInClassDBservices dbs = new StudentInClassDBservices();
            return dbs.getStudentByEmail(email);
        }

        public List<StudentInClass> getSQuest(string data)
        {
            StudentInClassDBservices dbs = new StudentInClassDBservices();
            return dbs.getSQuest(data);

        }
    }
}