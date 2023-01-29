using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.FreightEstimateFirstMode;
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
public class FreightEstimateFirstModeController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly FreightEstimateSelectService freightEstimateSelectService;

    public FreightEstimateFirstModeController(ISession session,
        LoginCustomerService loginCustomerService,
        FreightEstimateSelectService freightEstimateSelectService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.freightEstimateSelectService = freightEstimateSelectService;
    }

    [HttpPost]
    [Route("/client/freight/estimate/first/mode/list")]
    public IActionResult List() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var estimateSelect = session.Get<FreightEstimateSelectInfo>(loginCustomerId);
        if (estimateSelect == null || estimateSelect.Country == Country.None) {
            throw new ValidationException("运费预估选择数据不存在或者国家未选择！");
        }

        int selectFirstModeId = estimateSelect.FirstModeId;
        Country country = estimateSelect.Country;

        var firstModeList = session.Query<LogisticsModeInfo>()
            .Where(e => e.Country == country // todo 物品类型的问题怎么解决
                        && e.Cate == LogisticsModeCate.Customer
                        && e.Status == LogisticsModeStatus.Enabled)
            .OrderBy(e => e.Type)
            .ThenByDescending(e => e.Id)
            .ToList();

        // 对已选择的做个修正，如果当前列表中，没包含这个已选择的，就修正为0，就是未选择
        if (selectFirstModeId != 0 && firstModeList.Where(e => e.Id == selectFirstModeId).Count() == 0) {
            selectFirstModeId = 0; // 不把这个值当前保存到数据库
        }

        var firstModeItemList = firstModeList
            .Select(e => new FreightEstimateFirstModeListItem {
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
            Data = new FreightEstimateFirstModeListModel {
                FirstModeList = firstModeItemList,
                SelectFirstModeId = selectFirstModeId,
            },
        });
    }

    [HttpPost]
    [Route("/client/freight/estimate/first/mode/save")]
    [Transactional]
    public IActionResult Save([FromBody] FreightEstimateFirstModeSaveParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int firstModeId = param.FirstModeId;
        if (firstModeId <= 0) {
            throw new ValidationException("头程物流不能为空！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        freightEstimateSelectService.SaveFirstMode(loginCustomerId, firstModeId);

        return Json(new JsonResultData {
            Success = true,
            Message = "物流保存成功！",
        });
    }
}