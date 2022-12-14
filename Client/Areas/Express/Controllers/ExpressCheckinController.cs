using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Express.Models.ExpressCheckin;
using shunda.Client.Services;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressCheckinController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public ExpressCheckinController(ISession session, LoginCustomerService loginCustomerService)
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/express/checkin/list")]
    public IActionResult List([FromBody] ExpressCheckinListParam param)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        // 这个不分页，一次性全部取出来
        var expressList = GetListQuery(param)
            .OrderByDescending(e => e.Id)
            .ToList();

        int expressCount = GetListQuery(param).Count();

        // 已选择数据
        IList<int> selectedExpressIdList = session.Query<LogisticsOrderCreateSelectExpressInfo>()
            .Where(e => e.CustomerId == loginCustomerId)
            .Select(e => e.ExpressId)
            .ToList();

        IDictionary<int, bool> selectedExpressIdMap = selectedExpressIdList.ToDictionary(e => e, e => true);

        // 物品数据
        IList<int> expressIdList = expressList.Select(e => e.Id).ToList();
        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();

        IDictionary<int, IList<ExpressGoodsInfo>> expressGoodsListMap = new Dictionary<int, IList<ExpressGoodsInfo>>();
        foreach (ExpressGoodsInfo expressGoods in expressGoodsList)
        {
            int expressId = expressGoods.Express.Id;

            if (expressGoodsListMap.TryGetValue(expressId, out IList<ExpressGoodsInfo>? thisExpressGoodsList) == false)
            {
                thisExpressGoodsList = new List<ExpressGoodsInfo>();
            }

            thisExpressGoodsList.Add(expressGoods);

            expressGoodsListMap[expressId] = thisExpressGoodsList;
        }

        // 构建两个列表
        IList<ExpressCheckinListItem> expressWaitOrderList = new List<ExpressCheckinListItem>();
        IList<ExpressCheckinListItem> expressCheckinList = new List<ExpressCheckinListItem>();

        foreach (ExpressInfo express in expressList)
        {
            int expressId = express.Id;

            if (selectedExpressIdMap.ContainsKey(expressId))
            {
            }
            else
            {
            }
        }


        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new ExpressCheckinListModel {
                ExpressTotalCount = expressCount,
            }
        });
    }

    private IQueryable<ExpressInfo> GetListQuery(ExpressCheckinListParam param)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var query = session.Query<ExpressInfo>()
            .Where(e => e.Customer!.Id == loginCustomerId
                        && (e.Status == ExpressStatus.WaitOrder || e.Status == ExpressStatus.CheckIn));

        if (string.IsNullOrWhiteSpace(param.Query) == false)
        {
            string queryStr = param.Query.Trim();
            query = query.Where(e => e.TrackingNo.Contains(queryStr) || e.CheckinTrackingNo.Contains(queryStr));
        }

        return query;
    }
}