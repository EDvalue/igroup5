using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using project.Models.DAL;
using System.Data;
//using Microsoft.Office.Interop.Excel;
//using Excel = Microsoft.Office.Interop.Excel;

namespace project.Models
{
    public class User
    {
         string name;
         string idNumber;
         string lastName;
         string password;
         string mail;
         int sCode;
         
        

        public string Name { get => name; set => name = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string Password { get => password; set => password = value; }
        public string Mail { get => mail; set => mail = value; }
        public string IdNumber { get => idNumber; set => idNumber = value; }
        public int SCode { get => sCode; set => sCode = value; }

        public User() { }
        public User(string name, string lastName, string password, string mail,string idNumber,int sCode)
        {
            this.SCode = sCode;
            this.IdNumber = idNumber;
            this.Name = name;
            this.LastName = lastName;
            this.Password = password;
            this.Mail = mail;
        }


        public int postNewUser(List<Dictionary<string,string>> userDict)
        {
            int num = 0;

            foreach (var x in userDict)
           {
                
                if (x["Type"] == "תלמיד")
                {
                    Student s = new Student();
                    num+= s.postNewStudent(userDict);
                }
                else
                {
                    Teacher t = new Teacher();
                    num+= t.postNewTeacher(userDict);
                }

          
            }
            return num;

        }

        public int UpdateUser(List<Dictionary<string, string>> userDict)
        {
            int num = 0;
            foreach (var x in userDict)
            {
                if (x["Type"] == "תלמיד")
                {
                    DBservices dbs = new DBservices();
                    num += dbs.UpdateStudent(userDict);
                }
                else
                {
                    if (x["SchoolPass"] == "pass")
                    {
                        isEd(x["Email"]);
                    }
                    DBservices dbs = new DBservices();
                    num += dbs.UpdateTeacher(userDict);
                }
            }
            return num;
        }

        public int isEd(string idNum)
        {
            DBservices dbs = new DBservices();
            dbs = dbs.readFlightID( idNum);
            dbs.dt = insertOrUpdate(dbs.dt);
            dbs.update();


            return 0;
        }

        private DataTable insertOrUpdate( DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {

              dr["TeacherEmail"]= "none@gmail.com";
   
            }
            
            return dt;
        }

        public Dictionary<string,string> getUserByMail(string mail)
        {
            DBservices dbs = new DBservices();
            return dbs.getUserByMail(mail);
        }

    }
}