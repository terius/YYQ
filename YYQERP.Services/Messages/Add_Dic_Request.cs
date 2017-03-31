using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class Add_Dic_Request
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string ParentCode { get; set; }

        public string Remark { get; set; }

        public int sort { get; set; }

        public int Enabled { get; set; }

    }

    public class Edit_Dic_Request : Add_Dic_Request
    {
        public int Id { get; set; }
    }
}
