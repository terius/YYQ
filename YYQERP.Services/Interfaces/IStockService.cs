using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYQERP.Repository;
using YYQERP.Services.Messages;
using YYQERP.Services.Views;

namespace YYQERP.Services.Interfaces
{
    public interface IStockService : IBaseService<StockSet, int>
    {
        Search_Stock_Response SearchStock(Search_Stock_Request request, bool isPage = true);





        CEDResponse SaveStockIn(PageResponse<StockInView> pgInfo, string currUserName, string reason);

        Search_StockIn_Response SearchStockIn(Search_StockIn_Request request, bool isPage = true);
        string DeleteStockIn(int id);

        string GetStockDetailHtml(int id);


        Search_StockOut_Response SearchStockOut(Search_StockOut_Request request,bool isPage=true);



        string GetStockQuantity(int eleId, int shelfId, bool isProduct = false);

        CEDResponse SaveStockOut(IList<StockOut_ForAdd_View> list, string currUserName, string Reason);

        string DeleteStockOut(int id);

        //   string DeleteStockOutMain(int id);

        IList<Default_SelectItem> GetProductSelectListForStockOut();

        StockOut_ForAdd_ByProductView GetStockOutAddItemByProductId(int stockid);


        CEDResponse SaveStockOutByProduct(IList<StockOut_ForAdd_ByProductView> list, string currUserName, string Reason);

        CEDResponse SaveStockInByProduct(IList<StockIn_ForAdd_ByProductView> list, string currUserName, string Reason);

        Search_StockWarn_Response SearchStockWarn(Search_StockWarn_Request request);

        double GetStockQuantityNum(int eleId, int shelfId, bool isProduct = false);

        double GetStockQuantity(int eleId);
    }
}
