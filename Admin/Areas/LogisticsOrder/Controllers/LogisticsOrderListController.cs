using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderList;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain;
using shunda.Domain.Express;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

[LogisticsOrderArea]
[AdminAuth(PassRes.LogisticsOrder.OrderListActive, PassRes.LogisticsOrder.OrderListComplete, PassRes.LogisticsOrder.OrderListCancel)]
public class LogisticsOrderListController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly CustomerCodeService customerCodeService;

    public LogisticsOrderListController(ISession session, LoginUserService loginUserService, CustomerCodeService customerCodeService) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/logistics/order/active")]
    public IActionResult IndexActive() {
        var model = new LogisticsOrderListIndexModel {
            MenuType = OrderListMenuType.Active,
        };

        return IndexInternal(model);
    }

    [HttpGet]
    [Route("/admin/logistics/order/complete")]
    public IActionResult IndexComplete() {
        var model = new LogisticsOrderListIndexModel {
            MenuType = OrderListMenuType.Complete,
        };

        return IndexInternal(model);
    }

    [HttpGet]
    [Route("/admin/logistics/order/cancel")]
    public IActionResult IndexCancel() {
        var model = new LogisticsOrderListIndexModel {
            MenuType = OrderListMenuType.Cancel,
        };

        return IndexInternal(model);
    }

    [NonAction]
    private IActionResult IndexInternal(LogisticsOrderListIndexModel model) {
        model.CountryList = Country.GetList();
        model.FirstModeTypeList = LogisticsModeType.GetList();
        model.OrderStatusList = LogisticsOrderStatus.GetList();
        model.PayStatusList = PayStatus.GetList();

        model.ReceivingWarehouseList = session
            .Query<WarehouseInfo>()
            .Where(e => e.InnerStatus == WarehouseStatus.Enabled)
            .OrderBy(e => e.OrderNum)
            .ThenBy(e => e.Id)
            .ToList();

        model.TimeRangeTypeList = TimeRangeType.GetList();

        bool isAdmin = loginUserService.IsAdmin;
        Dictionary<int, bool> passResIdMap = loginUserService.GetPassResIdMap();
        model.CanImportOrder = PassUtil.HasPassRes(passResIdMap, isAdmin, PassRes.LogisticsOrder.ImportOrder);

        return View("LogisticsOrderListIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/list")]
    public IActionResult List(LogisticsOrderListQueryParam queryParam) {
        if (string.IsNullOrWhiteSpace(queryParam.TrackingNoQuery) == false) {
            string trackingNoQuery = queryParam.TrackingNoQuery.Trim().ToUpper();
            IList<int> orderIdList = session
                .Query<ExpressInfo>()
                .Where(e => e.TrackingNo.StartsWith(trackingNoQuery) && e.Order != null)
                .Select(e => e.Order!.Id)
                .ToList();
            queryParam.OrderIdList = queryParam.OrderIdList.Concat(orderIdList).Distinct().ToList();
        }

        if (string.IsNullOrWhiteSpace(queryParam.MobileQuery) == false) {
            var mobileQuery = queryParam.MobileQuery.Trim();
            var orderIdList = session.Query<LogisticsOrderReceiveAddressInfo>()
                .Where(e => e.Mobile.Contains(mobileQuery))
                .Select(e => e.Order.Id)
                .ToList();
            queryParam.OrderIdList = queryParam.OrderIdList.Concat(orderIdList).Distinct().ToList();
        }

        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsOrderInfo> logisticsOrderList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsOrderInfo>();


        var model = new LogisticsOrderListModel(listPager) {
            LogisticsOrderList = logisticsOrderList,
        };

        return View("LogisticsOrderList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(LogisticsOrderListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<LogisticsOrderInfo>();
        criteria = criteria.CreateAlias(nameof(LogisticsOrderInfo.Customer), nameof(LogisticsOrderInfo.Customer));
        criteria = criteria.CreateAlias(nameof(LogisticsOrderInfo.Warehouse), nameof(LogisticsOrderInfo.Warehouse));
        criteria = criteria.CreateAlias(nameof(LogisticsOrderInfo.FirstMode), nameof(LogisticsOrderInfo.FirstMode));

        criteria = criteria.SetReadOnly(true);

        if (queryParam.MenuType == OrderListMenuType.Complete) {
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.OrderStatus == LogisticsOrderStatus.Completed));
        }

        if (queryParam.MenuType == OrderListMenuType.Cancel) {
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.OrderStatus == LogisticsOrderStatus.Canceled));
        }

        if (queryParam.MenuType == OrderListMenuType.Active) {
            var activeStatusList = LogisticsOrderStatus.GetActiveList();
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => activeStatusList.Contains(e.OrderStatus)));
        }


        if (queryParam.Country != null && queryParam.Country != Country.None) {
            var country = queryParam.Country;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.Country == country));
        }

        if (queryParam.OrderIdQuery != 0) {
            int orderIdQuery = queryParam.OrderIdQuery;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.Id == orderIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CodeQuery) == false) {
            string codeQuery = queryParam.CodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.Code == codeQuery));
        }

        /*

        if (string.IsNullOrWhiteSpace(queryParam.TrackingNoQuery) == false) {
            string trackingNoQuery = queryParam.TrackingNoQuery.Trim().ToUpper();
            IList<int> orderIdList = session
                .Query<ExpressInfo>()
                .Where(e => e.TrackingNo.StartsWith(trackingNoQuery) && e.Order != null)
                .Select(e => e.Order!.Id)
                .ToList();
            criteria = criteria.Add(Restrictions.On<LogisticsOrderInfo>(e => e.Id).IsInG<int>(orderIdList));
        }

        if (string.IsNullOrWhiteSpace(queryParam.MobileQuery) == false) {
            var mobileQuery = queryParam.MobileQuery.Trim();
            var orderIdList = session.Query<LogisticsOrderReceiveAddressInfo>()
                .Where(e => e.Mobile.Contains(mobileQuery))
                .Select(e => e.Order.Id)
                .ToList();
            criteria = criteria.Add(Restrictions.On<LogisticsOrderInfo>(e => e.Id).IsInG<int>(orderIdList));
        }

        */

        if (queryParam.OrderIdList.Count != 0) {
            IList<int> orderIdList = queryParam.OrderIdList;
            criteria = criteria.Add(Restrictions.On<LogisticsOrderInfo>(e => e.Id).IsInG<int>(orderIdList));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false) {
            string customerCodeQuery = queryParam.CustomerCodeQuery;
            int customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerCodeQuery);
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.Customer.Id == customerId));
        }

        if (queryParam.OrderStatusQuery != null && queryParam.OrderStatusQuery != LogisticsOrderStatus.None) {
            var orderStatusQuery = queryParam.OrderStatusQuery;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.OrderStatus == orderStatusQuery));
        }

        if (queryParam.PayStatusQuery != null && queryParam.PayStatusQuery != PayStatus.None) {
            var payStatusQuery = queryParam.PayStatusQuery;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.PayStatus == payStatusQuery));
        }

        int receivingWarehouseIdQuery = queryParam.ReceivingWarehouseIdQuery;
        if (receivingWarehouseIdQuery != 0) {
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.Warehouse.Id == receivingWarehouseIdQuery));
        }

        if (queryParam.FirstModeTypeQuery != null && queryParam.FirstModeTypeQuery != LogisticsModeType.None) {
            var firstModeTypeQuery = queryParam.FirstModeTypeQuery;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.FirstType == firstModeTypeQuery));
        }

        if (queryParam.BeginDateQuery != null) {
            DateTime beginDateQueryValue = queryParam.BeginDateQuery.Value;

            if (queryParam.TimeRangeTypeQuery == TimeRangeType.OrderCreate) {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.CreateTime >= beginDateQueryValue));
            } else if (queryParam.TimeRangeTypeQuery == TimeRangeType.SubmitPack) {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.SubmitPackTime >= beginDateQueryValue));
            } else if (queryParam.TimeRangeTypeQuery == TimeRangeType.CompletePack) {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.CompletePackTime >= beginDateQueryValue));
            } else if (queryParam.TimeRangeTypeQuery == TimeRangeType.Pay) {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.PayTime >= beginDateQueryValue));
            } else if (queryParam.TimeRangeTypeQuery == TimeRangeType.Transfer) {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.TransferTime >= beginDateQueryValue));
            } else if (queryParam.TimeRangeTypeQuery == TimeRangeType.Deliver) {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.DeliverTime >= beginDateQueryValue));
            } else {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.CreateTime >= beginDateQueryValue));
            }
        }

        if (queryParam.EndDateQuery != null) {
            DateTime endDateQueryValue = queryParam.EndDateQuery.Value.AddDays(1);

            if (queryParam.TimeRangeTypeQuery == TimeRangeType.OrderCreate) {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.CreateTime < endDateQueryValue));
            } else if (queryParam.TimeRangeTypeQuery == TimeRangeType.SubmitPack) {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.SubmitPackTime < endDateQueryValue));
            } else if (queryParam.TimeRangeTypeQuery == TimeRangeType.CompletePack) {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.CompletePackTime < endDateQueryValue));
            } else if (queryParam.TimeRangeTypeQuery == TimeRangeType.Pay) {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.PayTime < endDateQueryValue));
            } else if (queryParam.TimeRangeTypeQuery == TimeRangeType.Transfer) {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.TransferTime < endDateQueryValue));
            } else if (queryParam.TimeRangeTypeQuery == TimeRangeType.Deliver) {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.DeliverTime < endDateQueryValue));
            } else {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.CreateTime < endDateQueryValue));
            }
        }

        if (queryParam.OrderIdQuery == 0
            && string.IsNullOrWhiteSpace(queryParam.CodeQuery)
            && string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery)
            && string.IsNullOrWhiteSpace(queryParam.MobileQuery)) {
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.CreateMethod == LogisticsOrderCreateMethod.CustomerCreate));
        }

        return criteria;
    }
}