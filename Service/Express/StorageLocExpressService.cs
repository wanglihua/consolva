using System.Text;
using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Storage;

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
    /// 更新货架所有仓位
    /// 存储过程内部有数据库事务
    /// </summary>
    public void UpdateShelf(int shelfId)
    {
        session.CreateSQLQuery($"CALL storage_loc_express_update_shelf({shelfId})").List();
    }

    /// <summary>
    /// 更新所有
    /// 存储过程内部有数据库事务
    /// </summary>
    public void UpdateAll()
    {
        session.CreateSQLQuery("CALL storage_loc_express_update_all()").List();
    }

    /// <summary>
    /// 删除仓位上的快递，一般在删除仓位时调用
    /// </summary>
    public void Delete(int storageLocId)
    {
        var storageLocExpressList = session.Query<StorageLocExpressInfo>()
            .Where(e => e.StorageLoc.Id == storageLocId)
            .ToList();

        foreach (var storageLocExpress in storageLocExpressList)
        {
            session.Delete(storageLocExpress);
        }

        session.Flush();
    }

    /// <summary>
    /// 删除仓位上的快递，一般在删除仓位时调用
    /// </summary>
    public void Delete(IList<int> storageLocIdList)
    {
        var storageLocExpressList = session.Query<StorageLocExpressInfo>()
            .Where(e => storageLocIdList.Contains(e.StorageLoc.Id))
            .ToList();

        foreach (var storageLocExpress in storageLocExpressList)
        {
            session.Delete(storageLocExpress);
        }

        session.Flush();
    }
}