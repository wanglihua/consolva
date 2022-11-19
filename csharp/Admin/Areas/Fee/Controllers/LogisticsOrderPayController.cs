using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Type;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Fee.Models.LogisticsOrderPay;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.LogisticsOrderPay)]
public class LogisticsOrderPayController : Controller
{
    private static readonly DateTime defaultDateBeginQuery = DateTime.ParseExact("2022-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
    private readonly CustomerCodeService customerCodeService;

    private readonly ISession session;

    public LogisticsOrderPayController(ISession session, CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/logistics/order/pay")]
    public IActionResult Index()
    {
        var model = new LogisticsOrderPayIndexModel();
        model.LogisticsModeTypeList = LogisticsModeType.GetList();

        return View("LogisticsOrderPayIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/pay/list")]
    public IActionResult List(LogisticsOrderPayListQueryParam queryParam)
    {
        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            var orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            int orderId = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCodeQuery)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();

            queryParam.OrderId = orderId;
        }

        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var logisticsOrderPayList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsOrderPayInfo>();

        foreach (var orderPay in logisticsOrderPayList)
        {
            // orderPay.Customer 肯定是非空的
            orderPay.CustomerCodeCache = customerCodeService.GetCustomerCode(orderPay.Customer.Id);
        }

        var model = new LogisticsOrderPayListModel(listPager);
        model.LogisticsOrderPayList = logisticsOrderPayList;

        return View("LogisticsOrderPayList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(LogisticsOrderPayListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<LogisticsOrderPayInfo>();
        // criteria = criteria.CreateAlias(nameof(LogisticsOrderPayInfo.Customer), nameof(LogisticsOrderPayInfo.Customer));
        // criteria = criteria.CreateAlias(nameof(LogisticsOrderPayInfo.Order), nameof(LogisticsOrderPayInfo.Order));
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.Status == PayStatus.Paid));

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            var customerCodeQuery = queryParam.CustomerCodeQuery.Trim();

