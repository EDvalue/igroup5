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
        int questionnaireId;
        int taskId;
        string subjectName;
        string intelligenceName;
        int grade;

        public StudentInClass(string studentEmail, int questionnaireId, int taskId, string subjectName, string intelligenceName, int grade)
        {
            this.studentEmail = studentEmail;
            this.questionnaireId = questionnaireId;
            this.taskId = taskId;
            this.subjectName = subjectName;
            this.intelligenceName = intelligenceName;
            this.grade = grade;
        }
        public StudentInClass()
        {

        }
        public string StudentEmail { get => studentEmail; set => studentEmail = value; }
        public int QuestionnaireId { get => questionnaireId; set => questionnaireId = value; }
        public int TaskId { get => taskId; set => taskId = value; }
        public string SubjectName { get => subjectName; set => subjectName = value; }
        public string IntelligenceName { get => intelligenceName; set => intelligenceName = value; }
        public int Grade { get => grade; set => grade = value; }

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
    }
}