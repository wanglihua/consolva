using shunda.Core.CodeType;

namespace shunda.Domain.LogisticsTrack;

/// <summary>
/// 17TRACK快递单号跟踪状态
/// </summary>
public class SeventeenTrackNumberTrackStatus : StringCodeTypeBase {
    /// <summary>
    /// 查询不到
    /// </summary>
    public static readonly SeventeenTrackNumberTrackStatus NotFound = new SeventeenTrackNumberTrackStatus(
        "NotFound",
        "查询不到",
        "查询不到，进行查询操作但没有得到结果，原因请参看子状态。"
    );

    /// <summary>
    /// 收到信息
    /// </summary>
    public static readonly SeventeenTrackNumberTrackStatus InfoReceived = new SeventeenTrackNumberTrackStatus(
        "InfoReceived",
        "收到信息",
        "收到信息，运输商收到下单信息，等待上门取件。"
    );

    /// <summary>
    /// 运输途中
    /// </summary>
    public static readonly SeventeenTrackNumberTrackStatus InTransit = new SeventeenTrackNumberTrackStatus(
        "InTransit",
        "运输途中",
        "运输途中，包裹正在运输途中，具体情况请参看子状态。"
    );

    /// <summary>
    /// 运输过久
    /// </summary>
    public static readonly SeventeenTrackNumberTrackStatus Expired = new SeventeenTrackNumberTrackStatus(
        "Expired",
        "运输过久",
        "运输过久，包裹已经运输了很长时间而仍未投递成功。"
    );

    /// <summary>
    /// 到达待取
    /// </summary>
    public static readonly SeventeenTrackNumberTrackStatus AvailableForPickup = new SeventeenTrackNumberTrackStatus(
        "AvailableForPickup",
        "到达待取",
        "到达待取，包裹已经到达目的地的投递点，需要收件人自取。"
    );

    /// <summary>
    /// 派送途中
    /// </summary>
    public static readonly SeventeenTrackNumberTrackStatus OutForDelivery = new SeventeenTrackNumberTrackStatus(
        "OutForDelivery",
        "派送途中",
        "派送途中，包裹正在投递过程中。"
    );

    /// <summary>
    /// 投递失败
    /// </summary>
    public static readonly SeventeenTrackNumberTrackStatus DeliveryFailure = new SeventeenTrackNumberTrackStatus(
        "DeliveryFailure",
        "投递失败",
        "投递失败，包裹尝试派送但未能成功交付，原因请参看子状态。原因可能是：派送时收件人不在家、投递延误重新安排派送、收件人要求延迟派送、地址不详无法派送、因偏远地区不提供派送服务等。"
    );

    /// <summary>
    /// 成功签收
    /// </summary>
    public static readonly SeventeenTrackNumberTrackStatus Delivered = new SeventeenTrackNumberTrackStatus(
        "Delivered",
        "成功签收",
        "成功签收，包裹已妥投。"
    );

    /// <summary>
    /// 可能异常
    /// </summary>
    public static readonly SeventeenTrackNumberTrackStatus Exception = new SeventeenTrackNumberTrackStatus(
        "Exception",
        "可能异常",
        "可能异常，包裹可能被退回，原因请参看子状态。原因可能是：收件人地址错误或不详、收件人拒收、包裹无人认领超过保留期等。包裹可能被海关扣留，常见扣关原因是：包含敏感违禁、限制进出口的物品、未交税款等。包裹可能在运输途中遭受损坏、丢失、延误投递等特殊情况。"
    );

    /// <summary>
    /// None
    /// </summary>
    public static readonly SeventeenTrackNumberTrackStatus None = new SeventeenTrackNumberTrackStatus("", "", "");

    private static readonly IReadOnlyList<SeventeenTrackNumberTrackStatus> codeTypeList = new List<SeventeenTrackNumberTrackStatus>() {
        NotFound,
        InfoReceived,
        InTransit,
        Expired,
        AvailableForPickup,
        OutForDelivery,
        DeliveryFailure,
        Delivered,
        Exception
    };

    private static readonly Dictionary<string, SeventeenTrackNumberTrackStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Code);


    public static IList<SeventeenTrackNumberTrackStatus> GetList() {
        return codeTypeList.ToList();
    }

    public static SeventeenTrackNumberTrackStatus GetByCode(string code) {
        if (string.IsNullOrWhiteSpace(code)) {
            return None;
        }

        code = code.ToUpper();
        if (codeTypeMap.TryGetValue(code, out SeventeenTrackNumberTrackStatus? codeType)) {
            return codeType;
        }

        return None;
    }

    private SeventeenTrackNumberTrackStatus(string code, string name, string description) : base(code, name, description) {
    }
}

public class SeventeenTrackNumberTrackStatusHibernateType : StringCodeHibernateTypeBase<SeventeenTrackNumberTrackStatus> {
    protected override SeventeenTrackNumberTrackStatus GetCodeTypeByCode(string code) {
        return SeventeenTrackNumberTrackStatus.GetByCode(code);
    }
}

public class SeventeenTrackNumberTrackStatusModelBinder : StringCodeTypeModelBinderBase<SeventeenTrackNumberTrackStatus> {
    protected override SeventeenTrackNumberTrackStatus GetCodeTypeByCode(string code) {
        return SeventeenTrackNumberTrackStatus.GetByCode(code);
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