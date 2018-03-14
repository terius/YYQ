using System;

namespace YYQERP.Services.Messages
{
    public class Search_Delivery_Request : PageRequest
    {
        public DateTime STime { get; set; }
        public DateTime ETime { get; set; }

        public string SaleName { get; set; }

        public string Customer { get; set; }

    }


}
