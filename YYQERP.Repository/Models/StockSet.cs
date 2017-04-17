using YYQERP.Infrastructure.Domain;

namespace YYQERP.Repository
{
    public partial class StockSet : EntityBase<int>, IEntity
    {
        protected override void Validate()
        {
            if (this.Quantity < 0)
            {
                base.AddBrokenRule(new BusinessRule(Property.NumberError, "库存数量不能小于0"));
            }
        }
    }
}
