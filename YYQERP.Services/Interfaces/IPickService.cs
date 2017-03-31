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
    public interface IPickService : IBaseService<PickSet, int>
    {
        Search_Pick_Response SearchPick(Search_Pick_Request request);

        string GetPickDetailHtml(int id);

        CEDResponse SavePick(IList<Pick_ForAdd_View> list, string currUserName, string Purpose);

        Search_PickOut_Response SearchPickOut(Search_PickOut_Request request);

        CEDResponse SavePickOut(IList<PickOut_ForAdd_View> list, string currUserName, int mainId);


        IList<PickOut_ForAdd_View> GetPickDetail_ForAdd(int ParentId);

        string DeletePick(int id);
    }
}
