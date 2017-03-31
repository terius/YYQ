using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYQERP.Infrastructure;
using YYQERP.Repository;
using YYQERP.Services.Messages;
using YYQERP.Services.Views;

namespace YYQERP.Services.Interfaces
{
    public interface IDicService : IBaseService<DicSet, int>
    {
        IList<DicView> GetUnitList();


        IEnumerable GetUnitSelectList();


        IList<DicTreeView> GetDicTreeViews(string parentCode = null);

        IList<DicChildrenView> GetDicChildrenViews(string parentCode);

        CEDResponse AddDic(Add_Dic_Request request);

        CEDResponse DeleteDic(int id);
        CEDResponse EditDic(Edit_Dic_Request request);
    }
}
