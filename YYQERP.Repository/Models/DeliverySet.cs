using System;
using YYQERP.Infrastructure.Domain;

namespace YYQERP.Repository
{
    public partial class DeliverySet : EntityBase<int>, IEntity
    {
        protected override void Validate()
        {
            if (!this.Addtime.HasValue)
            {
                this.Addtime = DateTime.Now;
            }

        }

    }
}
