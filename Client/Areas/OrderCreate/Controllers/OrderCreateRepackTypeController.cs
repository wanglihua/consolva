using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.OrderCreate.Models.OrderCreateRepackType;
using shunda.Client.Services;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.OrderCreate.Controllers;

[OrderCreateArea]
public class OrderCreateRepackTypeController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsOrderCreateSelectService logisticsOrderCreateSelectService;

    public OrderCreateRepackTypeController(ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsOrderCreateSelectService logisticsOrderCreateSelectService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsOrderCreateSelectService = logisticsOrderCreateSelectService;
    }

    [HttpPost]
    [Route("/client/order/create/repack/type/list")]
    public IActionResult List() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(loginCustomerId);

        int selectRepackTypeId = createSelect?.RepackType.Id ?? 0;
        var repackTypeList = session.Query<LogisticsRepackTypeSettingInfo>()
            .Where(e => e.Status == LogisticsRepackTypeStatus.Enabled)
            .OrderBy(e => e.OrderNum)
            .ToList();

        if (selectRepackTypeId != 0 && repackTypeList.Where(e => e.Type.Id == selectRepackTypeId).Count() == 0) {
            selectRepackTypeId = 0;
        }

        // 如果前面没选择的，就默认选择排序的第一个
        if (selectRepackTypeId == 0) {
            selectRepackTypeId = repackTypeList.First().Type.Id;
        }

        string remark = createSelect?.Remark ?? string.Empty;

        var repackTypeItemList = repackTypeList
            .Select(e => new OrderCreateRepackTypeListItem {
                Id = e.Type.Id, // 这里用的id 是 LogisticsRepackType 的Id
                Title = e.Title,
                Description = e.Description,
            })
            .ToList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderCreateRepackTypeListModel {
                RepackTypeList = repackTypeItemList,
                SelectRepackTypeId = selectRepackTypeId,
                Remark = remark,
            },
        });
    }

    [HttpPost]
    [Route("/client/order/create/repack/type/save")]
    [Transactional]
    public IActionResult Save([FromBody] OrderCreateRepackTypeSaveParam param) {
        ValidationUtil.Validate<OrderCreateRepackTypeSaveParamValidator, OrderCreateRepackTypeSaveParam>(param);

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        LogisticsRepackType repackType = LogisticsRepackType.GetById(param.RepackTypeId);
        logisticsOrderCreateSelectService.SaveRepackTypeAndRemark(loginCustomerId, repackType, param.Remark);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}