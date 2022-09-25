using System.Text;
using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Storage;

namespace shunda.Service.Storage;

[Service(ServiceType.Scoped)]
public class StorageLocDesignService
{
    private ISession session = null;

    public StorageLocDesignService(ISession session)
    {
        this.session = session;
    }

    private string[] emptyStorageLocCodes = new string[] { "_", "-" };

    /// <summary>
    /// 保存仓位规划
    /// </summary>
    /// <param name="shelfId"></param>
    /// <param name="designContent"></param>
    /// <exception cref="Exception"></exception>
    public void SaveStorageLocDesign(int shelfId, string designContent)
    {
        // 检查货架上的库位是不是都是无货状态

        /*
        如果货架上面一个仓位有货，会导致整个货架不能规划，所以先去掉这个检查，订单物品对仓位的关联要有外键，防止规划时仓位被错误的删除掉。
        IList<int> storageLocIdList = session.Query<StorageLocInfo>().Where(e => e.Shelf.Id == shelfId).Select(e => e.Id).ToList();

        // 这里也不考虑订单占用仓位的状态了，只考虑订单物品占用仓位的情况就行了，只要是用了的，就不能简单重新规划了。
        IList<LogisticsOrderGoodsStatus> occupyStorageLocStatusList = LogisticsOrderGoodsStatus.GetOccupyStorageLocStatusList();
        int orderGoodsCount = session
            .Query<LogisticsOrderGoodsInfo>()
            .Where(e => occupyStorageLocStatusList.Contains(e.Status)
                        && e.StorageLoc != null
                        && storageLocIdList.Contains(e.StorageLoc.Id))
            .Count();
        if (orderGoodsCount != 0)
        {
            throw new Exception("货架上有仓位处于有货状态，规划失败！");
        }
        */

        ShelfInfo shelf = session.Get<ShelfInfo>(shelfId);

        var shelfType = shelf.Type;
        IList<StorageLocSpecInfo> specList = session.Query<StorageLocSpecInfo>().Where(e => e.ShelfType == shelfType).ToList();
        IList<StorageLocInfo> storageLocListInDb = session.Query<StorageLocInfo>().Where(e => e.Shelf.Id == shelfId).ToList();

        string[] storageLocDesignRows = designContent.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        // 检查仓位规格编码是否正确
        for (int rowIndex = 0; rowIndex < storageLocDesignRows.Length; rowIndex++)
        {
            string designRow = storageLocDesignRows[rowIndex];
            for (int colIndex = 0; colIndex < designRow.Length; colIndex++)
            {
                string specCodeFromDesign = designRow[colIndex].ToString();
                if (emptyStorageLocCodes.Contains(specCodeFromDesign))
                {
                    continue;
                }

                StorageLocSpecInfo spec = specList.Where(e => e.Code == specCodeFromDesign).Take(1).SingleOrDefault();
                if (spec == null)
                {
                    throw new Exception($"{shelfType.Name}类型中，不存在编码为{specCodeFromDesign}的仓位规格！");
                }
            }
        }

        // 更新仓位数据
        for (int rowIndex = 0; rowIndex < storageLocDesignRows.Length; rowIndex++)
        {
            int rowNum = rowIndex + 1;
            IList<StorageLocInfo> storageLocListInRowInDb = storageLocListInDb.Where(e => e.RowNum == rowNum).ToList();

            IList<StorageLocInfo> storageLocListInRowFromDesign = new List<StorageLocInfo>();
            string designRow = storageLocDesignRows[rowIndex];
            for (int colIndex = 0; colIndex < designRow.Length; colIndex++)
            {
                int colNum = colIndex + 1;

                string specCodeFromDesign = designRow[colIndex].ToString();
                if (emptyStorageLocCodes.Contains(specCodeFromDesign))
                {
                    continue;
                }

                StorageLocSpecInfo spec = specList.Where(e => e.Code == specCodeFromDesign).Take(1).SingleOrDefault();

                StorageLocInfo storageLocInfo = new StorageLocInfo() {
                    Name = $"{rowNum}层{colNum}列",
                    RowNum = rowNum,
                    ColNum = colNum,
                    Spec = spec,
                    Shelf = session.Load<ShelfInfo>(shelfId),
                    Status = StorageLocStatus.Enabled,
                    OrderNum = 0
                };

                storageLocListInRowFromDesign.Add(storageLocInfo);
            }

            // 添加或者更新规划仓位
            foreach (var locFromDesign in storageLocListInRowFromDesign)
            {
                var locInDb = storageLocListInRowInDb.Where(e => e.RowNum == locFromDesign.RowNum && e.ColNum == locFromDesign.ColNum).Take(1).SingleOrDefault();
                if (locInDb != null)
                {
                    // 更新，就更新个规格，别的信息还是去原来的
                    locInDb.Spec = locFromDesign.Spec;
                    session.Update(locInDb);
                }
                else
                {
                    // 新增，把规划的那个保存到数据库
                    session.Save(locFromDesign);
                }
            }

            session.Flush();

            // 删除数据里面有但是规划里面没有的仓位
            foreach (var locInDb in storageLocListInRowInDb)
            {
                int count = storageLocListInRowFromDesign.Where(e => e.RowNum == locInDb.RowNum && e.ColNum == locInDb.ColNum).Count();
                if (count == 0)
                {
                    session.Delete(locInDb);
                }
            }

            session.Flush();
        }
    }

    /// <summary>
    /// 读取仓位规划值
    /// </summary>
    /// <param name="shelfId"></param>
    /// <returns></returns>
    public string ReadStorageLocDesign(int shelfId)
    {
        IList<StorageLocInfo> storageLocList = session.Query<StorageLocInfo>().Where(e => e.Shelf.Id == shelfId).ToList();
        if (storageLocList == null || storageLocList.Count == 0)
        {
            return string.Empty;
        }

        StringBuilder sb = new StringBuilder();
        int maxRowNum = storageLocList.Max(e => e.RowNum);
        for (int rowNum = 1; rowNum <= maxRowNum; rowNum++)
        {
            IList<StorageLocInfo> storageLocListInRow = storageLocList.Where(e => e.RowNum == rowNum).ToList();
            if (storageLocListInRow == null || storageLocListInRow.Count == 0)
            {
                sb.Append(emptyStorageLocCodes[0]);
            }
            else
            {
                int maxColNum = storageLocListInRow.Max(e => e.ColNum);
                for (int colNum = 1; colNum <= maxColNum; colNum++)
                {
                    StorageLocInfo loc = storageLocListInRow.Where(e => e.ColNum == colNum).Take(1).SingleOrDefault();
                    if (loc == null)
                    {
                        sb.Append(emptyStorageLocCodes[(colNum + 1) % 2]); // 同一行中，两个空符号交叉变换，先用其中第一个
                    }
                    else
                    {
                        sb.Append(loc.Spec.Code);
                    }
                }
            }

            if (rowNum != maxRowNum)
            {
                sb.Append("\r\n");
            }
        }

        return sb.ToString();
    }
}