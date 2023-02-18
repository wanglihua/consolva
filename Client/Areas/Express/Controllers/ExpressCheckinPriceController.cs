using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using shunda.Client.Areas.Express.Models.ExpressCheckin;
using shunda.Client.Areas.Express.Models.ExpressCheckinPrice;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressCheckinPriceController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public ExpressCheckinPriceController(ISession session,
        LoginCustomerService loginCustomerService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/express/checkin/price")]
    public IActionResult Price([FromBody] IdParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int expressId = param.Id;
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        if ((express.Customer?.Id ?? 0) != loginCustomerId) {
            throw new ValidationException($"指定的快递单号{express.TrackingNo}不属于当前登录客户#{loginCustomerId}！");
        }

        var goodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => e.Express.Id == expressId)
            .ToList();

        var model = new ExpressCheckinPriceModel {
            Id = express.Id,
            TrackingNo = express.TrackingNo,
            CheckinTrackingNo = express.CheckinTrackingNo,
            GoodsList = goodsList.OrderBy(e => e.Id)
                .Select(e => new ExpressCheckinGoodsListItem {
                    Id = e.Id,
                    GoodsName = e.Goods.Name,
                    Quantity = e.Quantity,
                    Unit = e.Unit,
                })
                .ToList(),
        };

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }

    [HttpPost]
    [Route("/client/express/checkin/price/save")]
    [Transactional]
    public IActionResult PriceSave([FromBody] ExpressCheckinPriceSaveParam param) {
        ValidationUtil.Validate<ExpressCheckinPriceSaveParamValidator, ExpressCheckinPriceSaveParam>(param);

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var expressId = param.ExpressId;
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        if ((express.Customer?.Id ?? 0) != loginCustomerId) {
            throw new ValidationException($"指定的快递单号{express.TrackingNo}不属于当前登录会员#{loginCustomerId}！");
        }

        // 检查价格录入
        if (decimal.TryParse(param.GoodsValue, out decimal goodsValue) == false || goodsValue <= 0) {
            throw new ValidationException("快递单货品总值录入有误！");
        }

        express.GoodsValue = goodsValue;

        session.Update(express);
        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "价格保存成功！",
        });
    }
}