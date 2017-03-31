using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YYQERP.Infrastructure.Configuration
{
    public interface IApplicationSettings
    {
        string LoggerName { get; }

     

      //  Guid ApplicationID { get; }

        int PageSize { get; }

    }
}
