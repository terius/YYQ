using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class CEDResponse
    {

        private string _message;

        public string Message
        {
            get
            {
                return _message == null ? "" : _message;
            }
            set { _message = value; }
        }
        public bool Result { get; set; }



        public int EdCount { get; set; }
    }
}
