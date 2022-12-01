using NHibernate.Linq;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Service.Storage;

[Service(ServiceType.Scoped)]
public class StorageLocService
{
    private readonly ISession session;

    public StorageLocService(ISession session)
    {
        this.session = session;
    }

    /// <summary>
    /// 检查仓位是否存在，可用（仓位+货架）
    /// </summary>
    /// <param name="storageLocId"></param>
    /// <param name="shelfType"></param>
    /// <exception cref="ValidationException"></exception>
    public void CheckStorageLoc(int storageLocId, ShelfType shelfType)
    {
        if (shelfType == null)
        {
            throw new ValidationException("货架类型未指定！");
        }

        StorageLocInfo? storageLoc = session.Query<StorageLocInfo>()
            .Fetch(e => e.Shelf)
            .Where(e => e.Id == storageLocId)
            .Take(1)
            .SingleOrDefault();

        if (storageLoc == null)
        {
            throw new ValidationException($"指定的仓位#{storageLocId}不存在！");
        }

        if (storageLoc.Shelf.Type != shelfType)
        {
            throw new ValidationException($"指定的仓位不是 {shelfType.Name} 仓位！");
        }

        if (storageLoc.Status != StorageLocStatus.Enabled)
        {
            throw new ValidationException("指定的仓位不是已启用仓位状态！");
        }

        if (storageLoc.Shelf.Status != ShelfStatus.Enabled)
        {
            throw new ValidationException("指定的仓位所在货架不是已启用状态！");
        }
    }
}