using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Express.Models.ExpressClaim;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressClaimController : Controller
{
    private readonly ISession session;
    private readonly ExpressTrackingNoService expressTrackingNoService;

    public ExpressClaimController(ISession session, ExpressTrackingNoService expressTrackingNoService)
    {
        this.session = session;
        this.expressTrackingNoService = expressTrackingNoService;
    }

    [HttpPost]
    [Route("/express/claim/list")]
    public IActionResult List([FromBody] ExpressClaimListParam param)
    {
        // 这里只显示面单客户ID不能解析获得的快递单

        var listQuery = GetListQuery(param);

        if (param.LastExpressId != 0)
        {
            // 跟降序排序匹配
            int lastExpressIdQuery = param.LastExpressId;
            listQuery = listQuery.Where(e => e.Id < lastExpressIdQuery); // 降序排序id大的会被先取得
        }

        IList<ExpressInfo> expressClaimList = listQuery
            .OrderByDescending(e => e.Id) // 按Id降序排序
            .ToList();

        IList<int> expressIdList = expressClaimList.Select(e => e.Id).ToList();
        IList<ExpressRemarkInfo> expressRemarkList = session.Query<ExpressRemarkInfo>()
            .Where(e => expressIdList.Contains(e.Express!.Id))
            .ToList();

        IList<ExpressClaimListItem> expressClaimItemList = new List<ExpressClaimListItem>();
        foreach (ExpressInfo express in expressClaimList)
        {
            int expressId = express.Id;

            var expressRemark = expressRemarkList
                .Where(e => e.Express!.Id == expressId && e.Type == ExpressRemarkType.Warehouse)
                .Take(1)
                .SingleOrDefault();
            string warehouseRemark = expressRemark?.Remark ?? string.Empty;

            expressClaimItemList.Add(new ExpressClaimListItem() {
                Id = expressId,
                BillCustomerCode = express.BillCustomerCode,
                TrackingNo = expressTrackingNoService.GetHiddenTrackingNo(express.TrackingNo),
                CheckinTrackingNo = express.CheckinTrackingNo,
                GoodsType = express.GoodsType.Name,
                AddDate = express.AddTime.ToString("yyyy-MM-dd"),
                ExpressCompany = express.ExpressCompany,
                WarehouseRemark = warehouseRemark,
            });
        }

        // 取最末尾，其实降序时就是最小的那一个Id
        var lastExpressClaim = expressClaimList.LastOrDefault();
        int lastExpressId = lastExpressClaim?.Id ?? 0; // 如果取值0的话也不破坏逻辑，0的就正好就是没数据了，查询是 <0 也是数据

        int expressClaimCount = GetListQuery(param).Count();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new ExpressClaimListModel() {
                ExpressClaimCount = expressClaimCount,
                ExpressClaimList = expressClaimItemList,
                LastExpressId = lastExpressId,
            }
        });
    }

    private IQueryable<ExpressInfo> GetListQuery(ExpressClaimListParam param)
    {
        var query = session.Query<ExpressInfo>()
            .Where(e => e.BillCustomerId == 0
                        && e.Status == ExpressStatus.WaitPick);

        if (string.IsNullOrWhiteSpace(param.Query) == false)
        {
            string queryStr = param.Query.Trim();
            query = query.Where(e => e.TrackingNo.Contains(queryStr) || e.CheckinTrackingNo.Contains(queryStr));
        }

        return query;
    }
}