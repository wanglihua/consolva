using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Express;
using shunda.Domain.Pass;
using shunda.Domain.Storage;
using shunda.Service.Pass;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressCheckinService {
    private readonly ISession session;
    private readonly ExpressCheckinConfigService expressCheckinConfigService;
    private readonly UserService userService;

    public ExpressCheckinService(ISession session,
        ExpressCheckinConfigService expressCheckinConfigService,
        UserService userService) {
        this.session = session;
        this.expressCheckinConfigService = expressCheckinConfigService;
        this.userService = userService;
    }

    /// <summary>
    /// 获取可入库的货架列表
    /// </summary>
    /// <param name="warehouseId">warehouseId</param>
    /// <param name="userId">userId</param>
    /// <param name="shelfIdInclude">
    /// 确切包含的一个货架ID，
    /// 比如是修改已经存在的申领件的信息，这时就必须要包含这个货架，不管这个货架的仓位是不是已经都占用了
    /// 当然，这时要求这个货架应该是指定仓库内的
    /// </param>
    /// <returns></returns>
    public IList<ShelfInfo> GetAvaibleShelfList(int warehouseId, int userId, int? shelfIdInclude) {
        WarehouseInfo warehouse = session.Get<WarehouseInfo>(warehouseId);
        if (warehouse == null) {
            throw new ValidationException($"指定的仓库 #{warehouseId} 不存在！");
        }

        ExpressCheckinConfigInfo expressCheckinConfig = expressCheckinConfigService.GetExpressCheckinConfig();
        if (expressCheckinConfig == null) {
            throw new ValidationException("快递单入库配置不存在！");
        }

        int expressCountPerStorageLoc = expressCheckinConfig.ExpressCountPerStorageLoc;

        bool isAdmin = userService.IsUserAdmin(userId);

        /*
SELECT DISTINCT se.shelf_id
FROM tbl_storage_loc_express se
WHERE se.express_id != 0
GROUP BY se.shelf_id, se.storage_loc_id
HAVING COUNT(*) < 8;
         */
        string sql = $@"
SELECT DISTINCT se.shelf_id
FROM tbl_storage_loc_express se
WHERE se.express_id != 0
GROUP BY se.shelf_id, se.storage_loc_id
HAVING COUNT(*) < {expressCountPerStorageLoc}
";

        IList<int> shelfIdList = session.CreateSQLQuery(sql).List<int>();

        if (isAdmin == false) {
            IList<int> userShelfIdList = session.Query<ShelfUserInfo>()
                .Where(e => e.User.Id == userId)
                .Select(e => e.Shelf.Id)
                .ToList();

            shelfIdList = shelfIdList.Where(e => userShelfIdList.Contains(e)).ToList();
        }

        int shelfIdIncludeValue = shelfIdInclude ?? 0;
        if (shelfIdIncludeValue != 0) {
            shelfIdList.Add(shelfIdIncludeValue);
        }

        IList<ShelfInfo> shelfList = session.Query<ShelfInfo>()
            .Where(e => shelfIdList.Contains(e.Id)
                        && e.Status == ShelfStatus.Enabled
                        && e.Type == ShelfType.Normal
                        && e.Warehouse.Id == warehouseId)
            .OrderBy(e => e.OrderNum).ThenBy(e => e.Id)
            .Distinct()
            .ToList();

        return shelfList;
    }

    /// <summary>
    /// 获取下一个推荐的仓位
    /// </summary>
    /// <param name="shelf">货架</param>
    /// <param name="storageLocSpec">仓位规格s</param>
    /// <returns></returns>
    public StorageLocInfo GetRecommandStorage(ShelfInfo shelf, StorageLocSpecInfo storageLocSpec) {
        if (shelf == null) {
            throw new ValidationException("获取推荐仓位，货架不能为空！");
        }

        if (storageLocSpec == null) {
            throw new ValidationException("获取推荐仓位，仓位规格不能为空！");
        }

        if (shelf.Status != ShelfStatus.Enabled) {
            throw new ValidationException($"货架 {shelf.Name} 当前状态为 已禁用 状态！");
        }

        ExpressCheckinConfigInfo expressCheckinConfig = expressCheckinConfigService.GetExpressCheckinConfig();
        if (expressCheckinConfig == null) {
            throw new ValidationException("快递单入库配置不存在！");
        }

        int expressCountPerStorageLoc = expressCheckinConfig.ExpressCountPerStorageLoc;

        int shelfId = shelf.Id;
        int storageLocSpecId = storageLocSpec.Id;

        /*
         
SELECT DISTINCT sl.id AS storage_loc_id
FROM tbl_storage_loc sl
         INNER JOIN tbl_shelf s ON sl.shelf_id = s.id
         INNER JOIN tbl_storage_loc_express sle ON sle.storage_loc_id = sl.id
WHERE s.id = 1
  AND s.status_id = 10000
  AND sl.status_id = 1000
  AND sl.spec_id = 1000
  AND sle.express_id != 0
GROUP BY sl.id, sl.col_num, sl.row_num, sl.order_num
HAVING COUNT(*) < 8
ORDER BY sl.order_num, sl.col_num, sl.row_num DESC, COUNT(*)
LIMIT 1
        
        */

        string sql = $@"
SELECT DISTINCT sl.id AS storage_loc_id
FROM tbl_storage_loc sl
         INNER JOIN tbl_shelf s ON sl.shelf_id = s.id
         INNER JOIN tbl_storage_loc_express sle ON sle.storage_loc_id = sl.id
WHERE s.id = {shelfId}
  AND s.status_id = {ShelfStatus.Enabled.Id}
  AND sl.status_id = {StorageLocStatus.Enabled.Id}
  AND sl.spec_id = {storageLocSpecId}
  AND sle.express_id != 0
GROUP BY sl.id, sl.col_num, sl.row_num, sl.order_num
HAVING COUNT(*) < {expressCountPerStorageLoc}
ORDER BY sl.order_num, sl.col_num, sl.row_num DESC, COUNT(*)
LIMIT 1
";

        int? storageLocId = session.CreateSQLQuery(sql).UniqueResult<int?>();

        if (storageLocId == null) {
            throw new ValidationException($"货架 {shelf.Name} 没有满足要求的仓位！");
        }

        StorageLocInfo storageLoc = session.Get<StorageLocInfo>((storageLocId ?? 0));
        if (storageLoc == null) {
            throw new Exception($"推荐的仓位 #{storageLocId} 不存在！");
        }

        return storageLoc;
    }

    /// <summary>
    /// 记住快递单入库时用户的货架选择
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="shelfId"></param>
    /// <param name="storageLocSpecId"></param>
    public void SaveUserShelfSelect(int userId, int shelfId, int storageLocSpecId) {
        ExpressCheckinUserShelfSelectInfo? userShelf = session.Query<ExpressCheckinUserShelfSelectInfo>()
            .Where(e => e.User.Id == userId)
            .Take(1)
            .SingleOrDefault();

        if (userShelf == null) {
            userShelf = new ExpressCheckinUserShelfSelectInfo();
        }

        UserInfo user = session.Get<UserInfo>(userId);
        if (user == null) {
            // throw new ValidationException("指定的用户不存在！");
            return;
        }

        ShelfInfo shelf = session.Get<ShelfInfo>(shelfId);
        if (shelf == null) {
            // throw new ValidationException("指定的货架不存在！");
            return;
        }

        StorageLocSpecInfo storageLocSpec = session.Get<StorageLocSpecInfo>(storageLocSpecId);
        if (storageLocSpec == null) {
            // throw new ValidationException("指定的仓位规格不存在！");
            return;
        }

        userShelf.User = user;
        userShelf.Shelf = shelf;
        userShelf.StorageLocSpec = storageLocSpec;

        session.SaveOrUpdate(userShelf);
        session.Flush();
    }

    /// <summary>
    /// 获取快递单入库时记住的货架选择
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public ExpressCheckinUserShelfSelectInfo GetUserShelfSelect(int userId) {
        ExpressCheckinUserShelfSelectInfo? userShelf = session.Query<ExpressCheckinUserShelfSelectInfo>()
            .Where(e => e.User.Id == userId)
            .Take(1)
            .SingleOrDefault();

        if (userShelf == null) {
            userShelf = new ExpressCheckinUserShelfSelectInfo() {
                Shelf = new ShelfInfo() { Id = 0 },
                StorageLocSpec = new StorageLocSpecInfo() { Id = 0 }
            };
        }

        return userShelf;
    }

    /// <summary>
    /// 入库是检查快递单照片数量，如果不符合就抛出异常
    /// </summary>
    /// <param name="expressTrackingNo"></param>
    /// <exception cref="ValidationException"></exception>
    public void CheckPhotoWhenCheckin(string expressTrackingNo) {
        var expressCheckinConfig = expressCheckinConfigService.GetExpressCheckinConfig();
        if (expressCheckinConfig.SigninPhotoMinQtyPerExpress > 0 || expressCheckinConfig.CheckinPhotoMinQtyPerExpress > 0) {
            // 检查快递单号图片
            IList<ExpressPhotoInfo> expressOrderPhotoList = session.Query<ExpressPhotoInfo>()
                .Where(e => e.TrackingNo == expressTrackingNo && (e.Type == ExpressPhotoType.SignIn || e.Type == ExpressPhotoType.CheckIn)) // 当前只查询了 签收照片 和 入库照片
                .OrderByDescending(e => e.Id)
                .ToList();

            if (expressCheckinConfig.SigninPhotoMinQtyPerExpress > 0) {
                int signInPhotoCount = expressOrderPhotoList.Where(e => e.Type == ExpressPhotoType.SignIn).Count();
                if (signInPhotoCount < expressCheckinConfig.SigninPhotoMinQtyPerExpress) {
                    throw new ValidationException($"快递单号对应的【签收图片】少于 {expressCheckinConfig.SigninPhotoMinQtyPerExpress} 张，不能入库！");
                }
            }

            if (expressCheckinConfig.CheckinPhotoMinQtyPerExpress > 0) {
                int checkinPhotoCount = expressOrderPhotoList.Where(e => e.Type == ExpressPhotoType.CheckIn).Count();
                if (checkinPhotoCount < expressCheckinConfig.CheckinPhotoMinQtyPerExpress) {
                    throw new ValidationException($"快递单号对应的【入库图片】少于 {expressCheckinConfig.CheckinPhotoMinQtyPerExpress} 张，请先拍照再入库！");
                }
            }
        }
    }
}