using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Fee.Models.FeeWriteOffApply;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.Order;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.WriteOffFeeApply)]
public class FeeWriteOffApplyController : Controller
{
    private ISession session;
    private CustomerCodeService customerCodeService;
    private LoginUserService loginUserService;
    private FeeApplyCountService feeApplyCountService;

    public FeeWriteOffApplyController(ISession session,
        CustomerCodeService customerCodeService,
        LoginUserService loginUserService,
        FeeApplyCountService feeApplyCountService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.loginUserService = loginUserService;
        this.feeApplyCountService = feeApplyCountService;
    }

    [HttpGet]
    [Route("/admin/fee/write/off/apply")]
    public IActionResult Index()
    {
        FeeWriteOffApplyIndexModel model = new FeeWriteOffApplyIndexModel();

        model.FeeReasonList = session.Query<ManualFeeReasonInfo>().Where(e => e.FeeType == ManualFeeType.WriteOffFee).ToList();
        model.ApplyStatusList = FeeWriteOffApplyStatus.GetList();
        model.ScopeList = FeeApplyDataViewScope.GetList();

        return View("FeeWriteOffApplyIndex", model);
    }

    [HttpPost]
    [Route("/admin/fee/write/off/apply/list")]
    public IActionResult List(FeeWriteOffApplyListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<FeeWriteOffApplyInfo> logisticsOrderFeeWriteOffApplyList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<FeeWriteOffApplyInfo>();

        IList<int> orderIdList = logisticsOrderFeeWriteOffApplyList.Select(e => e.Order?.Id ?? 0).ToList();

        IDictionary<int, int> dictApplyCount = feeApplyCountService.GetApplyCount(orderIdList);

        foreach (FeeWriteOffApplyInfo apply in logisticsOrderFeeWriteOffApplyList)
        {
            int orderId = apply.Order?.Id ?? 0;
            if (dictApplyCount.TryGetValue(orderId, out int applyCount))
            {
                apply.OrderApplyCountCache = applyCount;
            }

            apply.CustomerCodeCache = customerCodeService.GetCustomerCode(apply.Customer.Id);
        }

        FeeWriteOffApplyListModel model = new FeeWriteOffApplyListModel(listPager);
        model.FeeWriteOffApplyList = logisticsOrderFeeWriteOffApplyList;

        return View("FeeWriteOffApplyList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(FeeWriteOffApplyListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<FeeWriteOffApplyInfo>();

        criteria.SetReadOnly(true);

        // criteria = criteria.CreateAlias(nameof(FeeWriteOffApplyInfo.Order), nameof(FeeWriteOffApplyInfo.Order));
        // criteria = criteria.CreateAlias(nameof(FeeWriteOffApplyInfo.Customer), nameof(FeeWriteOffApplyInfo.Customer));

        if (queryParam.ViewScopeQuery == FeeApplyDataViewScope.MySelf)
        {
            int loginUserId = loginUserService.GetLoginUserId();
            criteria = criteria.Add(Restrictions.Where<FeeWriteOffApplyInfo>(e => e.ApplyUser.Id == loginUserId));
        }

        if (queryParam.ManualReasonIdQuery != 0)
        {
            criteria = criteria.Add(Restrictions.Where<FeeWriteOffApplyInfo>(e => e.WriteOffReason.Id == queryParam.ManualReasonIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"{nameof(FeeWriteOffApplyInfo.Order)}.{nameof(FeeWriteOffApplyInfo.Order.Code)}", orderCodeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Eq($"{nameof(FeeWriteOffApplyInfo.Customer)}.{nameof(FeeWriteOffApplyInfo.Customer.Id)}", customerIdQuery));
        }

        if (queryParam.StatusQuery != null && queryParam.StatusQuery != FeeWriteOffApplyStatus.None)
        {
            criteria = criteria.Add(Restrictions.Where<FeeWriteOffApplyInfo>(e => e.Status == queryParam.StatusQuery));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/fee/write/off/apply/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        FeeWriteOffApplyDetailModel model = new FeeWriteOffApplyDetailModel();

        FeeWriteOffApplyInfo feeWriteOffApply;

        if (id != null && id.Value != 0)
        {
            feeWriteOffApply = session.Get<FeeWriteOffApplyInfo>(id.Value);
            if (feeWriteOffApply == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }

            model.ApplyId = feeWriteOffApply.Id;
            model.OrderCode = feeWriteOffApply.Order.Code;
            model.CustomerCode = customerCodeService.GetCustomerCode(feeWriteOffApply.Customer.Id);
            model.Amount = feeWriteOffApply.Amount;
            model.FeeReasonId = feeWriteOffApply.WriteOffReason.Id;
            model.Description = feeWriteOffApply.Description;
        }

        model.FeeReasonList = session.Query<ManualFeeReasonInfo>().Where(e => e.FeeType == ManualFeeType.WriteOffFee).ToList();

        return View("FeeWriteOffApplyDetail", model);
    }

    [HttpPost]
    [Route("/admin/fee/write/off/apply/save")]
    [Transactional]
    public IActionResult Save(FeeWriteOffApplySaveModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        if (model.Amount == null || model.Amount.Value <= 0)
        {
            throw new ValidationException("金额不能为空，也不能小于等于0！");
        }

        if (model.FeeReasonId <= 0)
        {
            throw new ValidationException("核销原因不能为空！");
        }

        LogisticsOrderInfo? logisticsOrder = session.Query<LogisticsOrderInfo>()
            .Where(e => e.Code == model.OrderCode)
            .Take(1)
            .SingleOrDefault();

        // 订单
        if (logisticsOrder == null)
        {
            throw new ValidationException("指定的订单不存在！");
        }

        // 客户
        int customerId = customerCodeService.GetCustomerIdFromCustomerCode(model.CustomerCode);
        if (customerId == 0)
        {
            throw new ValidationException("客户ID输入格式有误！");
        }

        CustomerInfo customer = session.Get<CustomerInfo>(customerId);
        if (customer == null)
        {
            throw new ValidationException("指定的客户不存在！");
        }

        if (logisticsOrder.Customer.Id != customer.Id)
        {
            throw new ValidationException("指定的订单和客户不匹配！");
        }

        int applyId = model.ApplyId;

        bool isCreate = applyId == 0;

        FeeWriteOffApplyInfo feeWriteOffApply;

        if (isCreate)
        {
            feeWriteOffApply = new FeeWriteOffApplyInfo();
            feeWriteOffApply.Status = FeeWriteOffApplyStatus.Saved;
        }
        else
        {
            feeWriteOffApply = session.Get<FeeWriteOffApplyInfo>(applyId);

            // 判断状态是不是能修改
            if (feeWriteOffApply.Status != FeeWriteOffApplyStatus.Saved
                && feeWriteOffApply.Status != FeeWriteOffApplyStatus.Backed)
            {
                throw new ValidationException("只有【未提交】和【已回退】状态的申请才能修改！");
            }

            int loginUserId = loginUserService.GetLoginUserId();
            if (feeWriteOffApply.ApplyUser.Id != loginUserId)
            {
                throw new ValidationException("只能修改当前登录人创建的申请！");
            }
        }

        ManualFeeReasonInfo manualFeeReason = session.Get<ManualFeeReasonInfo>(model.FeeReasonId);
        if (manualFeeReason == null)
        {
            throw new ValidationException($"指定的核销费用原因#{model.FeeReasonId}不存在！");
        }

        feeWriteOffApply.Order = logisticsOrder;
        feeWriteOffApply.Customer = customer;
        feeWriteOffApply.Amount = model.Amount ?? 0;
        feeWriteOffApply.ApplyUser = session.Get<UserInfo>(loginUserService.GetLoginUserId());
        feeWriteOffApply.WriteOffReason = manualFeeReason;
        feeWriteOffApply.WriteOffReasonContent = manualFeeReason.Content;
        feeWriteOffApply.Description = model.Description;

        // 不修改申请状态，新建的时候会有个默认的
        // 审核人这里不修改，新增的时候为空，修改的时候保存为原来的。

        if (isCreate)
        {
            session.Save(feeWriteOffApply);
        }
        else
        {
            session.Update(feeWriteOffApply);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/fee/write/off/apply/submit")]
    [Transactional]
    public IActionResult Submit(int[] idList)
    {
        if (idList == null || idList.Length == 0)
        {
            throw new ValidationException("请选择要提交的数据项！");
        }

        int loginUserId = loginUserService.GetLoginUserId();

        IList<FeeWriteOffApplyInfo> feeWriteOffApplyList = session
            .Query<FeeWriteOffApplyInfo>()
            .Where(e => idList.Contains(e.Id))
            .ToList();

        foreach (FeeWriteOffApplyInfo apply in feeWriteOffApplyList)
        {
            if (apply.ApplyUser.Id != loginUserId)
            {
                throw new ValidationException("只能提交当前登录人保存的申请！");
            }

            if (apply.Status != FeeWriteOffApplyStatus.Saved && apply.Status != FeeWriteOffApplyStatus.Backed)
            {
                throw new ValidationException("只能提交【未提交】的和【已回退】的申请！");
            }
        }

        foreach (FeeWriteOffApplyInfo apply in feeWriteOffApplyList)
        {
            apply.Status = FeeWriteOffApplyStatus.Submitted;
            session.Update(apply);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "提交成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/fee/write/off/apply/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        if (idList == null || idList.Length == 0)
        {
            throw new ValidationException("请选择要删除的数据项！");
        }

        int loginUserId = loginUserService.GetLoginUserId();

        IList<FeeWriteOffApplyInfo> feeWriteOffApplyList = session
            .Query<FeeWriteOffApplyInfo>()
            .Where(e => idList.Contains(e.Id))
            .ToList();

        foreach (FeeWriteOffApplyInfo apply in feeWriteOffApplyList)
        {
            if (apply.ApplyUser.Id != loginUserId)
            {
                throw new ValidationException("只能删除当前登录人创建的申请！");
            }

            if (apply.Status != FeeWriteOffApplyStatus.Saved && apply.Status != FeeWriteOffApplyStatus.Backed)
            {
                throw new ValidationException("只能删除【未提交】的和【已回退】的申请！");
            }
        }

        foreach (FeeWriteOffApplyInfo apply in feeWriteOffApplyList)
        {
            session.Delete(apply);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/fee/write/off/apply/check/order")]
    [NoLoginRequired]
    public IActionResult CheckOrder(string orderCode)
    {
        if (string.IsNullOrWhiteSpace(orderCode))
        {
            return Json(new Dictionary<string, object>() {
                { "success", true },
                { "message", "订单检查成功，无提示！" },
            });
        }

        LogisticsOrderInfo? logisticsOrder = session.Query<LogisticsOrderInfo>()
            .Where(e => e.Code == orderCode)
            .Take(1)
            .SingleOrDefault();

        if (logisticsOrder == null)
        {
            return Json(new Dictionary<string, object>() {
                { "success", false },
                { "message", $"指定的订单编号 {orderCode}, 不存在！！" },
            });
        }

        int orderId = logisticsOrder?.Id ?? 0;

        IList<string> applyCountDescList = feeApplyCountService.GetApplyCountDescList(orderId);

        if (applyCountDescList.Count > 0)
        {
            string message = string.Join("<br/>", applyCountDescList);

            return Json(new Dictionary<string, object>() {
                { "success", false },
                { "message", message },
            });
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "订单检查成功，无提示！" },
        });
    }
}