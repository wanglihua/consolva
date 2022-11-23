using shunda.Domain.Storage;

namespace shunda.Domain.Express;

public class StorageLocExpressInfo
{
    /// <summary>
    /// 仓位，非空
    /// </summary>
    public virtual StorageLocInfo StorageLoc { get; set; } = new StorageLocInfo();

    /// <summary>
    /// 货架，非空
    /// </summary>
    public virtual ShelfInfo Shelf { get; set; } = new ShelfInfo();

    /// <summary>
    /// 快递单，非空
    /// </summary>
    public virtual ExpressInfo Express { get; set; } = new ExpressInfo();

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

        if (StorageLoc?.Id == storageLocExpressAnother.StorageLoc?.Id && Express?.Id == storageLocExpressAnother.Express?.Id)
        {
            return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        if ((StorageLoc?.Id ?? 0) == 0 || (Express?.Id ?? 0) == 0)
        {
            return base.GetHashCode();
        }

        return unchecked(StorageLoc.Id.GetHashCode() + unchecked(3 * Express.Id.GetHashCode()));
    }

    #endregion
}