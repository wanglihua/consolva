using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using shunda.Admin.Areas.Fee.Models.ManualFeeApply;
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
[AdminAuth(PassRes.Fee.ManualDecreaseFeeApply)]
public class ManualFeeApplyController : Controller
{
    private ISession session;
    private CustomerCodeService customerCodeService;
    private LoginUserService loginUserService;
    private FeeApplyCountService feeApplyCountService;

    public ManualFeeApplyController(
        ISession session,
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
    [Route("/admin/manual/fee/apply")]
    public IActionResult Index()
    {
        ManualFeeApplyIndexModel model = new ManualFeeApplyIndexModel();

        model.FeeReasonList = session.Query<ManualFeeReasonInfo>().Where(e => e.FeeType == ManualFeeType.ManualDecrease).ToList();
        model.ApplyStatusList = ManualFeeApplyStatus.GetList();
        model.ScopeList = FeeApplyDataViewScope.GetList();

        return View("ManualFeeApplyIndex", model);
    }

    [HttpPost]
    [Route("/admin/manual/fee/apply/list")]
    public IActionResult List(ManualFeeApplyListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ManualFeeApplyInfo> manualFeeApplyList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<ManualFeeApplyInfo>();

        IList<int> orderIdList = manualFeeApplyList.Select(e => (e.Order?.Id ?? 0)).ToList();

        IDictionary<int, int> dictApplyCount = feeApplyCountService.GetApplyCount(orderIdList);

        foreach (ManualFeeApplyInfo apply in manualFeeApplyList)
        {
            int orderId = apply.Order?.Id ?? 0;
            if (dictApplyCount.TryGetValue(orderId, out int applyCount))
            {
                apply.OrderApplyCountCache = applyCount;
            }

            apply.CustomerCodeCache = customerCodeService.GetCustomerCode(apply.Customer.Id);
        }

        ManualFeeApplyListModel model = new ManualFeeApplyListModel(listPager);
        model.ManualFeeApplyList = manualFeeApplyList;

        return View("ManualFeeApplyList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ManualFeeApplyListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<ManualFeeApplyInfo>();

        criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(ManualFeeApplyInfo.Order), nameof(ManualFeeApplyInfo.Order));
        criteria = criteria.CreateAlias(nameof(ManualFeeApplyInfo.Customer), nameof(ManualFeeApplyInfo.Customer));
        criteria = criteria.CreateAlias(nameof(ManualFeeApplyInfo.ApplyUser), nameof(ManualFeeApplyInfo.ApplyUser));
        criteria = criteria.CreateAlias(nameof(ManualFeeApplyInfo.ApproveUser), nameof(ManualFeeApplyInfo.ApproveUser), JoinType.LeftOuterJoin);

        if (queryParam.ViewScopeQuery == FeeApplyDataViewScope.MySelf)
        {
            int loginUserId = loginUserService.GetLoginUserId();
            criteria = criteria.Add(Restrictions.Where<ManualFeeApplyInfo>(e => e.ApplyUser.Id == loginUserId));
        }

        if (queryParam.ManualReasonIdQuery != 0)
        {
            criteria = criteria.Add(Restrictions.Where<ManualFeeApplyInfo>(e => e.ManualReason.Id == queryParam.ManualReasonIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Like($"{nameof(ManualFeeApplyInfo.Order)}.{nameof(ManualFeeApplyInfo.Order.Code)}", orderCodeQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Eq($"{nameof(ManualFeeApplyInfo.Customer)}.{nameof(ManualFeeApplyInfo.Customer.Id)}", customerIdQuery));
        }

        if (queryParam.StatusQuery != null && queryParam.StatusQuery != ManualFeeApplyStatus.None)
        {
            criteria = criteria.Add(Restrictions.Where<ManualFeeApplyInfo>(e => e.Status == queryParam.StatusQuery));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/manual/fee/apply/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        ManualFeeApplyDetailModel model = new ManualFeeApplyDetailModel();

        if (id != null && id.Value != 0)
        {
            ManualFeeApplyInfo manualFeeApply = session.Get<ManualFeeApplyInfo>(id.Value);
            if (manualFeeApply == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }

            model.OrderCode = manualFeeApply.Order.Code;
            model.CustomerCode = customerCodeService.GetCustomerCode(manualFeeApply.Customer.Id);
            model.Fee = manualFeeApply.Fee;
            model.ManualReasonId = manualFeeApply.ManualReason.Id;
            model.Description = manualFeeApply.Description;
        }

        model.ApplyId = id ?? 0;

        model.FeeReasonList = session.Query<ManualFeeReasonInfo>().Where(e => e.FeeType == ManualFeeType.ManualDecrease).ToList();

        return View("ManualFeeApplyDetail", model);
    }

    [HttpPost]
    [Route("/admin/manual/fee/apply/save")]
    [Transactional]
    public IActionResult Save(ManualFeeApplySaveModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        LogisticsOrderInfo? logisticsOrder = session.Query<LogisticsOrderInfo>()
            .Where(e => e.Code == model.OrderCode)
            .Take(1)
            .SingleOrDefault();

        if (logisticsOrder == null)
        {
            throw new ValidationException("指定的订单编号不存在！");
        }

        // 客户
        int customerId = customerCodeService.GetCustomerIdFromCustomerCode(model.CustomerCode); // todo error
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

        if (model.Fee <= 0)
        {
            throw new ValidationException("手工减少费用必须大于0！");
        }

        int applyId = model.ApplyId;

        bool isCreate = applyId == 0;

        ManualFeeApplyInfo manualFeeApply;

        if (isCreate)
        {
            manualFeeApply = new ManualFeeApplyInfo();
            manualFeeApply.Status = ManualFeeApplyStatus.Saved;
        }
        else
        {
            manualFeeApply = session.Get<ManualFeeApplyInfo>(applyId);

            // 判断状态是不是能修改
            if (manualFeeApply.Status != ManualFeeApplyStatus.Saved
                && manualFeeApply.Status != ManualFeeApplyStatus.Backed)
            {
                throw new ValidationException("只有【未提交】和【已回退】状态的申请才能修改！");
            }

            int loginUserId = loginUserService.GetLoginUserId();
            if (manualFeeApply.ApplyUser.Id != loginUserId)
            {
                throw new ValidationException("只能修改当前登录人创建的申请！");
            }
        }

        ManualFeeReasonInfo manualFeeReason = session.Get<ManualFeeReasonInfo>(model.ManualReasonId);
        if (manualFeeReason == null)
        {
            throw new ValidationException($"指定的减少费用原因#{model.ManualReasonId}不存在！");
        }

        manualFeeApply.Order = logisticsOrder;
        manualFeeApply.Customer = customer;
        manualFeeApply.Type = LogisticsOrderFeeType.ManualDecrease;
        manualFeeApply.Fee = model.Fee; // 申请时存储成是正的, 最终记录时存储的是负的
        manualFeeApply.Description = model.Description ?? string.Empty;
        manualFeeApply.ManualReason = manualFeeReason;
        manualFeeApply.ManualReasonContent = manualFeeReason.Content;
        manualFeeApply.ApplyUser = session.Get<UserInfo>(loginUserService.GetLoginUserId());

        // 不修改申请状态，新建的时候会有个默认的
        // 审核人这里不修改，新增的时候为空，修改的时候保存为原来的。

        if (isCreate)
        {
            session.Save(manualFeeApply);
        }
        else
        {
            session.Update(manualFeeApply);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/manual/fee/apply/submit")]
    [Transactional]
    public IActionResult Submit(int[] idList)
    {
        if (idList == null || idList.Length == 0)
        {
            throw new ValidationException("请选择要提交的数据项！");
        }

        int loginUserId = loginUserService.GetLoginUserId();

        IList<ManualFeeApplyInfo> manualFeeApplyList = session
            .Query<ManualFeeApplyInfo>()
            .Where(e => idList.Contains(e.Id))
            .ToList();

        foreach (ManualFeeApplyInfo manualFeeApply in manualFeeApplyList)
        {
            if (manualFeeApply.ApplyUser.Id != loginUserId)
            {
                throw new ValidationException("只能提交当前登录人保存的申请！");
            }

            if (manualFeeApply.Status != ManualFeeApplyStatus.Saved && manualFeeApply.Status != ManualFeeApplyStatus.Backed)
            {
                throw new ValidationException("只能提交【未提交】的和【已回退】的申请！");
            }
        }

        foreach (ManualFeeApplyInfo manualFeeApply in manualFeeApplyList)
        {
            manualFeeApply.Status = ManualFeeApplyStatus.Submitted;
            session.Update(manualFeeApply);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "提交成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/manual/fee/apply/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        if (idList == null || idList.Length == 0)
        {
            throw new ValidationException("请选择要删除的数据项！");
        }

        int loginUserId = loginUserService.GetLoginUserId();

        IList<ManualFeeApplyInfo> manualFeeApplyList = session
            .Query<ManualFeeApplyInfo>()
            .Where(e => idList.Contains(e.Id))
            .ToList();

        foreach (ManualFeeApplyInfo manualFeeApply in manualFeeApplyList)
        {
            if (manualFeeApply.ApplyUser.Id != loginUserId)
            {
                throw new ValidationException("只能删除当前登录人创建的申请！");
            }

            if (manualFeeApply.Status != ManualFeeApplyStatus.Saved && manualFeeApply.Status != ManualFeeApplyStatus.Backed)
            {
                throw new ValidationException("只能删除【未提交】的和【已回退】的申请！");
            }
        }

        foreach (ManualFeeApplyInfo manualFeeApply in manualFeeApplyList)
        {
            session.Delete(manualFeeApply);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/manual/fee/apply/check/order")]
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
            { "message", "订单检查成功，无重复！" },
        });
    }
}