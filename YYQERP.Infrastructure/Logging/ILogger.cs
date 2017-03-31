using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YYQERP.Infrastructure.Logging
{
    public interface ILogger
    {
        void Log(string message);


        void Log(string message, int Level);
        void Log(Exception ex);
    }

}
