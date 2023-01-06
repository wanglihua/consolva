using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderCreateFirstMode;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateFirstModeController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsOrderCreateSelectService logisticsOrderCreateSelectService;

    public OrderCreateFirstModeController(ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsOrderCreateSelectService logisticsOrderCreateSelectService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsOrderCreateSelectService = logisticsOrderCreateSelectService;
    }

    [HttpPost]
    [Route("/client/order/create/first/mode/list")]
    public IActionResult List() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(loginCustomerId);
        if (createSelect == null || createSelect.Country == Country.None) {
            throw new ValidationException("订单创建选择数据不存在或者国家未选择！");
        }

        int selectFirstModeId = createSelect.FirstModeId;
        Country country = createSelect.Country ?? Country.None;

        var firstModeList = session.Query<LogisticsModeInfo>()
            .Where(e => e.Country == country // todo 物品类型的问题怎么解决
                        && e.Status == LogisticsModeStatus.Enabled)
            .OrderBy(e => e.Type)
            .ThenByDescending(e => e.Id)
            .ToList();

        var firstModeItemList = firstModeList
            .Select(e => new OrderCreateFirstModeListItem {
                ModeId = e.Id,
                ModeName = e.Name,
                ModeDesc = e.Description,
            })
            .ToList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderCreateFirstModeListModel {
                FirstModeList = firstModeItemList,
                SelectFirstModeId = selectFirstModeId,
            },
        });
    }

    [HttpPost]
    [Route("/client/order/create/first/mode/save")]
    [Transactional]
    public IActionResult Save([FromBody] OrderCreateFirstModeSaveParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int firstModeId = param.FirstModeId;
        if (firstModeId <= 0) {
            throw new ValidationException("头程物流不能为空！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        logisticsOrderCreateSelectService.SaveFirstMode(loginCustomerId, firstModeId);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}