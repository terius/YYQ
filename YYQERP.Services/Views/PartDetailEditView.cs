using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Views
{
    public class PartDetailEditView
    {

        public int Id { get; set; }
        public int PartId { get; set; }

        public int ElementId { get; set; }

        public string ElementName { get; set; }
        public double Quantity { get; set; }

        public int IsSelect { get; set; }
    }
}
