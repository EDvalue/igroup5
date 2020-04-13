using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using project.Models;
using project.Models.DAL;

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

        public List<Dictionary<string, string>> excelLoad([FromBody]string path)
        {
            ExcelDocument ex = new ExcelDocument();
           return ex.getExcelFile(path);
        }

      

    }
}

