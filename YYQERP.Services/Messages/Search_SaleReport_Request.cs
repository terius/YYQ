using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYQERP.Services.Views;

namespace YYQERP.Services.Messages
{
    public class Search_SaleReport_Request : PageRequest
    {
        public DateTime STime { get; set; }
        public DateTime ETime { get; set; }


        public IList<filterRule> filterRules { get; set; }

        public bool QueryAll { get; set; }
        public string LoginTrueName { get; set; }
        //public string AfterSales { get; set; }
        //public string SaleDate { get; set; }
        //public string CustomerLevel { get; set; }
        //public string Company { get; set; }
        //public string SaleLeader { get; set; }
        //public string PayDate { get; set; }
        //public string PayWay { get; set; }
        //public string CustomerType { get; set; }
        //public string MachineType { get; set; }
        //public string Model { get; set; }
        //public string GoodsSpec { get; set; }
        //public string GoodsNum { get; set; }
        //public string TaxPrice { get; set; }
        //public string AllTaxPrice { get; set; }
        //public string LogisticsCost { get; set; }
        //public string NoTaxPrice { get; set; }
        //public string Invoice { get; set; }
        //public string DeliveryNote { get; set; }
        //public string Billing { get; set; }
        //public string MachineNo { get; set; }
        //public string OrderNo { get; set; }
        //public string Remark { get; set; }
        //public string Contacts { get; set; }
        //public string Phone { get; set; }
        //public string Tel { get; set; }
        //public string Email { get; set; }
        //public string QQ { get; set; }
        //public string CompanyAddress { get; set; }
        //public string Fax { get; set; }
        //public string Bonus { get; set; }
        //public string LogisticsCompany { get; set; }
        //public string Addtime { get; set; }
        //public string AddUserName { get; set; }
    }

    public class filterRule
    {
        public string field { get; set; }
        public string op { get; set; }
        public string value { get; set; }
    }

    public class Export_SaleReport_Request : Search_SaleReport_Request
    {
        public string ColumnTitles { get; set; }

        public string Columns { get; set; }

        public string FileName { get; set; }

        public bool ExportPageData { get; set; }
    }


}
