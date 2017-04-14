using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYQERP.Repository;
using YYQERP.Services.Messages;
using YYQERP.Services.Views;

namespace YYQERP.Services.Interfaces
{
    public interface IGoodsService : IBaseService<ElementSet, int>
    {
        //下拉框方法
        IEnumerable GetElementSelectList();
        IEnumerable GetShelfSelectList();
        IEnumerable GetModelSelectList();
        IEnumerable GetPartSelectList();
        IEnumerable GetBomSelectList();
        IList<Default_SelectItem> GetProductSelectListForStockIn();

        IList<Default_SelectItem> GetProductSelectListForDelivery();


        //原材料方法
        Search_Element_Response SearchElement(Search_Element_Request request, bool isPage = true);

        CEDResponse AddElement(Add_Element_Request request);

        CEDResponse EditElement(Edit_Element_Request request);

        CEDResponse DeleteElement(int id);

        string ImportElement(DataTable dt, string loginUserName);

        string RestoreElement(DataTable dt, string loginUserName);


        //型号方法
        Search_Model_Response SearchModels(Search_Model_Request request);
        CEDResponse AddModel(Add_Model_Request request);
        CEDResponse EditModel(Edit_Model_Request request);
        CEDResponse DeleteModel(int id);


        //部件方法
        Search_Part_Response SearchPart(Search_Part_Request request);
        CEDResponse AddPart(Add_Part_Request request);
        CEDResponse EditPart(Edit_Part_Request request);
        CEDResponse DeletePart(int id);

        IList<PartDetailEditView> GetPartDetail(int partId);

        CEDResponse SavePartDetail(List<PartDetailEditView> infos, int partid);

        IList<BomListInPartView> GetBomListInPart(string partCode);
        IList<BomDetailEditView> GetBomDetailListInPart(int bomId);


        //bom方法
        Search_Bom_Response SearchBom(Search_Bom_Request request);
        CEDResponse AddBom(Add_Bom_Request request);
        CEDResponse EditBom(Edit_Bom_Request request);
        CEDResponse DeleteBom(int id);

        IList<BomDetailEditView> GetBomDetail(int BomId);

        CEDResponse SaveBomDetail(List<BomDetailEditView> infos, int Bomid);



        IList<BomDetailEditView> GetPartDetailInBomDetail(int partId);

        BomDetailEditView GetEleDetailInBomDetail(int eleid);


        IList<Export_Bom_View> GetBomDetailExport(int BomId);

        //出库方法
        IList<StockOut_ForAdd_View> GetStockOutListByBomId(int bomid, int num);

        StockOut_ForAdd_View GetStockOutListByEleId(int eleid);

        //库位方法
        Search_Shelf_Response SearchShelf(Search_Shelf_Request request);
        CEDResponse AddShelf(Add_Shelf_Request request);
        CEDResponse EditShelf(Edit_Shelf_Request request);
        CEDResponse DeleteShelf(int id);

        //产品方法
        Search_Product_Response SearchProduct(Search_Product_Request request);
        IList<ProductDetailListView> GetProductDetail(int ProductId);

        CEDResponse AddProduct(Add_Product_Request request);
        CEDResponse EditProduct(Edit_Product_Request request);

        StockIn_ForAdd_ByProductView GetStockInAddItemByProductId(int pid);

        IEnumerable GetHalfProductSelectList();

        IList<ProductDetailListView> GetElementListByBomId_In_ProductDetail(int bomid);

        ProductDetailListView GetElementByEleId_In_ProductDetail(int eleid);

        IList<ProductDetailListView> GetElementListByHalfProductId_In_ProductDetail(int productId);

        CEDResponse SaveProductDetail(IList<ProductDetailListView> list, string currUserName, int productId);

      

        //领料方法
        IList<Pick_ForAdd_View> GetListByBomId_For_PickAdd(int bomid, int num);
        IList<Pick_ForAdd_View> GetListByPartId_For_PickAdd(int partid, int num);
        Pick_ForAdd_View GetViewByEleId_For_PickAdd(int eleid);
    }
}
