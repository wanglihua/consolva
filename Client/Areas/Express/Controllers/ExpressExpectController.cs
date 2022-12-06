using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Express.Models.ExpressExpect;
using shunda.Client.Services;
using shunda.Domain.Express;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressExpectController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public ExpressExpectController(ISession session, LoginCustomerService loginCustomerService)
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/express/expect/list")]
    public IActionResult List([FromBody] ExpressExpectListQueryParam queryParam)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int expressExpectCount = GetListQuery(loginCustomerId, queryParam).Count();

        IList<ExpressExpectInfo> expressExpectList = GetListQuery(loginCustomerId, queryParam)
            .OrderByDescending(e => e.Status)
            .ToList();

        IList<string> trackingNoList = expressExpectList.Select(e => e.TrackingNo).ToList();
        var expressList = session.Query<ExpressExpectInfo>().Where(e => trackingNoList.Contains(e.TrackingNo))
            .Select(e => new { e.Id, e.TrackingNo })
            .ToList();

        IList<ExpressExpectListItem> itemList = new List<ExpressExpectListItem>();
        foreach (ExpressExpectInfo expressExpect in expressExpectList)
        {
            string trackingNo = expressExpect.TrackingNo;
            var express = expressList.Where(e => e.TrackingNo == trackingNo).Take(1).SingleOrDefault();

            var status = expressExpect.Status;
            if (express == null && status == ExpressExpectStatus.Enter) // 这里是在做一个状态的纠正，正常情况不会发生状态错误
            {
                status = ExpressExpectStatus.Create;
            }

            itemList.Add(new ExpressExpectListItem() {
                Id = expressExpect.Id,
                TrackingNo = trackingNo,
                Description = expressExpect.Description,
                Status = status.Name,
                Enter = status == ExpressExpectStatus.Enter,
                ExpressId = express?.Id, // 可能会是 null
            });
        }

        ExpressExpectListModel model = new ExpressExpectListModel();
        model.ExpressExpectCount = expressExpectCount;
        model.ExpressExpectList = itemList;

        return Json(new {
            success = true,
            message = "操作成功！",
            data = model,
        });
    }

    [NonAction]
    private IQueryable<ExpressExpectInfo> GetListQuery(int loginCustomerId, ExpressExpectListQueryParam queryParam)
    {
        var trackingNoQuery = queryParam.TrackingNoQuery;
        if (string.IsNullOrWhiteSpace(trackingNoQuery) == false)
        {
            trackingNoQuery = trackingNoQuery.Trim();
        }

        IList<ExpressExpectStatus> notCheckinStatusList = new List<ExpressExpectStatus>() {
            ExpressExpectStatus.Create,
            ExpressExpectStatus.Enter,
        };

        var query = session.Query<ExpressExpectInfo>()
            .Where(e => e.Customer.Id == loginCustomerId && notCheckinStatusList.Contains(e.Status));

        if (string.IsNullOrWhiteSpace(trackingNoQuery) == false)
        {
            query = query.Where(e => e.TrackingNo.Contains(trackingNoQuery));
        }

        return query;
    }
}