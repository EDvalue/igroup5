using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using project.Models.DAL;

namespace project.Models
{
    public class Task
    {
        string taskId;
        string title;
        int grade;
        Subject sub;
        List<Quiz> quizList;

        public Task() { }
        public Task(string taskId, string title, int grade, Subject sub, List<Quiz> quizList)
        {
            this.taskId = taskId;
            this.title = title;
            this.grade = grade;
            this.sub = sub;
            this.quizList = quizList;
        }

        public string TaskId { get => taskId; set => taskId = value; }
        public string Title { get => title; set => title = value; }
        public int Grade { get => grade; set => grade = value; }
        public Subject Sub { get => sub; set => sub = value; }
        public List<Quiz> QuizList { get => quizList; set => quizList = value; }


        public int postTask()
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.postTask(this);
        }

        public List<Task> getTasksBySubject(string subName)
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.getTasksBySubject(subName);
        }

        public int postAssigment(Dictionary<string, string> assigment)
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.postAssigment(assigment);
        }

        public int hideTask(string id)
        {
            TeacherDBservices dbs = new TeacherDBservices();
            return dbs.hideTask(id);
        }

     

    }
}