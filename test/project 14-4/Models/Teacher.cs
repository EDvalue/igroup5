using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace project.Models
{
    public class Teacher : User
    {
        bool isEditor;
        bool schoolAdmin;
      


        public Teacher():base() { }
        public Teacher(string idNumber,string name,string lastName,string password,string mail,int scode,bool isEditor,bool schoolAdmin)
         : base(name, lastName, password, mail, idNumber,scode)
        {
            
            this.isEditor = isEditor;
            this.schoolAdmin = schoolAdmin;
        }
       

        public bool IsEditor { get => isEditor; set => isEditor = value; }
        public bool SchoolAdmin { get => schoolAdmin; set => schoolAdmin = value; }

        public int postNewTeacher(List<Dictionary<string, string>> userDict)
        {
            List<Teacher> list = new List<Teacher>();
            foreach(var x in userDict)
            {
                Teacher t = new Teacher(x["IdNumber"],x["Name"],x["LastName"],x["Password"],x["Mail"],Convert.ToInt32(x["SCode"]),false,Convert.ToBoolean(Convert.ToInt32(x["SchoolAdmin"])));
                list.Add(t);
            }
            
            DBservices dbs = new DBservices();
            return dbs.postnEd(list);
        }

        public List<Teacher> getTeacherByScode(int scode)
        {
            DBservices dbs = new DBservices();
            return dbs.getTeacherByScode(scode);
        }

        public List<Teacher> getAllteachers()
        {
            DBservices dbs = new DBservices();
           return dbs.getAllteachers();
        }


        public int updateEditors(List<Teacher> tlist)
        {
            DBservices dbs = new DBservices();
            return dbs.updateEditors(tlist);
        }

    }
}