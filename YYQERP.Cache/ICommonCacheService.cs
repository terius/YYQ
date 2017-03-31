using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYQERP.Infrastructure;

namespace YYQERP.Cache
{
    public interface ICommonCacheService
    {
        IList<DicView> GetCache_Dic();

        IList<ElementCacheView> GetCache_Element();

        void RemoveDicCache();

        void RemoveElementCache();

        void RemovePartCache();

        void RemoveModelCache();

        void RemoveBomCache();
        void RemoveUserCache();

        IList<DicView> GetCache_Unit();

        IList<PartCacheView> GetCache_Part();

        IList<ModelCacheView> GetCache_Model();

        IList<BomCacheView> GetCache_Bom();

        IList<UserCacheView> GetCache_User();
    }
}
