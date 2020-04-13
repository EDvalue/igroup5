using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;

namespace project.Models
{
    public class City
    {


        string hname;
        string ename;
        int code;
        string area;

        public City()
        {

        }

        public City(string hname, string ename, int code, string area)
        {
            this.hname = hname;
            this.ename = ename;
            this.code = code;
            this.area = area;
        }

        public string Hname { get => hname; set => hname = value; }
        public string Ename { get => ename; set => ename = value; }
        public int Code { get => code; set => code = value; }
        public string Area { get => area; set => area = value; }


        public List<City> getAllcities()
        {
            DBservices dbs = new DBservices();
            return dbs.getAllcities();
        }

        public int getExcelFile(string path)
        {

            //Create COM Objects. Create a COM object for everything that is referenced
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"C:\Users\User\Desktop\yeshuvim");
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!

            List<City> list = new List<City>();
            List<string> firstRow = new List<string>();
            for (int fr = 1; fr <= colCount; fr++)
            {
                firstRow.Add(xlRange.Cells[1, fr].Value2);
            }

            for (int i = 2; i <= rowCount; i++)
            {
                    City c = new City();
                for (int j = 1; j <= colCount; j++)
                {

                    if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                    {
                        switch (firstRow[j - 1])
                        {
                            case "מחוז":
                               c.Area  = xlRange.Cells[i, j].Value2;
                                break;

                            case "שם_ישוב_לועזי":
                                string en = xlRange.Cells[i, j].Value2;
                                string[] wordsE = en.Split(' ');
                                c.Ename = wordsE[0] + " " + wordsE[1];
                               
                                break;
                            case "שם_ישוב":
                                string hn = xlRange.Cells[i, j].Value2;
                                string[] wordsH = hn.Split(' ');
                                c.Hname = wordsH[0]+" "+wordsH[1];
                                break;
                            case "סמל_ישוב":
                                c.Code= Convert.ToInt32(xlRange.Cells[i, j].Value2);
                                break;


                        }
                    }

                }
                list.Add(c);
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
            return dbs.loadCity(list);
        }
    }
}