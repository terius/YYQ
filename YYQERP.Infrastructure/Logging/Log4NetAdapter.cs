using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YYQERP.Infrastructure.Configuration;
using log4net;
using log4net.Config;


namespace YYQERP.Infrastructure.Logging
{
    public class Log4NetAdapter : ILogger
    {
        private readonly log4net.ILog _log;

        public Log4NetAdapter()
        {
            XmlConfigurator.Configure();
            _log = LogManager
             .GetLogger(ApplicationSettingsFactory.GetApplicationSettings().LoggerName);
        }




        public void Log(string message)
        {
            Log(message, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="msgLevel">1-一般信息 2-警告信息 3-错误信息</param>
        public void Log(string message, int msgLevel)
        {
            if (msgLevel == 1)
            {
                if (_log.IsInfoEnabled)
                    _log.Info("\r\n" + message);
            }
            else if (msgLevel == 2)
            {
                if (_log.IsWarnEnabled)
                    _log.Warn("\r\n" + message);
            }
            else if (msgLevel == 3)
            {
                if (_log.IsErrorEnabled)
                {
                    _log.Error("\r\n" + message);
                }
            }
        }


        public void Log(Exception ex)
        {
            Log(string.Format("\r\n【错误信息:{0}】\r\n【内部错误信息:{1}】\r\n【位置:{2}】", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message, ex.StackTrace), 3);
        }
    }

}
