using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using project.Models.DAL;
using System.Data;

namespace project.Models
{
    public class EDsystem
    {

        public int sysUpdates()
        {
            SystemDBservices dbs = new SystemDBservices();

            dbs.backup();

            SystemDBservices dbs1 = new SystemDBservices();
            SystemDBservices dbs2 = new SystemDBservices();
            dbs1 =dbs1.getAllClasses();
            dbs2 = dbs2.getAllTeams();


            return 1;
            
        }
    }
}