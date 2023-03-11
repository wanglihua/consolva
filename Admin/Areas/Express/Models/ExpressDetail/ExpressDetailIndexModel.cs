using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Service.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressDetail;

public class ExpressDetailIndexModel {
    [BindNever]
    public ExpressListMenuCate Cate { get; set; } = ExpressListMenuCate.None;

    [BindNever]
    public ExpressInfo Express { get; set; } = new();

    [BindNever]
    public ExpressReturnInfo ExpressReturn { get; set; } = new();

    [BindNever]
    public UserInfo AddUser { get; set; } = new();

    [BindNever]
    public CustomerInfo? Customer { get; set; } = new();

    [BindNever]
    public ExpressExpectInfo? ExpressExpect { get; set; } = null;

    [BindNever]
    public LogisticsOrderInfo Order { get; set; } = new();

    [BindNever]
    public IList<ExpressGoodsInfo> ExpressGoodsList { get; set; } = new List<ExpressGoodsInfo>();

    [BindNever]
    public IList<ExpressSizeWeightInfo> ExpressSizeWeightList { get; set; } = new List<ExpressSizeWeightInfo>();

    [BindNever]
    public IList<ExpressPhotoInfo> ExpressPhotoList { get; set; } = new List<ExpressPhotoInfo>();

    [BindNever]
    public IList<ExpressLogInfo> ExpressLogList { get; set; } = new List<ExpressLogInfo>();

    public string GetBoolStr(bool? value) {
        if (value == null) {
            return string.Empty;
        }

        if (value.Value) {
            return "是";
        } else {
            return "否";
        }
    }

    [BindNever]
    public bool CanExpressRemarkEdit { get; set; } = false;

    [BindNever]
    public IList<ExpressRemarkInfo> ExpressRemarkList { get; set; } = new List<ExpressRemarkInfo>();

    [BindNever]
    public IList<UserTimeItem> PackUserTimeList { get; set; } = new List<UserTimeItem>();

    [BindNever]
    public IList<UserTimeItem> ShipUserTimeList { get; set; } = new List<UserTimeItem>();

    [BindNever]
    public decimal VolumeWeightDenominator { get; set; }
}