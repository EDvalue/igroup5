using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace project.Controllers
{
    public class uploadController : ApiController
    {
        public IHttpActionResult Post()
        {
            //set root path for uploads
            string root = $@"{HttpContext.Current.Server.MapPath(".")}\Uploads\Items\";

            //create all directories if doesnt exist
            var dir = Directory.CreateDirectory(root);

            Dictionary<string, string> savedFilesNames = new Dictionary<string, string>();
            foreach (string file in HttpContext.Current.Request.Files)
            {
                HttpPostedFile hpf = HttpContext.Current.Request.Files[file] as HttpPostedFile;
                if (hpf.ContentLength != 0)
                {
                    string savedFileName = "";
                    try
                    {
                        savedFileName = $@"{root}{DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss.fff")}_{hpf.FileName}";
                        hpf.SaveAs(savedFileName);
                        savedFilesNames.Add("url", savedFileName);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }

                }

            }
            var json = JsonConvert.SerializeObject(savedFilesNames);
            return Ok(json);

        }

    }
}