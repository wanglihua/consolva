using System.Collections.Generic;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Service.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressClaim;

public class ExpressClaimDetailModel
{
    public ExpressListMenuCate Cate { get; set; } = ExpressListMenuCate.None;

    public ExpressInfo ClaimGoods { get; set; } = new ExpressInfo(); // todo a 改名

    public UserInfo AddUser { get; set; } = new UserInfo();

    public CustomerInfo Customer { get; set; } = new CustomerInfo();

    public ExpressExpectInfo ExpressExpect { get; set; } = new ExpressExpectInfo();

    public LogisticsOrderInfo Order { get; set; } = new LogisticsOrderInfo();

    public LogisticsOrderPackInfo OrderPack { get; set; }

    public IList<ExpressGoodsInfo> EntryList { get; set; } = new List<ExpressGoodsInfo>();

    public IList<ExpressSizeWeightInfo> ExpressItemList { get; set; } = new List<ExpressSizeWeightInfo>(); // todo a 改名

    public IList<ExpressPhotoInfo> OrderPhotoList { get; set; } = new List<ExpressPhotoInfo>();

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

    public IList<UserTimeItem> PackUserTimeList { get; set; } = new List<UserTimeItem>();

    public IList<LoadUnloadUserTime> LoadUnloadUserTimeList { get; set; } = new List<LoadUnloadUserTime>();
}