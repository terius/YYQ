using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class Add_User_Request
    {
        public string UserName { get; set; }
        public string TrueName { get; set; }

        public int RoleId { get; set; }
    }

    public class Edit_User_Request : Add_User_Request
    {
        public int Id { get; set; }
    }
}
