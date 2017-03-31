using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class Add_Bom_Request
    {
        public string Name { get; set; }

        public int ModelId { get; set; }

        public string Remark { get; set; }

    }

    public class Edit_Bom_Request : Add_Bom_Request
    {
        public int Id { get; set; }
    }
}