            var customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerCodeQuery);
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.Customer.Id == customerId));
        }

        if (queryParam.OrderId != 0)
        {
            var orderId = queryParam.OrderId;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.Order.Id == orderId));
        }

        var firstModeType = LogisticsModeType.GetById(queryParam.LogisticsModeTypeIdQuery ?? 0);
        if (firstModeType != null && firstModeType != LogisticsModeType.None) 
        {
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsOrderPayInfo.Order)}.{nameof(LogisticsOrderPayInfo.Order.FirstType)}", firstModeType));
        }

        if (queryParam.PayTimeBeginQuery != null)
        {
            var payTimeBeginQuery = queryParam.PayTimeBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.CreateTime >= payTimeBeginQuery));
        }

        if (queryParam.PayTimeEndQuery != null)
        {
            var payTimeEndQuery = queryParam.PayTimeEndQuery.Value.Date;
            payTimeEndQuery = payTimeEndQuery.AddDays(1);

            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.CreateTime < payTimeEndQuery));
        }

        return criteria;
    }

    [NonAction]
    private IList<LogisticsOrderPayDateStatListItem> GetDateStatDataList(LogisticsOrderPayListQueryParam queryParam)
    {
        var criteria = GetListCriteria(queryParam); // 这里面会把 queryParam.PayTimeEndQuery 加上 1
        var alias = criteria.Alias + "_"; // 感觉这里很牵强

        var payDateStatObjArrayList = criteria
            .SetProjection(Projections.ProjectionList()
                .Add(Projections.SqlProjection($"SUM(IF({alias}.pay_type_id = {PayType.AliPay.Id}, {alias}.amount, 0)) AS alipay_amount", new[] { "alipay_amount" }, new IType[] { NHibernateUtil.Decimal }))
                .Add(Projections.SqlProjection($"SUM(IF({alias}.pay_type_id = {PayType.Wechat.Id}, {alias}.amount, 0)) AS wechat_amount", new[] { "wechat_amount" }, new IType[] { NHibernateUtil.Decimal }))
                .Add(Projections.SqlProjection($"SUM(IF({alias}.pay_type_id = {PayType.PayPal.Id}, {alias}.amount, 0)) AS paypal_amount", new[] { "paypal_amount" }, new IType[] { NHibernateUtil.Decimal }))
                .Add(Projections.SqlProjection($"SUM(IF({alias}.pay_type_id = {PayType.Balance.Id}, {alias}.amount, 0)) AS balance_amount", new[] { "balance_amount" }, new IType[] { NHibernateUtil.Decimal }))
                .Add(Projections.SqlProjection($"SUM(IF({alias}.pay_type_id = {PayType.WriteOff.Id}, {alias}.amount, 0)) AS write_off_amount", new[] { "write_off_amount" }, new IType[] { NHibernateUtil.Decimal }))
                .Add(Projections.SqlProjection($"SUM({alias}.amount) AS total_amount", new[] { "total_amount" }, new IType[] { NHibernateUtil.Decimal }))
                .Add(Projections.SqlGroupProjection($"DATE({alias}.create_time) AS create_date", $"DATE({alias}.create_time)", new[] { "create_date" }, new IType[] { NHibernateUtil.Date }))
            )
            .AddOrder(Order.Desc(Projections.SqlGroupProjection($"DATE({alias}.create_time) AS create_date", $"DATE({alias}.create_time)", new[] { "create_date" }, new IType[] { NHibernateUtil.Date })))
            .List<object[]>();

        IList<LogisticsOrderPayDateStatListItem> payDateStatList = new List<LogisticsOrderPayDateStatListItem>();
        foreach (var payDateStatObjArray in payDateStatObjArrayList)
            payDateStatList.Add(new LogisticsOrderPayDateStatListItem {
                AlipayAmount = (decimal)payDateStatObjArray[0],
                WechatAmount = (decimal)payDateStatObjArray[1],
                PaypalAmount = (decimal)payDateStatObjArray[2],
                BalanceAmount = (decimal)payDateStatObjArray[3],
                WriteOffAmount = (decimal)payDateStatObjArray[4],
                TotalAmount = (decimal)payDateStatObjArray[5],
                Date = (DateTime)payDateStatObjArray[6]
            });

        var dateBegin = defaultDateBeginQuery;
        if (queryParam.PayTimeBeginQuery != null) dateBegin = queryParam.PayTimeBeginQuery.Value;

        var dateEnd = DateTime.Today;
        if (queryParam.PayTimeEndQuery != null) dateEnd = queryParam.PayTimeEndQuery.Value;

        // 数据空白日期填充
        for (var date = dateEnd; date >= dateBegin; date = date.AddDays(-1))
        {
            var payDateStat = payDateStatList.Where(e => e.Date == date).Take(1).SingleOrDefault(); // 可以优化下效率
            if (payDateStat == null)
            {
                payDateStat = new LogisticsOrderPayDateStatListItem { Date = date };
                payDateStatList.Add(payDateStat);
            }
        }

        payDateStatList = payDateStatList.OrderByDescending(e => e.Date).ToList();

        return payDateStatList;
    }

    [HttpPost]
    [Route("/admin/logistics/order/pay/date/stat/list")]
    public IActionResult DateStatList(LogisticsOrderPayListQueryParam queryParam)
    {
        var dateBegin = defaultDateBeginQuery;
        if (queryParam.PayTimeBeginQuery != null) dateBegin = queryParam.PayTimeBeginQuery.Value;

        var dateEnd = DateTime.Today;
        if (queryParam.PayTimeEndQuery != null) dateEnd = queryParam.PayTimeEndQuery.Value;

        var totalCount = (int)((dateEnd - dateBegin).TotalDays + 1);

        queryParam.PayTimeBeginQuery = dateBegin;
        var calculateDateBegin = dateEnd.AddDays(1 - (queryParam.PageIndex + 1) * queryParam.PageSize);
        if (calculateDateBegin > dateBegin) queryParam.PayTimeBeginQuery = calculateDateBegin;

        queryParam.PayTimeEndQuery = dateEnd.AddDays(-queryParam.PageIndex * queryParam.PageSize);

        var listPager = new ListPager(queryParam, totalCount);
        var model = new LogisticsOrderPayDateStatListModel(listPager);
        model.DateStatList = GetDateStatDataList(queryParam);

        return View("LogisticsOrderPayDateStatList", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/pay/export")]
    public IActionResult Export(LogisticsOrderPayListQueryParam queryParam)
    {
        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            var orderCodeQuery = queryParam.OrderCodeQuery.Trim();

            int orderId = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCodeQuery)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();

            queryParam.OrderId = orderId;
        }

        var orderPayList = GetListCriteria(queryParam)
            .AddOrder(Order.Asc(nameof(Entity.Id)))
            .List<LogisticsOrderPayInfo>();

        var fileName = "实时收入" + ".xlsx";
        IWorkbook workbook = new XSSFWorkbook();

        var rowHeight = 22;
        var headerCenterCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook, HorizontalAlignment.Center);
        var contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);
        var contentCenterCellStyle = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Center);

        var dataFormat = workbook.CreateDataFormat();

        var precision0Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision0Style.DataFormat = dataFormat.GetFormat("0");

        var precision1Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision1Style.DataFormat = dataFormat.GetFormat("0.0");

        var precision2Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision2Style.DataFormat = dataFormat.GetFormat("0.00");

        var precision3Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision3Style.DataFormat = dataFormat.GetFormat("0.000");

        var precision6Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision6Style.DataFormat = dataFormat.GetFormat("0.000000");

        {
            var sheetName = "支付明细";
            var sheet = workbook.CreateSheet(sheetName);

            var rowIndex = 0;

            var row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            // 客户   订单   支付类型   支付宝/微信/PayPal单号   支付金额   操作用户   状态   完成时间

            var cellIndex = 0;
            sheet.SetColumnWidth(cellIndex, 16 * 256);
            var cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("客户");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("订单");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("支付类型");

            sheet.SetColumnWidth(cellIndex, 50 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("支付宝/微信/PayPal单号");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("支付金额");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("操作用户");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("状态");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("完成时间");

            foreach (var orderPay in orderPayList)
            {
                row = sheet.CreateRow(rowIndex++);
                row.HeightInPoints = rowHeight;

                cellIndex = 0;


                // 客户   订单   支付类型   支付宝/微信/PayPal单号   支付金额   操作用户   状态   完成时间

                // 客户
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                // cell.SetCellValue(orderPay.Customer?.CustomerCode);
                cell.SetCellValue(customerCodeService.GetCustomerCode(orderPay.Customer.Id));

                // 订单
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderPay.Order?.Code);

                // 支付类型
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderPay.PayType?.Name);

                // 支付宝/微信/PayPal单号 
                var outTradeNo = orderPay.OutTradeNo ?? string.Empty;
                if (string.IsNullOrWhiteSpace(orderPay.PayPalPaymentId) == false) outTradeNo = outTradeNo + ", " + orderPay.PayPalPaymentId;

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(outTradeNo);

                // 支付金额
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)orderPay.Amount);

                // 操作用户
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderPay.User?.UserName);

                // 状态
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderPay.Status?.Name);

                // 完成时间
                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(orderPay.UpdateTime.ToString("yyyy-MM-dd HH:mm"));
            }
        }

        {
            var sheetName = "按日期汇总";
            var sheet = workbook.CreateSheet(sheetName);

            var rowIndex = 0;

            var row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            var cellIndex = 0;
            sheet.SetColumnWidth(cellIndex, 16 * 256);
            var cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("日期");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("支付宝");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("微信");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("PayPal");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("余额");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("核销");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue("总金额");

            var dateBegin = defaultDateBeginQuery;
            if (queryParam.PayTimeBeginQuery != null) dateBegin = queryParam.PayTimeBeginQuery.Value;

            var dateEnd = DateTime.Today;
            if (queryParam.PayTimeEndQuery != null) dateEnd = queryParam.PayTimeEndQuery.Value;

            queryParam.PayTimeBeginQuery = dateBegin;
            queryParam.PayTimeEndQuery = dateEnd;

            var dateStatList = GetDateStatDataList(queryParam);

            foreach (var dateStat in dateStatList)
            {
                row = sheet.CreateRow(rowIndex++);
                row.HeightInPoints = rowHeight;

                cellIndex = 0;

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell.SetCellType(CellType.String);
                cell.SetCellValue(dateStat.Date.ToString("yyyy-MM-dd"));

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)dateStat.AlipayAmount);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)dateStat.WechatAmount);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)dateStat.PaypalAmount);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)dateStat.BalanceAmount);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)dateStat.WriteOffAmount);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double)dateStat.TotalAmount);
            }
        }

        var memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return File(memoryStream.ToArray(), ContentType.Xlsx, fileName);
    }
}