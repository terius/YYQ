using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace YYQERP.Repository.DataContextStorage
{
    public class ThreadDataContextStorageContainer : IDataContextStorageContainer
    {
        private static readonly Hashtable _libraryDataContexts = new Hashtable();

        public YYQERPEntities GetDataContext()
        {
            YYQERPEntities libraryDataContext = null;
            if (_libraryDataContexts.Contains(GetThreadName()))
            {
                libraryDataContext = (YYQERPEntities)_libraryDataContexts[GetThreadName()];
            }
            return libraryDataContext;
        }

        public void Store(YYQERPEntities libraryDataContext)
        {
            if (_libraryDataContexts.Contains(GetThreadName()))
                _libraryDataContexts[GetThreadName()] = libraryDataContext;
            else
                _libraryDataContexts.Add(GetThreadName(), libraryDataContext);
        }


        

        private static string GetThreadName()
        {
            return Thread.CurrentThread.Name ?? Guid.NewGuid().ToString();
        }
    }
}
