﻿using System;
using System.IO;

namespace YYQERP.Infrastructure.Logger
{
    public class FileLogger
    {
        public void LogException(Exception e)
        {
            File.WriteAllLines("C://Error//" + DateTime.Now.ToString("dd-MM-yyyy mm hh ss") + ".txt",
               new string[] 
                     {
                        "Message:"+e.Message,
                        "Stacktrace:"+e.StackTrace
                    });
        }
    }
}
