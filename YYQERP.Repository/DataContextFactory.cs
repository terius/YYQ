using YYQERP.Repository.DataContextStorage;


namespace YYQERP.Repository
{
    public class DataContextFactory
    {
        public static YYQERPEntities GetDataContext()
        {
          //  DateTime dt1 = DateTime.Now;
            IDataContextStorageContainer _dataContextStorageContainer = DataContextStorageFactory.CreateStorageContainer();
            YYQERPEntities YYQERPEntities = _dataContextStorageContainer.GetDataContext();
            if (YYQERPEntities == null)
            {
                YYQERPEntities = new YYQERPEntities();
                _dataContextStorageContainer.Store(YYQERPEntities);
            }
           // var ts = DateTime.Now.Subtract(dt1).TotalSeconds.ToString("0.00");
           // YYQERP.Infrastructure.Logging.LoggingFactory.GetLogger().Log("加载Context时间"+ ts + "秒");
            return YYQERPEntities;
        }

     
    }

}
