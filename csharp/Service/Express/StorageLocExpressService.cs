using System.Text;
using shunda.Core.Service;
using ISession = NHibernate.ISession;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class StorageLocExpressService
{
    private readonly ISession session;
    
    public StorageLocExpressService(ISession session)
    {
        this.session = session;
    }

    public void UpdateSingle(int storageLocId)
    {
        session.CreateSQLQuery($"CALL storage_loc_express_update_single({storageLocId})").List();
    }

    public void UpdateBatch(IList<int> storageLocIdList)
    {
        if (storageLocIdList == null || storageLocIdList.Count == 0)
        {
            return;
        }

        StringBuilder sqlBuilder = new StringBuilder();

        foreach (int storageLocId in storageLocIdList)
        {
            sqlBuilder.Append($"CALL storage_loc_express_update_single({storageLocId});");
        }

        session.CreateSQLQuery(sqlBuilder.ToString()).List();
    }

    /// <summary>
    /// 更新所有
    /// 存储过程内部有数据库事务
    /// </summary>
    public void UpdateAll()
    { 
        session.CreateSQLQuery("CALL storage_loc_express_update_all()").List();
    }
}