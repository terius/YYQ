using YYQERP.Repository;
using YYQERP.Services.Messages;

namespace YYQERP.Services.Interfaces
{
    public interface IDeliveryService : IBaseService<DeliverySet, int>
    {
        Search_Delivery_Response SearchDelivery(Search_Delivery_Request request);
    }
}
