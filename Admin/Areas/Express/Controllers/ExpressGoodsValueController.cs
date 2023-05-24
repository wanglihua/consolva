using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Admin.Areas.Express.Models.ExpressGoodsValue;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth]
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
    public IActionResult Save(ExpressGoodsValueSaveModel model) {
        ValidationUtil.Validate<ExpressDetailGoodsValueSaveModelValidator, ExpressGoodsValueSaveModel>(model);

        int id = model.Id;
        decimal expressGoodsValue = model.ExpressGoodsValue;

        var express = session.Get<ExpressInfo>(id);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{id}不存在！");
        }

        express.GoodsValue = expressGoodsValue;

        session.Update(express);
        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "货品总值保存成功！",
            Data = new {
                expressGoodsValue = expressGoodsValue.ToString("#0.00"),
            }
        });
    }
}