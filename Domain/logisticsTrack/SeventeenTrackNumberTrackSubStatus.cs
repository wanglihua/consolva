using shunda.Core.CodeType;

namespace shunda.Domain.LogisticsTrack;

/// <summary>
/// 17TRACK快递单号跟踪子状态
/// </summary>
public class SeventeenTrackNumberTrackSubStatus : StringCodeTypeBase {
    /// <summary>
    /// 运输商没有返回信息
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus NotFoundOther = new SeventeenTrackNumberTrackSubStatus(
        "NotFound_Other",
        "运输商没有返回信息",
        "运输商没有返回信息"
    );

    /// <summary>
    /// 物流单号无效，无法进行查询
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus NotFoundInvalidCode = new SeventeenTrackNumberTrackSubStatus(
        "NotFound_InvalidCode",
        "物流单号无效，无法进行查询",
        "物流单号无效，无法进行查询"
    );

    /// <summary>
    /// 收到信息
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus InfoReceived = new SeventeenTrackNumberTrackSubStatus(
        "InfoReceived",
        "收到信息",
        "收到信息"
    );

    /// <summary>
    /// 已揽收
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus InTransitPickedUp = new SeventeenTrackNumberTrackSubStatus(
        "InTransit_PickedUp",
        "已揽收",
        "已揽收"
    );

    /// <summary>
    /// 其它情况
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus InTransitOther = new SeventeenTrackNumberTrackSubStatus(
        "InTransit_Other",
        "其它情况",
        "其它情况"
    );

    /// <summary>
    /// 已离港
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus InTransitDeparture = new SeventeenTrackNumberTrackSubStatus(
        "InTransit_Departure",
        "已离港",
        "已离港"
    );

    /// <summary>
    /// 已到港
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus InTransitArrival = new SeventeenTrackNumberTrackSubStatus(
        "InTransit_Arrival",
        "已到港",
        "已到港"
    );

    /// <summary>
    /// 其它原因
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus ExpiredOther = new SeventeenTrackNumberTrackSubStatus(
        "Expired_Other",
        "其它原因",
        "其它原因"
    );

    /// <summary>
    /// 其它原因
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus AvailableForPickupOther = new SeventeenTrackNumberTrackSubStatus(
        "AvailableForPickup_Other",
        "其它原因",
        "其它原因"
    );

    /// <summary>
    /// 其它原因
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus OutForDeliveryOther = new SeventeenTrackNumberTrackSubStatus(
        "OutForDelivery_Other",
        "其它原因",
        "其它原因"
    );

    /// <summary>
    /// 其它原因
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus DeliveryFailureOther = new SeventeenTrackNumberTrackSubStatus(
        "DeliveryFailure_Other",
        "其它原因",
        "其它原因"
    );

    /// <summary>
    /// 找不到收件人
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus DeliveryFailureNoBody = new SeventeenTrackNumberTrackSubStatus(
        "DeliveryFailure_NoBody",
        "找不到收件人",
        "找不到收件人"
    );

    /// <summary>
    /// 安全原因
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus DeliveryFailureSecurity = new SeventeenTrackNumberTrackSubStatus(
        "DeliveryFailure_Security",
        "安全原因",
        "安全原因"
    );

    /// <summary>
    /// 拒收包裹
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus DeliveryFailureRejected = new SeventeenTrackNumberTrackSubStatus(
        "DeliveryFailure_Rejected",
        "拒收包裹",
        "拒收包裹"
    );

    /// <summary>
    /// 收件地址错误
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus DeliveryFailureInvalidAddress = new SeventeenTrackNumberTrackSubStatus(
        "DeliveryFailure_InvalidAddress",
        "收件地址错误",
        "收件地址错误"
    );

    /// <summary>
    /// 其它原因
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus DeliveredOther = new SeventeenTrackNumberTrackSubStatus(
        "Delivered_Other",
        "其它原因",
        "其它原因"
    );

    /// <summary>
    /// 其它原因
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus ExceptionOther = new SeventeenTrackNumberTrackSubStatus(
        "Exception_Other",
        "其它原因",
        "其它原因"
    );

    /// <summary>
    /// 退件处理中
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus ExceptionReturning = new SeventeenTrackNumberTrackSubStatus(
        "Exception_Returning",
        "退件处理中",
        "退件处理中"
    );

    /// <summary>
    /// 退件已签收
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus ExceptionReturned = new SeventeenTrackNumberTrackSubStatus(
        "Exception_Returned",
        "退件已签收",
        "退件已签收"
    );

    /// <summary>
    /// 没人签收
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus ExceptionNoBody = new SeventeenTrackNumberTrackSubStatus(
        "Exception_NoBody",
        "没人签收",
        "没人签收"
    );

    /// <summary>
    /// 安全原因
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus ExceptionSecurity = new SeventeenTrackNumberTrackSubStatus(
        "Exception_Security",
        "安全原因",
        "安全原因"
    );

    /// <summary>
    /// 货品损坏了
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus ExceptionDamage = new SeventeenTrackNumberTrackSubStatus(
        "Exception_Damage",
        "货品损坏了",
        "货品损坏了"
    );

    /// <summary>
    /// 被拒收了
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus ExceptionRejected = new SeventeenTrackNumberTrackSubStatus(
        "Exception_Rejected",
        "被拒收了",
        "被拒收了"
    );

    /// <summary>
    /// 因各种延迟情况导致的异常
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus ExceptionDelayed = new SeventeenTrackNumberTrackSubStatus(
        "Exception_Delayed",
        "因各种延迟情况导致的异常",
        "因各种延迟情况导致的异常"
    );

