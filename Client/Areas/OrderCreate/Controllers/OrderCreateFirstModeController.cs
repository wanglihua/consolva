using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.OrderCreate.Models.OrderCreateFirstMode;
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

namespace shunda.Client.Areas.OrderCreate.Controllers;

[OrderCreateArea]
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
        Country country = createSelect.Country;

        var firstModeList = session.Query<LogisticsModeInfo>()
            .Where(e => e.Country == country // todo 物品类型的问题怎么解决
                        && e.Cate == LogisticsModeCate.Customer // 必须是客户端物流方式，不是内部物流方式
                        && e.Status == LogisticsModeStatus.Enabled)
            .OrderBy(e => e.Type)
            .ThenByDescending(e => e.Id)
            .ToList();

        // 对已选择的做个修正，如果当前列表中，没包含这个已选择的，就修正为0，就是未选择
        if (selectFirstModeId != 0 && firstModeList.Where(e => e.Id == selectFirstModeId).Count() == 0) {
            selectFirstModeId = 0; // 不把这个值当前保存到数据库
        }

        var firstModeItemList = firstModeList
            .Select(e => new OrderCreateFirstModeListItem {
                Id = e.Id,
                Country = e.Country.Name,
                Type = e.Type.Name,
                Name = e.Name,
                Description = e.Description,
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
            Message = "物流保存成功！",
        });
    }
}