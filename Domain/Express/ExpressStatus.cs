using shunda.Core.CodeType;

namespace shunda.Domain.Express;

/// <summary>
/// 申领件状态
/// </summary>
public class ExpressStatus : CodeTypeBase {
    /// <summary>
    /// 待认领
    /// </summary>
    public static readonly ExpressStatus WaitPick = new ExpressStatus(1000, "待认领", "待认领");

    /// <summary>
    /// 已预报
    /// 快递单号有预报，相当于已完成认领，但还没录货值
    /// </summary>
    public static readonly ExpressStatus Expect = new ExpressStatus(2000, "已预报", "已预报");

    //  todo 这里是不是可以加个状态 enter，录货值状态

    /// <summary>
    /// 已认领
    /// 系统的已入库状态
    /// 已完成认领和录完货值
    /// </summary>
    public static readonly ExpressStatus CheckIn = new ExpressStatus(3000, "已认领", "已认领");

    /// <summary>
    /// 待退运
    /// </summary>
    public static readonly ExpressStatus WaitReturn = new ExpressStatus(4000, "待退运", "待退运");

    /// <summary>
    /// 已退运
    /// </summary>
    public static readonly ExpressStatus Returned = new ExpressStatus(5000, "已退运", "已退运");

    /// <summary>
    /// 已丢弃
    /// </summary>
    public static readonly ExpressStatus ThrowAway = new ExpressStatus(6000, "已丢弃", "已丢弃");

    // todo 这个状态，可以不要了
    /// <summary>
    /// 待建单
    /// 这是一个客户端操作时的一个临时选中状态，创建一个新订单时会用待建单状态的快递单号生成
    /// </summary>
    public static readonly ExpressStatus WaitOrder = new ExpressStatus(7000, "待建单", "待建单");

    /// <summary>
    /// 已建单
    /// </summary>
    public static readonly ExpressStatus Order = new ExpressStatus(8000, "已建单", "已建单");

    /// <summary>
    /// 待打包
    /// </summary>
    public static readonly ExpressStatus WaitPack = new ExpressStatus(9000, "待打包", "待打包");

    /// <summary>
    /// 部分已下架
    /// </summary>
    public static readonly ExpressStatus PartOffShelf = new ExpressStatus(10000, "部分已下架", "部分已下架");

    /// <summary>
    /// 已下架
    /// </summary>
    public static readonly ExpressStatus OffShelf = new ExpressStatus(11000, "已下架", "已下架");

    /// <summary>
    /// 部分打包中
    /// </summary>
    public static readonly ExpressStatus PartPacking = new ExpressStatus(12000, "部分打包中", "部分打包中");

    /// <summary>
    /// 打包中
    /// </summary>
    public static readonly ExpressStatus Packing = new ExpressStatus(13000, "打包中", "打包中");

    /// <summary>
    /// 已打包
    /// </summary>
    public static readonly ExpressStatus Pack = new ExpressStatus(14000, "已打包", "已打包");

    /// <summary>
    /// 待转运
    /// </summary>
    public static readonly ExpressStatus WaitTransport = new ExpressStatus(15000, "待转运", "待转运");

    /// <summary>
    /// 转运中
    /// </summary>
    public static readonly ExpressStatus Transporting = new ExpressStatus(16000, "转运中", "转运中");

    /// <summary>
    /// 已交付
    /// </summary>
    public static readonly ExpressStatus Complete = new ExpressStatus(88000, "已交付", "已交付");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressStatus None = new ExpressStatus(0, "", "");

    private static readonly IReadOnlyList<ExpressStatus> codeTypeList = new List<ExpressStatus>() {
        WaitPick,
        Expect,
        CheckIn,
        WaitReturn,
        Returned,
        ThrowAway,
        WaitOrder,
        Order,
        WaitPack,
        PartOffShelf,
        OffShelf,
        PartPacking,
        Packing,
        Pack,
        WaitTransport,
        Transporting,
        Complete,
    };

    private static readonly Dictionary<int, ExpressStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ExpressStatus GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out ExpressStatus? codeType) == true) {
            return codeType;
        }

        return None;
    }

    public static IList<ExpressStatus> GetList() {
        return codeTypeList.ToList();
    }

    /// <summary>
    /// 这些状态下，有可能还需要上架；别的状态的，即使前面没上过架，也都不需要上架了
    /// </summary>
    public static IList<ExpressStatus> GetMayToOnShelfList() {
        return new List<ExpressStatus>() {
            WaitPick,
            Expect,
            CheckIn,
            WaitReturn,
            WaitOrder,
            Order,
            WaitPack, // 这个状态是不是应该放到里面，还有点争议
        };
    }

    /// <summary>
    /// 获取下架及后续状态 
    /// </summary>
    public static IList<ExpressStatus> GetOffShelfList() {
        return new List<ExpressStatus>() {
            Returned,
            OffShelf,
            PartPacking,
            Packing,
            Pack,
            WaitTransport,
            Transporting,
            Complete,
        };
    }

    /// <summary>
    /// 退运状态列表
    /// </summary>
    public static IList<ExpressStatus> GetReturnList() {
        return new List<ExpressStatus>() {
            WaitReturn,
            Returned,
        };
    }

    /// <summary>
    /// 已入库前的状态
    /// </summary>
    private static readonly IReadOnlyList<ExpressStatus> codeTypeListNotCheckedIn = new List<ExpressStatus>() {
        WaitPick,
        Expect,
    };

    /// <summary>
    /// 是不是已入库及后续状态
    ///
    /// 这个也可以用来判断是不是已经被认领
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public static bool IsCheckedIn(ExpressStatus status) {
        if (status == null || status == None) {
            return false;
        }

        foreach (ExpressStatus notCheckedInStatus in codeTypeListNotCheckedIn) {
            if (notCheckedInStatus == status) {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 订单未创建的快递单号状态
    /// </summary>
    private static readonly IReadOnlyList<ExpressStatus> codeTypeListNoOrder = new List<ExpressStatus>() {
        WaitPick,
        Expect,
        CheckIn,
        WaitReturn,
        Returned,
        ThrowAway,
        WaitOrder,
    };

    /// <summary>
    /// 是不是已建单及后续状态
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public static bool IsOrderCreate(ExpressStatus status) {
        if (status == null || status == None) {
            return false;
        }

        foreach (ExpressStatus statusNoOrder in codeTypeListNoOrder) {
            if (statusNoOrder == status) {
                return false;
            }
        }

        return true;
    }

    private static IReadOnlyList<ExpressStatus> codeTypePacked = new List<ExpressStatus>() {
        Pack,
        WaitTransport,
        Transporting,
        Complete,
    };

    public static bool IsPacked(ExpressStatus status) {
        foreach (ExpressStatus statusPacked in codeTypePacked) {
            if (statusPacked == status) {
                return true;
            }
        }

        return false;
    }

    public static IList<ExpressStatus> GetOccupyStorageLocStatusList() {
        return new List<ExpressStatus>() {
            WaitPick,
            Expect,
            CheckIn,
            WaitReturn,
            WaitOrder,
            Order,
            WaitPack,
            PartOffShelf,
        };
    }


    private ExpressStatus(int id, string name, string description) : base(id, name, description) {
    }
}

public class ExpressStatusHibernateType : CodeHibernateTypeBase<ExpressStatus> {
    protected override ExpressStatus GetCodeTypeById(int id) {
        return ExpressStatus.GetById(id);
    }
}

public class ExpressStatusModelBinder : CodeTypeModelBinderBase<ExpressStatus> {
    protected override ExpressStatus GetCodeTypeById(int id) {
        return ExpressStatus.GetById(id);
    }
}