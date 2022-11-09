using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Fee.Models.FeeWriteOff;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Fee;
using shunda.Domain.Order;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using shunda.Service.Customer;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.WriteOffFee)]
public class FeeWriteOffController : Controller
{
    private ISession session;
    private CustomerCodeService customerCodeService;
    private LogisticsOrderFeeService logisticsOrderFeeService;

    public FeeWriteOffController(ISession session,
        CustomerCodeService customerCodeService,
        LogisticsOrderFeeService logisticsOrderFeeService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.logisticsOrderFeeService = logisticsOrderFeeService;
    }

    [HttpGet]
    [Route("/admin/fee/write/off")]
    public IActionResult Index()
    {
        FeeWriteOffIndexModel model = new FeeWriteOffIndexModel();

        model.FeeReasonList = session.Query<ManualFeeReasonInfo>().Where(e => e.FeeType == ManualFeeType.WriteOffFee).ToList();

        return View("FeeWriteOffIndex", model);
    }

    [HttpPost]
    [Route("/admin/fee/write/off/list")]
    public IActionResult List(FeeWriteOffListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsOrderPayInfo> logisticsOrderPayList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsOrderPayInfo>();

        foreach (LogisticsOrderPayInfo orderPay in logisticsOrderPayList)
        {
            orderPay.CustomerCodeCache = customerCodeService.GetCustomerCode(orderPay.Customer.Id);
        }

        FeeWriteOffListModel model = new FeeWriteOffListModel(listPager);
        model.LogisticsOrderPayList = logisticsOrderPayList;

        // 汇总信息
        var sumData = GetListCriteria(queryParam)
            .SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count(Projections.Distinct(Projections.Property($"{nameof(LogisticsOrderPayInfo.Customer)}.{nameof(LogisticsOrderPayInfo.Customer.Id)}"))))
                    .Add(Projections.Count(Projections.Distinct(Projections.Property($"{nameof(LogisticsOrderPayInfo.Order)}.{nameof(LogisticsOrderPayInfo.Order.Id)}"))))
                    .Add(Projections.Sum(Projections.Property(nameof(LogisticsOrderPayInfo.Amount))))
            )
            .UniqueResult<object[]>();

        model.CustomerCount = (int)sumData[0];
        model.OrderCount = (int)sumData[1];
        model.AmountSum = (decimal)(sumData[2] ?? 0m);

        return View("FeeWriteOffList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(FeeWriteOffListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<LogisticsOrderPayInfo>();

        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(LogisticsOrderPayInfo.Order), nameof(LogisticsOrderPayInfo.Order));

        criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.Status == PayStatus.Paid && e.PayType == PayType.WriteOff));

        if (queryParam.ManualReasonIdQuery != 0)
        {
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.WriteOffReasonId == queryParam.ManualReasonIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Like($"{nameof(LogisticsOrderPayInfo.Order)}.{nameof(LogisticsOrderPayInfo.Order.Code)}", orderCodeQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsOrderPayInfo.Customer)}.{nameof(LogisticsOrderPayInfo.Customer.Id)}", customerIdQuery));
        }

        if (queryParam.BeginDateQuery != null)
        {
            DateTime beginDateQueryValue = queryParam.BeginDateQuery.Value;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.UpdateTime >= beginDateQueryValue)); // 这里 CreateTime 和 UpdateTime 都可以
        }

        if (queryParam.EndDateQuery != null)
        {
            DateTime endDateQueryValue = queryParam.EndDateQuery.Value.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.UpdateTime < endDateQueryValue)); // 这里 CreateTime 和 UpdateTime 都可以
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/fee/write/off/export")]
    public IActionResult Export(FeeWriteOffListQueryParam queryParam)
    {
        IList<LogisticsOrderPayInfo> orderPayList = GetListCriteria(queryParam)
            .AddOrder(new Order($"{nameof(LogisticsOrderPayInfo.Id)}", true))
            .List<LogisticsOrderPayInfo>();

        foreach (LogisticsOrderPayInfo orderPay in orderPayList)
        {
            orderPay.CustomerCodeCache = customerCodeService.GetCustomerCode(orderPay.Customer.Id);
        }

        string fileName = "费用核销.xlsx";
        IWorkbook workbook = new XSSFWorkbook();

        ISheet sheet = workbook.CreateSheet("费用核销");

        int rowHeight = 22;
        ICellStyle headerCenterCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook, HorizontalAlignment.Center);
        ICellStyle contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);
        ICellStyle contentCenterCellStyle = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Center);

        IDataFormat dataFormat = workbook.CreateDataFormat();

        ICellStyle precision0Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision0Style.DataFormat = dataFormat.GetFormat("0");

        ICellStyle precision1Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision1Style.DataFormat = dataFormat.GetFormat("0.0");

        ICellStyle precision2Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision2Style.DataFormat = dataFormat.GetFormat("0.00");

        ICellStyle precision3Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision3Style.DataFormat = dataFormat.GetFormat("0.000");

        ICellStyle precision6Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision6Style.DataFormat = dataFormat.GetFormat("0.000000");

        int rowIndex = 0;
        int cellIndex = 0;

        // row 
        IRow row = sheet.CreateRow(rowIndex++);
        row.HeightInPoints = rowHeight;

        cellIndex = 0;

        // 订单    客户    支付类型    支付金额    核销原因    操作用户    状态    生成时间   

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        ICell cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("订单");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("客户");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("支付类型");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("支付金额");

        sheet.SetColumnWidth(cellIndex, 30 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("核销原因");

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
        cell.SetCellValue("生成时间");

        foreach (LogisticsOrderPayInfo orderPay in orderPayList)
        {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 订单    客户    支付类型    支付金额    核销原因    操作用户    状态    生成时间    

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(orderPay.Order?.Code);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(orderPay.CustomerCodeCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(orderPay.PayType?.Name);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = precision2Style;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue((double)orderPay.Amount);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(orderPay.WriteOffReasonContent);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue($"{orderPay.User?.LoginName}({orderPay.User?.UserName})");

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(orderPay.Status?.Name);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(orderPay.CreateTime.ToString("yyyy-MM-dd HH:mm"));
        }


        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return File(memoryStream.ToArray(), ContentType.Xlsx, fileName);
    }

    /// <summary>
    /// 这个方法原来是更新历史数据的，现在基本没用了
    /// </summary>
    /// <returns></returns>
    [NoLoginRequired]
    [HttpPost]
    [Route("/admin/fee/write/off/update/all/write/off/order/pay/status")]
    public IActionResult UpdateAllWriteOffOrderPayStatus()
    {
        var writeOffOrderList = session.Query<LogisticsOrderPayInfo>()
            .Where(e => e.PayType == PayType.WriteOff)
            .Select(e => e.Order)
            .Distinct()
            .ToList();

        foreach (LogisticsOrderInfo logisticsOrder in writeOffOrderList)
        {
            logisticsOrderFeeService.SumFeeAndUpdatePayStatus(logisticsOrder);
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "更新核销订单支付状态成功！" }
        });
    }
}