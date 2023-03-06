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
public class ExpressExpectController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly ExpressExpectService expressExpectService;

    public ExpressExpectController(ISession session,
        LoginCustomerService loginCustomerService,
        ExpressExpectService expressExpectService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.expressExpectService = expressExpectService;
    }

    [HttpPost]
    [Route("/client/express/expect/list")]
    public IActionResult List([FromBody] ExpressExpectListQueryParam queryParam) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var model = GetListData(loginCustomerId, queryParam);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }

    [NonAction]
    private ExpressExpectListModel GetListData(int loginCustomerId, ExpressExpectListQueryParam queryParam) {
        // int expressExpectCount = GetListQuery(loginCustomerId, queryParam).Count();

        IList<ExpressExpectInfo> expressExpectList = GetListQuery(loginCustomerId, queryParam)
            .OrderByDescending(e => e.Status).ThenByDescending(e => e.Id)
            .ToList();

        IList<string> trackingNoList = expressExpectList.Select(e => e.TrackingNo).ToList();
        var expressList = session.Query<ExpressExpectInfo>().Where(e => trackingNoList.Contains(e.TrackingNo))
            .Select(e => new { e.Id, e.TrackingNo })
            .ToList();

        IList<ExpressExpectListItem> itemList = new List<ExpressExpectListItem>();
        foreach (ExpressExpectInfo expressExpect in expressExpectList) {
            string trackingNo = expressExpect.TrackingNo;
            var express = expressList.Where(e => e.TrackingNo == trackingNo).Take(1).SingleOrDefault();

            itemList.Add(new ExpressExpectListItem {
                Id = expressExpect.Id,
                TrackingNo = trackingNo,
                GoodsValue = expressExpect.GoodsValue.ToString("#0.00"),
                Description = expressExpect.Description,
                Status = expressExpect.Status.Name,
                AddTime = expressExpect.CreateTime.ToString("yyyy-MM-dd"),
                ExpressId = express?.Id, // 可能会是 null
            });
        }

        int expressExpectCount = itemList.Count;

        return new ExpressExpectListModel {
            ExpressExpectCount = expressExpectCount,
            ExpressExpectList = itemList,
        };
    }

    [NonAction]
    private IQueryable<ExpressExpectInfo> GetListQuery(int loginCustomerId, ExpressExpectListQueryParam queryParam) {
        var trackingNoQuery = queryParam.TrackingNoQuery;
        if (string.IsNullOrWhiteSpace(trackingNoQuery) == false) {
            trackingNoQuery = trackingNoQuery.Trim();
        }

        IList<ExpressExpectStatus> notCheckinStatusList = new List<ExpressExpectStatus>() {
            ExpressExpectStatus.Create,
        };

        var query = session.Query<ExpressExpectInfo>()
            .Where(e => e.Customer.Id == loginCustomerId && notCheckinStatusList.Contains(e.Status));

        if (string.IsNullOrWhiteSpace(trackingNoQuery) == false) {
            query = query.Where(e => e.TrackingNo.Contains(trackingNoQuery));
        }

        return query;
    }

    [HttpPost]
    [Route("/client/express/expect/detail")]
    public IActionResult Detail([FromBody] IdParam param) {
        ValidationUtil.Validate<IdParamValidator, IdParam>(param);

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int expressExpectId = param.Id;
        var expressExpect = session.Get<ExpressExpectInfo>(expressExpectId);

        if (expressExpect == null) {
            throw new ValidationException($"指定的快递单号预告 #{expressExpectId} 不存在！");
        }

        if (expressExpect.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的快递单号预告 #{expressExpectId} 不属于当前客户 #{loginCustomerId} ！");
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new ExpressExpectDetailModel() {
                Id = expressExpect.Id,
                TrackingNo = expressExpect.TrackingNo,
                GoodsValue = expressExpect.GoodsValue.ToString("#0.00"),
                Description = expressExpect.Description,
            },
        });
    }

    [HttpPost]
    [Route("/client/express/expect/add/save")]
    [Transactional]
    public IActionResult AddSave([FromBody] ExpressExpectAddParam model) {
        ValidationUtil.Validate<ExpressExpectAddParamValidator, ExpressExpectAddParam>(model);

        if (decimal.TryParse(model.GoodsValue.Trim(), out decimal goodsValue) == false) {
            throw new ValidationException("货品总值输入格式不对！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        bool claim = expressExpectService.Add(new ExpressExpectService.AddParam {
            CustomerId = loginCustomerId,
            TrackingNo = model.TrackingNo,
            GoodsValue = goodsValue,
            Description = model.Description ?? string.Empty,
        });

        string message = "添加保存成功！";
        if (claim) {
            message = "预报添加成功，并已成功认领对应快递单号！";
        }

        return Json(new JsonResultData {
            Success = true,
            Message = message,
            Data = new ExpressExpectAddModel {
                Claim = claim,
            },
        });
    }

    [HttpPost]
    [Route("/client/express/expect/edit/save")]
    [Transactional]
    public IActionResult EditSave([FromBody] ExpressExpectEditParam model) {
        ValidationUtil.Validate<ExpressExpectEditParamValidator, ExpressExpectEditParam>(model);

        if (decimal.TryParse(model.GoodsValue.Trim(), out decimal goodsValue) == false) {
            throw new ValidationException("货品总值输入格式不对！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        bool claim = expressExpectService.Edit(new ExpressExpectService.EditParam {
            ExpressExpectId = model.Id,
            CustomerId = loginCustomerId,
            TrackingNo = model.TrackingNo,
            GoodsValue = goodsValue,
            Description = model.Description ?? string.Empty,
        });

        string message = "编辑保存成功！";
        if (claim) {
            message = "预报编辑成功，并已成功认领对应快递单号！";
        }

        return Json(new JsonResultData {
            Success = true,
            Message = message,
            Data = new ExpressExpectEditModel {
                Claim = claim,
            },
        });
    }

    [HttpPost]
    [Route("/client/express/expect/delete")]
    [Transactional]
    public IActionResult Delete([FromBody] ExpressExpectDeleteParam param) {
        ValidationUtil.Validate<ExpressExpectDeleteParamValidator, ExpressExpectDeleteParam>(param);

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        expressExpectService.Delete(expressExpectId: param.Id, customerId: loginCustomerId);

        // 马上重新获取数据，客户端可以少一次访问
        var model = GetListData(loginCustomerId, param);

        return Json(new JsonResultData {
            Success = true,
            Message = "单号删除成功！",
            Data = model,
        });
    }
}