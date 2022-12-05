using shunda.Domain.Storage;

namespace shunda.Domain.Express;

public class StorageLocExpressInfo
{
    /// <summary>
    /// 仓位，非空
    /// </summary>
    public virtual StorageLocInfo StorageLoc { get; set; } = new ();

    /// <summary>
    /// 货架，非空
    /// </summary>
    public virtual ShelfInfo Shelf { get; set; } = new ();

    /// <summary>
    /// 快递单，非空
    /// 如果仓位上没有快递单号，会插入一条 express id 为 0 的记录
    /// </summary>
    public virtual int ExpressId { get; set; }

    #region override

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is StorageLocExpressInfo == false)
        {
            return false;
        }

        StorageLocExpressInfo? storageLocExpressAnother = obj as StorageLocExpressInfo;
        if (storageLocExpressAnother == null)
        {
            return false;
        }

        if (StorageLoc?.Id == storageLocExpressAnother.StorageLoc?.Id && ExpressId == storageLocExpressAnother.ExpressId)
        {
            return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        if (StorageLoc.Id == 0 || ExpressId == 0)
        {
            return base.GetHashCode();
        }

        return unchecked(StorageLoc.Id.GetHashCode() + unchecked(3 * ExpressId.GetHashCode()));
    }

    #endregion
}