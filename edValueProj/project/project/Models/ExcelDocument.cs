using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Web.Hosting;
using System.Threading;
using NPOI.SS.UserModel;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace project.Models
{


    public class ExcelDocument 
    {
       

        public ExcelDocument() { }

        public object MessageBoxButton { get; private set; }
        public object MessageBox { get; private set; }
        public bool DialogResult { get; private set; }
        public object MessageBoxImage { get; private set; }

        public List<Dictionary<string,string>> getExcelFile(string path)
        {
            
            //Student s;
            //Create COM Objects. Create a COM object for everything that is referenced
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(path);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            int numEffected = 0;

            Teacher t;
            List<Teacher> nEd = new List<Teacher>();
            List<Classroom> newC = new List<Classroom>();
            List<string> firstRow = new List<string>();
            List<Dictionary<string, string>> report = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> slist = new List<Dictionary<string, string>>();
            Dictionary<int, Dictionary<int, Dictionary<int, Classroom>>> schoolDict = new Dictionary<int, Dictionary<int, Dictionary<int, Classroom>>>();

            for (int fr = 1; fr<=colCount ; fr++)
            {
                firstRow.Add(xlRange.Cells[1, fr].Value2);
            }

            for (int i = 2; i <= rowCount; i++)
            {

                User u = new User();
                Classroom c1 = new Classroom();
                string type = xlRange.Cells[i, 6].Value2;
            
                for (int j = 1; j <= colCount; j++)
                {

                        if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                        {
                      

                            switch (firstRow[j - 1])
                            {

                                case "id":
                                
                                u.IdNumber = xlRange.Cells[i, j].Value2.ToString();
                                
                                break;

                                case "Email":
                                    u.Mail = xlRange.Cells[i, j].Value2;
                                    break;

                                case "lastName":
                                    u.LastName = xlRange.Cells[i, j].Value2;
                                    break;

                                case "name":
                                    u.Name = xlRange.Cells[i, j].Value2;
                                    break;

                                case "school":

                                    c1.InSchool = (int)xlRange.Cells[i, j].Value2;
                                    break;

                                case "Grade":
                                    c1.Grade = (int)xlRange.Cells[i, j].Value2;
                                    break;
                                case "ClassNumber":

                                     c1.GradeNumber = (int)xlRange.Cells[i, j].Value2;
                                    break;


                            }
                            
                        }                                            
                }

                if (type == "מורה") { 
                
                    t = new Teacher(u.IdNumber, u.Name, u.LastName, u.IdNumber, u.Mail,c1.InSchool,false,false);
                    
                    if (c1.GradeNumber > 0)
                    {
                        c1.Name = u.Name + "-" + c1.Grade + "`" + c1.GradeNumber;
                        c1.EdTeacher = t;

                        

                        if (!schoolDict.ContainsKey(c1.InSchool))
                        {
                             schoolDict.Add(c1.InSchool,new Dictionary<int, Dictionary<int, Classroom>>());
                        }
                        if (!schoolDict[c1.InSchool].ContainsKey(c1.Grade))
                        {
                            schoolDict[c1.InSchool].Add(c1.Grade, new Dictionary<int, Classroom>());
                        }
                        if (!schoolDict[c1.InSchool][c1.Grade].ContainsKey(c1.GradeNumber))
                        {
                            schoolDict[c1.InSchool][c1.Grade].Add(c1.GradeNumber,new Classroom());
                        }
                        schoolDict[c1.InSchool][c1.Grade][c1.GradeNumber] = c1;


                    }
                    else
                    {
                        nEd.Add(t);
                    }
               
                }
                else if(type=="תלמיד")
                {

                    Dictionary<string, string> sdict = new Dictionary<string, string>();

                    sdict.Add("IdNumber", u.IdNumber);
                    sdict.Add("Mail", u.Mail);
                    sdict.Add("Name", u.Name);
                    sdict.Add("LastName", u.LastName);
                    sdict.Add("Password", u.IdNumber);
                    sdict.Add("SCode", c1.InSchool.ToString());
                    sdict.Add("Grade",c1.Grade.ToString());
                    sdict.Add("ClassNumber",c1.GradeNumber.ToString());

                    slist.Add(sdict);
                }


            }

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

            DBservices dbs = new DBservices();
            foreach(var item in schoolDict)
            {
                List<Dictionary<string, string>> r = new List<Dictionary<string, string>>();
                r = dbs.postNewClasses(item.Value);
                foreach (var row in r)
                    report.Add(row);

            }
            
            if (nEd.Count > 0)
            {
                List<Dictionary<string, string>> r = new List<Dictionary<string, string>>();
                r = dbs.postnEdlFile(nEd);
                foreach (var row in r)
                    report.Add(row);
               
               
            }

            if (slist.Count > 0)
            {
                List<Dictionary<string, string>> r = new List<Dictionary<string, string>>();
                r = dbs.postStudentFile(slist);
                foreach (var row in r)
                    report.Add(row);
            }


           
            return report;
        }


        // Attemps to read workbook as XLSX, then XLS, then fails.
        public List<Dictionary<string,string>> ReadWorkbook(string path)
        {

            List<Dictionary<string, string>> report = new List<Dictionary<string, string>>();
            IWorkbook book;

            try
            {
                

                FileStream fs = new FileStream(path,FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                //var fs = System.IO.File.OpenRead(path);
                // Try to read workbook as XLSX:
                try
                {
                    book = new XSSFWorkbook(fs);
                    ISheet sheet = book.GetSheetAt(0);

                    report=readFile(sheet);

                    
                }   
                catch(Exception ex)
                {
                    book = null;
                }

                // If reading fails, try to read workbook as XLS:
                if (book == null)
                {
                    book = new HSSFWorkbook(fs);
                    ISheet sheet = book.GetSheetAt(0);

                    report = readFile(sheet);


                }
            }
            catch (Exception ex)
            {
               
                this.Close();
               
            }
            return report;
        }

        private void Close()
        {
            throw new NotImplementedException();
        }


        public List<Dictionary<string, string>> readFile(ISheet sheet)
        {
            
            Teacher t;
            List<Teacher> nEd = new List<Teacher>();
            List<Classroom> newC = new List<Classroom>();
            Dictionary<string, int> firstRow = new Dictionary<string, int>();
            List<Dictionary<string, string>> report = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> slist = new List<Dictionary<string, string>>();
            Dictionary<int, Dictionary<int, Dictionary<int, Classroom>>> schoolDict = new Dictionary<int, Dictionary<int, Dictionary<int, Classroom>>>();
            int flg = 0;
            int colRange = 0;
            User u = new User();



            while (sheet.GetRow(0).GetCell(colRange) != null)
            {
                firstRow[sheet.GetRow(0).GetCell(colRange).ToString()] = colRange;
                colRange++;
            }

            for (int row = 1; row <= sheet.LastRowNum - 1; row++)
            {
                u = new User();
                Classroom c1 = new Classroom();
                string type = sheet.GetRow(row).GetCell(firstRow["Type"]).ToString();           /* xlRange.Cells[i, 6].Value2;*/

                foreach (var j in firstRow)
                {
                    if (flg == 1)
                    {
                        return report;
                    }
                    switch (j.Key)
                    {

                        case "id":

                            u.IdNumber = sheet.GetRow(row).GetCell(j.Value).ToString();
                            if(!(IsAllDigits(u.IdNumber) && u.IdNumber.Length == 9))
                            {
                                Dictionary<string, string> reportRow = new Dictionary<string, string>();
                                reportRow.Add("msg", "in line"+row+"column"+j.Key+" content is invalid,must be 9 digits ID");
                                report.Add(reportRow);
                                flg = 1;

                            }
                            break;

                        case "Email":
                            u.Mail = sheet.GetRow(row).GetCell(j.Value).ToString();
                            if (!IsValid(u.Mail))
                            {
                                Dictionary<string, string> reportRow = new Dictionary<string, string>();
                                reportRow.Add("msg", "in line" + row + "column" + j.Key + " content is invalid,email is incorrect");
                                report.Add(reportRow);
                                flg = 1;
                            }
                            break;

                        case "lastName":
                            u.LastName = sheet.GetRow(row).GetCell(j.Value).ToString();
                            
                            if(!Regex.IsMatch(u.LastName, "[א-ת]{2,15}")){
                                Dictionary<string, string> reportRow = new Dictionary<string, string>();
                                reportRow.Add("msg", "in line" + row + "column" + j.Key + " content is invalid,must be only hebrew letters");
                                report.Add(reportRow);
                                flg = 1;
                            }
          
                            break;

                        case "name":
                            u.Name = sheet.GetRow(row).GetCell(j.Value).ToString();
                            if (!Regex.IsMatch(u.Name, "[א-ת]{2,15}"))
                            {
                                Dictionary<string,string> reportRow = new Dictionary<string, string>();
                                reportRow.Add("msg", "in line " + row + "column " + j.Key + " content is invalid,must be only hebrew letters");
                                report.Add(reportRow);
                                flg = 1;
                            }
                            break;

                        case "school":
                            try
                            {
                                c1.InSchool = Convert.ToInt32(sheet.GetRow(row).GetCell(j.Value).ToString());
                            }
                            catch
                            {
                                Dictionary<string, string> reportRow = new Dictionary<string, string>();
                                reportRow.Add("msg", "in line " + row + "column" + j.Key + " content is invalid,must be only digits");
                                report.Add(reportRow);
                                flg = 1;
                                continue;
                                
                            }
                            
                           
                            break;

                        case "Grade":
                            if (sheet.GetRow(row).GetCell(j.Value) == null)
                            {
                                c1.Grade = Convert.ToInt32(sheet.GetRow(row).GetCell(j.Value));
                            }
                            else
                            {
                                c1.Grade = Convert.ToInt32(sheet.GetRow(row).GetCell(j.Value).ToString());
                            }
                            if(type=="תלמיד" )
                            {
                                if (c1.Grade<7 || c1.Grade > 9)
                                {
                                    Dictionary<string, string> reportRow = new Dictionary<string, string>();
                                    reportRow.Add("msg", "in line " + row + "column" + j.Key + " content is invalid,must be only digits");
                                    report.Add(reportRow);
                                    flg = 1;
                                } 
                            }
                            break;
                        case "ClassNumber":
                            if (sheet.GetRow(row).GetCell(j.Value) == null)
                            {
                                c1.GradeNumber = Convert.ToInt32(sheet.GetRow(row).GetCell(j.Value));
                            }
                            else
                            {
                                c1.GradeNumber = Convert.ToInt32(sheet.GetRow(row).GetCell(j.Value).ToString());
                            }
                            if (type == "תלמיד" && c1.GradeNumber==0)
                            {
                                
                                flg = 1;
                            }

                            break;


                    }

                }

                if (type == "מורה")
                {

                    t = new Teacher(u.IdNumber, u.Name, u.LastName, u.IdNumber, u.Mail, c1.InSchool, false, false);

                    if (c1.GradeNumber > 0)
                    {
                        c1.Name = u.Name + "-" + c1.Grade + "`" + c1.GradeNumber;
                        c1.EdTeacher = t;



                        if (!schoolDict.ContainsKey(c1.InSchool))
                        {
                            schoolDict.Add(c1.InSchool, new Dictionary<int, Dictionary<int, Classroom>>());
                        }
                        if (!schoolDict[c1.InSchool].ContainsKey(c1.Grade))
                        {
                            schoolDict[c1.InSchool].Add(c1.Grade, new Dictionary<int, Classroom>());
                        }
                        if (!schoolDict[c1.InSchool][c1.Grade].ContainsKey(c1.GradeNumber))
                        {
                            schoolDict[c1.InSchool][c1.Grade].Add(c1.GradeNumber, new Classroom());
                        }
                        schoolDict[c1.InSchool][c1.Grade][c1.GradeNumber] = c1;


                    }
                    else
                    {
                        nEd.Add(t);
                    }

                }
                else if (type == "תלמיד")
                {

                    Dictionary<string, string> sdict = new Dictionary<string, string>();

                    sdict.Add("IdNumber", u.IdNumber);
                    sdict.Add("Mail", u.Mail);
                    sdict.Add("Name", u.Name);
                    sdict.Add("LastName", u.LastName);
                    sdict.Add("Password", u.IdNumber);
                    sdict.Add("SCode", c1.InSchool.ToString());
                    sdict.Add("Grade", c1.Grade.ToString());
                    sdict.Add("ClassNumber", c1.GradeNumber.ToString());

                    slist.Add(sdict);
                }


            }


            DBservices dbs = new DBservices();
            foreach (var item in schoolDict)
            {
                List<Dictionary<string, string>> r = new List<Dictionary<string, string>>();
                r = dbs.postNewClasses(item.Value);
                foreach (var row in r)
                    report.Add(row);

            }

            if (nEd.Count > 0)
            {
                List<Dictionary<string, string>> r = new List<Dictionary<string, string>>();
                r = dbs.postnEdlFile(nEd);
                foreach (var row in r)
                    report.Add(row);

            }

            if (slist.Count > 0)
            {
                List<Dictionary<string, string>> r = new List<Dictionary<string, string>>();
                r = dbs.postStudentFile(slist);
                foreach (var row in r)
                    report.Add(row);
            }

            return report;
        }

        bool IsAllDigits(string s) => s.All(char.IsDigit);
        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}  

 