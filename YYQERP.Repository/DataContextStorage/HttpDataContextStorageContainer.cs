using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace YYQERP.Repository.DataContextStorage
{
    public class HttpDataContextStorageContainer : IDataContextStorageContainer 
    {
        private string _dataContextKey = "DataContext";

        public YYQERPEntities GetDataContext()
        {
            YYQERPEntities objectContext = null;
            if (HttpContext.Current == null)
            {
                return new YYQERPEntities();
            }
            if (HttpContext.Current.Items.Contains(_dataContextKey))
            {
                objectContext = (YYQERPEntities)HttpContext.Current.Items[_dataContextKey];
            }
            return objectContext;
        }

        public void Store(YYQERPEntities libraryDataContext)
        {
            if (HttpContext.Current.Items.Contains(_dataContextKey))
                HttpContext.Current.Items[_dataContextKey] = libraryDataContext;
            else
                HttpContext.Current.Items.Add(_dataContextKey, libraryDataContext);  
        }
    }
}
