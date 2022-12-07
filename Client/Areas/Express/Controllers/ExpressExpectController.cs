using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Express.Models.ExpressExpect;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressExpectController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly ExpressExpectService expressExpectService;

    public ExpressExpectController(ISession session,
        LoginCustomerService loginCustomerService,
        ExpressExpectService expressExpectService)
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.expressExpectService = expressExpectService;
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

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new ExpressExpectListModel {
                ExpressExpectCount = expressExpectCount,
                ExpressExpectList = itemList,
            },
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

    [HttpPost]
    [Route("/client/express/expect/detail")]
    public IActionResult Detail([FromBody] IdParam param)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        if (ModelState.IsValid == false)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int expressExpectId = param.Id;
        var expressExpect = session.Get<ExpressExpectInfo>(expressExpectId);

        if (expressExpect == null)
        {
            throw new ValidationException($"指定的快递单号预告 #{expressExpectId} 不存在！");
        }

        if (expressExpect.Customer.Id != loginCustomerId)
        {
            throw new ValidationException($"指定的快递单号预告 #{expressExpectId} 不属于当前会员 #{loginCustomerId} ！");
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new ExpressExpectDetailModel() {
                Id = expressExpect.Id,
                TrackingNo = expressExpect.TrackingNo,
                Description = expressExpect.Description,
            },
        });
    }

    [HttpPost]
    [Route("/client/express/expect/add/save")]
    [Transactional]
    public IActionResult AddSave([FromBody] ExpressExpectAddModel model)
    {
        if (ModelState.IsValid == false)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        expressExpectService.Add(loginCustomerId, model.TrackingNo, model.Description ?? string.Empty);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [HttpPost]
    [Route("/client/express/expect/edit/save")]
    [Transactional]
    public IActionResult EditSave([FromBody] ExpressExpectEditModel model)
    {
        if (ModelState.IsValid == false)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        expressExpectService.Edit(model.Id, loginCustomerId, model.TrackingNo, model.Description ?? string.Empty);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [HttpPost]
    [Route("/client/express/expect/delete")]
    public IActionResult Delete([FromBody] IdParam param)
    {
        if (ModelState.IsValid == false)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        expressExpectService.Delete(expressExpectId: param.Id, customerId: loginCustomerId);

        return Json(new JsonResultData {
            Success = true,
            Message = "快递单号预告删除成功！",
        });
    }
}