using YYQERP.Infrastructure.Domain;

namespace YYQERP.Repository
{
    public partial class ModelSet : EntityBase<int>, IEntity
    {
        public string GetModelText()
        {
            return Name + "【" + Code + "】";
        }
    }
}
