using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;

namespace project.Models
{
    public class School
    {

        int schoolCode;
        string name;
        int cityCode;
        List<Classroom> clist = new List<Classroom>();

        public School(int schoolCode, string name, int cityCode)
        {
            this.schoolCode = schoolCode;
            this.name = name;
            this.cityCode = cityCode;
            
        }

        public School() { }

        public int SchoolCode { get => schoolCode; set => schoolCode = value; }
        public string Name { get => name; set => name = value; }
        public int CityCode { get => cityCode; set => cityCode = value; }
        public List<Classroom> Clist { get => clist; set => clist = value; }

        public List<School> getSchoolByCity(int code)
        {
            DBservices dbs = new DBservices();
            return dbs.getSchoolByCity(code);
        }

        public List<School> getAllschool()
        {
            DBservices dbs = new DBservices();
            return dbs.getAllschool();
        }

        public int postSchool()
        {
            DBservices dbs = new DBservices();
            return dbs.postSchool(this);
        }

        public School getSchoolByID(int id)
        {
            DBservices dbs = new DBservices();
            return dbs.getSchoolByID(id);
        }



    }
}