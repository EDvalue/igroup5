using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using project.Models.DAL;
namespace project.Models
{
    public class StudentInClass
    {
        Student st;
        string questionnaireId;
        string taskId;
        string subjectName;
        string intelligenceName;
        int grade;
        int isperform;

       
        public StudentInClass()
        {

        }

        public StudentInClass(Student st, string questionnaireId, string taskId, string subjectName, string intelligenceName, int grade, int isperform)
        {
            this.St = st;
            this.QuestionnaireId = questionnaireId;
            this.TaskId = taskId;
            this.SubjectName = subjectName;
            this.IntelligenceName = intelligenceName;
            this.Grade = grade;
            this.Isperform = isperform;
        }

        public Student St { get => st; set => st = value; }
        public string QuestionnaireId { get => questionnaireId; set => questionnaireId = value; }
        public string TaskId { get => taskId; set => taskId = value; }
        public string SubjectName { get => subjectName; set => subjectName = value; }
        public string IntelligenceName { get => intelligenceName; set => intelligenceName = value; }
        public int Grade { get => grade; set => grade = value; }
        public int Isperform { get => isperform; set => isperform = value; }


        //public List<StudentInClass> getStudents()
        //{
        //    StudentInClassDBservices dbs = new StudentInClassDBservices();
        //    return dbs.getStudents();
        //}
        public List<StudentInClass> getStudentByEmail(string email)
        {
            StudentInClassDBservices dbs = new StudentInClassDBservices();
            return dbs.getStudentByEmail(email);
        }
       /* public int updatePtime(Dictionary<string, string> info)
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.updatePtime(info);
        }*/

    }
}