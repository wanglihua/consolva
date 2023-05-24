using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Admin.Areas.Express.Models.ExpressGoodsValue;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressGoodsValue)]
public class ExpressGoodsValueController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public ExpressGoodsValueController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/admin/express/goods/value/show")]
    public IActionResult Show(int id) {
        var express = session.Get<ExpressInfo>(id);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{id}不存在！");
        }

        var model = new ExpressGoodsValueShowModel {
            Id = id,
            TrackingNo = express.TrackingNo,
            ExpressGoodsValue = express.GoodsValue,
        };

        return View("ExpressGoodsValueDialog", model);
    }

    [HttpPost]
    [Route("/admin/express/goods/value/save")]
    [Transactional]
    public IActionResult Save(ExpressGoodsValueSaveParam param) {
        ValidationUtil.Validate<ExpressDetailGoodsValueSaveParamValidator, ExpressGoodsValueSaveParam>(param);

        int id = param.Id;
        decimal expressGoodsValue = param.ExpressGoodsValue;

        var express = session.Get<ExpressInfo>(id);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{id}不存在！");
        }

        express.GoodsValue = expressGoodsValue;

        session.Update(express);
        session.Flush();

        decimal orderGoodsValue = 0m;
        int orderId = express.Order?.Id ?? 0;
        if (orderId != 0) {
            var expressList = session.Query<ExpressInfo>()
                .Where(e => e.Order!.Id == orderId)
                .ToList();

            // 这个判断一般是肯定成立的
            if(expressList.Count != 0) {
                orderGoodsValue = expressList.Sum(e => e.GoodsValue);
            }

            var order = session.Get<LogisticsOrderInfo>(orderId);
            if(order != null) {
                order.GoodsValue = orderGoodsValue;

                session.Update(order);
                session.Flush();
            }
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "货品总值保存成功！",
            Data = new ExpressGoodsValueSaveModel {
                ExpressId = express.Id,
                ExpressGoodsValue = expressGoodsValue.ToString("#0.00"),
                OrderGoodsValue = orderGoodsValue.ToString("#0.00"),
            }
        });
    }
}