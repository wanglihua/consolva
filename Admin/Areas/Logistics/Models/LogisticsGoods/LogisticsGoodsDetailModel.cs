using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoods;

public class LogisticsGoodsDetailModel : LogisticsGoodsSaveModel {
    [BindNever]
    public bool IsCreate { get; set; }

    [BindNever]
    public bool IsViewOnly { get; set; }

    [BindNever]
    public int? StatusId { get; set; }

    [BindNever]
    public IList<LogisticsGoodsType> TypeList { get; set; } = LogisticsGoodsType.GetInnerList();

    [BindNever]
    public IList<LogisticsGoodsStatus> StatusList { get; set; } = LogisticsGoodsStatus.GetList();

    [BindNever]
    public IList<LogisticsGoodsLogInfo> LogList { get; set; } = new List<LogisticsGoodsLogInfo>();

    [BindNever]
    public IList<ExpressPhotoInfo> CreateExpressPhotoListCache { get; set; } = new List<ExpressPhotoInfo>();

    [BindNever]
    public IList<ExpressPhotoInfo> FirstOrderExpressPhotoListCache { get; set; } = new List<ExpressPhotoInfo>();

    [BindNever]
    public IList<ExpressPhotoInfo> SecondOrderExpressPhotoListCache { get; set; } = new List<ExpressPhotoInfo>();

    [BindNever]
    public IList<ExpressPhotoInfo> ThirdOrderExpressPhotoListCache { get; set; } = new List<ExpressPhotoInfo>();

    [BindNever]
    public UserInfo AddUser { get; set; } = new();
}