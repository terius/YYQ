using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YYQERP.Repository.DataContextStorage
{
    public interface IDataContextStorageContainer
    {
        YYQERPEntities GetDataContext();
        void Store(YYQERPEntities libraryDataContext);

    
    }
}