    /// <summary>
    /// 包裹丢失了
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus ExceptionLost = new SeventeenTrackNumberTrackSubStatus(
        "Exception_Lost",
        "包裹丢失了",
        "包裹丢失了"
    );

    /// <summary>
    /// 包裹被销毁了
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus ExceptionDestroyed = new SeventeenTrackNumberTrackSubStatus(
        "Exception_Destroyed",
        "包裹被销毁了",
        "包裹被销毁了"
    );

    /// <summary>
    /// 物流订单被取消了
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus ExceptionCancel = new SeventeenTrackNumberTrackSubStatus(
        "Exception_Cancel",
        "物流订单被取消了",
        "物流订单被取消了"
    );

    /// <summary>
    /// None
    /// </summary>
    public static readonly SeventeenTrackNumberTrackSubStatus None = new SeventeenTrackNumberTrackSubStatus("", "", "");

    private static readonly IReadOnlyList<SeventeenTrackNumberTrackSubStatus> codeTypeList = new List<SeventeenTrackNumberTrackSubStatus>() {
        NotFoundOther,
        NotFoundInvalidCode,
        InfoReceived,
        InTransitPickedUp,
        InTransitOther,
        InTransitDeparture,
        InTransitArrival,
        ExpiredOther,
        AvailableForPickupOther,
        OutForDeliveryOther,
        DeliveryFailureOther,
        DeliveryFailureNoBody,
        DeliveryFailureSecurity,
        DeliveryFailureRejected,
        DeliveryFailureInvalidAddress,
        DeliveredOther,
        ExceptionOther,
        ExceptionReturning,
        ExceptionReturned,
        ExceptionNoBody,
        ExceptionSecurity,
        ExceptionDamage,
        ExceptionRejected,
        ExceptionDelayed,
        ExceptionLost,
        ExceptionDestroyed,
        ExceptionCancel,
    };

    private static readonly Dictionary<string, SeventeenTrackNumberTrackSubStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Code);


    public static IList<SeventeenTrackNumberTrackSubStatus> GetList() {
        return codeTypeList.ToList();
    }

    public static SeventeenTrackNumberTrackSubStatus GetByCode(string code) {
        if (string.IsNullOrWhiteSpace(code)) {
            return None;
        }

        code = code.ToUpper();
        if (codeTypeMap.TryGetValue(code, out SeventeenTrackNumberTrackSubStatus? codeType)) {
            return codeType;
        }

        return None;
    }

    private SeventeenTrackNumberTrackSubStatus(string code, string name, string description) : base(code, name, description) {
    }
}

public class SeventeenTrackNumberTrackSubStatusHibernateType : StringCodeHibernateTypeBase<SeventeenTrackNumberTrackSubStatus> {
    protected override SeventeenTrackNumberTrackSubStatus GetCodeTypeByCode(string code) {
        return SeventeenTrackNumberTrackSubStatus.GetByCode(code);
    }
}

public class SeventeenTrackNumberTrackSubStatusModelBinder : StringCodeTypeModelBinderBase<SeventeenTrackNumberTrackSubStatus> {
    protected override SeventeenTrackNumberTrackSubStatus GetCodeTypeByCode(string code) {
        return SeventeenTrackNumberTrackSubStatus.GetByCode(code);
    }
}


/*

主状态


NotFound            查询不到，进行查询操作但没有得到结果，原因请参看子状态。
InfoReceived        收到信息，运输商收到下单信息，等待上门取件。
InTransit           运输途中，包裹正在运输途中，具体情况请参看子状态。
Expired             运输过久，包裹已经运输了很长时间而仍未投递成功。
AvailableForPickup  到达待取，包裹已经到达目的地的投递点，需要收件人自取。
OutForDelivery      派送途中，包裹正在投递过程中。
DeliveryFailure     投递失败，包裹尝试派送但未能成功交付，原因请参看子状态。原因可能是：派送时收件人不在家、投递延误重新安排派送、收件人要求延迟派送、地址不详无法派送、因偏远地区不提供派送服务等。
Delivered           成功签收，包裹已妥投。
Exception           可能异常，包裹可能被退回，原因请参看子状态。原因可能是：收件人地址错误或不详、收件人拒收、包裹无人认领超过保留期等。包裹可能被海关扣留，常见扣关原因是：包含敏感违禁、限制进出口的物品、未交税款等。包裹可能在运输途中遭受损坏、丢失、延误投递等特殊情况。

子状态

NotFound
NotFound_Other         运输商没有返回信息。
NotFound_InvalidCode   物流单号无效，无法进行查询。

InfoReceived
InfoReceived  收到信息

InTransit
InTransit_PickedUp   已揽收。
InTransit_Other      其它情况。
InTransit_Departure  已离港。
InTransit_Arrival    已到港。

Expired  
Expired_Other  其它原因

AvailableForPickup  
AvailableForPickup_Other  其它原因

OutForDelivery
OutForDelivery_Other  其它原因

DeliveryFailure
DeliveryFailure_Other           其它原因。
DeliveryFailure_NoBody          找不到收件人。
DeliveryFailure_Security        安全原因。
DeliveryFailure_Rejected        拒收包裹。
DeliveryFailure_InvalidAddress  收件地址错误。

Delivered  
Delivered_Other  其它原因

Exception  
Exception_Other      其它原因。
Exception_Returning  退件处理中。
Exception_Returned   退件已签收。
Exception_NoBody     没人签收。
Exception_Security   安全原因。
Exception_Damage     货品损坏了。
Exception_Rejected   被拒收了。
Exception_Delayed    因各种延迟情况导致的异常。
Exception_Lost       包裹丢失了。
Exception_Destroyed  包裹被销毁了。
Exception_Cancel     物流订单被取消了。

*/