using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using project.Models;
using project.Models.DAL;
using System.IO;
using System.Web.Hosting;

namespace project.Controllers
{
    public class EDvalueController : ApiController
    {
        [HttpPost]
        [Route("api/EDvalue/postCity")]

        public int postCity()
        {
            City c = new City();
            return c.getExcelFile("cc");
        }

        [HttpPut]
        [Route("api/EDvalue/Conect")]

        public User conect([FromBody] Dictionary<string,string> con)
        {
            SystemDBservices dbs = new SystemDBservices();
            return dbs.conect(con);
        }


        [HttpPost]
        [Route("api/EDvalue/excelLoad")]
        public List<Dictionary<string, string>> excelLoad()
        {
            //HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            HttpPostedFile file1 = httpRequest.Files[0];

         
          
            //To save file, use SaveAs method
            var filePath = HttpContext.Current.Server.MapPath("~/uploadedFile\\" + file1.FileName);
            file1.SaveAs(filePath); //File will be saved in application root

            ExcelDocument ex = new ExcelDocument();
           return ex.getExcelFile(filePath);
        }

      

    }
}

