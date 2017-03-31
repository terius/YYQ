using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace YYQERP.Infrastructure.Configuration
{
    public class WebConfigApplicationSettings : IApplicationSettings
    {
        public string LoggerName
        {
            get { return ConfigurationManager.AppSettings["LoggerName"]; }
        }

      

        //public Guid ApplicationID
        //{
        //    get
        //    {
        //        string appid = ConfigurationManager
        //                     .AppSettings["ApplicationID"];
        //        return new Guid(appid);
        //    }
        //}

        public int PageSize
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["PageSize"]) ? 20 : Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

            }
        }

    }

}
