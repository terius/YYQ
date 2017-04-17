using YYQERP.Repository;
using YYQERP.Services.Messages;
using YYQERP.Services.Views;

namespace YYQERP.Services.Interfaces
{
    public interface IDeliveryService : IBaseService<DeliverySet, int>
    {
        Search_Delivery_Response SearchDelivery(Search_Delivery_Request request);

        void SaveAdd(Delivery_Add_View addInfo);
    }
}
