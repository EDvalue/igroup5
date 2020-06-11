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
            User u= dbs.conect(con);
            if(u is Student)
              ((Student)u).updateINT();
            return u;
        }

        [HttpPost]
        [Route("api/EDvalue/excelLoad")]
        public List<Dictionary<string,string>> excelLoad()
        {
            
            //HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            HttpPostedFile file1 = httpRequest.Files[0];

         
          
            //To save file, use SaveAs method
            var filePath = HttpContext.Current.Server.MapPath("~/uploadedFile\\" + file1.FileName);
            file1.SaveAs(filePath); //File will be saved in application root
            try {
                ExcelDocument ex = new ExcelDocument();
                return ex.ReadWorkbook(filePath);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message + ", file path: " + filePath);
            }
          



        }

      [HttpPut]
      [Route("api/EDvalue/updatePass")]
      public int updatePass([FromBody]Dictionary<string, string> dict)
      {
            User u = new User();
            return u.updatePass(dict);
      }

        [HttpPut]
        [Route("api/EDvalue/FP")]
        public string fp(Dictionary<string, string> conection)
        {
            User u = new User();
            return u.fp(conection);
        }

        [HttpPut]
        [Route("api/EDvalue/SignRequest")]
        public Classroom SignRequest([FromBody]string code)
        {
            Student s = new Student();
            return s.SignRequest(code);
        }

        [HttpPut]
        [Route("api/EDvalue/sysUpdates/")]
        public int sysUpdates()
        {

            EDsystem s = new EDsystem();
            return s.sysUpdates();
        }

    }
}

