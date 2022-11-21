using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Service.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressClaim;

public class ExpressClaimDetailModel
{
    public ExpressListMenuCate Cate { get; set; } = ExpressListMenuCate.None;

    public ExpressInfo Express { get; set; } = new ExpressInfo();

    public UserInfo AddUser { get; set; } = new UserInfo();

    public CustomerInfo? Customer { get; set; } = new CustomerInfo();

    public ExpressExpectInfo? ExpressExpect { get; set; } = null;

    public LogisticsOrderInfo Order { get; set; } = new LogisticsOrderInfo();

    public IList<ExpressGoodsInfo> ExpressGoodsList { get; set; } = new List<ExpressGoodsInfo>();

    public IList<ExpressSizeWeightInfo> ExpressSizeWeightList { get; set; } = new List<ExpressSizeWeightInfo>();

    public IList<ExpressPhotoInfo> ExpressPhotoList { get; set; } = new List<ExpressPhotoInfo>();

    public IList<ExpressLogInfo> ExpressLogList { get; set; } = new List<ExpressLogInfo>();

    public string GetBoolStr(bool? value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        if (value.Value)
        {
            return "是";
        }
        else
        {
            return "否";
        }
    }

    public bool CanCustomerServiceRemark { get; set; } = false;

    public IList<ExpressRemarkInfo> ExpressRemarkList { get; set; } = new List<ExpressRemarkInfo>();

    public IList<UserTimeItem> PackUserTimeList { get; set; } = new List<UserTimeItem>();

    public IList<UserTimeItem> ShipUserTimeList { get; set; } = new List<UserTimeItem>();
}